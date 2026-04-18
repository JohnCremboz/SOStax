namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK I – Bankrekening en telefoonnummer(s).</summary>
public class VakIData
{
    // ── 1. Bankrekening ──────────────────────────────────────────────────────
    /// IBAN-rekeningnummer (alleen in te vullen als u het te wijzigen of nieuw op te geven rekeningnummer wilt invullen)
    public string? Iban { get; set; }

    /// BIC-code (alleen verplicht als het een rekening in het buitenland betreft)
    public string? Bic { get; set; }

    // ── 2. Telefoonnummer(s) ─────────────────────────────────────────────────
    /// Telefoonnummer belastingplichtige
    public string? TelefoonBelastingplichtige { get; set; }

    /// Telefoonnummer partner
    public string? TelefoonPartner { get; set; }
}

