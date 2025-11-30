namespace GUtils.SwaggerDescriptionInterrogator;

using System.Text.Json;

public static class SwaggerDescriptionInterrogator
{
    private static readonly char[] LineSeparators = ['\r', '\n'];
    
    public static List<ApiPathDescription>? GetPathDescriptions(string swaggerJson)
    {
        ArgumentNullException.ThrowIfNull(swaggerJson);

        try
        {
            using var document = JsonDocument.Parse(swaggerJson);
            var root = document.RootElement;

            if (!root.TryGetProperty("paths", out var pathsElement))
            {
                return null;
            }

            List<ApiPathDescription>? pathDescriptions = null;

            foreach (var path in pathsElement.EnumerateObject())
            {
                var pathName = path.Name;

                foreach (var method in path.Value.EnumerateObject())
                {
                    if (!method.Value.TryGetProperty("description", out var descriptionElement))
                    {
                        continue;
                    }

                    var description = descriptionElement.GetString();
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        continue;
                    }

                    var apiPathDescription = ParseDescription(pathName, description);
                    if (apiPathDescription != null)
                    {
                        pathDescriptions ??= [];
                        pathDescriptions.Add(apiPathDescription);
                    }
                }
            }

            return pathDescriptions;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static ApiPathDescription? ParseDescription(string pathName, string description)
    {
        List<string>? titles = null;
        Dictionary<string, string>? tags = null;

        var span = description.AsSpan();
        var currentIndex = 0;

        while (currentIndex < span.Length)
        {
            var lineStart = currentIndex;
            var lineEnd = span[currentIndex..].IndexOfAny(LineSeparators);
            
            if (lineEnd == -1)
            {
                lineEnd = span.Length - currentIndex;
            }

            var line = span.Slice(lineStart, lineEnd).Trim();

            if (!line.IsEmpty)
            {
                if (TryParseTitle(line, out var title))
                {
                    titles ??= [];
                    titles.Add(title);
                }
                else if (TryParseTag(line, out var tagName, out var tagValue))
                {
                    tags ??= [];
                    tags[tagName] = tagValue;
                }
            }

            currentIndex = lineStart + lineEnd;
            while (currentIndex < span.Length && (span[currentIndex] == '\r' || span[currentIndex] == '\n'))
            {
                currentIndex++;
            }
        }

        return titles == null && tags == null
            ? null
            : new ApiPathDescription
            {
                Name = pathName,
                Titles = titles,
                Tags = tags
            };
    }

    private static bool TryParseTitle(ReadOnlySpan<char> line, out string title)
    {
        title = null!;
        
        if (line.Length > 3 && line[0] == '#' && line[1] == '#' && line[2] == ' ')
        {
            var possibleTitle = line[3..].Trim();
            if (!possibleTitle.IsEmpty)
            {
                title = possibleTitle.ToString();
                return true;
            }
        }

        return false;
    }

    private static bool TryParseTag(ReadOnlySpan<char> line, out string tagName, out string tagValue)
    {
        tagName = null!;
        tagValue = null!;
        
        if (line.Length <= 2 || line[0] != '-' || line[1] != ' ')
        {
            return false;
        }

        var tagContent = line[2..].Trim();
        var colonIndex = tagContent.IndexOf(':');
        
        if (colonIndex <= 0)
        {
            return false;
        }

        var possibleTagName = tagContent[..colonIndex].Trim();
        
        if (possibleTagName.IsEmpty)
        {
            return false;
        }

        var possibleTagValue = tagContent[(colonIndex + 1)..].Trim();
        tagName = possibleTagName.ToString();
        tagValue = possibleTagValue.ToString();
        return true;
    }
}
