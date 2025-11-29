namespace GUtilsTests.SwaggerDescriptionBuilderTests;

using FluentAssertions;
using GUtils.SwaggerDescriptionBuilder;

public sealed class Performance_Should
{
    [Fact]
    public void HandleExtremelyLargeNumberOfChainedCalls()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        for (var i = 0; i < 10000; i++)
        {
            _ = builder.WithTitle($"Title {i}");
            _ = builder.WithTag($"Tag{i}", $"Value{i}");
        }

        var result = builder.Build();
        
        _ = result.Should().NotBeNullOrEmpty();
        _ = result.Should().Contain("Title 0");
        _ = result.Should().Contain("Title 9999");
        _ = result.Should().Contain("Tag0");
        _ = result.Should().Contain("Tag9999");
    }

    [Fact]
    public void HandleExtremelyLongSingleTitle()
    {
        var longTitle = new string('X', 100000);
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTitle(longTitle)
            .Build();

        _ = actual.Should().Contain(longTitle);
        _ = actual.Should().StartWith("##");
    }

    [Fact]
    public void HandleExtremelyLongSingleTagValue()
    {
        var longValue = new string('Y', 100000);
        
        var actual = SwaggerDescriptionBuilder
            .Create()
            .WithTag("Name", longValue)
            .Build();

        _ = actual.Should().Contain(longValue);
    }

    [Fact]
    public void HandleManyTitlesOnly()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        for (var i = 0; i < 1000; i++)
        {
            _ = builder.WithTitle($"Title {i}");
        }

        var result = builder.Build();
        
        _ = result.Should().Contain("Title 0");
        _ = result.Should().Contain("Title 500");
        _ = result.Should().Contain("Title 999");
    }

    [Fact]
    public void HandleManyTagsOnly()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        for (var i = 0; i < 1000; i++)
        {
            _ = builder.WithTag($"Tag{i}", $"Value{i}");
        }

        var result = builder.Build();
        
        _ = result.Should().Contain("Tag0");
        _ = result.Should().Contain("Tag500");
        _ = result.Should().Contain("Tag999");
    }

    [Fact]
    public void BuildMultipleTimesOnLargeBuilder()
    {
        var builder = SwaggerDescriptionBuilder.Create();

        for (var i = 0; i < 100; i++)
        {
            _ = builder.WithTitle($"Title {i}");
            _ = builder.WithTag($"Tag{i}", $"Value{i}");
        }

        var results = new List<string>();
        for (var i = 0; i < 100; i++)
        {
            results.Add(builder.Build());
        }

        _ = results.Should().OnlyContain(r => r == results[0]);
    }

    [Fact]
    public void HandleVeryDeepChaining()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("T1").WithTag("Tag1", "V1")
            .WithTitle("T2").WithTag("Tag2", "V2")
            .WithTitle("T3").WithTag("Tag3", "V3")
            .WithTitle("T4").WithTag("Tag4", "V4")
            .WithTitle("T5").WithTag("Tag5", "V5")
            .WithTitle("T6").WithTag("Tag6", "V6")
            .WithTitle("T7").WithTag("Tag7", "V7")
            .WithTitle("T8").WithTag("Tag8", "V8")
            .WithTitle("T9").WithTag("Tag9", "V9")
            .WithTitle("T10").WithTag("Tag10", "V10");

        var result = builder.Build();
        
        _ = result.Should().Contain("T1");
        _ = result.Should().Contain("T10");
        _ = result.Should().Contain("Tag1");
        _ = result.Should().Contain("Tag10");
    }

    [Fact]
    public void BeThreadSafe_SingleSharedBuilderInstance()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var threadCount = 100;
        var itemsPerThread = 10;

        _ = Parallel.For(0, threadCount, i =>
        {
            for (var j = 0; j < itemsPerThread; j++)
            {
                _ = builder.WithTitle($"Thread{i}-Title{j}");
                _ = builder.WithTag($"Thread{i}-Tag{j}", $"Value{i}-{j}");
            }
        });

        var result = builder.Build();

        // Verify all threads' data is present
        for (var i = 0; i < threadCount; i++)
        {
            for (var j = 0; j < itemsPerThread; j++)
            {
                _ = result.Should().Contain($"Thread{i}-Title{j}");
                _ = result.Should().Contain($"Thread{i}-Tag{j}");
                _ = result.Should().Contain($"Value{i}-{j}");
            }
        }

        // Verify correct total number of lines
        var lines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        _ = lines.Should().HaveCount(threadCount * itemsPerThread * 2); // Each iteration adds 2 lines
    }

    [Fact]
    public void BeThreadSafe_ConcurrentBuildsOnSharedInstance()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        
        _ = builder.WithTitle("Initial Title");
        _ = builder.WithTag("Initial Tag", "Initial Value");

        var results = new string[100];

        _ = Parallel.For(0, 100, i => results[i] = builder.Build());

        // All builds should return the same result
        _ = results.Should().OnlyContain(r => r == results[0]);
        _ = results[0].Should().Contain("Initial Title");
        _ = results[0].Should().Contain("Initial Tag");
    }

    [Fact]
    public async Task BeThreadSafe_ConcurrentModificationsAndBuilds()
    {
        var builder = SwaggerDescriptionBuilder.Create();
        var barrier = new Barrier(2);
        var results = new List<string>();
        var resultsLock = new object();

        var modifyTask = Task.Run(() =>
        {
            barrier.SignalAndWait(); // Synchronize start
            for (var i = 0; i < 50; i++)
            {
                _ = builder.WithTitle($"Title {i}");
                _ = builder.WithTag($"Tag{i}", $"Value{i}");
                Thread.Sleep(1); // Small delay to increase contention
            }
        });

        var buildTask = Task.Run(() =>
        {
            barrier.SignalAndWait(); // Synchronize start
            for (var i = 0; i < 50; i++)
            {
                var result = builder.Build();
                lock (resultsLock)
                {
                    results.Add(result);
                }

                Thread.Sleep(1); // Small delay to increase contention
            }
        });

        await Task.WhenAll(modifyTask, buildTask);

        // Each successive build should have same or more content
        for (var i = 1; i < results.Count; i++)
        {
            _ = results[i].Length.Should().BeGreaterThanOrEqualTo(results[i - 1].Length);
        }

        // Final state should contain all modifications
        var finalResult = builder.Build();
        for (var i = 0; i < 50; i++)
        {
            _ = finalResult.Should().Contain($"Title {i}");
            _ = finalResult.Should().Contain($"Tag{i}");
        }
    }
}
