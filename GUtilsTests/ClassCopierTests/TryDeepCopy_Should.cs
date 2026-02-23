namespace GUtilsTests.ClassCopierTests;

using System.Net;
using FluentAssertions;
using GUtils.ClassCopier;

public class TryDeepCopy_Should
{
    [Fact]
    public void ReturnTrueAndCopy_WhenSourceIsValid()
    {
        var original = new SimpleTestClass { Id = 1, Name = "Test" };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().NotBeNull();
        _ = copy.Should().NotBeSameAs(original);
        _ = copy!.Id.Should().Be(1);
        _ = copy.Name.Should().Be("Test");
    }

    [Fact]
    public void ReturnFalse_WhenSourceIsNull()
    {
        SimpleTestClass? original = null;

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeFalse();
        _ = copy.Should().BeNull();
    }

    [Fact]
    public void ReturnTrueAndCopy_ForComplexObject()
    {
        var original = new ComplexTestClass
        {
            Id = 1,
            Inner = new SimpleTestClass { Id = 2, Name = "Inner" },
            Items = ["A", "B", "C"]
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().NotBeNull();
        _ = copy!.Inner.Should().NotBeSameAs(original.Inner);
        _ = copy.Items.Should().BeEquivalentTo("A", "B", "C");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForValueTypes()
    {
        var original = 42;

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().Be(42);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForStrings()
    {
        var original = "Test String";

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().Be("Test String");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForCollections()
    {
        var original = new List<int> { 1, 2, 3, 4, 5 };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForDictionaries()
    {
        var original = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().NotBeSameAs(original);
        _ = copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForNestedObjects()
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

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().NotBeNull();
        _ = copy!.Level2!.Level3!.Level4!.Value.Should().Be("L4");
    }

    [Fact]
    public void ModifyingCopyShouldNotAffectOriginal()
    {
        var original = new SimpleTestClass { Id = 1, Name = "Original" };

        _ = ClassCopier.TryDeepCopy(original, out var copy);
        copy!.Id = 99;
        copy.Name = "Modified";

        _ = original.Id.Should().Be(1);
        _ = original.Name.Should().Be("Original");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForRecord()
    {
        var original = new TestRecord(42, "RecordTest");

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().Be(original);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForRecordStruct()
    {
        var original = new TestRecordStruct(99, "StructTest");

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Id.Should().Be(99);
        _ = copy.Name.Should().Be("StructTest");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForStruct()
    {
        var original = new TestStruct { X = 10, Y = 20, Name = "TestStruct" };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.X.Should().Be(10);
        _ = copy.Y.Should().Be(20);
        _ = copy.Name.Should().Be("TestStruct");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForEnum()
    {
        var original = TestEnum.SecondValue;

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithEnum()
    {
        var original = new ClassWithEnum { Id = 5, Status = TestEnum.ThirdValue };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Id.Should().Be(5);
        _ = copy.Status.Should().Be(TestEnum.ThirdValue);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithDateTime()
    {
        var now = DateTime.Now;
        var offset = DateTimeOffset.UtcNow;
        var original = new ClassWithDateTime
        {
            CreatedDate = now,
            ModifiedDate = offset,
            Duration = TimeSpan.FromHours(5)
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.CreatedDate.Should().Be(now);
        _ = copy.ModifiedDate.Should().Be(offset);
        _ = copy.Duration.Should().Be(TimeSpan.FromHours(5));
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithNullableProperties_WithValues()
    {
        var original = new ClassWithNullableProperties
        {
            NullableInt = 42,
            NullableString = "NotNull",
            NullableObject = new SimpleTestClass { Id = 1, Name = "Test" }
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.NullableInt.Should().Be(42);
        _ = copy.NullableString.Should().Be("NotNull");
        _ = copy.NullableObject.Should().NotBeNull();
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithNullableProperties_WithNulls()
    {
        var original = new ClassWithNullableProperties
        {
            NullableInt = null,
            NullableString = null,
            NullableObject = null
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.NullableInt.Should().BeNull();
        _ = copy.NullableString.Should().BeNull();
        _ = copy.NullableObject.Should().BeNull();
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithPrivateSetters()
    {
        var original = new ClassWithPrivateSetters(100, "PrivateName");

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Id.Should().Be(100);
        _ = copy.Name.Should().Be("PrivateName");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithPrivateFields()
    {
        var original = new ClassWithPrivateFields(123, "Secret");

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.GetPrivateInt().Should().Be(123);
        _ = copy.GetPrivateString().Should().Be("Secret");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForByteArray()
    {
        var original = new ClassWithByteArray
        {
            Id = 1,
            Data = [0x01, 0x02, 0x03, 0xFF]
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Data.Should().NotBeSameAs(original.Data);
        _ = copy.Data.Should().BeEquivalentTo(original.Data);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForIPAddress()
    {
        var original = new ClassWithIPAddress
        {
            Address = IPAddress.Parse("192.168.1.100")
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Address.Should().Be(original.Address);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForIPv6Address()
    {
        var original = new ClassWithIPAddress
        {
            Address = IPAddress.Parse("::1")
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Address.Should().Be(original.Address);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithCircularReference()
    {
        var parent = new NodeWithCircularRef { Id = 1 };
        var child = new NodeWithCircularRef { Id = 2 };
        parent.Child = child;
        child.Parent = parent;

        var result = ClassCopier.TryDeepCopy(parent, out var copy);

        _ = result.Should().BeTrue();
        _ = copy.Should().NotBeNull();
        _ = copy!.Id.Should().Be(1);
        _ = copy.Child.Should().NotBeNull();
        _ = copy.Child!.Id.Should().Be(2);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForDerivedClass()
    {
        var original = new DerivedClass
        {
            BaseProperty = "BaseValue",
            DerivedProperty = "DerivedValue"
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.BaseProperty.Should().Be("BaseValue");
        _ = copy.DerivedProperty.Should().Be("DerivedValue");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForConcreteImplementation()
    {
        var original = new ConcreteImplementation
        {
            BaseValue = 10,
            ConcreteValue = 20
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.BaseValue.Should().Be(10);
        _ = copy.ConcreteValue.Should().Be(20);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForNestedCollections()
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

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Data["first"].Should().BeEquivalentTo([1, 2, 3]);
        _ = copy.JaggedArray[1].Should().BeEquivalentTo([3, 4, 5]);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForEmptyCollections()
    {
        var original = new ComplexTestClass
        {
            Id = 1,
            Inner = new SimpleTestClass { Id = 2, Name = "Inner" },
            Items = []
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Items.Should().BeEmpty();
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithDelegate_IgnoringDelegate()
    {
        var original = new ClassWithDelegate
        {
            Id = 42,
            OnExecute = () => { },
            Calculator = x => x * 2
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Id.Should().Be(42);
        // Delegates should be ignored (null)
        _ = copy.OnExecute.Should().BeNull();
        _ = copy.Calculator.Should().BeNull();
    }

    [Fact]
    public void ReturnTrueAndCopy_ForClassWithEvents_IgnoringEvents()
    {
        var original = new ClassWithEvents { Id = 100 };
        original.ValueChanged += (_, _) => { };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Id.Should().Be(100);
    }

    [Fact]
    public void ReturnTrueAndCopy_ForInitOnlyProperty()
    {
        var original = new ClassWithInitOnlyProperty
        {
            Id = 5,
            InitValue = "InitOnly"
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.Id.Should().Be(5);
        _ = copy.InitValue.Should().Be("InitOnly");
    }

    [Fact]
    public void ReturnTrueAndCopy_ForRequiredProperty()
    {
        var original = new ClassWithRequiredProperty
        {
            RequiredId = 999,
            RequiredName = "RequiredTest"
        };

        var result = ClassCopier.TryDeepCopy(original, out var copy);

        _ = result.Should().BeTrue();
        _ = copy!.RequiredId.Should().Be(999);
        _ = copy.RequiredName.Should().Be("RequiredTest");
    }
}
