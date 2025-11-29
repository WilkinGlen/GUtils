namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class WithTitle_Should
{
    [Fact]
    public void ReturnSameBuilderInstance()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var result = builder.WithTitle("Test");

        _ = result.Should().BeSameAs(builder);
    }

    [Fact]
    public void AppendMarkdownHeadingWithNewline()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("My Title")
            .Build();

        _ = actual.Should().Be($"## My Title{Environment.NewLine}");
    }

    [Fact]
    public void AllowMultipleCalls()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("First")
            .WithTitle("Second")
            .WithTitle("Third")
            .Build();

        var expected = $"## First{Environment.NewLine}## Second{Environment.NewLine}## Third{Environment.NewLine}";
        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void HandleEmptyString()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(string.Empty)
            .Build();

        _ = actual.Should().Be($"## {Environment.NewLine}");
    }

    [Fact]
    public void HandleWhitespace()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("   ")
            .Build();

        _ = actual.Should().Be($"##    {Environment.NewLine}");
    }

    [Fact]
    public void PreserveSpecialCharacters()
    {
        var title = "Title with <special> & \"characters\"";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(title)
            .Build();

        _ = actual.Should().Contain(title);
    }

    [Fact]
    public void PreserveUnicodeCharacters()
    {
        var title = "Title ?? ??";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(title)
            .Build();

        _ = actual.Should().Contain(title);
    }

    [Fact]
    public void HandleVeryLongString()
    {
        var longTitle = new string('X', 10000);
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(longTitle)
            .Build();

        _ = actual.Should().Contain(longTitle);
        _ = actual.Should().StartWith("##");
    }

    [Fact]
    public void AppendToExistingContent()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Tag", "Value")
            .WithTitle("Title")
            .Build();

        _ = actual.Should().Contain("Tag");
        _ = actual.Should().Contain("Title");
    }

    [Theory]
    [InlineData("Simple Title")]
    [InlineData("Title with spaces")]
    [InlineData("Title_with_underscores")]
    [InlineData("Title.with.dots")]
    [InlineData("Title123")]
    public void HandleVariousTitleFormats(string title)
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(title)
            .Build();

        _ = actual.Should().Contain(title);
        _ = actual.Should().StartWith("##");
    }

    [Fact]
    public void HandleTitleWithEmbeddedNewlines()
    {
        var title = "Line1\nLine2\nLine3";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(title)
            .Build();

        _ = actual.Should().Contain(title);
    }

    [Fact]
    public void HandleTitleWithMarkdownCharacters()
    {
        var title = "Title with **bold** and *italic*";
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(title)
            .Build();

        _ = actual.Should().Contain(title);
    }

    [Fact]
    public void SupportMethodChaining()
    {
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Title1")
            .WithTitle("Title2")
            .WithTag("Tag1", "Value1")
            .WithTitle("Title3")
            .Build();

        _ = actual.Should().Contain("Title1");
        _ = actual.Should().Contain("Title2");
        _ = actual.Should().Contain("Title3");
        _ = actual.Should().Contain("Tag1");
    }
}
