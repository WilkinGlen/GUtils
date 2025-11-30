namespace GUtilsTests.SwaggerDescriptionInterrogatorTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionInterrogator;

public sealed class ThreadSafety_Should
{
    [Fact]
    public void HandleConcurrentCallsWithSameInput()
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

        var results = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        _ = Parallel.For(0, 100, _ =>
        {
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
            results.Add(result);
        });

        _ = results.Should().HaveCount(100);
        _ = results.Should().OnlyContain(r => r != null && r.Count == 1);
        _ = results.Should().OnlyContain(r => r![0].Name == "/test");
        _ = results.Should().OnlyContain(r => r![0].Titles!.Count == 1);
        _ = results.Should().OnlyContain(r => r![0].Tags!.Count == 2);
    }

    [Fact]
    public async Task HandleConcurrentCallsWithDifferentInputs()
    {
        var swaggerJson1 = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/api1": {
                    "get": {
                        "description": "## API 1\r\n- Tag1: Value1\r\n"
                    }
                }
            }
        }
        """;

        var swaggerJson2 = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/api2": {
                    "post": {
                        "description": "## API 2\r\n- Tag2: Value2\r\n"
                    }
                }
            }
        }
        """;

        var swaggerJson3 = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/api3": {
                    "put": {
                        "description": "## API 3\r\n- Tag3: Value3\r\n"
                    }
                }
            }
        }
        """;

        var results1 = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();
        var results2 = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();
        var results3 = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        var tasks = new List<Task>
        {
            Task.Run(() => Parallel.For(0, 50, _ => results1.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson1)))),
            Task.Run(() => Parallel.For(0, 50, _ => results2.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson2)))),
            Task.Run(() => Parallel.For(0, 50, _ => results3.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson3))))
        };

        await Task.WhenAll(tasks);

        _ = results1.Should().OnlyContain(r => r![0].Name == "/api1");
        _ = results2.Should().OnlyContain(r => r![0].Name == "/api2");
        _ = results3.Should().OnlyContain(r => r![0].Name == "/api3");
    }

    [Fact]
    public void HandleConcurrentCallsWithComplexInput()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/users": {
                    "get": {
                        "description": "## User Management\r\n- Category: Users\r\n- Version: 2.0\r\n## Authentication\r\n- Required: Yes\r\n- Type: Bearer\r\n"
                    },
                    "post": {
                        "description": "## Create User\r\n- Method: POST\r\n- Body: JSON\r\n"
                    }
                },
                "/products": {
                    "get": {
                        "description": "## Product Listing\r\n- Pagination: Supported\r\n"
                    }
                }
            }
        }
        """;

        var results = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        _ = Parallel.For(0, 200, _ =>
        {
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
            results.Add(result);
        });

        _ = results.Should().HaveCount(200);
        _ = results.Should().OnlyContain(r => r != null && r.Count == 3);
        
        foreach (var result in results)
        {
            var userGet = result!.First(p => p.Name == "/users" && p.Titles!.Contains("User Management"));
            _ = userGet.Titles.Should().HaveCount(2);
            _ = userGet.Tags.Should().HaveCount(4);
            
            var userPost = result!.First(p => p.Name == "/users" && p.Titles!.Contains("Create User"));
            _ = userPost.Tags.Should().HaveCount(2);
            
            _ = result.Should().Contain(p => p.Name == "/products");
        }
    }

    [Fact]
    public async Task HandleConcurrentCallsWithAsyncTasks()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/async-test": {
                    "get": {
                        "description": "## Async Test\r\n- ThreadId: {0}\r\n"
                    }
                }
            }
        }
        """;

        var tasks = Enumerable.Range(0, 100).Select(async i =>
        {
            await Task.Delay(Random.Shared.Next(0, 10));
            return SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
        });

        var results = await Task.WhenAll(tasks);

        _ = results.Should().HaveCount(100);
        _ = results.Should().OnlyContain(r => r != null && r.Count == 1);
        _ = results.Should().OnlyContain(r => r![0].Name == "/async-test");
    }

    [Fact]
    public void HandleConcurrentCallsWithInvalidJson()
    {
        var validJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/valid": {
                    "get": {
                        "description": "## Valid\r\n"
                    }
                }
            }
        }
        """;

        var invalidJson = "{ invalid json }";

        var validResults = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();
        var invalidResults = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        Parallel.Invoke(
            () => Parallel.For(0, 50, _ => validResults.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(validJson))),
            () => Parallel.For(0, 50, _ => invalidResults.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(invalidJson)))
        );

        _ = validResults.Should().OnlyContain(r => r != null && r.Count == 1);
        _ = invalidResults.Should().OnlyContain(r => r == null);
    }

    [Fact]
    public void HandleConcurrentCallsWithEmptyPaths()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {}
        }
        """;

        var results = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        _ = Parallel.For(0, 100, _ =>
        {
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
            results.Add(result);
        });

        _ = results.Should().HaveCount(100);
        _ = results.Should().OnlyContain(r => r == null);
    }

    [Fact]
    public void HandleConcurrentCallsWithVaryingLineEndings()
    {
        var swaggerJsonCRLF = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/test": {
                    "get": {
                        "description": "## Title\r\n- Tag: Value\r\n"
                    }
                }
            }
        }
        """;

        var swaggerJsonLF = """
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

        var swaggerJsonCR = """
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

        var resultsCRLF = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();
        var resultsLF = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();
        var resultsCR = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        Parallel.Invoke(
            () => Parallel.For(0, 30, _ => resultsCRLF.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJsonCRLF))),
            () => Parallel.For(0, 30, _ => resultsLF.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJsonLF))),
            () => Parallel.For(0, 30, _ => resultsCR.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJsonCR)))
        );

        _ = resultsCRLF.Should().OnlyContain(r => r![0].Titles![0] == "Title");
        _ = resultsLF.Should().OnlyContain(r => r![0].Titles![0] == "Title");
        _ = resultsCR.Should().OnlyContain(r => r![0].Titles![0] == "Title");
    }

    [Fact]
    public void HandleConcurrentCallsWithLargePayloads()
    {
        var pathsBuilder = new System.Text.StringBuilder();
        
        for (var i = 0; i < 50; i++)
        {
            if (i > 0)
            {
                _ = pathsBuilder.Append(',');
            }

            _ = pathsBuilder.Append($"\"/path{i}\": {{");
            _ = pathsBuilder.Append("\"get\": {");
            _ = pathsBuilder.Append($"\"description\": \"## Path {i}\\r\\n- Index: {i}\\r\\n- Type: GET\\r\\n\"");
            _ = pathsBuilder.Append('}');
            _ = pathsBuilder.Append('}');
        }

        var swaggerJson = $"{{\"openapi\": \"3.0.0\",\"paths\": {{{pathsBuilder}}}}}";

        var results = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        _ = Parallel.For(0, 50, _ =>
        {
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
            results.Add(result);
        });

        _ = results.Should().HaveCount(50);
        _ = results.Should().OnlyContain(r => r != null && r.Count == 50);
    }

    [Fact]
    public void HandleStressTestWithMixedScenarios()
    {
        var scenarios = new[]
        {
            """{"openapi": "3.0.0", "paths": {"/s1": {"get": {"description": "## S1\r\n"}}}}""",
            """{"openapi": "3.0.0", "paths": {"/s2": {"post": {"description": "- Tag: Value\r\n"}}}}""",
            """invalid json""",
            """{"openapi": "3.0.0", "paths": {}}""",
            """{"openapi": "3.0.0", "paths": {"/s5": {"put": {"description": "## Title\r\n- T1: V1\r\n- T2: V2\r\n"}}}}"""
        };

        var allResults = new System.Collections.Concurrent.ConcurrentBag<(int scenarioIndex, List<ApiPathDescription>? result)>();

        _ = Parallel.For(0, 500, i =>
        {
            var scenarioIndex = i % scenarios.Length;
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(scenarios[scenarioIndex]);
            allResults.Add((scenarioIndex, result));
        });

        _ = allResults.Should().HaveCount(500);

        var scenario0Results = allResults.Where(r => r.scenarioIndex == 0).Select(r => r.result).ToList();
        var scenario2Results = allResults.Where(r => r.scenarioIndex == 2).Select(r => r.result).ToList();
        var scenario3Results = allResults.Where(r => r.scenarioIndex == 3).Select(r => r.result).ToList();

        _ = scenario0Results.Should().OnlyContain(r => r != null && r.Count == 1);
        _ = scenario2Results.Should().OnlyContain(r => r == null);
        _ = scenario3Results.Should().OnlyContain(r => r == null);
    }

    [Fact]
    public void MaintainConsistentResultsAcrossThreads()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/consistent": {
                    "get": {
                        "description": "## Consistency Test\r\n- Author: Glen Wilkin\r\n- Version: 1.2.3\r\n- Status: Active\r\n"
                    }
                }
            }
        }
        """;

        var results = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        _ = Parallel.For(0, 1000, _ =>
        {
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
            results.Add(result);
        });

        var firstResult = results.First()!;
        
        foreach (var result in results)
        {
            _ = result.Should().NotBeNull();
            _ = result!.Should().HaveCount(1);
            _ = result[0].Name.Should().Be(firstResult[0].Name);
            _ = result[0].Titles.Should().BeEquivalentTo(firstResult[0].Titles);
            _ = result[0].Tags.Should().BeEquivalentTo(firstResult[0].Tags);
        }
    }

    [Fact]
    public void HandleConcurrentCallsWithSpecialCharacters()
    {
        var swaggerJson = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/unicode": {
                    "get": {
                        "description": "## API ?? ??\r\n- ??: Glen Wilkin ?????\r\n- Version: 1.0 ?\r\n"
                    }
                }
            }
        }
        """;

        var results = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        _ = Parallel.For(0, 100, _ =>
        {
            var result = SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson);
            results.Add(result);
        });

        _ = results.Should().HaveCount(100);
        _ = results.Should().OnlyContain(r => r![0].Titles![0].Contains("??"));
        _ = results.Should().OnlyContain(r => r![0].Tags!["??"].Contains("?????"));
    }

    [Fact]
    public void NoSharedStateLeakageBetweenCalls()
    {
        var swaggerJson1 = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/leak-test-1": {
                    "get": {
                        "description": "## Test 1\r\n- Key1: Value1\r\n"
                    }
                }
            }
        }
        """;

        var swaggerJson2 = """
        {
            "openapi": "3.0.0",
            "paths": {
                "/leak-test-2": {
                    "get": {
                        "description": "## Test 2\r\n- Key2: Value2\r\n"
                    }
                }
            }
        }
        """;

        var results1 = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();
        var results2 = new System.Collections.Concurrent.ConcurrentBag<List<ApiPathDescription>?>();

        Parallel.Invoke(
            () => Parallel.For(0, 100, _ => results1.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson1))),
            () => Parallel.For(0, 100, _ => results2.Add(SwaggerDescriptionInterrogator.GetPathDescriptions(swaggerJson2)))
        );

        _ = results1.Should().OnlyContain(r => r![0].Tags!.ContainsKey("Key1"));
        _ = results1.Should().OnlyContain(r => !r![0].Tags!.ContainsKey("Key2"));
        
        _ = results2.Should().OnlyContain(r => r![0].Tags!.ContainsKey("Key2"));
        _ = results2.Should().OnlyContain(r => !r![0].Tags!.ContainsKey("Key1"));
    }
}
