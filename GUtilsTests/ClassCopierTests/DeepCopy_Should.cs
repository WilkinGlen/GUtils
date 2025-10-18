namespace GUtilsTests.PocoCopierTests;
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

    private class SimpleTestClass
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    private class ComplexTestClass
    {
        public int Id { get; set; }
        public required SimpleTestClass Inner { get; set; }
        public required List<string> Items { get; set; }
    }

    private class ClassWithNestedCollections
    {
        public required Dictionary<string, List<int>> Data { get; set; }
        public required int[][] JaggedArray { get; set; }
    }

    private class ClassWithDateTime
    {
        public DateTime CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public TimeSpan Duration { get; set; }
    }

    private class ClassWithNullableProperties
    {
        public int? NullableInt { get; set; }
        public string? NullableString { get; set; }
        public SimpleTestClass? NullableObject { get; set; }
    }

    private class ClassWithMethods
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

    private class ClassWithEvents
    {
        public int Id { get; set; }
        public event EventHandler? ValueChanged;
        public event EventHandler<string>? NameChanged;

        public void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseNameChanged(string newName)
        {
            NameChanged?.Invoke(this, newName);
        }
    }

    private class ClassWithDelegate
    {
        public int Id { get; set; }
        public Action? OnExecute { get; set; }
        public Func<int, int>? Calculator { get; set; }
    }

    private record TestRecord(int Id, string Name);

    private class NodeWithCircularRef
    {
        public int Id { get; set; }
        public NodeWithCircularRef? Parent { get; set; }
        public NodeWithCircularRef? Child { get; set; }
    }

    private enum TestEnum
    {
        FirstValue,
        SecondValue,
        ThirdValue
    }

    private class ClassWithEnum
    {
        public int Id { get; set; }
        public TestEnum Status { get; set; }
    }

    private class ClassWithPrivateSetters(int id, string name)
    {
        public int Id { get; private set; } = id;
        public string Name { get; private set; } = name;
    }

    private class ClassWithReadOnlyField(string readOnlyValue)
    {
        public readonly string? ReadOnlyValue = readOnlyValue;
        public int Id { get; set; }
    }

    private class ClassWithIndexer
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

    private class ClassWithStatic
    {
        public static int StaticValue { get; set; }
        public int InstanceValue { get; set; }
    }

    private class BaseClass
    {
        public string? BaseProperty { get; set; }
    }

    private class DerivedClass : BaseClass
    {
        public string? DerivedProperty { get; set; }
    }

    private abstract class AbstractBase
    {
        public int BaseValue { get; set; }
    }

    private class ConcreteImplementation : AbstractBase
    {
        public int ConcreteValue { get; set; }
    }

    private interface ITestInterface
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    private class InterfaceImplementation : ITestInterface
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    private class NestedLevel1
    {
        public string? Value { get; set; }
        public NestedLevel2? Level2 { get; set; }
    }

    private class NestedLevel2
    {
        public string? Value { get; set; }
        public NestedLevel3? Level3 { get; set; }
    }

    private class NestedLevel3
    {
        public string? Value { get; set; }
        public NestedLevel4? Level4 { get; set; }
    }

    private class NestedLevel4
    {
        public string? Value { get; set; }
    }

    private class ClassWithInitOnlyProperty
    {
        public int Id { get; set; }
        public string? InitValue { get; init; }
    }

    private class ClassWithRequiredProperty
    {
        public required int RequiredId { get; set; }
        public required string RequiredName { get; set; }
    }

    private class ClassWithByteArray
    {
        public int Id { get; set; }
        public required byte[] Data { get; set; }
    }

    private class ClassWithNestedRecord
    {
        public int Id { get; set; }
        public required TestRecord Record { get; set; }
    }

    private class ClassWithTimeTypes
    {
        public TimeOnly Time { get; set; }
        public DateOnly Date { get; set; }
    }

    private class ClassWithVersion
    {
        public required Version Version { get; set; }
    }

    private class ClassWithIPAddress
    {
        public required System.Net.IPAddress Address { get; set; }
    }

    private class ClassWithMixedAccessModifiers(int privateValue)
    {
        private readonly int _privateValue = privateValue;
        public string? PublicValue { get; set; }

        public int GetPrivateValue()
        {
            return this._privateValue;
        }
    }

    private record struct TestRecordStruct(int Id, string Name);

    private class ClassWithPrivateFields(int privateInt, string privateString)
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

    private class ClassWithPrivateNestedObject(SimpleTestClass nested)
    {
        private readonly SimpleTestClass _nested = nested;

        public SimpleTestClass GetNestedObject()
        {
            return this._nested;
        }
    }

    private class ClassWithPrivateCollection(List<int> items)
    {
        private readonly List<int> _items = items;

        public List<int> GetItems()
        {
            return this._items;
        }
    }

    private class ClassWithExplicitBackingField
    {
        public List<string> Items { get; } = [];

        public void AddItem(string item)
        {
            this.Items.Add(item);
        }
    }

    private struct TestStruct
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string? Name { get; set; }
    }

    private class ClassWithNestedPrivateClass
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

    private class ClassWithConstants
    {
        public const int ConstantValue = 42;
        public int InstanceValue { get; set; }
    }

    private class ClassWithMultipleBackingFields
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

    private class ClassWithUnconventionalNaming
    {
        public List<string> Items { get; } = [];

        public void AddItem(string item)
        {
            this.Items.Add(item);
        }
    }

    private class ClassWithMixedBackingFields(int value)
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

    private class OuterClassWithBackingField
    {
        public ClassWithExplicitBackingField Inner { get; set; } = new();
    }

    private class ClassWithModifiableCollection
    {
        private readonly List<string> _items = [];
        public IReadOnlyList<string> Items => this._items;

        public void AddItem(string item)
        {
            this._items.Add(item);
        }
    }

    private class ClassWithTransformingProperty
    {
        private int _value;
        public int DoubledValue => this._value * 2;

        public void SetValue(int value)
        {
            this._value = value;
        }
    }

    private class ClassWithNullableBackingField
    {
        public List<string>? Items { get; } = null;
    }
}
