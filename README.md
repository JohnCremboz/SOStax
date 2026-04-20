# SOStax

**Blazor WebAssembly-toepassing voor de Belgische personenbelasting AJ2026**  
(Inkomstenjaar 2025 · Vlaams Gewest · FOD Financiën)

SOStax is een volledig client-side webtoepassing die:
- alle **22 vakken** van de aangifte personenbelasting bevat met invoervelden,
- een **belastingberekening** uitvoert conform de regels voor AJ2026,
- de resultaten vergelijkt met de officiële **Tax-Calc-tool** van FOD Financiën.

---

## Inhoud

- [Snel starten](#snel-starten)
- [Projectstructuur](#projectstructuur)
- [Architectuur](#architectuur)
- [Berekening-pipeline](#berekening-pipeline)
- [Een nieuw vak toevoegen](#een-nieuw-vak-toevoegen)
- [Verificatie met TestCalc](#verificatie-met-testcalc)

---

## Snel starten

```bash
# Vereiste: .NET 10 SDK
dotnet run --project BlazorTax
```

De app is daarna beschikbaar op `https://localhost:5001` (of de poort in `launchSettings.json`).

---

## Projectstructuur

```
SOStax/
├── BlazorTax/                         # Blazor WASM-hoofdproject
│   ├── Program.cs                     # App-bootstrapping
│   ├── App.razor                      # Root-component
│   │
│   ├── belastingen/                   # Domeinlaag
│   │   ├── AangifteState.cs           # Centrale state: alle 22 vakken + instellingen
│   │   ├── Vak*Data.cs                # Datamodellen per vak (één bestand per vak)
│   │   ├── Vak*.csv / VAKI*.csv       # Velddefinities geladen via HTTP bij opstart
│   │   ├── structuur.md               # Markdown met de tab-structuur van de app
│   │   ├── VakStructuurParser.cs      # Parseert structuur.md → VakSection-lijst
│   │   ├── VakIiFormParser.cs         # Parseert CSV-bestanden → formuliervelden
│   │   │
│   │   └── Berekening/                # Belastingberekeningsengine
│   │       ├── TaxConstants2026.cs    # Alle geïndexeerde bedragen en tarieven AJ2026
│   │       ├── BerekeningResultaat.cs # Uitvoermodel + Gewest-enum
│   │       ├── PersonenbelastingCalculator.cs      # Berekening voor één persoon
│   │       ├── GezamenlijkeBerekeningCalculator.cs # Twee-kolomsberekening (gehuwden)
│   │       ├── PartnerBelastingCalculator.cs       # Belasting per partner
│   │       ├── PartnerInkomen.cs                   # Inkomensextractie per partner
│   │       ├── BelastingschijvenCalculator.cs      # Progressieve tarieven (art. 130 WIB92)
│   │       ├── BelastingvrijeSomCalculator.cs      # Belastingvrije som + gezinslasten
│   │       ├── ForfaitaireBeroepskostenCalculator.cs # Forfait vs werkelijke kosten
│   │       ├── HuwelijksquotientCalculator.cs      # Huwelijksquotiënt (30 %-regel)
│   │       ├── VervangingsInkomstenCalculator.cs   # Vermindering op vervangingsinkomsten
│   │       ├── GewestelijkeVerminderingenCalculator.cs # Vlaamse/Waalse/Brusselse korting
│   │       ├── GemeentelijkeOpcentiemenCalculator.cs   # Gemeentebelasting
│   │       ├── GezamenlijkResultaat.cs             # Uitvoermodel gemeenschappelijke aanslag
│   │       └── BerekeningInput.cs (via AangifteState) # Invoermodel voor de engine
│   │
│   ├── Components/                    # Razor-formuliercomponenten
│   │   ├── VakIForm.razor … VakXXIIForm.razor  # Eén component per vak
│   │   └── SnelleInvoerForm.razor     # Vereenvoudigd invoerscherm
│   │
│   ├── Pages/
│   │   ├── Belastingen.razor          # Hoofdpagina: tabs + routing per vak
│   │   └── Home.razor                 # Welkomstpagina
│   │
│   └── Layout/                        # Shell: navigatie + lay-out
│
├── TestCalc/                          # Console-verificatieproject
│   ├── Program.cs                     # Scenario's + vergelijking met FOD Tax-Calc
│   └── TestCalc.csproj
│
├── test_taxcalc.py                    # Python-script: scraping FOD Tax-Calc AJ2025
└── BlazorTax.sln
```

---

## Architectuur

```
┌─────────────────────────────────────────────────────────────┐
│                     Browser (WASM)                          │
│                                                             │
│  Pages/Belastingen.razor                                    │
│    └─ laadt structuur.md + CSV-bestanden via HttpClient     │
│    └─ rendert VakXForm-componenten per actieve tab          │
│                                                             │
│  AangifteState  ←──────────────────────────────────────┐   │
│    └─ VakIData … VakXXIIData                           │   │
│                                                        │   │
│  VakXForm.razor ──→ bindt aan VakXData-properties ─────┘   │
│                                                             │
│  PersonenbelastingCalculator / GezamenlijkeBerekeningCalc   │
│    └─ leest AangifteState → geeft BerekeningResultaat       │
└─────────────────────────────────────────────────────────────┘

Gegevensstroming:
  CSV/MD-bestanden (wwwroot/belastingen/)
      ↓  HTTP GET bij opstart
  VakStructuurParser / VakIiFormParser
      ↓
  Razor-formulieren (twee-weg binding)
      ↓
  AangifteState
      ↓
  Berekeningsengine (Berekening/)
      ↓
  BerekeningResultaat (weergegeven in de UI)
```

---

## Berekening-pipeline

De berekening verloopt in vaste stappen, uitgevoerd door `GezamenlijkeBerekeningCalculator`:

| Stap | Klasse | Wat |
|------|--------|-----|
| 1 | `PartnerInkomen` | Inkomen extraheren per partner uit de vakken |
| 2 | `ForfaitaireBeroepskostenCalculator` | Forfait of werkelijke beroepskosten |
| 3 | `HuwelijksquotientCalculator` | Herverdelng tot 30 % bij gehuwden |
| 4 | `BelastingschijvenCalculator` | Progressieve tarieven (25 %–50 %) |
| 5 | `BelastingvrijeSomCalculator` | Basis + verhogingen (kinderen, handicap …) |
| 6 | `VervangingsInkomstenCalculator` | Vermindering op uitkeringen/pensioenen |
| 7 | `GewestelijkeVerminderingenCalculator` | Vlaamse / Waalse / Brusselse kortingen |
| 8 | `GemeentelijkeOpcentiemenCalculator` | Gemeentebelasting op federale belasting |

Alle tarieven en drempelwaarden staan gecentraliseerd in **`TaxConstants2026.cs`**.

---

## Een nieuw vak toevoegen

Elk vak volgt dit vaste patroon. Volg de stappen in volgorde:

### 1. Datamodel (`belastingen/VakXXXData.cs`)

```csharp
namespace BlazorTax.Belastingen;

public class VakXXXData
{
    public decimal? Code1234 { get; set; }   // veld belastingplichtige
    public decimal? Code2234 { get; set; }   // zelfde veld voor partner
}
```

### 2. Veld toevoegen aan `AangifteState`

```csharp
public VakXXXData VakXXX { get; set; } = new();
```

### 3. CSV-velddefinitie (`belastingen/VAKXXX.csv`)

```
Code;Omschrijving;Type
1234;Omschrijving van het veld;number
```

### 4. Razor-component (`Components/VakXXXForm.razor`)

```razor
@using BlazorTax.Belastingen

<h3>VAK XXX — Titel</h3>
@* Gebruik dezelfde opmaak als bestaande Vak*Form.razor-bestanden *@
```

### 5. Tab registreren (`belastingen/structuur.md`)

Voeg een `## VAK XXX — Titel`-sectie toe op de juiste positie in `structuur.md`.

### 6. Component koppelen (`Pages/Belastingen.razor`)

```csharp
else if (string.Equals(activeSection.Code, "VAK XXX", StringComparison.OrdinalIgnoreCase))
{
    <VakXXXForm Data="_state.VakXXX" />
}
```

---

## Verificatie met TestCalc

Het project `TestCalc/` bevat scenario's die de berekeningsengine vergelijken met de
officiële **Tax-Calc AJ2025** van FOD Financiën:

```bash
dotnet run --project TestCalc
```

Elk scenario print een gedetailleerde stap-voor-stap afrekening en markeert
afwijkingen ten opzichte van de referentiewaarden.

Het Python-script `test_taxcalc.py` automatiseert het ophalen van referentiewaarden
via de webinterface van Tax-Calc (cookies + POST-formulieren).

---

## Technologie

| Component | Keuze |
|-----------|-------|
| UI-framework | Blazor WebAssembly (.NET 10) |
| Taal | C# 13 |
| Hosting | Volledig client-side (geen server vereist) |
| CSV/MD-parsing | Eigen parsers (`VakStructuurParser`, `VakIiFormParser`) |
| Testen | Console-scenario's (`TestCalc`) + Python-scraping |

---

## Licentie

Dit project is vrijgegeven onder de **GNU General Public License v3.0**.  
Zie [https://www.gnu.org/licenses/gpl-3.0.html](https://www.gnu.org/licenses/gpl-3.0.html) voor de volledige licentietekst.

Bijdragen zijn welkom via pull requests op de `master`-branch.
