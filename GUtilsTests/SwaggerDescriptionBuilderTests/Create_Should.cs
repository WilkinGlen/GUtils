namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class Create_Should
{
    [Fact]
    public void ReturnNonNullInstance()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        _ = builder.Should().NotBeNull();
    }

    [Fact]
    public void ReturnSwaggerDescriptionBuilderType()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        _ = builder.Should().BeOfType<SwaggerDescriptionBuilder>();
    }

    [Fact]
    public void ReturnNewInstanceEachTime()
    {
        var builder1 = SwaggerDescriptionBuilder.Create();
        var builder2 = SwaggerDescriptionBuilder.Create();
        var builder3 = SwaggerDescriptionBuilder.Create();

        _ = builder1.Should().NotBeSameAs(builder2);
        _ = builder2.Should().NotBeSameAs(builder3);
        _ = builder1.Should().NotBeSameAs(builder3);
    }

    [Fact]
    public void ReturnInstanceWithEmptyDescription()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var result = builder.Build();

        _ = result.Should().BeEmpty();
    }

    [Fact]
    public void AllowImmediateChaining()
    {
        var result = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Test")
            .Build();

        _ = result.Should().Contain("Test");
    }
}
