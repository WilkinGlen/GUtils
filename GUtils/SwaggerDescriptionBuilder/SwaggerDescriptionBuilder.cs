namespace GUtils.SwaggerDescriptionBuilder;

public sealed class SwaggerDescriptionBuilder
{
    private string? description;

    private SwaggerDescriptionBuilder() => this.description = string.Empty;

    public static SwaggerDescriptionBuilder Create() => new();

    public SwaggerDescriptionBuilder WithTitle(string description)
    {
        this.description = this.description + $"##{description}{Environment.NewLine}";
        return this;
    }

    public SwaggerDescriptionBuilder WithTag(string tagName, string tagValue)
    {
        this.description = this.description + $"- {tagName}: {tagValue}{Environment.NewLine}";
        return this;
    }

    public string Build() => $"""{this.description}""";
}
