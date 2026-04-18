namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Alle geïndexeerde bedragen en tarieven voor AJ2026 (inkomsten 2025).
/// Bron: Practicali, FOD Financiën, Wetboek Inkomstenbelastingen.
/// </summary>
public static class TaxConstants2026
{
    // ── Belastingschijven (art. 130 WIB92) ──────────────────────────────────
    public static readonly (decimal Grens, decimal Vast, decimal Percentage)[] Schijven =
    [
        (16_320m, 0m, 0.25m),
        (28_800m, 4_080m, 0.40m),
        (49_840m, 9_072m, 0.45m),
        (decimal.MaxValue, 18_540m, 0.50m),
    ];

    // ── Barema belastingvrije som (apart barema sinds AJ2017) ───────────────
    public static readonly (decimal Grens, decimal Vast, decimal Percentage)[] BaremaVrijeSom =
    [
        (11_460m, 0m, 0.25m),
        (16_320m, 2_865m, 0.30m),
        (27_190m, 4_323m, 0.40m),
        (49_840m, 8_671m, 0.45m),
        (decimal.MaxValue, 18_863.50m, 0.50m),
    ];

    // ── Belastingvrije som ──────────────────────────────────────────────────
    public const decimal BelastingvrijeSomBasis = 10_910m;
    public const decimal VerhogingHandicapBelastingplichtige = 1_980m;

    // Kinderen ten laste
    public static readonly decimal[] VerhogingKinderen =
    [
        0m,          // 0 kinderen
        1_980m,      // 1 kind
        5_110m,      // 2 kinderen
        11_440m,     // 3 kinderen
        18_510m,     // 4 kinderen
    ];
    public const decimal VerhogingPerKindBoven4 = 7_070m;
    public const decimal ToeslagKindJongerDan3 = 740m;

    // Co-ouderschap: helft van de verhoging kinderen
    // Andere personen ten laste
    public const decimal VerhogingAnderePersonenTenLaste = 1_980m;

    // Ouders/grootouders ≥66 jaar zorgbehoevend
    public const decimal VerhogingAscendentZorgbehoevend = 5_950m;

    // Alleenstaande met kinderen
    public const decimal VerhogingAlleenstaandeMetKinderen = 1_980m;

    // Bijkomende toeslag alleenstaande ouder (niet-samenwonend)
    public const decimal MaxInkomenAlleenstaandeOuderToeslag = 24_390m;
    public const decimal MinBeroepsinkomenAlleenstaandeOuder = 4_100m;
    public const decimal MaxToeslagAlleenstaandeOuder = 1_290m;
    public const decimal BeginAfbouwAlleenstaandeOuder = 19_250m;
    public const decimal EindAfbouwAlleenstaandeOuder = 24_390m;

    // Jaar van huwelijk/wettelijke samenwoning
    public const decimal VerhogingJaarHuwelijk = 1_980m;
    public const decimal MaxBestaansmiddelenPartnerHuwelijk = 4_100m;

    // Belastingkrediet kinderen ten laste (bevroren sinds AJ2026)
    public const decimal MaxBelastingkredietKinderen = 550m;
    public const decimal MaxBelastingkredietCoOuderschap = 275m;

    // ── Forfaitaire beroepskosten ───────────────────────────────────────────
    // Werknemers en winstenbehalers
    public const decimal ForfaitWerknemersPercentage = 0.30m;
    public const decimal ForfaitWerknemersMaximum = 5_930m;

    // Batenbehalers
    public static readonly (decimal Grens, decimal Vast, decimal Percentage)[] ForfaitBaten =
    [
        (7_540m, 0m, 0.287m),
        (14_970m, 2_163.98m, 0.10m),
        (24_920m, 2_906.98m, 0.05m),
        (85_103.83m, 3_404.48m, 0.03m),
        (decimal.MaxValue, 5_210m, 0m),
    ];
    public const decimal ForfaitBatenMaximum = 5_210m;

    // Bedrijfsleiders
    public const decimal ForfaitBedrijfsleiderPercentage = 0.03m;
    public const decimal ForfaitBedrijfsleiderMaximum = 3_130m;

    // Meewerkende echtgenoot
    public const decimal ForfaitMeewerkPercentage = 0.05m;
    public const decimal ForfaitMeewerkMaximum = 5_210m;

    // Minimum aftrekbare kosten
    public const decimal MinimumAftrekbareKosten = 570m;

    // ── Huwelijksquotiënt ───────────────────────────────────────────────────
    public const decimal MaxHuwelijksquotient = 13_460m;

    // ── Autonomiefactor en gewestelijke opcentiemen ─────────────────────────
    public const decimal Autonomiefactor = 0.24957m;

    // Gewestelijke opcentiemen (op de gereduceerde federale belasting)
    public const decimal OpcentiemenVlaanderen = 0.33257m;
    public const decimal OpcentiemenWallonie = 0.33257m;
    public const decimal OpcentiemenBrussel = 0.32591m;

    // Maximale afwijking progressiviteitsregel
    public const decimal MaxAfwijkingProgressiviteit = 1_320m;

    // ── Belastingvermindering vervangingsinkomsten ──────────────────────────
    public const decimal VerminderingZiekteInvaliditeit = 2_977.93m;

    public const decimal VerminderingPensioenBasis = 2_219.27m;
    public const decimal VerminderingPensioenAanvullend = 457.23m;

    public const decimal VerminderingWerkloosheidBasis = 2_219.27m;
    public const decimal VerminderingWerkloosheidAanvullend = 457.23m;
    public const decimal AfbouwPercentageWerkloosheidAanvullend = 0.40m; // 40% voor AJ2026

    // Inkomensgrenzen afbouw vermindering pensioenen
    public const decimal GrensPensioenVerminderingVolledig = 28_780m;
    public const decimal GrensPensioenVerminderingAfbouwStart = 19_630m;
    public const decimal GrensPensioenVerminderingAfbouwBreedte = 9_150m;

    // Inkomensgrenzen afbouw vermindering werkloosheid
    public const decimal GrensWerkloosheidVerminderingVolledig = 28_780m;
    public const decimal GrensWerkloosheidVerminderingBreedte = 7_150m;

    // Bijkomende vermindering max inkomen
    public const decimal MaxInkomenBijkomendeVerminderingPensioen = 19_630m;
    public const decimal MaxInkomenBijkomendeVerminderingWerkloosheid = 19_630m;

    // ── Belastingkrediet lage activiteit / werkbonus ────────────────────────
    public const decimal MaxBelastingkredietLageActiviteit = 880m;
    public const decimal MaxBelastingkredietMeewerkend = 400m;
    public const decimal MaxBelastingkredietStatutair = 970m;
    public const decimal MaxBelastingkredietWerkbonus = 1_540m;

    // ── Onroerende inkomsten ────────────────────────────────────────────────
    public const decimal IndexatiecoeffKI = 2.2446m;
    public const decimal Revalorisatiecoeff = 5.63m;

    // ── Belastingverminderingen (federaal) ──────────────────────────────────
    public const decimal MaxPensioensparen30 = 1_050m;
    public const decimal MaxPensioensparen25 = 1_350m;
    public const decimal MaxWerkgeversaandelen = 820m;
    public const decimal MaxKinderoppasPerDag = 16.90m;
    public const decimal MinGift = 40m;
    public const decimal MaxGift = 408_130m;
    public const decimal PercentageGiften = 0.30m; // verlaagd van 45% naar 30% vanaf AJ2026

    // ── Vrijstellingen ──────────────────────────────────────────────────────
    public const decimal VrijstellingWoonWerkverkeerAndere = 490m; // bevroren AJ2026
    public const decimal VrijstellingFlexijobNietGepensioneerd = 18_000m;
    public const decimal VrijstellingBrandweerVrijwilligers = 7_540m;
    public const decimal VrijstellingNietRecurrent = 3_622m;
    public const decimal MaxVrijstellingSpaardepositos = 1_020m; // bevroren
    public const decimal VrijstellingDividenden = 833m; // bevroren

    // ── Woonbonus Vlaanderen ────────────────────────────────────────────────
    public const decimal WoonbonusVlBasis2014 = 2_280m;
    public const decimal WoonbonusVlBasis2015 = 1_520m;
    public const decimal WoonbonusVlVerhoging10j = 760m;
    public const decimal WoonbonusVlExtraKinderen = 80m;

    // ── Langetermijnsparen (federaal, bevroren op AJ2025) ───────────────────
    public const decimal MaxLangetermijnsparenFederaalAbsoluut = 2_450m;
    public const decimal MaxLangetermijnsparenFederaalEersteSchijf = 2_040m;
}
