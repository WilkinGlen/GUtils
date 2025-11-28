namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class Build_Should
{
    [Fact]
    public void ReturnEmptyStringWhenNoMethodsCalled()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .Build();

        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void ReturnCorrectDescriptionWithSingleTitle()
    {
        var expected = $"##API for the GUtils ClassCopier utility{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("API for the GUtils ClassCopier utility")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void ReturnCorrectDescriptionWithMultipleTitles()
    {
        var expected = $"##First Title{Environment.NewLine}##Second Title{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("First Title")
            .WithTitle("Second Title")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void ReturnCorrectDescriptionWithSingleTag()
    {
        var expected = $"- Author: Glen Wilkin{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Author", "Glen Wilkin")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void ReturnCorrectDescriptionWithMultipleTags()
    {
        var expected = $"- Author: Glen Wilkin{Environment.NewLine}- Version: 1.0.0{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Author", "Glen Wilkin")
            .WithTag("Version", "1.0.0")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void ReturnCorrectDescriptionWithTitleAndTags()
    {
        var expected = $"##API Documentation{Environment.NewLine}- Author: Glen Wilkin{Environment.NewLine}- License: MIT{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("API Documentation")
            .WithTag("Author", "Glen Wilkin")
            .WithTag("License", "MIT")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void ReturnCorrectDescriptionWithComplexChaining()
    {
        var expected = $"##Main Title{Environment.NewLine}- Tag1: Value1{Environment.NewLine}##Sub Title{Environment.NewLine}- Tag2: Value2{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Main Title")
            .WithTag("Tag1", "Value1")
            .WithTitle("Sub Title")
            .WithTag("Tag2", "Value2")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleEmptyTitleString()
    {
        var expected = $"##{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(string.Empty)
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleEmptyTagStrings()
    {
        var expected = $"- : {Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(string.Empty, string.Empty)
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleWhitespaceTitleString()
    {
        var expected = $"##   {Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("   ")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleWhitespaceTagStrings()
    {
        var expected = $"-    :    {Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("   ", "   ")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleSpecialCharactersInTitle()
    {
        var expected = $"##API & Documentation <with> special \"chars\"{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("API & Documentation <with> special \"chars\"")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleSpecialCharactersInTags()
    {
        var expected = $"- Author & Owner: <Glen> \"Wilkin\"{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Author & Owner", "<Glen> \"Wilkin\"")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleUnicodeCharactersInTitle()
    {
        var expected = $"##API 文档 🚀{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("API 文档 🚀")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleUnicodeCharactersInTags()
    {
        var expected = $"- 作者: Glen Wilkin 👨‍💻{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("作者", "Glen Wilkin 👨‍💻")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleVeryLongTitle()
    {
        var longTitle = new string('A', 1000);
        var expected = $"##" + longTitle + Environment.NewLine;
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(longTitle)
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleVeryLongTags()
    {
        var longName = new string('B', 500);
        var longValue = new string('C', 500);
        var expected = $"- {longName}: {longValue}{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(longName, longValue)
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleManyChainedCalls()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var expectedParts = new List<string>();

        for (var i = 0; i < 100; i++)
        {
            _ = builder.WithTitle($"Title {i}");
            expectedParts.Add($"##Title {i}{Environment.NewLine}");
        }

        var expected = string.Join(string.Empty, expectedParts);
        var actual = builder.Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void SupportFluentInterfacePattern()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        
        var result1 = builder.WithTitle("Title");
        var result2 = result1.WithTag("Name", "Value");
        
        _ = result1.Should().BeSameAs(builder);
        _ = result2.Should().BeSameAs(builder);
    }

    [Fact]
    public void BuildCanBeCalledMultipleTimes()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title");

        var firstBuild = builder.Build();
        var secondBuild = builder.Build();

        _ = firstBuild.Should().Be(secondBuild);
    }

    [Fact]
    public void ModifyingAfterBuildAffectsSubsequentBuilds()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("First");

        var firstBuild = builder.Build();
        
        _ = builder.WithTag("Name", "Value");
        var secondBuild = builder.Build();

        _ = firstBuild.Should().NotBe(secondBuild);
        _ = secondBuild.Should().Contain("Name");
    }

    [Fact]
    public void CreateReturnsNewInstance()
    {
        var builder1 = SwaggerDescriptionBuilder.Create();
        var builder2 = SwaggerDescriptionBuilder.Create();

        _ = builder1.Should().NotBeSameAs(builder2);
    }

    [Fact]
    public void IndependentBuilderInstancesDoNotInterfere()
    {
        var builder1 = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Builder 1");

        var builder2 = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Builder 2");

        var result1 = builder1.Build();
        var result2 = builder2.Build();

        _ = result1.Should().Contain("Builder 1");
        _ = result1.Should().NotContain("Builder 2");
        _ = result2.Should().Contain("Builder 2");
        _ = result2.Should().NotContain("Builder 1");
    }

    [Fact]
    public void HandleColonInTagValue()
    {
        var expected = $"- URL: https://example.com{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("URL", "https://example.com")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleNewlineCharactersInTitle()
    {
        var expected = $"##Title\nWith\nNewlines{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title\nWith\nNewlines")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleNewlineCharactersInTagValue()
    {
        var expected = $"- Description: Line1\nLine2{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Description", "Line1\nLine2")
            .Build();

        _ = actual.Should().Be(expected);
    }
}
