namespace GUtils.SwaggerDescriptionBuilder;

using System.Threading;

public sealed class SwaggerDescriptionBuilder
{
    private readonly Lock myLock = new();
    private string? description;

    private SwaggerDescriptionBuilder() => this.description = string.Empty;

    public static SwaggerDescriptionBuilder Create() => new();

    public SwaggerDescriptionBuilder WithTitle(string description)
    {
        ArgumentNullException.ThrowIfNull(description);

        lock (this.myLock)
        {
            this.description += $"## {description}{Environment.NewLine}";
        }

        return this;
    }

    public SwaggerDescriptionBuilder WithTag(string tagName, string tagValue)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        ArgumentNullException.ThrowIfNull(tagValue);

        lock (this.myLock)
        {
            this.description += $"- {tagName}: {tagValue}{Environment.NewLine}";
        }

        return this;
    }

    public string Build()
    {
        lock (this.myLock)
        {
            return $"""{this.description}""";
        }
    }
}
