namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class NullHandling_Should
{
    [Fact]
    public void WithTitle_ThrowArgumentNullException_WhenTitleIsNull()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTitle(null!)
            .Build();

        _ = action.Should().Throw<ArgumentNullException>()
            .WithParameterName("description");
    }

    [Fact]
    public void WithTag_ThrowArgumentNullException_WhenTagNameIsNull()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag(null!, "Value")
            .Build();

        _ = action.Should().Throw<ArgumentNullException>()
            .WithParameterName("tagName");
    }

    [Fact]
    public void WithTag_ThrowArgumentNullException_WhenTagValueIsNull()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", null!)
            .Build();

        _ = action.Should().Throw<ArgumentNullException>()
            .WithParameterName("tagValue");
    }

    [Fact]
    public void WithTag_ThrowArgumentNullException_WhenBothAreNull()
    {
        var action = () => SwaggerDescriptionBuilder
            .Create()
            .WithTag(null!, null!)
            .Build();

        _ = action.Should().Throw<ArgumentNullException>();
    }
}
