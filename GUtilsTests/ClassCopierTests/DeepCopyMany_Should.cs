namespace GUtilsTests.ClassCopierTests;

using System.Net;
using FluentAssertions;
using GUtils.ClassCopier;

public class DeepCopyMany_Should
{
    [Fact]
    public void CopyAllItems_FromList()
    {
        var originals = new List<SimpleTestClass>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" },
            new() { Id = 3, Name = "Third" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(3);
        _ = copies[0].Should().NotBeSameAs(originals[0]);
        _ = copies[1].Should().NotBeSameAs(originals[1]);
        _ = copies[2].Should().NotBeSameAs(originals[2]);
        _ = copies[0].Id.Should().Be(1);
        _ = copies[1].Id.Should().Be(2);
        _ = copies[2].Id.Should().Be(3);
    }

    [Fact]
    public void ThrowArgumentNullException_WhenSourcesIsNull()
    {
        IEnumerable<SimpleTestClass>? sources = null;

        var action = () => ClassCopier.DeepCopyMany(sources!);

        _ = action.Should().Throw<ArgumentNullException>()
            .WithParameterName("sources");
    }

    [Fact]
    public void ReturnEmptyEnumerable_WhenSourcesIsEmpty()
    {
        var originals = new List<SimpleTestClass>();

        var copies = ClassCopier.DeepCopyMany(originals);

        _ = copies.Should().BeEmpty();
    }

    [Fact]
    public void CopyAllItems_FromArray()
    {
        var originals = new[]
        {
            new SimpleTestClass { Id = 1, Name = "First" },
            new SimpleTestClass { Id = 2, Name = "Second" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Should().NotBeSameAs(originals[0]);
        _ = copies[1].Should().NotBeSameAs(originals[1]);
    }

    [Fact]
    public void CopyComplexObjects()
    {
        var originals = new List<ComplexTestClass>
        {
            new()
            {
                Id = 1,
                Inner = new SimpleTestClass { Id = 10, Name = "Inner1" },
                Items = ["A", "B"]
            },
            new()
            {
                Id = 2,
                Inner = new SimpleTestClass { Id = 20, Name = "Inner2" },
                Items = ["C", "D"]
            }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Inner.Should().NotBeSameAs(originals[0].Inner);
        _ = copies[1].Inner.Should().NotBeSameAs(originals[1].Inner);
        _ = copies[0].Items.Should().BeEquivalentTo("A", "B");
        _ = copies[1].Items.Should().BeEquivalentTo("C", "D");
    }

    [Fact]
    public void RespectMaxDegreeOfParallelism()
    {
        var originals = Enumerable.Range(1, 100)
            .Select(i => new SimpleTestClass { Id = i, Name = $"Item{i}" })
            .ToList();

        var copies = ClassCopier.DeepCopyMany(originals, maxDegreeOfParallelism: 2).ToList();

        _ = copies.Should().HaveCount(100);
        // With parallelism, order might not be preserved, so check all values are present
        _ = copies.Select(c => c.Id).Should().BeEquivalentTo(Enumerable.Range(1, 100));
    }

    [Fact]
    public void ModifyingCopiesShouldNotAffectOriginals()
    {
        var originals = new List<SimpleTestClass>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();
        copies[0].Id = 99;
        copies[0].Name = "Modified";

        _ = originals[0].Id.Should().Be(1);
        _ = originals[0].Name.Should().Be("First");
    }

    [Fact]
    public void CopyLargeNumberOfItems()
    {
        var originals = Enumerable.Range(1, 1000)
            .Select(i => new SimpleTestClass { Id = i, Name = $"Item{i}" })
            .ToList();

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(1000);
        _ = copies[500].Id.Should().Be(501);
        _ = copies[500].Name.Should().Be("Item501");
    }

    [Fact]
    public void CopyValueTypes()
    {
        var originals = new List<int> { 1, 2, 3, 4, 5 };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().BeEquivalentTo([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void CopyStrings()
    {
        var originals = new List<string> { "One", "Two", "Three" };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().BeEquivalentTo("One", "Two", "Three");
    }

    [Fact]
    public void CopyRecords()
    {
        var originals = new List<TestRecord>
        {
            new(1, "First"),
            new(2, "Second")
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Should().Be(originals[0]);
        _ = copies[1].Should().Be(originals[1]);
    }

    [Fact]
    public void CopyFromEnumerable()
    {
        var originals = Enumerable.Range(1, 5)
            .Select(i => new SimpleTestClass { Id = i, Name = $"Item{i}" });

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(5);
        for (var i = 0; i < 5; i++)
        {
            _ = copies[i].Id.Should().Be(i + 1);
        }
    }

    [Fact]
    public void CopyRecordStructs()
    {
        var originals = new List<TestRecordStruct>
        {
            new(1, "First"),
            new(2, "Second"),
            new(3, "Third")
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(3);
        _ = copies[0].Id.Should().Be(1);
        _ = copies[1].Id.Should().Be(2);
        _ = copies[2].Id.Should().Be(3);
    }

    [Fact]
    public void CopyStructs()
    {
        var originals = new List<TestStruct>
        {
            new() { X = 10, Y = 20, Name = "Struct1" },
            new() { X = 30, Y = 40, Name = "Struct2" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].X.Should().Be(10);
        _ = copies[0].Y.Should().Be(20);
        _ = copies[1].X.Should().Be(30);
        _ = copies[1].Y.Should().Be(40);
    }

    [Fact]
    public void CopyClassesWithEnums()
    {
        var originals = new List<ClassWithEnum>
        {
            new() { Id = 1, Status = TestEnum.FirstValue },
            new() { Id = 2, Status = TestEnum.SecondValue },
            new() { Id = 3, Status = TestEnum.ThirdValue }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(3);
        _ = copies[0].Status.Should().Be(TestEnum.FirstValue);
        _ = copies[1].Status.Should().Be(TestEnum.SecondValue);
        _ = copies[2].Status.Should().Be(TestEnum.ThirdValue);
    }

    [Fact]
    public void CopyClassesWithDateTime()
    {
        var now = DateTime.Now;
        var originals = new List<ClassWithDateTime>
        {
            new() { CreatedDate = now, ModifiedDate = DateTimeOffset.UtcNow, Duration = TimeSpan.FromHours(1) },
            new() { CreatedDate = now.AddDays(1), ModifiedDate = DateTimeOffset.UtcNow.AddHours(1), Duration = TimeSpan.FromMinutes(30) }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].CreatedDate.Should().Be(now);
        _ = copies[0].Duration.Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void CopyClassesWithIPAddress()
    {
        var originals = new List<ClassWithIPAddress>
        {
            new() { Address = IPAddress.Parse("192.168.1.1") },
            new() { Address = IPAddress.Parse("10.0.0.1") },
            new() { Address = IPAddress.IPv6Loopback }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(3);
        _ = copies[0].Address.Should().Be(IPAddress.Parse("192.168.1.1"));
        _ = copies[1].Address.Should().Be(IPAddress.Parse("10.0.0.1"));
        _ = copies[2].Address.Should().Be(IPAddress.IPv6Loopback);
    }

    [Fact]
    public void CopyClassesWithPrivateFields()
    {
        var originals = new List<ClassWithPrivateFields>
        {
            new(100, "Secret1"),
            new(200, "Secret2")
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].GetPrivateInt().Should().Be(100);
        _ = copies[0].GetPrivateString().Should().Be("Secret1");
        _ = copies[1].GetPrivateInt().Should().Be(200);
        _ = copies[1].GetPrivateString().Should().Be("Secret2");
    }

    [Fact]
    public void CopyClassesWithNullableProperties()
    {
        var originals = new List<ClassWithNullableProperties>
        {
            new() { NullableInt = 42, NullableString = "NotNull", NullableObject = new SimpleTestClass { Id = 1, Name = "Test" } },
            new() { NullableInt = null, NullableString = null, NullableObject = null }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].NullableInt.Should().Be(42);
        _ = copies[0].NullableString.Should().Be("NotNull");
        _ = copies[1].NullableInt.Should().BeNull();
        _ = copies[1].NullableString.Should().BeNull();
    }

    [Fact]
    public void CopyDerivedClasses()
    {
        var originals = new List<DerivedClass>
        {
            new() { BaseProperty = "Base1", DerivedProperty = "Derived1" },
            new() { BaseProperty = "Base2", DerivedProperty = "Derived2" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].BaseProperty.Should().Be("Base1");
        _ = copies[0].DerivedProperty.Should().Be("Derived1");
        _ = copies[1].BaseProperty.Should().Be("Base2");
        _ = copies[1].DerivedProperty.Should().Be("Derived2");
    }

    [Fact]
    public void CopyClassesWithNestedCollections()
    {
        var originals = new List<ClassWithNestedCollections>
        {
            new()
            {
                Data = new Dictionary<string, List<int>> { ["key1"] = [1, 2, 3] },
                JaggedArray = [[10, 20], [30]]
            },
            new()
            {
                Data = new Dictionary<string, List<int>> { ["key2"] = [4, 5, 6] },
                JaggedArray = [[40], [50, 60]]
            }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Data["key1"].Should().BeEquivalentTo([1, 2, 3]);
        _ = copies[1].Data["key2"].Should().BeEquivalentTo([4, 5, 6]);
    }

    [Fact]
    public void CopyDictionaries()
    {
        var originals = new List<Dictionary<string, int>>
        {
            new() { ["one"] = 1, ["two"] = 2 },
            new() { ["three"] = 3, ["four"] = 4 }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Should().NotBeSameAs(originals[0]);
        _ = copies[0]["one"].Should().Be(1);
        _ = copies[1]["three"].Should().Be(3);
    }

    [Fact]
    public void CopyClassesWithByteArrays()
    {
        var originals = new List<ClassWithByteArray>
        {
            new() { Id = 1, Data = [0x01, 0x02, 0x03] },
            new() { Id = 2, Data = [0xAA, 0xBB, 0xCC] }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Data.Should().BeEquivalentTo([0x01, 0x02, 0x03]);
        _ = copies[1].Data.Should().BeEquivalentTo([0xAA, 0xBB, 0xCC]);
        _ = copies[0].Data.Should().NotBeSameAs(originals[0].Data);
    }

    [Fact]
    public void CopyClassesWithDelegates_IgnoringDelegates()
    {
        var originals = new List<ClassWithDelegate>
        {
            new() { Id = 1, OnExecute = () => { }, Calculator = x => x * 2 },
            new() { Id = 2, OnExecute = () => { }, Calculator = x => x + 10 }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].Id.Should().Be(1);
        _ = copies[0].OnExecute.Should().BeNull();
        _ = copies[0].Calculator.Should().BeNull();
        _ = copies[1].Id.Should().Be(2);
    }

    [Fact]
    public void UseParallelProcessing_WhenMaxDegreeGreaterThanOne()
    {
        var originals = Enumerable.Range(1, 50)
            .Select(i => new SimpleTestClass { Id = i, Name = $"Item{i}" })
            .ToList();

        var copies = ClassCopier.DeepCopyMany(originals, maxDegreeOfParallelism: 4).ToList();

        _ = copies.Should().HaveCount(50);
        _ = copies.Select(c => c.Id).Should().BeEquivalentTo(Enumerable.Range(1, 50));
    }

    [Fact]
    public void UseSequentialProcessing_WhenMaxDegreeIsOne()
    {
        var originals = Enumerable.Range(1, 10)
            .Select(i => new SimpleTestClass { Id = i, Name = $"Item{i}" })
            .ToList();

        var copies = ClassCopier.DeepCopyMany(originals, maxDegreeOfParallelism: 1).ToList();

        _ = copies.Should().HaveCount(10);
        // With sequential processing, order is preserved
        for (var i = 0; i < 10; i++)
        {
            _ = copies[i].Id.Should().Be(i + 1);
        }
    }

    [Fact]
    public void UseSequentialProcessing_WhenMaxDegreeIsZeroOrNegative()
    {
        var originals = Enumerable.Range(1, 5)
            .Select(i => new SimpleTestClass { Id = i, Name = $"Item{i}" })
            .ToList();

        var copies = ClassCopier.DeepCopyMany(originals, maxDegreeOfParallelism: 0).ToList();

        _ = copies.Should().HaveCount(5);
        for (var i = 0; i < 5; i++)
        {
            _ = copies[i].Id.Should().Be(i + 1);
        }
    }

    [Fact]
    public void CopyInitOnlyProperties()
    {
        var originals = new List<ClassWithInitOnlyProperty>
        {
            new() { Id = 1, InitValue = "Init1" },
            new() { Id = 2, InitValue = "Init2" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].InitValue.Should().Be("Init1");
        _ = copies[1].InitValue.Should().Be("Init2");
    }

    [Fact]
    public void CopyRequiredProperties()
    {
        var originals = new List<ClassWithRequiredProperty>
        {
            new() { RequiredId = 100, RequiredName = "Required1" },
            new() { RequiredId = 200, RequiredName = "Required2" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].RequiredId.Should().Be(100);
        _ = copies[0].RequiredName.Should().Be("Required1");
        _ = copies[1].RequiredId.Should().Be(200);
        _ = copies[1].RequiredName.Should().Be("Required2");
    }

    [Fact]
    public void HandleSingleItemCollection()
    {
        var originals = new List<SimpleTestClass>
        {
            new() { Id = 42, Name = "SingleItem" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(1);
        _ = copies[0].Id.Should().Be(42);
        _ = copies[0].Name.Should().Be("SingleItem");
        _ = copies[0].Should().NotBeSameAs(originals[0]);
    }

    [Fact]
    public void CopyMixedInheritanceHierarchy()
    {
        var originals = new List<BaseClass>
        {
            new() { BaseProperty = "JustBase" },
            new DerivedClass { BaseProperty = "FromDerived", DerivedProperty = "DerivedProp" }
        };

        var copies = ClassCopier.DeepCopyMany(originals).ToList();

        _ = copies.Should().HaveCount(2);
        _ = copies[0].BaseProperty.Should().Be("JustBase");
        var derivedCopy = copies[1] as DerivedClass;
        _ = derivedCopy.Should().NotBeNull();
        _ = derivedCopy!.DerivedProperty.Should().Be("DerivedProp");
    }
}
