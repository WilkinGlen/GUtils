namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class HashValidation_Should
{
    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameContainsHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag#Name", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueContainsHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "Value#With#Hash");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hash symbols.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenBothContainHashSymbols()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag#Name", "Value#With#Hash");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameStartsWithHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("#TagName", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameEndsWithHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName#", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueStartsWithHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "#Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hash symbols.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueEndsWithHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "Value#");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hash symbols.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameContainsMultipleHashSymbols()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag##Name###", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueContainsSingleHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "Val#ue");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hash symbols.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_AllowTagWithoutHashSymbols()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "TagValue")
            .Build();

        _ = result.Should().Contain("TagName");
        _ = result.Should().Contain("TagValue");
    }

    [Fact]
    public void WithTag_AllowTagWithSpecialCharactersExceptHashAndHyphen()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag_Name!", "Tag@Value$")
            .Build();

        _ = result.Should().Contain("Tag_Name!");
        _ = result.Should().Contain("Tag@Value$");
    }

    [Fact]
    public void WithTag_AllowTagWithNumbers()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag123", "Value456")
            .Build();

        _ = result.Should().Contain("Tag123");
        _ = result.Should().Contain("Value456");
    }

    [Fact]
    public void ComplexChaining_WithMultipleCallsWithoutHashSymbols()
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
    public void ComplexChaining_ThrowOnFirstHashSymbolEncountered()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Valid Title")
            .WithTag("ValidTag", "ValidValue");

        var action = () => builder.WithTag("Invalid#Tag", "ValidValue");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagNameIsJustHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("#", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueIsJustHashSymbol()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "#");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hash symbols.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenBothTagNameAndValueAreHashSymbols()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("#", "#");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hash symbols.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ChecksHyphenBeforeHash_WhenBothPresent()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag-Name#", "Value");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag name cannot contain hyphens.*")
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowsHashError_WhenOnlyHashPresent()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", "V#alue");

        _ = action.Should().Throw<ArgumentException>()
            .WithMessage("Tag value cannot contain hash symbols.*")
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentException_WhenTagValueIsEmpty()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("TagName", string.Empty);

        _ = action.Should().Throw<ArgumentException>()
            .WithParameterName("tagValue")
            .WithMessage("Tag value cannot be empty or whitespace.*");
    }
}
