namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class HyphenValidation_Should
{
    [Fact]
    public void WithTitle_ThrowArgumentException_WhenTitleContainsHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title-With-Hyphens");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot contain hyphens.*")
            .WithParameterName("description");
    }

    [Fact]
    public void WithTitle_ThrowArgumentException_WhenTitleContainsSingleHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title-Here");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot contain hyphens.*")
            .WithParameterName("description");
    }

    [Fact]
    public void WithTitle_AllowTitleWithoutHyphens()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title Without Hyphens")
            .Build();

        _ = result.Should().Contain("Title Without Hyphens");
    }

    [Fact]
    public void WithTitle_AllowTitleWithUnderscores()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title_With_Underscores")
            .Build();

        _ = result.Should().Contain("Title_With_Underscores");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameContainsHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag-Name", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hyphens.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueContainsHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "Value-With-Hyphen");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hyphens.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenBothContainHyphens()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag-Name", "Value-With-Hyphen");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hyphens.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_AllowTagWithoutHyphens()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "TagValue")
            .Build();

        _ = result.Should().Contain("TagName");
        _ = result.Should().Contain("TagValue");
    }

    [Fact]
    public void WithTag_AllowTagWithUnderscores()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag_Name", "Tag_Value")
            .Build();

        _ = result.Should().Contain("Tag_Name");
        _ = result.Should().Contain("Tag_Value");
    }

    [Fact]
    public void WithTitle_ThrowArgumentException_WhenTitleStartsWithHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTitle("-Title");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot contain hyphens.*");
    }

    [Fact]
    public void WithTitle_ThrowArgumentException_WhenTitleEndsWithHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title-");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot contain hyphens.*");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameStartsWithHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("-TagName", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hyphens.*");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueEndsWithHyphen()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "Value-");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hyphens.*");
    }

    [Fact]
    public void ComplexChaining_WithMultipleCallsWithoutHyphens()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("First Title")
            .WithTag("Author", "Glen Wilkin")
            .WithTitle("Second Title")
            .WithTag("Version", "1.0.0")
            .Build();

        _ = result.Should().Contain("First Title");
        _ = result.Should().Contain("Second Title");
        _ = result.Should().Contain("Glen Wilkin");
        _ = result.Should().Contain("1.0.0");
    }

    [Fact]
    public void ComplexChaining_ThrowOnFirstHyphenEncountered()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Valid Title")
            .WithTag("ValidTag", "ValidValue");

        var action = () => builder.WithTitle("Invalid-Title");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot contain hyphens.*");
    }
}
