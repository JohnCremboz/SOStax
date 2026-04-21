using System.Text;

namespace BlazorTax.Belastingen;

public sealed record VakSection(string Code, string Title, string Content);

public static class VakStructuurParser
{
    public static IReadOnlyList<VakSection> ParseSections(string markdown)
    {
        var sections = new List<VakSection>();
        using var reader = new StringReader(markdown.Replace("\r\n", "\n", StringComparison.Ordinal));

        string? line;
        string? currentHeading = null;
        var contentBuilder = new StringBuilder();

        while ((line = reader.ReadLine()) is not null)
        {
            var trimmedLine = line.TrimStart();

            if (trimmedLine.StartsWith("## VAK ", StringComparison.OrdinalIgnoreCase))
            {
                if (currentHeading is not null)
                {
                    sections.Add(BuildVakSection(currentHeading, contentBuilder.ToString()));
                }

                currentHeading = trimmedLine[3..].Trim();
                contentBuilder.Clear();
                continue;
            }

            if (currentHeading is not null)
            {
                contentBuilder.AppendLine(line);
            }
        }

        if (currentHeading is not null)
        {
            sections.Add(BuildVakSection(currentHeading, contentBuilder.ToString()));
        }

        return sections;
    }

    private static VakSection BuildVakSection(string heading, string content)
    {
        var separatorIndex = heading.IndexOf('—');
        var code = separatorIndex >= 0 ? heading[..separatorIndex].Trim() : heading;

        return new VakSection(
            Code: code,
            Title: heading,
            Content: content.Trim());
    }
}

