namespace GUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Reflection;

public class PocoCopier
{
    /// <summary>
    /// Creates a deep copy of an object using JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of object to copy. Must be JSON-serializable.</typeparam>
    /// <param name="source">The source object to copy</param>
    /// <returns>A deep copy of the source object</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when deserialization fails or results in null.</exception>
    public static T DeepCopy<T>(T source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), "Source object cannot be null");
        }

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnoreDelegateContractResolver(),
            TypeNameHandling = TypeNameHandling.All,
            Converters = [new IPAddressConverter()],
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        var jsonString = JsonConvert.SerializeObject(source, settings);
        var result = JsonConvert.DeserializeObject<T>(jsonString, settings);

        return result == null
            ? throw new InvalidOperationException(
                "Deserialization resulted in null. The type may not be supported for deserialization.")
            : result;
    }

    private class IgnoreDelegateContractResolver : DefaultContractResolver
    {
        private readonly HashSet<string> readOnlyPropertiesWithBackingFields = [];

        protected override List<System.Reflection.MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);

            var privateFields = objectType
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => !members.Contains(f) && !f.Name.Contains("k__BackingField"))
                .ToList();

            var backingFieldsToExclude = new HashSet<FieldInfo>();
            var publicProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            this.readOnlyPropertiesWithBackingFields.Clear();

            foreach (var property in publicProperties)
            {
                if (property.CanRead && property.GetSetMethod(true) == null)
                {
                    var potentialBackingField = privateFields.FirstOrDefault(f =>
                    {
                        var expectedName1 = $"_{char.ToLower(property.Name[0])}{property.Name[1..]}";
                        var expectedName2 = $"_{property.Name}";

                        return (f.Name == expectedName1 || f.Name == expectedName2) && (f.FieldType == property.PropertyType ||
                               property.PropertyType.IsAssignableFrom(f.FieldType));
                    });

                    if (potentialBackingField != null)
                    {
                        _ = backingFieldsToExclude.Add(potentialBackingField);
                        _ = this.readOnlyPropertiesWithBackingFields.Add($"{objectType.FullName}.{property.Name}");
                    }
                }
            }

            var fieldsToInclude = privateFields
                .Where(f => !backingFieldsToExclude.Contains(f))
                .Cast<MemberInfo>();

            var allMembers = members.Concat(fieldsToInclude).ToList();

            return allMembers;
        }

        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = _ => false;
            }

            if (property.DeclaringType == typeof(IPAddress) &&
                property.PropertyName == "ScopeId")
            {
                property.ShouldSerialize = _ => false;
            }

            property.Readable = true;

            if (member is PropertyInfo propInfo)
            {
                var fullPropertyName = $"{propInfo.DeclaringType?.FullName}.{propInfo.Name}";
                if (this.readOnlyPropertiesWithBackingFields.Contains(fullPropertyName))
                {
                    property.Writable = false;
                }
                else
                {
                    // Only mark writable if the property actually has a setter
                    property.Writable = propInfo.GetSetMethod(true) != null;
                }
            }
            else
            {
                property.Writable = true;
            }

            return property;
        }
    }

    private class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override IPAddress? ReadJson(
            JsonReader reader,
            Type objectType,
            IPAddress? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var addressString = reader.Value?.ToString();
            return addressString == null ? null : IPAddress.Parse(addressString);
        }
    }
}
