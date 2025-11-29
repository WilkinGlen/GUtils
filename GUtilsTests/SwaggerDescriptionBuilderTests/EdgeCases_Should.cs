namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class EdgeCases_Should
{
    [Fact]
    public void HandleMultipleConsecutiveSpacesInTitle()
    {
        var expected = $"##      {Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("     ")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleTabCharactersInTitle()
    {
        var expected = $"## Title\tWith\tTabs{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title\tWith\tTabs")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleTabCharactersInTagName()
    {
        var expected = $"- Tag\tName: Value{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag\tName", "Value")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleTabCharactersInTagValue()
    {
        var expected = $"- Name: Value\tWith\tTabs{Environment.NewLine}";
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", "Value\tWith\tTabs")
            .Build();

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleCarriageReturnInTitle()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Line1\rLine2")
            .Build();

        _ = actual.Should().Contain("Line1\rLine2");
    }

    [Fact]
    public void HandleMixedLineEndingsInTitle()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Line1\r\nLine2\nLine3\rLine4")
            .Build();

        _ = actual.Should().Contain("Line1\r\nLine2\nLine3\rLine4");
    }

    [Fact]
    public void HandleMixedLineEndingsInTagValue()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Description", "Line1\r\nLine2\nLine3\rLine4")
            .Build();

        _ = actual.Should().Contain("Line1\r\nLine2\nLine3\rLine4");
    }

    [Fact]
    public void HandleMarkdownHeadingSyntaxInTitle()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("# This is H1")
            .Build();

        _ = actual.Should().Be($"## # This is H1{Environment.NewLine}");
    }

    [Fact]
    public void HandleMarkdownListSyntaxInTagValue()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Items", "- Item 1\n- Item 2")
            .Build();

        _ = actual.Should().Contain("- Item 1\n- Item 2");
    }

    [Fact]
    public void HandleZeroWidthCharactersInTitle()
    {
        var title = "Title\u200B\u200CWith\u200DControl";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(title)
            .Build();

        _ = actual.Should().Contain(title);
    }

    [Fact]
    public void HandleZeroWidthCharactersInTagName()
    {
        var tagName = "Tag\u200BName";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(tagName, "Value")
            .Build();

        _ = actual.Should().Contain(tagName);
    }

    [Fact]
    public void HandleZeroWidthCharactersInTagValue()
    {
        var tagValue = "Value\u200CWith\u200DControl";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Fact]
    public void BuildReturnsConsistentString()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Test");

        var result1 = builder.Build();
        var result2 = builder.Build();

        _ = result1.Should().Be(result2);
        // Note: String interning may cause identical strings to share references
    }

    [Fact]
    public void HandleRepeatedTitleTagTitleTagPattern()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("T1").WithTag("Tag1", "V1")
            .WithTitle("T2").WithTag("Tag2", "V2")
            .WithTitle("T3").WithTag("Tag3", "V3")
            .WithTitle("T4").WithTag("Tag4", "V4")
            .Build();

        var lines = actual.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        _ = lines.Should().HaveCount(8);
        
        for (var i = 0; i < 4; i++)
        {
            _ = lines[i * 2].Should().StartWith("##");
            _ = lines[i * 2 + 1].Should().StartWith("-");
        }
    }

    [Fact]
    public void HandleBackslashCharactersInTitle()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(@"Path\To\File")
            .Build();

        _ = actual.Should().Contain(@"Path\To\File");
    }

    [Fact]
    public void HandleBackslashCharactersInTagValue()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Path", @"C:\Windows\System32")
            .Build();

        _ = actual.Should().Contain(@"C:\Windows\System32");
    }

    [Fact]
    public void HandleQuotesAndApostrophesInTitle()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title with 'single' and \"double\" quotes")
            .Build();

        _ = actual.Should().Contain("Title with 'single' and \"double\" quotes");
    }

    [Fact]
    public void HandleQuotesAndApostrophesInTagValue()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Description", "Value's \"quoted\" text")
            .Build();

        _ = actual.Should().Contain("Value's \"quoted\" text");
    }
}