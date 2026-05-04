# SOStax

**Belgische personenbelasting AJ2026 — desktop-app op basis van Blazor Hybrid**  
(Inkomstenjaar 2025 · Vlaams/Waals/Brussels Gewest · FOD Financiën)

SOStax is een volledig lokale desktoptoepassing die:
- alle **22 vakken** van de aangifte personenbelasting bevat met invoervelden,
- een **belastingberekening** uitvoert conform de regels voor AJ2026,
- de resultaten vergelijkt met de officiële **Tax-Calc-tool** van FOD Financiën,
- draait als **WPF-app** (Windows) en als **MAUI-app** (Windows, macOS, Android, iOS).

---

## Inhoud

- [Snel starten](#snel-starten)
- [Projectstructuur](#projectstructuur)
- [Architectuur](#architectuur)
- [Berekening-pipeline](#berekening-pipeline)
- [Een nieuw vak toevoegen](#een-nieuw-vak-toevoegen)
- [Verificatie met TestCalc](#verificatie-met-testcalc)
- [Technologie](#technologie)
- [Licentie](#licentie)

---

## Snel starten

Vereiste: **.NET 10 SDK**

```bash
# WPF-versie (Windows)
dotnet run --project BlazorTax.Wpf

# MAUI-versie (Windows)
dotnet run --project BlazorTax.Maui -f net10.0-windows10.0.19041.0

# Console-verificatie
dotnet run --project TestCalc
```

---

## Projectstructuur

```
SOStax/
├── BlazorTax.Shared/                  # Razor Class Library — alle gedeelde logica
│   ├── belastingen/                   # Domeinlaag
│   │   ├── AangifteState.cs           # Centrale state: alle 22 vakken + instellingen
│   │   ├── Vak*Data.cs                # Datamodellen per vak (één bestand per vak)
│   │   ├── BurgerlijkeStaatCodes.cs   # Enum-waarden burgerlijke staat
│   │   ├── Vak*.csv                   # Velddefinities per vak
│   │   ├── APB_2008_2026.csv          # Algemeen Pensioenbijdragenbestand
│   │   ├── structuur.md               # Tab-structuur van de app
│   │   ├── VakStructuurParser.cs      # Parseert structuur.md → VakSection-lijst
│   │   ├── VakIiFormParser.cs         # Parseert CSV-bestanden → formuliervelden
│   │   │
│   │   ├── Berekening/                # Belastingberekeningsengine
│   │   │   ├── TaxConstants2026.cs    # Alle geïndexeerde bedragen en tarieven AJ2026
│   │   │   ├── BerekeningResultaat.cs # Uitvoermodel + Gewest-enum
│   │   │   ├── PersonenbelastingCalculator.cs
│   │   │   ├── GezamenlijkeBerekeningCalculator.cs
│   │   │   ├── PartnerBelastingCalculator.cs
│   │   │   ├── PartnerInkomen.cs
│   │   │   ├── BelastingschijvenCalculator.cs
│   │   │   ├── BelastingvrijeSomCalculator.cs
│   │   │   ├── ForfaitaireBeroepskostenCalculator.cs
│   │   │   ├── HuwelijksquotientCalculator.cs
│   │   │   ├── VervangingsInkomstenCalculator.cs
│   │   │   ├── GewestelijkeVerminderingenCalculator.cs
│   │   │   ├── GemeentelijkeOpcentiemenCalculator.cs
│   │   │   └── GezamenlijkResultaat.cs
│   │   │
│   │   └── Validatie/
│   │       └── AangifteStateValidator.cs  # FluentValidation-regels
│   │
│   ├── Components/                    # Razor-formuliercomponenten
│   │   ├── VakIForm.razor … VakXXIIForm.razor
│   │   ├── BerekeningPaneel.razor     # Weergave berekeningsresultaten
│   │   └── SnelleInvoerForm.razor
│   │
│   ├── Pages/
│   │   ├── Belastingen.razor          # Hoofdpagina: tabs + routing per vak
│   │   ├── SnelleInvoer.razor         # Vereenvoudigd invoerscherm
│   │   ├── Home.razor
│   │   └── NotFound.razor
│   │
│   ├── Services/
│   │   ├── IAssetReader.cs            # Abstractie voor lezen van statische bestanden
│   │   ├── AangifteStateService.cs    # Validatie + berekeningsorchestrator
│   │   ├── IGemeenteAanslagvoetService.cs
│   │   ├── GemeenteAanslagvoetService.cs
│   │   └── GemeenteAanslagvoetData.cs
│   │
│   └── Layout/                        # Shell: navigatie + lay-out
│
├── BlazorTax.Wpf/                     # WPF-hostproject (Windows desktop)
│   ├── App.xaml.cs                    # DI-setup + service registratie
│   ├── MainWindow.xaml(.cs)           # BlazorWebView-venster
│   ├── WpfAssetReader.cs              # IAssetReader via bestandssysteem
│   └── Components/Routes.razor
│
├── BlazorTax.Maui/                    # MAUI-hostproject (cross-platform)
│   ├── MauiProgram.cs                 # DI-setup + service registratie
│   ├── MainPage.xaml(.cs)             # BlazorWebView-pagina
│   ├── MauiAssetReader.cs             # IAssetReader via FileSystem API
│   ├── Platforms/                     # Android / iOS / macOS / Windows
│   └── Components/Routes.razor
│
├── TestCalc/                          # Console-verificatieproject
│   └── Program.cs                     # Scenario's + vergelijking met FOD Tax-Calc
│
├── test_taxcalc.py                    # Python-script: scraping FOD Tax-Calc AJ2025
└── BlazorTax.sln
```

---

## Architectuur

De applicatie gebruikt het **Blazor Hybrid**-model: alle UI en bedrijfslogica zit in de gedeelde Razor Class Library `BlazorTax.Shared`. De platformprojecten (`BlazorTax.Wpf`, `BlazorTax.Maui`) zijn dunne hosts die enkel een `BlazorWebView` starten en platformspecifieke services registreren.

```
┌─────────────────────────────────────────────────────────────┐
│              BlazorTax.Wpf  /  BlazorTax.Maui               │
│  (BlazorWebView + DI-setup + platformspecifieke services)    │
└───────────────────────┬─────────────────────────────────────┘
                        │ ProjectReference
┌───────────────────────▼─────────────────────────────────────┐
│                    BlazorTax.Shared                          │
│                                                             │
│  Pages/Belastingen.razor                                    │
│    └─ laadt structuur.md + CSV-bestanden via IAssetReader   │
│    └─ rendert VakXForm-componenten per actieve tab          │
│                                                             │
│  AangifteState  ←──────────────────────────────────────┐   │
│    └─ VakIData … VakXXIIData                           │   │
│                                                        │   │
│  VakXForm.razor ──→ bindt aan VakXData-properties ─────┘   │
│                                                             │
│  AangifteStateService                                       │
│    ├─ AangifteStateValidator (FluentValidation)             │
│    └─ GezamenlijkeBerekeningCalculator                      │
│         └─ geeft GezamenlijkResultaat / BerekeningResultaat │
└─────────────────────────────────────────────────────────────┘
```

**`IAssetReader`-abstractie** ontkoppelt het lezen van CSV- en MD-bestanden van het platform:

| Platform | Implementatie | Bron |
|----------|---------------|------|
| WPF | `WpfAssetReader` | Bestandssysteem naast de exe |
| MAUI | `MauiAssetReader` | `FileSystem.OpenAppPackageFileAsync` |

**Gegevensstroming:**

```
CSV/MD-bestanden (belastingen/)
    ↓  via IAssetReader bij opstart
VakStructuurParser / VakIiFormParser
    ↓
Razor-formulieren (twee-weg binding)
    ↓
AangifteState
    ↓
AangifteStateService → AangifteStateValidator (FluentValidation)
    ↓
GezamenlijkeBerekeningCalculator (Berekening/)
    ↓
GezamenlijkResultaat (weergegeven in BerekeningPaneel)
```

---

## Berekening-pipeline

De berekening verloopt in vaste stappen, uitgevoerd door `GezamenlijkeBerekeningCalculator`:

| Stap | Klasse | Wat |
|------|--------|-----|
| 1 | `PartnerInkomen` | Inkomen extraheren per partner uit de vakken |
| 2 | `ForfaitaireBeroepskostenCalculator` | Forfait of werkelijke beroepskosten |
| 3 | `HuwelijksquotientCalculator` | Herverdeling tot 30 % bij gehuwden |
| 4 | `BelastingschijvenCalculator` | Progressieve tarieven (25 %–50 %) |
| 5 | `BelastingvrijeSomCalculator` | Basis + verhogingen (kinderen, handicap …) |
| 6 | `VervangingsInkomstenCalculator` | Vermindering op uitkeringen/pensioenen |
| 7 | `GewestelijkeVerminderingenCalculator` | Vlaamse / Waalse / Brusselse kortingen |
| 8 | `GemeentelijkeOpcentiemenCalculator` | Gemeentebelasting op federale belasting |

Alle tarieven en drempelwaarden staan gecentraliseerd in **`TaxConstants2026.cs`**.

---

## Een nieuw vak toevoegen

Elk vak volgt dit vaste patroon. Volg de stappen in volgorde:

### 1. Datamodel (`BlazorTax.Shared/belastingen/VakXXXData.cs`)

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

### 3. CSV-velddefinitie (`BlazorTax.Shared/belastingen/VAKXXX.csv`)

```
Code;Omschrijving;Type
1234;Omschrijving van het veld;number
```

### 4. Razor-component (`BlazorTax.Shared/Components/VakXXXForm.razor`)

```razor
@using BlazorTax.Belastingen

<h3>VAK XXX — Titel</h3>
@* Gebruik dezelfde opmaak als bestaande Vak*Form.razor-bestanden *@
```

### 5. Tab registreren (`BlazorTax.Shared/belastingen/structuur.md`)

Voeg een `## VAK XXX — Titel`-sectie toe op de juiste positie in `structuur.md`.

### 6. Component koppelen (`BlazorTax.Shared/Pages/Belastingen.razor`)

```csharp
else if (string.Equals(activeSection.Code, "VAK XXX", StringComparison.OrdinalIgnoreCase))
{
    <VakXXXForm Data="_state.VakXXX" />
}
```

---

## Verificatie met TestCalc

Het project `TestCalc/` bevat scenario's die de berekeningsengine vergelijken met de
officiële **Tax-Calc AJ2026** van FOD Financiën:

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
| UI-framework | Blazor Hybrid (.NET 10) |
| Taal | C# 13 |
| Desktop (Windows) | WPF + BlazorWebView (`Microsoft.AspNetCore.Components.WebView.Wpf`) |
| Desktop + mobiel | .NET MAUI + BlazorWebView (`Microsoft.AspNetCore.Components.WebView.Maui`) |
| Validatie | FluentValidation 11 |
| CSV/MD-parsing | Eigen parsers (`VakStructuurParser`, `VakIiFormParser`) |
| Asset-abstractie | `IAssetReader` (platformspecifieke implementaties) |
| Testen | Console-scenario's (`TestCalc`) + Python-scraping |

---

## Licentie

Dit project is vrijgegeven onder de **GNU General Public License v3.0**.  
Zie [https://www.gnu.org/licenses/gpl-3.0.html](https://www.gnu.org/licenses/gpl-3.0.html) voor de volledige licentietekst.

Bijdragen zijn welkom via pull requests op de `master`-branch.
