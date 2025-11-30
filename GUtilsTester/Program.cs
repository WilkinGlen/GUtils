using GUtils.SwaggerDescriptionBuilder;
using GUtils.SwaggerDescriptionInterrogator;
using System.Text.Json;

Console.WriteLine("Generating Swagger description");

var description = SwaggerDescriptionBuilder.Create()
    .WithTitle("GUtils Tester API")
    .WithTag("GUtilsTester", "APIs for testing GUtils library")
    .WithTag("Utilities", "Utility functions and helpers")
    .WithTag("Swagger", "Swagger description generation")
    .Build();

Console.WriteLine($"Description created: {description}");

var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test1": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                },
                "/test2": {
                    "post": {
                        "description": {{JsonSerializer.Serialize("## Test 2\r\n- Tag2: Value2\r\n")}}
                    }
                }
            }
        }
        """;

Console.WriteLine("Parsing description");

Console.WriteLine("\nSwagger JSON:");
Console.WriteLine(swaggerJson);
Console.WriteLine();

var paths = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

Console.WriteLine($"\nNumber of paths found: {paths?.Count ?? 0}");

if (paths != null)
{
    foreach (var path in paths)
    {
        Console.WriteLine($"\nPath: {path.Name}");
        if (path.Tags != null)
        {
            Console.WriteLine($"Path Name: path.Name");
            foreach(var title in path.Titles ?? [])
            {
                Console.WriteLine($"  Title: {title}");
            }

            foreach (var tag in path.Tags)
            {
                Console.WriteLine($"  Tag: {tag.Key}");
                Console.WriteLine($"  Description: {tag.Value}");
            }
        }
    }
}
