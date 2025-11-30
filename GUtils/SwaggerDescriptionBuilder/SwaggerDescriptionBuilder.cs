namespace GUtils.SwaggerDescriptionBuilder;

public sealed class SwaggerDescriptionBuilder
{
    private readonly Lock myLock = new();
    private string? description;

    private SwaggerDescriptionBuilder() => this.description = string.Empty;

    public static SwaggerDescriptionBuilder Create() => new();

    public SwaggerDescriptionBuilder WithTitle(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        
        if (description.Contains('-'))
        {
            throw new ArgumentException("Title cannot contain hyphens.", nameof(description));
        }

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
        
        if (tagName.Contains('-'))
        {
            throw new ArgumentException("Tag name cannot contain hyphens.", nameof(tagName));
        }
        
        if (tagName.Contains('#'))
        {
            throw new ArgumentException("Tag name cannot contain hash symbols.", nameof(tagName));
        }
        
        if (tagValue.Contains('-'))
        {
            throw new ArgumentException("Tag value cannot contain hyphens.", nameof(tagValue));
        }
        
        if (tagValue.Contains('#'))
        {
            throw new ArgumentException("Tag value cannot contain hash symbols.", nameof(tagValue));
        }

        lock (this.myLock)
        {
            this.description += $"- {tagName}: {tagValue}{Environment.NewLine}";
        }

        return this;
    }

    public SwaggerDescriptionBuilder Clear()
    {
        lock (this.myLock)
        {
            this.description = string.Empty;
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
