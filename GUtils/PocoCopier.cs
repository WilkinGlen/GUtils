namespace GUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

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
        protected override List<System.Reflection.MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);
            var privateFields = objectType
                .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(f => !members.Contains(f) && !f.Name.Contains("k__BackingField"))
                .Cast<System.Reflection.MemberInfo>();
            var allMembers = members.Concat(privateFields).ToList();

            return allMembers;
        }

        protected override JsonProperty CreateProperty(
            System.Reflection.MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = _ => false;
            }

            if (property.DeclaringType == typeof(System.Net.IPAddress) &&
                property.PropertyName == "ScopeId")
            {
                property.ShouldSerialize = _ => false;
            }

            property.Readable = true;
            property.Writable = true;

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
