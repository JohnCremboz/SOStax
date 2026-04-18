namespace BlazorTax.Belastingen;

public class VakXIData
{
    // 1. Jaarlijks belastingkrediet — Winwinleningen
    public decimal? Code3377 { get; set; }   // uitstaand saldo 1.1.2025
    public decimal? Code4377 { get; set; }
    public decimal? Code3378 { get; set; }   // uitstaand saldo 31.12.2025
    public decimal? Code4378 { get; set; }

    // 1. Jaarlijks belastingkrediet — Vriendenaandelen
    public decimal? Code3376 { get; set; }   // volgestort bedrag
    public decimal? Code4376 { get; set; }

    // 2. Eenmalig belastingkrediet — definitief verloren hoofdsom
    public decimal? Code3368 { get; set; }   // winwinleningen 2020–2021
    public decimal? Code4368 { get; set; }
    public decimal? Code3379 { get; set; }   // winwinleningen vóór 2020 en na 2021
    public decimal? Code4379 { get; set; }
}

