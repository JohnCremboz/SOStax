namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XIV – Beroep en Ondernemingsnummer</summary>
public class VakXIVData
{
    // Belastingplichtige
    public string Beroep1 { get; set; } = string.Empty;
    public string Ondernemingsnummer1 { get; set; } = string.Empty;

    // Partner
    public string Beroep2 { get; set; } = string.Empty;
    public string Ondernemingsnummer2 { get; set; } = string.Empty;
}
