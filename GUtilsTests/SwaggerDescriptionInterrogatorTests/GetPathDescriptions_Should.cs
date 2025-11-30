namespace GUtilsTests.SwaggerDescriptionInterrogatorTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionInterrogator;

public sealed class GetPathDescriptions_Should
{
    [Fact]
    public void ThrowArgumentNullException_WhenSwaggerJsonIsNull()
    {
        var action = () => SwaggerDescriptionInterrogator.GetPathDescriptions(null!);

        _ = action.Should().Throw<ArgumentNullException>()
            .WithParameterName("swaggerJson");
    }

    [Fact]
    public void ReturnNull_WhenSwaggerJsonIsInvalid()
    {
        var result = SwaggerDescriptionInterrogator.GetPathDescriptions("invalid json");

        _ = result.Should().BeNull();
    }

    [Fact]
    public void ReturnNull_WhenSwaggerJsonHasNoPaths()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "info": {
                "title": "Test API",
                "version": "1.0.0"
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().BeNull();
    }

    [Fact]
    public void ReturnNull_WhenPathsHaveNoDescriptions()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "summary": "Test endpoint"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().BeNull();
    }

    [Fact]
    public void ParseSinglePathWithTitleOnly()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Test Endpoint\r\n"
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
        _ = result[0].Titles![0].Should().Be("Test Endpoint");
        _ = result[0].Tags.Should().BeNull();
    }

    [Fact]
    public void ParseSinglePathWithTagOnly()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "- Author: Glen Wilkin\r\n"
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
        _ = result[0].Tags!["Author"].Should().Be("Glen Wilkin");
    }

    [Fact]
    public void ParseSinglePathWithTitleAndTags()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Test Endpoint\r\n- Author: Glen Wilkin\r\n- Version: 1.0.0\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);
        _ = result![0].Name.Should().Be("/test");
        _ = result[0].Titles.Should().HaveCount(1);
        _ = result[0].Titles![0].Should().Be("Test Endpoint");
        _ = result[0].Tags.Should().HaveCount(2);
        _ = result[0].Tags!["Author"].Should().Be("Glen Wilkin");
        _ = result[0].Tags!["Version"].Should().Be("1.0.0");
    }

    [Fact]
    public void ParseMultipleTitles()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## First Title\r\n## Second Title\r\n## Third Title\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles.Should().HaveCount(3);
        _ = result[0].Titles![0].Should().Be("First Title");
        _ = result[0].Titles![1].Should().Be("Second Title");
        _ = result[0].Titles![2].Should().Be("Third Title");
    }

    [Fact]
    public void ParseComplexDescriptionWithMultipleTitlesAndTags()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Overview\r\n- Category: Testing\r\n- Version: 1.0\r\n## Details\r\n- Author: Glen Wilkin\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles.Should().HaveCount(2);
        _ = result[0].Titles![0].Should().Be("Overview");
        _ = result[0].Titles![1].Should().Be("Details");
        _ = result[0].Tags.Should().HaveCount(3);
        _ = result[0].Tags!["Category"].Should().Be("Testing");
        _ = result[0].Tags!["Version"].Should().Be("1.0");
        _ = result[0].Tags!["Author"].Should().Be("Glen Wilkin");
    }

    [Fact]
    public void ParseMultiplePaths()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test1": {
                    "get": {
                        "description": "## Test 1\r\n- Tag1: Value1\r\n"
                    }
                },
                "/test2": {
                    "post": {
                        "description": "## Test 2\r\n- Tag2: Value2\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(2);
        _ = result![0].Name.Should().Be("/test1");
        _ = result[1].Name.Should().Be("/test2");
    }

    [Fact]
    public void HandleTagsWithColonsInValue()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "- URL: https://example.com\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Tags!["URL"].Should().Be("https://example.com");
    }

    [Fact]
    public void HandleEmptyLines()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Title\r\n\r\n- Tag: Value\r\n\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles.Should().HaveCount(1);
        _ = result[0].Tags.Should().HaveCount(1);
    }

    [Fact]
    public void HandleUnixLineEndings()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Title\n- Tag: Value\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles![0].Should().Be("Title");
        _ = result[0].Tags!["Tag"].Should().Be("Value");
    }

    [Fact]
    public void HandleMacLineEndings()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Title\r- Tag: Value\r"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles![0].Should().Be("Title");
        _ = result[0].Tags!["Tag"].Should().Be("Value");
    }

    [Fact]
    public void IgnoreInvalidTagFormat()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "- NoColon\r\n- Valid: Value\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Tags.Should().HaveCount(1);
        _ = result[0].Tags!["Valid"].Should().Be("Value");
    }

    [Fact]
    public void HandleEmptyTitleAfterMarker()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## \r\n- Tag: Value\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles.Should().BeNull();
        _ = result[0].Tags.Should().HaveCount(1);
    }

    [Fact]
    public void HandleWhitespaceInTitles()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "##    Padded Title   \r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Titles![0].Should().Be("Padded Title");
    }

    [Fact]
    public void HandleWhitespaceInTags()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "-    Tag   :   Value   \r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result![0].Tags!["Tag"].Should().Be("Value");
    }

    [Fact]
    public void HandleMultipleMethodsOnSamePath()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## GET Endpoint\r\n"
                    },
                    "post": {
                        "description": "## POST Endpoint\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(2);
        _ = result![0].Titles![0].Should().Be("GET Endpoint");
        _ = result[1].Titles![0].Should().Be("POST Endpoint");
    }

    [Fact]
    public void ReturnNull_WhenDescriptionIsEmpty()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": ""
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().BeNull();
    }

    [Fact]
    public void HandleRealWorldExample()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/api/users": {
                    "get": {
                        "description": "## User Management\r\n- Category: Users\r\n- Version: 2.0\r\n- Author: Glen Wilkin\r\n## Authentication\r\n- Required: Yes\r\n- Type: Bearer\r\n"
                    }
                }
            }
        }
        """;

        var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);

        _ = result.Should().NotBeNull();
        _ = result.Should().HaveCount(1);
        _ = result![0].Name.Should().Be("/api/users");
        _ = result[0].Titles.Should().HaveCount(2);
        _ = result[0].Titles![0].Should().Be("User Management");
        _ = result[0].Titles![1].Should().Be("Authentication");
        _ = result[0].Tags.Should().HaveCount(5);
        _ = result[0].Tags!["Category"].Should().Be("Users");
        _ = result[0].Tags!["Version"].Should().Be("2.0");
        _ = result[0].Tags!["Author"].Should().Be("Glen Wilkin");
        _ = result[0].Tags!["Required"].Should().Be("Yes");
        _ = result[0].Tags!["Type"].Should().Be("Bearer");
    }
}
