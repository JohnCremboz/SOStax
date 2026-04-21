namespace BlazorTax.Belastingen;

using BlazorTax.Belastingen.Berekening;

/// <summary>Shared state containing all form data for the full tax declaration.</summary>
public class AangifteState
{
    public VakIData VakI { get; set; } = new();
    public VakIIData VakII { get; set; } = new();
    public VakIIIData VakIII { get; set; } = new();
    public VakIVData VakIV { get; set; } = new();
    public VakVData VakV { get; set; } = new();
    public VakVIData VakVI { get; set; } = new();
    public VakVIIData VakVII { get; set; } = new();
    public VakVIIIData VakVIII { get; set; } = new();
    public VakIXData VakIX { get; set; } = new();
    public VakXData VakX { get; set; } = new();
    public VakXIData VakXI { get; set; } = new();
    public VakXIIData VakXII { get; set; } = new();
    public VakXIIIData VakXIII { get; set; } = new();

    // Deel 2
    public VakXIVData VakXIV { get; set; } = new();
    public VakXVData VakXV { get; set; } = new();
    public VakXVIData VakXVI { get; set; } = new();
    public VakXVIIData VakXVII { get; set; } = new();
    public VakXVIIIData VakXVIII { get; set; } = new();
    public VakXIXData VakXIX { get; set; } = new();
    public VakXXData VakXX { get; set; } = new();
    public VakXXIData VakXXI { get; set; } = new();
    public VakXXIIData VakXXII { get; set; } = new();

    // Berekeningsinstellingen
    public Gewest Gewest { get; set; } = Gewest.Vlaanderen;
    public decimal GemeentebelastingPercentage { get; set; } = 7m;
    public TypeBeroep TypeBeroep { get; set; } = TypeBeroep.Werknemer;
    public decimal NettoInkomenPartner { get; set; }
}
