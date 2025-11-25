namespace GUtilsTests.ClassCopierTests;

internal class SimpleTestClass
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

internal class ComplexTestClass
{
    public int Id { get; set; }
    public required SimpleTestClass Inner { get; set; }
    public required List<string> Items { get; set; }
}

internal class ClassWithNestedCollections
{
    public required Dictionary<string, List<int>> Data { get; set; }
    public required int[][] JaggedArray { get; set; }
}

internal class ClassWithDateTime
{
    public DateTime CreatedDate { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }
    public TimeSpan Duration { get; set; }
}

internal class ClassWithNullableProperties
{
    public int? NullableInt { get; set; }
    public string? NullableString { get; set; }
    public SimpleTestClass? NullableObject { get; set; }
}

internal class ClassWithMethods
{
    public int Value { get; set; }
    public required string Name { get; set; }

    public int GetDoubleValue()
    {
        return this.Value * 2;
    }

    public string GetFormattedName()
    {
        return $"Name: {this.Name}";
    }
}

internal class ClassWithEvents
{
    public int Id { get; set; }
    public event EventHandler? ValueChanged;
    public event EventHandler<string>? NameChanged;

    public void RaiseValueChanged()
    {
        this.ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RaiseNameChanged(string newName)
    {
        this.NameChanged?.Invoke(this, newName);
    }
}

internal class ClassWithDelegate
{
    public int Id { get; set; }
    public Action? OnExecute { get; set; }
    public Func<int, int>? Calculator { get; set; }
}

internal record TestRecord(int Id, string Name);

internal class NodeWithCircularRef
{
    public int Id { get; set; }
    public NodeWithCircularRef? Parent { get; set; }
    public NodeWithCircularRef? Child { get; set; }
}

internal enum TestEnum
{
    FirstValue,
    SecondValue,
    ThirdValue
}

internal class ClassWithEnum
{
    public int Id { get; set; }
    public TestEnum Status { get; set; }
}

internal class ClassWithPrivateSetters(int id, string name)
{
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;
}

internal class ClassWithReadOnlyField(string readOnlyValue)
{
    public readonly string? ReadOnlyValue = readOnlyValue;
    public int Id { get; set; }
}

internal class ClassWithIndexer
{
    private readonly Dictionary<string, int> _data = [];

    public int this[string key]
    {
        get => this._data[key];
        set => this._data[key] = value;
    }

    public void Add(string key, int value)
    {
        this._data[key] = value;
    }
}

internal class ClassWithStatic
{
    public static int StaticValue { get; set; }
    public int InstanceValue { get; set; }
}

internal class BaseClass
{
    public string? BaseProperty { get; set; }
}

internal class DerivedClass : BaseClass
{
    public string? DerivedProperty { get; set; }
}

internal abstract class AbstractBase
{
    public int BaseValue { get; set; }
}

internal class ConcreteImplementation : AbstractBase
{
    public int ConcreteValue { get; set; }
}

internal interface ITestInterface
{
    int Id { get; set; }
    string Name { get; set; }
}

internal class InterfaceImplementation : ITestInterface
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

internal class NestedLevel1
{
    public string? Value { get; set; }
    public NestedLevel2? Level2 { get; set; }
}

internal class NestedLevel2
{
    public string? Value { get; set; }
    public NestedLevel3? Level3 { get; set; }
}

internal class NestedLevel3
{
    public string? Value { get; set; }
    public NestedLevel4? Level4 { get; set; }
}

internal class NestedLevel4
{
    public string? Value { get; set; }
}

internal class ClassWithInitOnlyProperty
{
    public int Id { get; set; }
    public string? InitValue { get; init; }
}

internal class ClassWithRequiredProperty
{
    public required int RequiredId { get; set; }
    public required string RequiredName { get; set; }
}

internal class ClassWithByteArray
{
    public int Id { get; set; }
    public required byte[] Data { get; set; }
}

internal class ClassWithNestedRecord
{
    public int Id { get; set; }
    public required TestRecord Record { get; set; }
}

internal class ClassWithTimeTypes
{
    public TimeOnly Time { get; set; }
    public DateOnly Date { get; set; }
}

internal class ClassWithVersion
{
    public required Version Version { get; set; }
}

internal class ClassWithIPAddress
{
    public required System.Net.IPAddress Address { get; set; }
}

internal class ClassWithMixedAccessModifiers(int privateValue)
{
    private readonly int _privateValue = privateValue;
    public string? PublicValue { get; set; }

    public int GetPrivateValue()
    {
        return this._privateValue;
    }
}

internal record struct TestRecordStruct(int Id, string Name);

internal class ClassWithPrivateFields(int privateInt, string privateString)
{
    private readonly int _privateInt = privateInt;
    private readonly string _privateString = privateString;

    public int GetPrivateInt()
    {
        return this._privateInt;
    }

    public string GetPrivateString()
    {
        return this._privateString;
    }
}

internal class ClassWithPrivateNestedObject(SimpleTestClass nested)
{
    private readonly SimpleTestClass _nested = nested;

    public SimpleTestClass GetNestedObject()
    {
        return this._nested;
    }
}

internal class ClassWithPrivateCollection(List<int> items)
{
    private readonly List<int> _items = items;

    public List<int> GetItems()
    {
        return this._items;
    }
}

internal class ClassWithExplicitBackingField
{
    public List<string> Items { get; } = [];

    public void AddItem(string item)
    {
        this.Items.Add(item);
    }
}

internal struct TestStruct
{
    public int X { get; set; }
    public int Y { get; set; }
    public string? Name { get; set; }
}

internal class ClassWithNestedPrivateClass
{
    private readonly PrivateNestedClass _nested;

    private ClassWithNestedPrivateClass(PrivateNestedClass nested)
    {
        this._nested = nested;
    }

    public static ClassWithNestedPrivateClass Create(int value)
    {
        return new ClassWithNestedPrivateClass(new PrivateNestedClass(value));
    }

    public int GetValue()
    {
        return this._nested.Value;
    }

    private class PrivateNestedClass(int value)
    {
        public int Value { get; } = value;
    }
}

internal class ClassWithConstants
{
    public const int ConstantValue = 42;
    public int InstanceValue { get; set; }
}

internal class ClassWithMultipleBackingFields
{
    public List<string> List1 { get; } = [];
    public List<string> List2 { get; } = [];

    public void AddToList1(string item)
    {
        this.List1.Add(item);
    }

    public void AddToList2(string item)
    {
        this.List2.Add(item);
    }
}

internal class ClassWithUnconventionalNaming
{
    public List<string> Items { get; } = [];

    public void AddItem(string item)
    {
        this.Items.Add(item);
    }
}

internal class ClassWithMixedBackingFields(int value)
{
    private readonly int _privateValue = value;

    public List<string> Items { get; } = [];

    public void AddItem(string item)
    {
        this.Items.Add(item);
    }

    public int GetPrivateValue()
    {
        return this._privateValue;
    }
}

internal class OuterClassWithBackingField
{
    public ClassWithExplicitBackingField Inner { get; set; } = new();
}

internal class ClassWithModifiableCollection
{
    private readonly List<string> _items = [];
    public IReadOnlyList<string> Items => this._items;

    public void AddItem(string item)
    {
        this._items.Add(item);
    }
}

internal class ClassWithTransformingProperty
{
    private int _value;
    public int DoubledValue => this._value * 2;

    public void SetValue(int value)
    {
        this._value = value;
    }
}

internal class ClassWithNullableBackingField
{
    public List<string>? Items { get; } = null;
}

internal class NestedBackingFieldClass
{
    public List<string> Outer { get; } = [];
    public InnerBackingFieldClass Inner { get; } = new();

    public void AddOuter(string item) => this.Outer.Add(item);
}

internal class InnerBackingFieldClass
{
    public List<string> Inner { get; } = [];

    public void AddInner(string item) => this.Inner.Add(item);
}

internal class ClassWithConstructorBackingField(List<int> items)
{
    public List<int> Items { get; } = items;
}

internal class ClassWithNullableReferences
{
    public required string RequiredValue { get; set; }
    public string? OptionalValue { get; set; }
}

internal class GrandParentClass
{
    public string? GrandParentProperty { get; set; }
}

internal class ParentClass : GrandParentClass
{
    public string? ParentProperty { get; set; }
}

internal class GrandChildClass : ParentClass
{
    public string? ChildProperty { get; set; }
}

internal class GenericBase<T>
{
    public T? Value { get; set; }
}

internal class GenericDerived : GenericBase<int>
{
    public string? Name { get; set; }
}
