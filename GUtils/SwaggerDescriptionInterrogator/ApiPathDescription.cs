namespace GUtils.SwaggerDescriptionInterrogator;

public sealed class ApiPathDescription
{
    public required string Name { get; set; }

    public List<string>? Titles { get; set; }

    public Dictionary<string, string>? Tags { get; set; }
}
