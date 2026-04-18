namespace BlazorTax.Belastingen;

public enum VakIiInputType
{
    Checkbox,
    Number,
    Text
}

public sealed class VakIiFieldState
{
    public required string Code { get; init; }
    public required string Omschrijving { get; init; }
    public required VakIiInputType InputType { get; init; }

    public bool BoolValue { get; set; }
    public decimal? NumberValue { get; set; }
    public string TextValue { get; set; } = string.Empty;
}

public static class VakIiFormParser
{
    public static IReadOnlyList<VakIiFieldState> ParseFields(string csv)
    {
        var fields = new List<VakIiFieldState>();
        using var reader = new StringReader(csv.Replace("\r\n", "\n", StringComparison.Ordinal));

        var isHeaderSkipped = false;
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (!isHeaderSkipped)
            {
                isHeaderSkipped = true;
                continue;
            }

            var separatorIndex = line.IndexOf(',');
            if (separatorIndex <= 0 || separatorIndex >= line.Length - 1)
            {
                continue;
            }

            var code = line[..separatorIndex].Trim();
            var omschrijving = line[(separatorIndex + 1)..].Trim();

            fields.Add(new VakIiFieldState
            {
                Code = code,
                Omschrijving = omschrijving,
                InputType = InferInputType(omschrijving)
            });
        }

        return fields;
    }

    private static VakIiInputType InferInputType(string omschrijving)
    {
        if (omschrijving.Contains("(Ja)", StringComparison.OrdinalIgnoreCase) ||
            omschrijving.Contains("(Neen)", StringComparison.OrdinalIgnoreCase))
        {
            return VakIiInputType.Checkbox;
        }

        if (omschrijving.Contains("aantal", StringComparison.OrdinalIgnoreCase) ||
            omschrijving.Contains("maanden", StringComparison.OrdinalIgnoreCase) ||
            omschrijving.Contains("euro", StringComparison.OrdinalIgnoreCase))
        {
            return VakIiInputType.Number;
        }

        return VakIiInputType.Text;
    }
}

