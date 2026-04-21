namespace BlazorTax.Belastingen;

public class VakVIData
{
    // 1. Niet-gekapitaliseerde uitkeringen
    public decimal? Code1192 { get; set; }
    public decimal? Code2192 { get; set; }

    // 2. Uitkeringen met terugwerkende kracht
    public decimal? Code1193 { get; set; }
    public decimal? Code2193 { get; set; }

    // 3. Gekapitaliseerde uitkeringen
    public string? Code1195 { get; set; }   // datum
    public string? Code2195 { get; set; }
    public decimal? Code1194 { get; set; }   // fictief jaarbedrag
    public decimal? Code2194 { get; set; }
    public decimal? Code1196 { get; set; }   // bedrag kapitaal
    public decimal? Code2196 { get; set; }

    // 4. Schuldenaars
    public List<SchuldenaarItem> Rijksinwoners  { get; set; } = [new()];
    public List<SchuldenaarItem> NietRijksinwoners { get; set; } = [new()];
}

public class SchuldenaarItem
{
    public string Naam { get; set; } = string.Empty;
}

