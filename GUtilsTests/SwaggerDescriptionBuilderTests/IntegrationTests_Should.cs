namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class IntegrationTests_Should
{
    [Fact]
    public void GenerateCompleteApiDescription()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("GUtils ClassCopier API")
            .WithTag("Author", "Glen Wilkin")
            .WithTag("Version", "1.0.0")
            .WithTag("License", "MIT")
            .WithTag("GitHub", "https://github.com/WilkinGlen/GUtils")
            .Build();

        _ = actual.Should().Contain("## GUtils ClassCopier API");
        _ = actual.Should().Contain("- Author: Glen Wilkin");
        _ = actual.Should().Contain("- Version: 1.0.0");
        _ = actual.Should().Contain("- License: MIT");
        _ = actual.Should().Contain("- GitHub: https://github.com/WilkinGlen/GUtils");
    }

    [Fact]
    public void GenerateMultipleSectionDescription()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Overview")
            .WithTag("Description", "Main API")
            .WithTitle("Authentication")
            .WithTag("Type", "Bearer Token")
            .WithTitle("Contact")
            .WithTag("Email", "contact@example.com")
            .Build();

        _ = actual.Should().Contain("## Overview");
        _ = actual.Should().Contain("## Authentication");
        _ = actual.Should().Contain("## Contact");
        _ = actual.IndexOf("Overview", StringComparison.Ordinal)
            .Should().BeLessThan(actual.IndexOf("Authentication", StringComparison.Ordinal));
        _ = actual.IndexOf("Authentication", StringComparison.Ordinal)
            .Should().BeLessThan(actual.IndexOf("Contact", StringComparison.Ordinal));
    }

    [Fact]
    public void GenerateDescriptionWithOnlyTitles()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title 1")
            .WithTitle("Title 2")
            .WithTitle("Title 3")
            .Build();

        _ = actual.Should().NotContain("- ");
        _ = actual.Should().Contain("## Title 1");
        _ = actual.Should().Contain("## Title 2");
        _ = actual.Should().Contain("## Title 3");
    }

    [Fact]
    public void GenerateDescriptionWithOnlyTags()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag1", "Value1")
            .WithTag("Tag2", "Value2")
            .WithTag("Tag3", "Value3")
            .Build();

        _ = actual.Should().NotContain("##");
        _ = actual.Should().Contain("- Tag1: Value1");
        _ = actual.Should().Contain("- Tag2: Value2");
        _ = actual.Should().Contain("- Tag3: Value3");
    }

    [Fact]
    public void GenerateLargeDescription()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        for (var i = 0; i < 50; i++)
        {
            _ = builder.WithTitle($"Section {i}");
            for (var j = 0; j < 5; j++)
            {
                _ = builder.WithTag($"Tag{i}-{j}", $"Value{i}-{j}");
            }
        }

        var actual = builder.Build();

        _ = actual.Should().Contain("Section 0");
        _ = actual.Should().Contain("Section 49");
        _ = actual.Should().Contain("Tag0-0: Value0-0");
        _ = actual.Should().Contain("Tag49-4: Value49-4");
    }

    [Fact]
    public void GenerateMarkdownCompatibleOutput()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("API Documentation")
            .WithTag("Author", "**Glen Wilkin**")
            .WithTag("Link", "[GitHub](https://github.com)")
            .Build();

        _ = actual.Should().MatchRegex(@"## API Documentation\r?\n");
        _ = actual.Should().Contain("**Glen Wilkin**");
        _ = actual.Should().Contain("[GitHub](https://github.com)");
    }

    [Fact]
    public void HandleRealWorldScenario_DocumentationHeader()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Welcome to the API")
            .WithTag("Base URL", "https://api.example.com/v1")
            .WithTag("Authentication", "Bearer token required")
            .WithTag("Rate Limit", "1000 requests per hour")
            .WithTitle("Support")
            .WithTag("Email", "support@example.com")
            .WithTag("Documentation", "https://docs.example.com")
            .Build();

        _ = actual.Should().NotBeNullOrWhiteSpace();
        _ = actual.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Should().HaveCount(7);
    }

    [Fact]
    public void HandleRealWorldScenario_VersionHistory()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Version History")
            .WithTag("v1.0.0", "Initial release - 2024-01-01")
            .WithTag("v1.1.0", "Added new features - 2024-02-01")
            .WithTag("v2.0.0", "Breaking changes - 2024-03-01")
            .Build();

        _ = actual.Should().Contain("## Version History");
        _ = actual.Should().Contain("v1.0.0");
        _ = actual.Should().Contain("v1.1.0");
        _ = actual.Should().Contain("v2.0.0");
    }

    [Fact]
    public void BeThreadSafe_MultipleBuilders()
    {
        var builders = Enumerable.Range(0, 100)
            .Select(_ => SwaggerDescriptionBuilder.Create())
            .ToList();

        _ = Parallel.ForEach(builders, (builder, state, index) => builder
                .WithTitle($"Title {index}")
                .WithTag($"Tag {index}", $"Value {index}"));

        var results = builders.Select(b => b.Build()).ToList();

        for (var i = 0; i < results.Count; i++)
        {
            _ = results[i].Should().Contain($"Title {i}");
            _ = results[i].Should().Contain($"Tag {i}");
            _ = results[i].Should().Contain($"Value {i}");
        }
    }

    [Fact]
    public void ProduceConsistentOutput()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title")
            .WithTag("Tag", "Value");

        var results = Enumerable.Range(0, 100)
            .Select(_ => builder.Build())
            .Distinct()
            .ToList();

        _ = results.Should().HaveCount(1);
    }

    [Fact]
    public void HandleEdgeCase_AlternatingTitlesAndTags()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("T1")
            .WithTag("Tag1", "V1")
            .WithTitle("T2")
            .WithTag("Tag2", "V2")
            .WithTitle("T3")
            .WithTag("Tag3", "V3")
            .Build();

        var lines = actual.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        _ = lines.Should().HaveCount(6);
        _ = lines[0].Should().StartWith("## T1");
        _ = lines[1].Should().StartWith("- Tag1");
        _ = lines[2].Should().StartWith("## T2");
        _ = lines[3].Should().StartWith("- Tag2");
        _ = lines[4].Should().StartWith("## T3");
        _ = lines[5].Should().StartWith("- Tag3");
    }

    [Fact]
    public void GenerateValidSwaggerDescription_MinimalExample()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("API")
            .Build();

        _ = actual.Should().Be($"## API{Environment.NewLine}");
    }

    [Fact]
    public void GenerateValidSwaggerDescription_ComplexExample()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("ClassCopier API Documentation")
            .WithTag("Purpose", "Deep copy objects in .NET")
            .WithTag("Author", "Glen Wilkin")
            .WithTitle("Getting Started")
            .WithTag("Installation", "dotnet add package GUtils")
            .WithTag("Usage", "ClassCopier.DeepCopy(myObject)")
            .WithTitle("Additional Resources")
            .WithTag("GitHub", "https://github.com/WilkinGlen/GUtils")
            .WithTag("License", "MIT")
            .Build();

        _ = actual.Should().Contain("## ClassCopier API Documentation");
        _ = actual.Should().Contain("## Getting Started");
        _ = actual.Should().Contain("## Additional Resources");
        _ = actual.Should().Contain("Deep copy objects in .NET");
        _ = actual.Should().Contain("https://github.com/WilkinGlen/GUtils");
    }

    [Fact]
    public void MaintainBuilderStateAcrossMultipleOperations()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        
        _ = builder.WithTitle("Initial Title");
        var firstState = builder.Build();
        
        _ = builder.WithTag("Tag1", "Value1");
        var secondState = builder.Build();
        
        _ = builder.WithTitle("Another Title");
        var thirdState = builder.Build();

        _ = firstState.Should().Contain("Initial Title");
        _ = firstState.Should().NotContain("Tag1");
        
        _ = secondState.Should().Contain("Initial Title");
        _ = secondState.Should().Contain("Tag1");
        _ = secondState.Should().NotContain("Another Title");
        
        _ = thirdState.Should().Contain("Initial Title");
        _ = thirdState.Should().Contain("Tag1");
        _ = thirdState.Should().Contain("Another Title");
    }
}
