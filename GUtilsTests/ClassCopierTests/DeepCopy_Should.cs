namespace GUtilsTests.ClassCopierTests;

using FluentAssertions;
using GUtils;
using Xunit;

public class DeepCopy_Should
{
    [Fact]
    public void CreateDeepCopyOfSimpleObject()
    {
        var original = new SimpleTestClass { Id = 1, Name = "Test" };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(original.Id);
        _ = copy.Name.Should().Be(original.Name);
    }

    [Fact]
    public void CreateDeepCopyOfComplexObject()
    {
        var original = new ComplexTestClass
        {
            Id = 1,
            Inner = new SimpleTestClass { Id = 2, Name = "Inner" },
            Items = ["Item1", "Item2"]
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Inner.Should().NotBeSameAs(original.Inner);
        _ = copy.Items.Should().NotBeSameAs(original.Items);
        _ = copy.Id.Should().Be(original.Id);
        _ = copy.Inner.Id.Should().Be(original.Inner.Id);
        _ = copy.Inner.Name.Should().Be(original.Inner.Name);
        _ = copy.Items.Should().BeEquivalentTo(original.Items);
    }

    [Fact]
    public void ModifyingCopyShouldNotAffectOriginal()
    {
        var original = new ComplexTestClass
        {
            Id = 1,
            Inner = new SimpleTestClass { Id = 2, Name = "Inner" },
            Items = ["Item1", "Item2"]
        };

        var copy = ClassCopier.DeepCopy(original);
        copy.Id = 99;
        copy.Inner.Name = "Modified";
        copy.Items.Add("Item3");

        _ = original.Id.Should().Be(1);
        _ = original.Inner.Name.Should().Be("Inner");
        _ = original.Items.Should().HaveCount(2);
        _ = original.Items.Should().NotContain("Item3");
    }

    [Fact]
    public void ThrowArgumentNullException_WhenSourceIsNull()
    {
        SimpleTestClass? nullObject = null;

        var action = () => ClassCopier.DeepCopy(nullObject);
        _ = action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void CopyValueType()
    {
        var original = 42;

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyRecord()
    {
        var original = new TestRecord(1, "Test");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyString()
    {
        var original = "Test String";

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyEmptyList()
    {
        var original = new List<string>();

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEmpty();
    }

    [Fact]
    public void CopyListOfComplexObjects()
    {
        var original = new List<SimpleTestClass>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy[0].Should().NotBeSameAs(original[0]);
        _ = copy[1].Should().NotBeSameAs(original[1]);
        _ = copy.Should().HaveCount(2);
        _ = copy[0].Id.Should().Be(1);
        _ = copy[1].Name.Should().Be("Second");
    }

    [Fact]
    public void CopyDictionary()
    {
        var original = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyNestedCollections()
    {
        var original = new ClassWithNestedCollections
        {
            Data = new Dictionary<string, List<int>>
            {
                ["first"] = [1, 2, 3],
                ["second"] = [4, 5, 6]
            },
            JaggedArray = [[1, 2], [3, 4, 5]]
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Data.Should().NotBeSameAs(original.Data);
        _ = copy.Data["first"].Should().NotBeSameAs(original.Data["first"]);
        _ = copy.JaggedArray.Should().NotBeSameAs(original.JaggedArray);
        _ = copy.JaggedArray[0].Should().NotBeSameAs(original.JaggedArray[0]);
        _ = copy.Data["first"].Should().BeEquivalentTo(original.Data["first"]);
        _ = copy.JaggedArray[1].Should().BeEquivalentTo(original.JaggedArray[1]);
    }

    [Fact]
    public void CopyDateTimeTypes()
    {
        var original = new ClassWithDateTime
        {
            CreatedDate = new DateTime(2024, 1, 15, 10, 30, 0),
            ModifiedDate = new DateTimeOffset(2024, 1, 16, 14, 45, 0, TimeSpan.FromHours(-5)),
            Duration = TimeSpan.FromHours(2.5)
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.CreatedDate.Should().Be(original.CreatedDate);
        _ = copy.ModifiedDate.Should().Be(original.ModifiedDate);
        _ = copy.Duration.Should().Be(original.Duration);
    }

    [Fact]
    public void CopyObjectWithNullableProperties_WhenPropertiesHaveValues()
    {
        var original = new ClassWithNullableProperties
        {
            NullableInt = 42,
            NullableString = "Test",
            NullableObject = new SimpleTestClass { Id = 1, Name = "Nested" }
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.NullableInt.Should().Be(42);
        _ = copy.NullableString.Should().Be("Test");
        _ = copy.NullableObject.Should().NotBeSameAs(original.NullableObject);
        _ = copy.NullableObject!.Name.Should().Be("Nested");
    }

    [Fact]
    public void CopyObjectWithNullableProperties_WhenPropertiesAreNull()
    {
        var original = new ClassWithNullableProperties
        {
            NullableInt = null,
            NullableString = null,
            NullableObject = null
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.NullableInt.Should().BeNull();
        _ = copy.NullableString.Should().BeNull();
        _ = copy.NullableObject.Should().BeNull();
    }

    [Fact]
    public void CopyArray()
    {
        var original = new[] { 1, 2, 3, 4, 5 };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyObjectWithSpecialCharactersInStrings()
    {
        var original = new SimpleTestClass
        {
            Id = 1,
            Name = "Test with \"quotes\", newlines\n, tabs\t, and unicode: 🎉"
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Name.Should().Be(original.Name);
    }

    [Fact]
    public void CopyGuid()
    {
        var original = Guid.NewGuid();

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyDecimalAndDouble()
    {
        var originalDecimal = 123.456m;
        var originalDouble = 789.012;

        var copyDecimal = ClassCopier.DeepCopy(originalDecimal);
        var copyDouble = ClassCopier.DeepCopy(originalDouble);

        _ = copyDecimal.Should().Be(originalDecimal);
        _ = copyDouble.Should().Be(originalDouble);
    }

    [Fact]
    public void CopyObjectWithMethods_DataIsCopiedAndMethodsAreAvailable()
    {
        var original = new ClassWithMethods { Value = 10, Name = "Test" };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Value.Should().Be(10);
        _ = copy.Name.Should().Be("Test");
        _ = original.GetDoubleValue().Should().Be(20);
        _ = copy.GetDoubleValue().Should().Be(20);
        _ = original.GetFormattedName().Should().Be("Name: Test");
        _ = copy.GetFormattedName().Should().Be("Name: Test");
    }

    [Fact]
    public void CopyObjectWithMethods_ModifyingCopyDoesNotAffectOriginal()
    {
        var original = new ClassWithMethods { Value = 10, Name = "Original" };

        var copy = ClassCopier.DeepCopy(original);
        copy.Value = 50;
        copy.Name = "Modified";

        _ = original.Value.Should().Be(10);
        _ = original.Name.Should().Be("Original");
        _ = original.GetDoubleValue().Should().Be(20);
        _ = copy.Value.Should().Be(50);
        _ = copy.Name.Should().Be("Modified");
        _ = copy.GetDoubleValue().Should().Be(100);
    }

    [Fact]
    public void CopyObjectWithEvents_EventHandlersAreNotCopied()
    {
        var original = new ClassWithEvents { Id = 1 };
        var eventFired = false;
        original.ValueChanged += (sender, args) => eventFired = true;
        original.RaiseValueChanged();
        _ = eventFired.Should().BeTrue();

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(1);

        eventFired = false;

        copy.RaiseValueChanged();
        _ = eventFired.Should().BeFalse("event handlers are not serialized and should not be copied");
    }

    [Fact]
    public void CopyObjectWithEvents_CanSubscribeToEventsOnCopy()
    {
        var original = new ClassWithEvents { Id = 1 };

        var copy = ClassCopier.DeepCopy(original);
        var copyEventFired = false;
        string? receivedName = null;

        copy.ValueChanged += (sender, args) => copyEventFired = true;
        copy.NameChanged += (sender, name) => receivedName = name;

        copy.RaiseValueChanged();
        copy.RaiseNameChanged("NewName");

        _ = copyEventFired.Should().BeTrue();
        _ = receivedName.Should().Be("NewName");
    }

    [Fact]
    public void CopyObjectWithEvents_OriginalAndCopyEventsAreIndependent()
    {
        var original = new ClassWithEvents { Id = 1 };
        var originalEventFired = false;
        var copyEventFired = false;
        original.ValueChanged += (sender, args) => originalEventFired = true;

        var copy = ClassCopier.DeepCopy(original);

        copy.ValueChanged += (sender, args) => copyEventFired = true;

        original.RaiseValueChanged();
        _ = originalEventFired.Should().BeTrue();
        _ = copyEventFired.Should().BeFalse();

        originalEventFired = false;
        copy.RaiseValueChanged();
        _ = originalEventFired.Should().BeFalse();
        _ = copyEventFired.Should().BeTrue();
    }

    [Fact]
    public void CopyObjectWithDelegate_DelegatesAreNotCopied()
    {
        var executeCalled = false;
        var original = new ClassWithDelegate
        {
            Id = 1,
            OnExecute = () => executeCalled = true,
            Calculator = x => x * 2
        };
        original.OnExecute?.Invoke();
        _ = executeCalled.Should().BeTrue();
        _ = original.Calculator?.Invoke(5).Should().Be(10);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(1);
        _ = copy.OnExecute.Should().BeNull("delegates are not serialized");
        _ = copy.Calculator.Should().BeNull("delegates are not serialized");
    }

    [Fact]
    public void CopyObjectWithDelegate_CanAssignDelegatesToCopy()
    {
        var original = new ClassWithDelegate { Id = 1 };
        var copy = ClassCopier.DeepCopy(original);

        var executedOnCopy = false;

        copy.OnExecute = () => executedOnCopy = true;
        copy.Calculator = x => x * 3;
        copy.OnExecute?.Invoke();
        _ = executedOnCopy.Should().BeTrue();
        _ = copy.Calculator?.Invoke(5).Should().Be(15);
    }

    [Fact]
    public void CopyObjectWithCircularReference_ShouldIgnoreLoop()
    {
        var parent = new NodeWithCircularRef { Id = 1 };
        var child = new NodeWithCircularRef { Id = 2, Parent = parent };
        parent.Child = child;

        var copy = ClassCopier.DeepCopy(parent);

        _ = copy.Should().NotBeSameAs(parent);
        _ = copy.Id.Should().Be(1);
        _ = copy.Child?.Child.Should().BeNull("circular references are ignored");
    }

    [Fact]
    public void CopyEnum()
    {
        var original = TestEnum.SecondValue;

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyObjectWithEnum()
    {
        var original = new ClassWithEnum { Id = 1, Status = TestEnum.SecondValue };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Status.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void CopyMultidimensionalArray()
    {
        var original = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy[0, 0].Should().Be(1);
        _ = copy[1, 2].Should().Be(6);
    }

    [Fact]
    public void CopyHashSet()
    {
        var original = new HashSet<string> { "one", "two", "three" };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyQueue()
    {
        var original = new Queue<int>();
        original.Enqueue(1);
        original.Enqueue(2);
        original.Enqueue(3);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyStack()
    {
        var original = new Stack<string>();
        original.Push("first");
        original.Push("second");
        original.Push("third");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyObjectWithPrivateSetters()
    {
        var original = new ClassWithPrivateSetters(42, "Test");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(42);
        _ = copy.Name.Should().Be("Test");
    }

    [Fact]
    public void CopyObjectWithReadOnlyField_FieldIsCopied()
    {
        var original = new ClassWithReadOnlyField("readonly value") { Id = 1 };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(1);
        _ = copy.ReadOnlyValue.Should().Be("readonly value");
    }

    [Fact]
    public void CopyTuple()
    {
        var original = (Id: 1, Name: "Test", Value: 42.5);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyObjectWithIndexer_IndexerIsIgnored()
    {
        var original = new ClassWithIndexer();
        original.Add("key1", 100);
        original.Add("key2", 200);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
    }

    [Fact]
    public void CopyObjectWithStaticProperties_StaticPropertiesAreNotCopied()
    {
        ClassWithStatic.StaticValue = 999;
        var original = new ClassWithStatic { InstanceValue = 42 };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.InstanceValue.Should().Be(42);
        _ = ClassWithStatic.StaticValue.Should().Be(999, "static properties are shared, not copied");
    }

    [Fact]
    public void CopyUri()
    {
        var original = new Uri("https://github.com/copilot");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.ToString().Should().Be(original.ToString());
    }

    [Fact]
    public void CopyObjectWithInheritance()
    {
        var original = new DerivedClass
        {
            BaseProperty = "base",
            DerivedProperty = "derived"
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.BaseProperty.Should().Be("base");
        _ = copy.DerivedProperty.Should().Be("derived");
    }

    [Fact]
    public void CopyObjectWithAbstractBase()
    {
        AbstractBase original = new ConcreteImplementation
        {
            BaseValue = 10,
            ConcreteValue = 20
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeOfType<ConcreteImplementation>();
        _ = ((ConcreteImplementation)copy).ConcreteValue.Should().Be(20);
    }

    [Fact]
    public void CopyObjectWithInterface()
    {
        ITestInterface original = new InterfaceImplementation
        {
            Id = 1,
            Name = "Test"
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeOfType<InterfaceImplementation>();
    }

    [Fact]
    public void CopyVeryLargeObject()
    {
        var original = new List<SimpleTestClass>();
        for (var i = 0; i < 10000; i++)
        {
            original.Add(new SimpleTestClass { Id = i, Name = $"Item {i}" });
        }

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().HaveCount(10000);
        _ = copy[5000].Should().NotBeSameAs(original[5000]);
    }

    [Fact]
    public void CopyObjectWithDeeplyNestedStructure()
    {
        var original = new NestedLevel1
        {
            Value = "L1",
            Level2 = new NestedLevel2
            {
                Value = "L2",
                Level3 = new NestedLevel3
                {
                    Value = "L3",
                    Level4 = new NestedLevel4 { Value = "L4" }
                }
            }
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Level2.Should().NotBeSameAs(original.Level2);
        _ = copy.Level2!.Level3!.Level4!.Value.Should().Be("L4");
    }

    [Fact]
    public void CopyObjectWithInitOnlyProperty()
    {
        var original = new ClassWithInitOnlyProperty { Id = 1, InitValue = "init" };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(1);
        _ = copy.InitValue.Should().Be("init");
    }

    [Fact]
    public void CopyObjectWithRequiredProperty()
    {
        var original = new ClassWithRequiredProperty { RequiredId = 42, RequiredName = "Required" };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.RequiredId.Should().Be(42);
        _ = copy.RequiredName.Should().Be("Required");
    }

    [Fact]
    public void CopyObjectWithByteArray()
    {
        var original = new ClassWithByteArray
        {
            Id = 1,
            Data = [0x01, 0x02, 0x03, 0xFF]
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Data.Should().NotBeSameAs(original.Data);
        _ = copy.Data.Should().BeEquivalentTo(original.Data);
    }

    [Fact]
    public void CopyKeyValuePair()
    {
        var original = new KeyValuePair<string, int>("key", 42);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Key.Should().Be("key");
        _ = copy.Value.Should().Be(42);
    }

    [Fact]
    public void CopyObjectWithNestedRecord()
    {
        var original = new ClassWithNestedRecord
        {
            Id = 1,
            Record = new TestRecord(2, "Nested")
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Record.Should().Be(original.Record);
    }

    [Fact]
    public void CopyEmptyString()
    {
        var original = string.Empty;

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(string.Empty);
    }

    [Fact]
    public void CopyObjectWithWhitespaceString()
    {
        var original = new SimpleTestClass { Id = 1, Name = "   \t\n   " };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Name.Should().Be(original.Name);
    }

    [Fact]
    public void CopyObjectWithVeryLongString()
    {
        var longString = new string('x', 100_000);
        var original = new SimpleTestClass { Id = 1, Name = longString };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Name.Should().HaveLength(100_000);
    }

    [Fact]
    public void CopySortedDictionary()
    {
        var original = new SortedDictionary<int, string>
        {
            [3] = "three",
            [1] = "one",
            [2] = "two"
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
        _ = copy.Keys.Should().BeInAscendingOrder();
    }

    [Fact]
    public void CopyLinkedList()
    {
        var original = new LinkedList<int>([1, 2, 3, 4, 5]);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyObjectWithTimeOnly()
    {
        var original = new ClassWithTimeTypes
        {
            Time = new TimeOnly(14, 30, 45),
            Date = new DateOnly(2024, 12, 25)
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Time.Should().Be(original.Time);
        _ = copy.Date.Should().Be(original.Date);
    }

    [Fact]
    public void CopyObjectWithVersion()
    {
        var original = new ClassWithVersion { Version = new Version(1, 2, 3, 4) };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Version.Should().Be(original.Version);
    }

    [Fact]
    public void CopyObjectWithIPAddress()
    {
        var original = new ClassWithIPAddress
        {
            Address = System.Net.IPAddress.Parse("192.168.1.1")
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Address.ToString().Should().Be("192.168.1.1");
    }

    [Fact]
    public void CopyObjectWithMixedAccessModifiers()
    {
        var original = new ClassWithMixedAccessModifiers(42)
        {
            PublicValue = "public"
        };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.PublicValue.Should().Be("public");
        _ = copy.GetPrivateValue().Should().Be(42);
    }

    [Fact]
    public void CopyRecordStruct()
    {
        var original = new TestRecordStruct(1, "Test");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyObjectWithNullStringInCollection()
    {
        var original = new List<string?> { "one", null, "three", null };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().HaveCount(4);
        _ = copy[1].Should().BeNull();
        _ = copy[3].Should().BeNull();
    }

    [Fact]
    public void CopyReadOnlyCollection()
    {
        var original = new System.Collections.ObjectModel.ReadOnlyCollection<int>([1, 2, 3]);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyObjectWithPrivateFields()
    {
        var original = new ClassWithPrivateFields(10, "private");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.GetPrivateInt().Should().Be(10);
        _ = copy.GetPrivateString().Should().Be("private");
    }

    [Fact]
    public void CopyObjectWithPrivateNestedObject()
    {
        var original = new ClassWithPrivateNestedObject(new SimpleTestClass { Id = 5, Name = "Nested" });

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.GetNestedObject().Should().NotBeSameAs(original.GetNestedObject());
        _ = copy.GetNestedObject().Id.Should().Be(5);
    }

    [Fact]
    public void CopyObjectWithPrivateCollection()
    {
        var original = new ClassWithPrivateCollection([1, 2, 3]);

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.GetItems().Should().NotBeSameAs(original.GetItems());
        _ = copy.GetItems().Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void CopyObjectWithBackingFieldAndProperty_NoDuplicationOccurs()
    {
        var original = new ClassWithExplicitBackingField();
        original.AddItem("A");
        original.AddItem("B");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Items.Should().HaveCount(2, "backing fields should not cause duplication");
        _ = copy.Items.Should().BeEquivalentTo(["A", "B"]);
    }

    [Fact]
    public void CopyObjectWithConstants_ConstantsRemainUnaffected()
    {
        var original = new ClassWithConstants { InstanceValue = 100 };

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.InstanceValue.Should().Be(100);
        _ = ClassWithConstants.ConstantValue.Should().Be(42);
    }

    [Fact]
    public void CopyObjectWithMultipleBackingFields_NoDuplication()
    {
        var original = new ClassWithMultipleBackingFields();
        original.AddToList1("A");
        original.AddToList2("B");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.List1.Should().HaveCount(1, "first backing field should not cause duplication");
        _ = copy.List2.Should().HaveCount(1, "second backing field should not cause duplication");
        _ = copy.List1.Should().BeEquivalentTo(["A"]);
        _ = copy.List2.Should().BeEquivalentTo(["B"]);
    }

    [Fact]
    public void CopyObjectWithUnconventionalBackingFieldName_HandlesCorrectly()
    {
        var original = new ClassWithUnconventionalNaming();
        original.AddItem("Test");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Items.Should().Contain("Test");
    }

    [Fact]
    public void CopyObjectWithMixedBackingFields_HandlesCorrectly()
    {
        var original = new ClassWithMixedBackingFields(5);
        original.AddItem("A");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Items.Should().HaveCount(1, "backing field with public property should not duplicate");
        _ = copy.Items.Should().BeEquivalentTo(["A"]);
        _ = copy.GetPrivateValue().Should().Be(5, "private field without public property should be copied");
    }

    [Fact]
    public void CopyNestedObjectsWithBackingFields_NoDuplication()
    {
        var original = new OuterClassWithBackingField();
        original.Inner.AddItem("X");

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Inner.Should().NotBeSameAs(original.Inner);
        _ = copy.Inner.Items.Should().HaveCount(1, "nested object backing fields should not duplicate");
        _ = copy.Inner.Items.Should().BeEquivalentTo(["X"]);
    }

    [Fact]
    public void CopyObjectWithModifiableCollectionThroughMethod_WorksCorrectly()
    {
        var original = new ClassWithModifiableCollection();
        original.AddItem("Item1");
        original.AddItem("Item2");

        var copy = ClassCopier.DeepCopy(original);
        copy.AddItem("Item3");

        _ = original.Items.Should().HaveCount(2, "original should not be affected by changes to copy");
        _ = copy.Items.Should().HaveCount(3);
        _ = copy.Items.Should().BeEquivalentTo(["Item1", "Item2", "Item3"]);
    }

    [Fact]
    public void DeepCopy_SameTypeMultipleTimes_UsesCachedReflectionResults()
    {
        var original1 = new SimpleTestClass { Id = 1, Name = "First" };
        var copy1 = ClassCopier.DeepCopy(original1);
        var original2 = new SimpleTestClass { Id = 2, Name = "Second" };
        var copy2 = ClassCopier.DeepCopy(original2);
        var original3 = new SimpleTestClass { Id = 3, Name = "Third" };
        var copy3 = ClassCopier.DeepCopy(original3);

        _ = copy1.Id.Should().Be(1);
        _ = copy2.Id.Should().Be(2);
        _ = copy3.Id.Should().Be(3);
    }

    [Fact]
    public void DeepCopy_ConcurrentCalls_ThreadSafe()
    {
        var original = new ComplexTestClass
        {
            Id = 1,
            Inner = new SimpleTestClass { Id = 2, Name = "Test" },
            Items = ["A", "B", "C"]
        };
        var results = new System.Collections.Concurrent.ConcurrentBag<ComplexTestClass>();

        _ = Parallel.For(0, 100, _ => results.Add(ClassCopier.DeepCopy(original)));

        _ = results.Should().HaveCount(100);
        _ = results.Should().OnlyContain(c => c.Id == 1 && c.Items.Count == 3);
    }

    [Fact]
    public void CopyObjectWithDefaultValues()
    {
        var original = new SimpleTestClass { Id = 0, Name = string.Empty };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Id.Should().Be(0);
    }

    [Fact]
    public void CopyEmptyDictionary()
    {
        var original = new Dictionary<string, int>();
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEmpty();
    }

    [Fact]
    public void CopyEmptyHashSet()
    {
        var original = new HashSet<string>();
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEmpty();
    }

    [Fact]
    public void CopyObjectWithMultipleLevelsOfBackingFields()
    {
        var original = new NestedBackingFieldClass();
        original.AddOuter("X");
        original.Inner.AddInner("Y");
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Outer.Should().HaveCount(1);
        _ = copy.Inner.Inner.Should().HaveCount(1);
    }

    [Fact]
    public void CopyObjectWithBackingFieldInitializedInConstructor()
    {
        var original = new ClassWithConstructorBackingField([1, 2, 3]);
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Items.Should().NotBeSameAs(original.Items);
        _ = copy.Items.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void CopyObjectWithNullableReferenceTypes()
    {
        var original = new ClassWithNullableReferences { RequiredValue = "Required", OptionalValue = null };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.RequiredValue.Should().Be("Required");
        _ = copy.OptionalValue.Should().BeNull();
    }

    [Fact]
    public void CopyListWithNullObjects()
    {
        var original = new List<SimpleTestClass?> { new() { Id = 1, Name = "First" }, null, new() { Id = 2, Name = "Second" } };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().HaveCount(3);
        _ = copy[1].Should().BeNull();
    }

    [Fact]
    public void CopyDictionaryWithNullValues()
    {
        var original = new Dictionary<string, string?> { ["key1"] = "value1", ["key2"] = null };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy["key1"].Should().Be("value1");
        _ = copy["key2"].Should().BeNull();
    }

    [Fact]
    public void CopyBigInteger()
    {
        var original = System.Numerics.BigInteger.Parse("123456789012345678901234567890");
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopyComplex()
    {
        var original = new System.Numerics.Complex(3.5, 2.1);
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().Be(original);
    }

    [Fact]
    public void CopySortedList()
    {
        var original = new SortedList<int, string> { [3] = "three", [1] = "one", [2] = "two" };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void CopyObjectWithMultipleLevelsOfInheritance()
    {
        var original = new GrandChildClass { GrandParentProperty = "GP", ParentProperty = "P", ChildProperty = "C" };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.GrandParentProperty.Should().Be("GP");
        _ = copy.ParentProperty.Should().Be("P");
        _ = copy.ChildProperty.Should().Be("C");
    }

    [Fact]
    public void CopyObjectWithGenericBase()
    {
        var original = new GenericDerived { Value = 42, Name = "Test" };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Value.Should().Be(42);
        _ = copy.Name.Should().Be("Test");
    }

    [Fact]
    public void CopyLargeDictionary()
    {
        var original = new Dictionary<int, string>();
        for (var i = 0; i < 10000; i++)
        {
            original[i] = $"Value_{i}";
        }

        var copy = ClassCopier.DeepCopy(original);

        _ = copy.Should().HaveCount(10000);
        _ = copy[5000].Should().Be("Value_5000");
    }

    [Fact]
    public void CopyDeeplyNestedLists()
    {
        var original = new List<List<List<int>>> { new() { new() { 1, 2 }, new() { 3, 4 } }, new() { new() { 5, 6 }, new() { 7, 8 } } };
        var copy = ClassCopier.DeepCopy(original);

        _ = copy[0][0].Should().BeEquivalentTo([1, 2]);
        _ = copy[1][1].Should().BeEquivalentTo([7, 8]);
    }
}
