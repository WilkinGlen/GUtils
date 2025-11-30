namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class Clear_Should
{
    [Fact]
    public void ReturnSameBuilderInstance()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var result = builder.Clear();

        _ = result.Should().BeSameAs(builder);
    }

    [Fact]
    public void ResetBuilderToEmptyState()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title")
            .WithTag("Tag", "Value");

        _ = builder.Clear();

        var actual = builder.Build();
        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void AllowChainingAfterClear()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Old Title")
            .Clear()
            .WithTitle("New Title")
            .Build();

        _ = actual.Should().Be($"## New Title{Environment.NewLine}");
        _ = actual.Should().NotContain("Old Title");
    }

    [Fact]
    public void ClearEmptyBuilderShouldSucceed()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        
        _ = builder.Clear();
        
        var actual = builder.Build();
        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void AllowMultipleClearCalls()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title1")
            .Clear()
            .WithTitle("Title2")
            .Clear()
            .WithTitle("Title3")
            .Clear();

        var actual = builder.Build();
        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void ReuseBuilderAfterClear()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        // First use
        _ = builder
            .WithTitle("First Title")
            .WithTag("Author", "Glen Wilkin");
        var first = builder.Build();

        // Clear and reuse
        _ = builder
            .Clear()
            .WithTitle("Second Title")
            .WithTag("Version", "2.0");
        var second = builder.Build();

        // Verify first result
        _ = first.Should().Contain("First Title");
        _ = first.Should().Contain("Glen Wilkin");

        // Verify second result doesn't contain first data
        _ = second.Should().NotContain("First Title");
        _ = second.Should().NotContain("Glen Wilkin");
        _ = second.Should().Contain("Second Title");
        _ = second.Should().Contain("2.0");
    }

    [Fact]
    public void ClearAfterBuild()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title")
            .WithTag("Tag", "Value");

        var firstBuild = builder.Build();
        _ = builder.Clear();
        var secondBuild = builder.Build();

        _ = firstBuild.Should().Contain("Title");
        _ = secondBuild.Should().BeEmpty();
    }

    [Fact]
    public void HandleLargeDataBeforeClear()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        for (var i = 0; i < 1000; i++)
        {
            _ = builder.WithTitle($"Title {i}");
            _ = builder.WithTag($"Tag{i}", $"Value{i}");
        }

        _ = builder.Clear();
        var actual = builder.Build();

        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void SupportComplexWorkflow()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        // Build first description
        _ = builder
            .WithTitle("API v1")
            .WithTag("Version", "1.0");
        var v1 = builder.Build();

        // Clear and build second description
        _ = builder
            .Clear()
            .WithTitle("API v2")
            .WithTag("Version", "2.0")
            .WithTag("Breaking", "Yes");
        var v2 = builder.Build();

        // Clear and build third description
        _ = builder
            .Clear()
            .WithTitle("API v3")
            .WithTag("Version", "3.0");
        var v3 = builder.Build();

        // Verify independence
        _ = v1.Should().Contain("v1");
        _ = v1.Should().NotContain("v2");
        _ = v1.Should().NotContain("v3");

        _ = v2.Should().Contain("v2");
        _ = v2.Should().Contain("Breaking");
        _ = v2.Should().NotContain("v1");
        _ = v2.Should().NotContain("v3");

        _ = v3.Should().Contain("v3");
        _ = v3.Should().NotContain("v1");
        _ = v3.Should().NotContain("v2");
        _ = v3.Should().NotContain("Breaking");
    }

    [Fact]
    public void BeThreadSafe()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        _ = Parallel.For(0, 100, i => _ = builder
                .WithTitle($"Title {i}")
                .WithTag($"Tag{i}", $"Value{i}")
                .Clear());

        var result = builder.Build();
        _ = result.Should().BeEmpty();
    }

    [Fact]
    public void ClearBetweenConcurrentOperations()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var results = new System.Collections.Concurrent.ConcurrentBag<string>();

        _ = Parallel.For(0, 50, i =>
        {
            _ = builder.WithTitle($"Title {i}");
            var beforeClear = builder.Build();
            results.Add(beforeClear);
            
            _ = builder.Clear();
            var afterClear = builder.Build();
            results.Add(afterClear);
        });

        // At least some results should be empty (after Clear)
        _ = results.Should().Contain(string.Empty);
    }

    [Fact]
    public void AllowImmediateChaining()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .Clear()
            .WithTitle("Title")
            .Build();

        _ = actual.Should().Be($"## Title{Environment.NewLine}");
    }

    [Fact]
    public void PreserveBuilderBehaviorAfterClear()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        // Use builder normally
        _ = builder
            .WithTitle("Title1")
            .WithTag("Tag1", "Value1");
        
        // Clear
        _ = builder.Clear();

        // Verify all methods still work
        _ = builder
            .WithTitle("Title2")
            .WithTag("Tag2", "Value2");

        var result = builder.Build();
        _ = result.Should().Contain("Title2");
        _ = result.Should().Contain("Tag2");
        _ = result.Should().NotContain("Title1");
        _ = result.Should().NotContain("Tag1");
    }

    [Fact]
    public void ClearDoesNotAffectPreviouslyBuiltStrings()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title")
            .WithTag("Tag", "Value");

        var beforeClear = builder.Build();
        _ = builder.Clear();
        var afterClear = builder.Build();

        // Previous build result should be unchanged
        _ = beforeClear.Should().Contain("Title");
        _ = beforeClear.Should().Contain("Tag");

        // New build should be empty
        _ = afterClear.Should().BeEmpty();

        // Original should still be intact
        _ = beforeClear.Should().Contain("Title");
    }

    [Fact]
    public void SupportMultipleBuildsWithClearInBetween()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var results = new List<string>();

        for (var i = 0; i < 10; i++)
        {
            _ = builder.WithTitle($"Title {i}");
            results.Add(builder.Build());
            _ = builder.Clear();
        }

        _ = results.Should().HaveCount(10);
        for (var i = 0; i < results.Count; i++)
        {
            _ = results[i].Should().Contain($"Title {i}");
            
            // Each result should only contain its own title
            for (var j = 0; j < results.Count; j++)
            {
                if (i != j)
                {
                    _ = results[i].Should().NotContain($"Title {j}");
                }
            }
        }
    }
}
