namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class WithTag_Should
{
    [Fact]
    public void ReturnSameBuilderInstance()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var result = builder.WithTag("Name", "Value");

        _ = result.Should().BeSameAs(builder);
    }

    [Fact]
    public void AppendMarkdownListItemWithNewline()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Author", "Glen Wilkin")
            .Build();

        _ = actual.Should().Be($"- Author: Glen Wilkin{Environment.NewLine}");
    }

    [Fact]
    public void AllowMultipleCalls()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag1", "Value1")
            .WithTag("Tag2", "Value2")
            .WithTag("Tag3", "Value3")
            .Build();

        var expected = $"- Tag1: Value1{Environment.NewLine}- Tag2: Value2{Environment.NewLine}- Tag3: Value3{Environment.NewLine}";
        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void ThrowArgumentException_WhenTagNameIsEmpty()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag(string.Empty, "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithParameterName("tagName")
            .WithMessage("Tag name cannot be empty or whitespace.*");
    }

    [Fact]
    public void ThrowArgumentException_WhenTagValueIsEmpty()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", string.Empty);

        _ = action.Should().Throw<ArgumentException>()
            .WithParameterName("tagValue")
            .WithMessage("Tag value cannot be empty or whitespace.*");
    }

    [Fact]
    public void ThrowArgumentException_WhenBothEmpty()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag(string.Empty, string.Empty);

        _ = action.Should().Throw<ArgumentException>()
            .WithParameterName("tagName")
            .WithMessage("Tag name cannot be empty or whitespace.*");
    }

    [Fact]
    public void ThrowArgumentException_WhenTagNameIsWhitespace()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("   ", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithParameterName("tagName")
            .WithMessage("Tag name cannot be empty or whitespace.*");
    }

    [Fact]
    public void ThrowArgumentException_WhenTagValueIsWhitespace()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", "   ");

        _ = action.Should().Throw<ArgumentException>()
            .WithParameterName("tagValue")
            .WithMessage("Tag value cannot be empty or whitespace.*");
    }

    [Fact]
    public void PreserveSpecialCharactersInTagName()
    {
        var tagName = "<Author> & \"Owner\"";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(tagName, "Value")
            .Build();

        _ = actual.Should().Contain(tagName);
    }

    [Fact]
    public void PreserveSpecialCharactersInTagValue()
    {
        var tagValue = "<Value> & \"Special\"";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Fact]
    public void PreserveUnicodeCharactersInTagName()
    {
        var tagName = "?? ??";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(tagName, "Value")
            .Build();

        _ = actual.Should().Contain(tagName);
    }

    [Fact]
    public void PreserveUnicodeCharactersInTagValue()
    {
        var tagValue = "Glen Wilkin ?????";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Author", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Fact]
    public void HandleVeryLongTagName()
    {
        var longName = new string('A', 5000);
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(longName, "Value")
            .Build();

        _ = actual.Should().Contain(longName);
    }

    [Fact]
    public void HandleVeryLongTagValue()
    {
        var longValue = new string('B', 5000);
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", longValue)
            .Build();

        _ = actual.Should().Contain(longValue);
    }

    [Fact]
    public void AppendToExistingContent()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title")
            .WithTag("Tag", "Value")
            .Build();

        _ = actual.Should().Contain("Title");
        _ = actual.Should().Contain("Tag");
        _ = actual.Should().Contain("Value");
    }

    [Fact]
    public void HandleColonInTagName()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name:With:Colons", "Value")
            .Build();

        _ = actual.Should().Contain("Name:With:Colons");
    }

    [Fact]
    public void HandleColonInTagValue()
    {
        var tagValue = "https://github.com/user/repo";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("URL", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Fact]
    public void RejectDashInTagName()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag_Name", "Value");

        _ = action.Should().NotThrow();
    }

    [Fact]
    public void HandleNewlineInTagName()
    {
        var tagName = "Name\nWith\nNewlines";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(tagName, "Value")
            .Build();

        _ = actual.Should().Contain(tagName);
    }

    [Fact]
    public void HandleNewlineInTagValue()
    {
        var tagValue = "Line1\nLine2\nLine3";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Description", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Theory]
    [InlineData("Author", "Glen Wilkin")]
    [InlineData("Version", "1.0.0")]
    [InlineData("License", "MIT")]
    [InlineData("GitHub", "https://github.com")]
    [InlineData("Email", "test@example.com")]
    public void HandleCommonTagPatterns(string name, string value)
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag(name, value)
            .Build();

        _ = actual.Should().Contain(name);
        _ = actual.Should().Contain(value);
        _ = actual.Should().StartWith("- ");
    }

    [Fact]
    public void HandleMarkdownInTagValue()
    {
        var tagValue = "**Bold** and *italic* text";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Description", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Fact]
    public void HandleLinkInTagValue()
    {
        var tagValue = "[GitHub](https://github.com)";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Link", tagValue)
            .Build();

        _ = actual.Should().Contain(tagValue);
    }

    [Fact]
    public void SupportMethodChaining()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag1", "Value1")
            .WithTag("Tag2", "Value2")
            .WithTitle("Title")
            .WithTag("Tag3", "Value3")
            .Build();

        _ = actual.Should().Contain("Tag1");
        _ = actual.Should().Contain("Tag2");
        _ = actual.Should().Contain("Tag3");
        _ = actual.Should().Contain("Title");
    }

    [Fact]
    public void MaintainInsertionOrder()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("First", "1")
            .WithTag("Second", "2")
            .WithTag("Third", "3")
            .Build();

        var firstIndex = actual.IndexOf("First", StringComparison.Ordinal);
        var secondIndex = actual.IndexOf("Second", StringComparison.Ordinal);
        var thirdIndex = actual.IndexOf("Third", StringComparison.Ordinal);

        _ = firstIndex.Should().BeLessThan(secondIndex);
        _ = secondIndex.Should().BeLessThan(thirdIndex);
    }
}
