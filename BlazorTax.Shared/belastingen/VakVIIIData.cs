namespace BlazorTax.Belastingen;

public class VakVIIIData
{
    // 1. Beroepsverliezen
    public decimal? Code1350 { get; set; }   // feitelijke vereniging
    public decimal? Code2350 { get; set; }
    public decimal? Code1349 { get; set; }   // andere werkzaamheid
    public decimal? Code2349 { get; set; }

    // 2. Onderhoudsuitkeringen
    public decimal? Code1390 { get; set; }   // verschuldigd door uzelf
    public decimal? Code2390 { get; set; }
    public decimal? Code1392 { get; set; }   // verschuldigd door beide partners

    // Genieter(s)
    public List<SchuldenaarItem> Genieter { get; set; } = [new()];

    // 3. Bijzondere bijdragen 1982-1988
    public decimal? Code1388 { get; set; }
}

