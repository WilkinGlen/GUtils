namespace GUtilsTests.SwaggerDescriptionInterrogatorTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;
using GUtils.SwaggerDescriptionInterrogator;
using System.Text.Json;

public sealed class IntegrationWithBuilder_Should
{
    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_SingleTitle()
    {
        var description = SwaggerDescriptionBuilder.Create()
            .WithTitle("Test API")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);
        _ = result![0].Name.Should().Be("/test");
        _ = result[0].Titles.Should().NotBeNull();
        _ = result[0].Titles.Should().HaveCount(1);
        _ = result[0].Titles![0].Should().Be("Test API");
        _ = result[0].Tags.Should().BeNull();
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_SingleTag()
    {
        var description = SwaggerDescriptionBuilder.Create()
            .WithTag("Version", "1.0.0")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);
        _ = result![0].Name.Should().Be("/test");
        _ = result[0].Titles.Should().BeNull();
        _ = result[0].Tags.Should().NotBeNull();
        _ = result[0].Tags.Should().HaveCount(1);
        _ = result[0].Tags!["Version"].Should().Be("1.0.0");
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_TitleAndSingleTag()
    {
        var description = SwaggerDescriptionBuilder.Create()
            .WithTitle("User API")
            .WithTag("Category", "Authentication")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/api/users": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);
        _ = result![0].Name.Should().Be("/api/users");
        _ = result[0].Titles.Should().HaveCount(1);
        _ = result[0].Titles![0].Should().Be("User API");
        _ = result[0].Tags.Should().HaveCount(1);
        _ = result[0].Tags!["Category"].Should().Be("Authentication");
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_TitleAndMultipleTags()
    {
        var description = SwaggerDescriptionBuilder.Create()
            .WithTitle("GUtils Tester API")
            .WithTag("GUtilsTester", "APIs for testing GUtils library")
            .WithTag("Utilities", "Utility functions and helpers")
            .WithTag("Swagger", "Swagger description generation")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test1": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);
        _ = result![0].Name.Should().Be("/test1");
        _ = result[0].Titles.Should().HaveCount(1);
        _ = result[0].Titles![0].Should().Be("GUtils Tester API");
        _ = result[0].Tags.Should().HaveCount(3);
        _ = result[0].Tags!["GUtilsTester"].Should().Be("APIs for testing GUtils library");
        _ = result[0].Tags!["Utilities"].Should().Be("Utility functions and helpers");
        _ = result[0].Tags!["Swagger"].Should().Be("Swagger description generation");
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_MultiplePaths()
    {
        var description1 = SwaggerDescriptionBuilder.Create()
            .WithTitle("Endpoint 1")
            .WithTag("Type", "GET")
            .Build();

        var description2 = SwaggerDescriptionBuilder.Create()
            .WithTitle("Endpoint 2")
            .WithTag("Type", "POST")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/endpoint1": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description1)}}
                    }
                },
                "/endpoint2": {
                    "post": {
                        "description": {{JsonSerializer.Serialize(description2)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(2);
        _ = result![0].Name.Should().Be("/endpoint1");
        _ = result[0].Titles![0].Should().Be("Endpoint 1");
        _ = result[0].Tags!["Type"].Should().Be("GET");
        _ = result[1].Name.Should().Be("/endpoint2");
        _ = result[1].Titles![0].Should().Be("Endpoint 2");
        _ = result[1].Tags!["Type"].Should().Be("POST");
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_ComplexScenario()
    {
        var description = SwaggerDescriptionBuilder.Create()
            .WithTitle("User Management API")
            .WithTag("Version", "2.0")
            .WithTag("Category", "Users")
            .WithTag("Author", "Glen Wilkin")
            .WithTag("Documentation", "https://docs.example.com")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "info": {
                "title": "My API",
                "version": "1.0.0"
            },
            "paths": {
                "/api/users": {
                    "get": {
                        "summary": "Get all users",
                        "description": {{JsonSerializer.Serialize(description)}}
                    },
                    "post": {
                        "summary": "Create user",
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(2);
        
        foreach (var path in result!)
        {
            _ = path.Name.Should().Be("/api/users");
            _ = path.Titles.Should().HaveCount(1);
            _ = path.Titles![0].Should().Be("User Management API");
            _ = path.Tags.Should().HaveCount(4);
            _ = path.Tags!["Version"].Should().Be("2.0");
            _ = path.Tags!["Category"].Should().Be("Users");
            _ = path.Tags!["Author"].Should().Be("Glen Wilkin");
            _ = path.Tags!["Documentation"].Should().Be("https://docs.example.com");
        }
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_AndClearCalled()
    {
        var builder = SwaggerDescriptionBuilder.Create()
            .WithTitle("First Title")
            .WithTag("OldTag", "OldValue")
            .Clear()
            .WithTitle("New Title")
            .WithTag("NewTag", "NewValue");

        var description = builder.Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles.Should().HaveCount(1);
        _ = result[0].Titles![0].Should().Be("New Title");
        _ = result[0].Tags.Should().HaveCount(1);
        _ = result[0].Tags!["NewTag"].Should().Be("NewValue");
        _ = result[0].Tags.Should().NotContainKey("OldTag");
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_EmptyBuilder()
    {
        var description = SwaggerDescriptionBuilder.Create().Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().BeNull();
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_SpecialCharactersInValues()
    {
        var description = SwaggerDescriptionBuilder.Create()
            .WithTitle("API with Special Characters")
            .WithTag("URL", "https://example.com/path?query=value&other=123")
            .WithTag("Email", "test@example.com")
            .Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles![0].Should().Be("API with Special Characters");
        _ = result[0].Tags!["URL"].Should().Be("https://example.com/path?query=value&other=123");
        _ = result[0].Tags!["Email"].Should().Be("test@example.com");
    }

    [Fact]
    public void CorrectlyParseDescription_WhenBuiltWithSwaggerDescriptionBuilder_RoundTripConsistency()
    {
        var builder = SwaggerDescriptionBuilder.Create()
            .WithTitle("Consistency Test")
            .WithTag("Environment", "Production")
            .WithTag("Status", "Active");

        var description = builder.Build();

        var swaggerJson = $$"""
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": {{JsonSerializer.Serialize(description)}}
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);

        var rebuiltDescription = SwaggerDescriptionBuilder.Create()
            .WithTitle(result![0].Titles![0])
            .WithTag("Environment", result[0].Tags!["Environment"])
            .WithTag("Status", result[0].Tags!["Status"])
            .Build();

        _ = rebuiltDescription.Should().Be(description);
    }
}
