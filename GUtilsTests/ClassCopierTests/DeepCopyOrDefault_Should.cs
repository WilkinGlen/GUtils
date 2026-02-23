namespace GUtilsTests.ClassCopierTests;

using System.Net;
using FluentAssertions;
using GUtils.ClassCopier;

public class DeepCopyOrDefault_Should
{
    [Fact]
    public void ReturnCopy_WhenSourceIsValid()
    {
        var original = new SimpleTestClass { Id = 1, Name = "Test" };
        var defaultValue = new SimpleTestClass { Id = -1, Name = "Default" };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().NotBeSameAs(original);
        _ = result.Should().NotBeSameAs(defaultValue);
        _ = result!.Id.Should().Be(1);
        _ = result.Name.Should().Be("Test");
    }

    [Fact]
    public void ReturnDefaultValue_WhenSourceIsNull()
    {
        SimpleTestClass? original = null;
        var defaultValue = new SimpleTestClass { Id = -1, Name = "Default" };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().BeSameAs(defaultValue);
        _ = result!.Id.Should().Be(-1);
        _ = result.Name.Should().Be("Default");
    }

    [Fact]
    public void ReturnCopy_ForComplexObject()
    {
        var original = new ComplexTestClass
        {
            Id = 1,
            Inner = new SimpleTestClass { Id = 2, Name = "Inner" },
            Items = ["A", "B"]
        };
        var defaultValue = new ComplexTestClass
        {
            Id = -1,
            Inner = new SimpleTestClass { Id = -2, Name = "DefaultInner" },
            Items = ["Default"]
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().NotBeSameAs(original);
        _ = result.Should().NotBeSameAs(defaultValue);
        _ = result!.Id.Should().Be(1);
        _ = result.Inner.Name.Should().Be("Inner");
    }

    [Fact]
    public void ReturnCopy_ForValueTypes()
    {
        var original = 42;
        var defaultValue = -1;

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().Be(42);
    }

    [Fact]
    public void ReturnCopy_ForStrings()
    {
        var original = "Original";
        var defaultValue = "Default";

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().Be("Original");
    }

    [Fact]
    public void ReturnCopy_ForCollections()
    {
        var original = new List<int> { 1, 2, 3 };
        var defaultValue = new List<int> { -1 };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().NotBeSameAs(original);
        _ = result.Should().NotBeSameAs(defaultValue);
        _ = result.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void ModifyingResultShouldNotAffectOriginalOrDefault()
    {
        var original = new SimpleTestClass { Id = 1, Name = "Original" };
        var defaultValue = new SimpleTestClass { Id = -1, Name = "Default" };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);
        result!.Id = 99;
        result.Name = "Modified";

        _ = original.Id.Should().Be(1);
        _ = original.Name.Should().Be("Original");
        _ = defaultValue.Id.Should().Be(-1);
        _ = defaultValue.Name.Should().Be("Default");
    }

    [Fact]
    public void ReturnNull_WhenSourceIsNull_AndDefaultIsNull()
    {
        SimpleTestClass? original = null;
        SimpleTestClass? defaultValue = null;

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().BeNull();
    }

    [Fact]
    public void ReturnDefault_WhenDefaultIsNull_ButNoExplicitDefault()
    {
        SimpleTestClass? original = null;

        var result = ClassCopier.DeepCopyOrDefault(original);

        _ = result.Should().BeNull();
    }

    [Fact]
    public void ReturnCopy_ForRecord()
    {
        var original = new TestRecord(42, "RecordTest");
        var defaultValue = new TestRecord(-1, "DefaultRecord");

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().Be(original);
        _ = result.Should().NotBeSameAs(defaultValue);
    }

    [Fact]
    public void ReturnCopy_ForRecordStruct()
    {
        var original = new TestRecordStruct(99, "StructTest");
        var defaultValue = new TestRecordStruct(-1, "DefaultStruct");

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Id.Should().Be(99);
        _ = result.Name.Should().Be("StructTest");
    }

    [Fact]
    public void ReturnCopy_ForStruct()
    {
        var original = new TestStruct { X = 10, Y = 20, Name = "TestStruct" };
        var defaultValue = new TestStruct { X = -1, Y = -1, Name = "DefaultStruct" };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.X.Should().Be(10);
        _ = result.Y.Should().Be(20);
        _ = result.Name.Should().Be("TestStruct");
    }

    [Fact]
    public void ReturnCopy_ForClassWithEnum()
    {
        var original = new ClassWithEnum { Id = 5, Status = TestEnum.ThirdValue };
        var defaultValue = new ClassWithEnum { Id = -1, Status = TestEnum.FirstValue };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.Id.Should().Be(5);
        _ = result.Status.Should().Be(TestEnum.ThirdValue);
    }

    [Fact]
    public void ReturnCopy_ForClassWithDateTime()
    {
        var now = DateTime.Now;
        var original = new ClassWithDateTime
        {
            CreatedDate = now,
            ModifiedDate = DateTimeOffset.UtcNow,
            Duration = TimeSpan.FromMinutes(30)
        };
        var defaultValue = new ClassWithDateTime
        {
            CreatedDate = DateTime.MinValue,
            ModifiedDate = DateTimeOffset.MinValue,
            Duration = TimeSpan.Zero
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.CreatedDate.Should().Be(now);
        _ = result.Duration.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public void ReturnCopy_ForClassWithNullableProperties()
    {
        var original = new ClassWithNullableProperties
        {
            NullableInt = 42,
            NullableString = "NotNull",
            NullableObject = new SimpleTestClass { Id = 1, Name = "Test" }
        };
        var defaultValue = new ClassWithNullableProperties
        {
            NullableInt = -1,
            NullableString = "DefaultString",
            NullableObject = null
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.NullableInt.Should().Be(42);
        _ = result.NullableString.Should().Be("NotNull");
        _ = result.NullableObject.Should().NotBeNull();
    }

    [Fact]
    public void ReturnCopy_ForClassWithPrivateSetters()
    {
        var original = new ClassWithPrivateSetters(100, "PrivateName");
        var defaultValue = new ClassWithPrivateSetters(-1, "DefaultName");

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.Id.Should().Be(100);
        _ = result.Name.Should().Be("PrivateName");
    }

    [Fact]
    public void ReturnCopy_ForClassWithPrivateFields()
    {
        var original = new ClassWithPrivateFields(123, "Secret");
        var defaultValue = new ClassWithPrivateFields(-1, "DefaultSecret");

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.GetPrivateInt().Should().Be(123);
        _ = result.GetPrivateString().Should().Be("Secret");
    }

    [Fact]
    public void ReturnCopy_ForIPAddress()
    {
        var original = new ClassWithIPAddress
        {
            Address = IPAddress.Parse("10.0.0.1")
        };
        var defaultValue = new ClassWithIPAddress
        {
            Address = IPAddress.Loopback
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.Address.Should().Be(original.Address);
    }

    [Fact]
    public void ReturnCopy_ForDerivedClass()
    {
        var original = new DerivedClass
        {
            BaseProperty = "BaseValue",
            DerivedProperty = "DerivedValue"
        };
        var defaultValue = new DerivedClass
        {
            BaseProperty = "DefaultBase",
            DerivedProperty = "DefaultDerived"
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.BaseProperty.Should().Be("BaseValue");
        _ = result.DerivedProperty.Should().Be("DerivedValue");
    }

    [Fact]
    public void ReturnCopy_ForNestedCollections()
    {
        var original = new ClassWithNestedCollections
        {
            Data = new Dictionary<string, List<int>>
            {
                ["key1"] = [1, 2, 3],
                ["key2"] = [4, 5]
            },
            JaggedArray = [[10, 20], [30]]
        };
        var defaultValue = new ClassWithNestedCollections
        {
            Data = [],
            JaggedArray = []
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.Data["key1"].Should().BeEquivalentTo([1, 2, 3]);
        _ = result.JaggedArray.Should().HaveCount(2);
    }

    [Fact]
    public void ReturnCopy_ForEmptyList()
    {
        var original = new List<int>();
        var defaultValue = new List<int> { -1, -2, -3 };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().BeEmpty();
        _ = result.Should().NotBeSameAs(original);
    }

    [Fact]
    public void ReturnCopy_ForClassWithCircularReference()
    {
        var parent = new NodeWithCircularRef { Id = 1 };
        var child = new NodeWithCircularRef { Id = 2 };
        parent.Child = child;
        child.Parent = parent;
        var defaultValue = new NodeWithCircularRef { Id = -1 };

        var result = ClassCopier.DeepCopyOrDefault(parent, defaultValue);

        _ = result.Should().NotBeNull();
        _ = result!.Id.Should().Be(1);
        _ = result.Child.Should().NotBeNull();
    }

    [Fact]
    public void ReturnCopy_ForDictionary()
    {
        var original = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3
        };
        var defaultValue = new Dictionary<string, int>
        {
            ["default"] = -1
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result.Should().NotBeSameAs(original);
        _ = result.Should().HaveCount(3);
        _ = result!["one"].Should().Be(1);
    }

    [Fact]
    public void ReturnCopy_ForByteArray()
    {
        var original = new ClassWithByteArray
        {
            Id = 1,
            Data = [0xAA, 0xBB, 0xCC]
        };
        var defaultValue = new ClassWithByteArray
        {
            Id = -1,
            Data = [0x00]
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.Data.Should().BeEquivalentTo([0xAA, 0xBB, 0xCC]);
        _ = result.Data.Should().NotBeSameAs(original.Data);
    }

    [Fact]
    public void ReturnCopy_ForInitOnlyProperty()
    {
        var original = new ClassWithInitOnlyProperty
        {
            Id = 5,
            InitValue = "InitOnly"
        };
        var defaultValue = new ClassWithInitOnlyProperty
        {
            Id = -1,
            InitValue = "DefaultInit"
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.Id.Should().Be(5);
        _ = result.InitValue.Should().Be("InitOnly");
    }

    [Fact]
    public void ReturnCopy_ForRequiredProperty()
    {
        var original = new ClassWithRequiredProperty
        {
            RequiredId = 999,
            RequiredName = "RequiredTest"
        };
        var defaultValue = new ClassWithRequiredProperty
        {
            RequiredId = -1,
            RequiredName = "DefaultRequired"
        };

        var result = ClassCopier.DeepCopyOrDefault(original, defaultValue);

        _ = result!.RequiredId.Should().Be(999);
        _ = result.RequiredName.Should().Be("RequiredTest");
    }
}
