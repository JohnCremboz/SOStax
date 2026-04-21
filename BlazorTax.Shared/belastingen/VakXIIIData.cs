namespace BlazorTax.Belastingen;

public class VakXIIIData
{
    // A. Rekeningen in het buitenland
    public bool Code1075 { get; set; }   // Ja
    public List<BuitenlandseRekeningItem> Rekeningen { get; set; } = [new()];

    // B. Individuele levensverzekeringen
    public bool Code1076 { get; set; }   // belastingplichtige
    public bool Code1076Partner { get; set; }  // partner
    public List<BuitenlandseVerzekeringItem> Verzekeringen { get; set; } = [new()];

    // C. Juridische constructies
    public bool Code1077 { get; set; }
    public List<JuridischeConstructieItem> Constructies { get; set; } = [new()];

    // D. Leningen startende vennootschappen
    public int? Code1088 { get; set; }
    public int? Code2088 { get; set; }

    // E. Werkelijke beroepskosten
    public bool HuurAlsBeroepskosten { get; set; }
    public bool Code1072 { get; set; }   // vakje aankruisen + bijlage 270 MLH
    public bool Code2072 { get; set; }
}

public class BuitenlandseRekeningItem
{
    public string NaamTitularis  { get; set; } = string.Empty;
    public string Land           { get; set; } = string.Empty;
    public bool   NbbMelding     { get; set; }
}

public class BuitenlandseVerzekeringItem
{
    public string NaamVerzekeringnemer { get; set; } = string.Empty;
    public string Land                 { get; set; } = string.Empty;
}

public class JuridischeConstructieItem
{
    public string Omschrijving { get; set; } = string.Empty;
    /// "oprichter" | "genieter" | ""
    public string Hoedanigheid { get; set; } = string.Empty;
}

