# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SOStax is a Belgian personal income tax calculation desktop app (aanslagjaar 2026 / inkomstenjaar 2025). It is built with Blazor Hybrid: a single shared Razor Class Library hosts all UI and business logic, with thin platform hosts for WPF (Windows) and MAUI (cross-platform).

## Build & Run Commands

Requires .NET 10 SDK.

```powershell
# Run WPF app (Windows)
dotnet run --project BlazorTax.Wpf

# Run MAUI app (Windows target)
dotnet run --project BlazorTax.Maui -f net10.0-windows10.0.19041.0

# Build solution
dotnet build BlazorTax.sln

# Run tax calculation verification tests (console app)
dotnet run --project TestCalc
```

## Architecture

### Shared Core: `BlazorTax.Shared` (Razor Class Library)

All domain logic, UI components, and services live here. Platform hosts are thin wrappers.

**Domain layer** (`BlazorTax.Shared/belastingen/`):
- `AangifteState.cs` — central state container holding all 22 form datasets
- `VakIData.cs` … `VakXXIIData.cs` — data models for each tax form (vak = section)
- `VAKXX.csv` files — field metadata (label, code, type) for each form
- `structuur.md` — tab/navigation structure definition (parsed at runtime)

**Calculation engine** (`BlazorTax.Shared/belastingen/Berekening/`):
- `GezamenlijkeBerekeningCalculator.cs` — main orchestrator, runs an 8-step pipeline for Belgian personal income tax
- `TaxConstants2026.cs` — all indexed 2026 tax rates, brackets, and thresholds (single source of truth)
- Specialized calculators: `PersonenbelastingCalculator`, `PartnerBelastingCalculator`, and 6 step-specific calculators (brackets, exemptions, spouse quotient, deductions, regional, municipal)

**8-step calculation pipeline:**
1. Extract income per partner from all 22 forms
2. Forfait or actual professional costs
3. Spouse quotient redistribution (30% max)
4. Progressive tax brackets (25%–50%)
5. Tax-free exemption + increments (children, disability)
6. Replacement income deductions
7. Regional deductions (Flemish/Walloon/Brussels via `Gewest` enum)
8. Municipal surtax on federal tax

**Validation** (`BlazorTax.Shared/belastingen/Validatie/`): FluentValidation 11 rules per form.

**Services** (`BlazorTax.Shared/Services/`):
- `AangifteStateService` — manages `AangifteState` lifecycle
- `GemeenteAanslagvoetService` — municipality surtax rate lookup from static data
- `IAssetReader` — platform abstraction for reading embedded CSV/MD assets

**UI** (`BlazorTax.Shared/Components/`, `Pages/`):
- 24 Razor form components (`VakIForm.razor` … `VakXXIIForm.razor`)
- Main pages: `Belastingen.razor` (main tax form), `SnelleInvoer.razor` (quick entry), `Home.razor`

### Platform Hosts

**`BlazorTax.Wpf/`** — WPF host:
- `App.xaml.cs` — DI container setup, registers all shared services + `WpfAssetReader`
- `MainWindow.xaml` — hosts `BlazorWebView`
- `WpfAssetReader.cs` — implements `IAssetReader` via filesystem paths

**`BlazorTax.Maui/`** — MAUI cross-platform host:
- `MauiProgram.cs` — DI setup, registers `MauiAssetReader`
- `MainPage.xaml` — hosts `BlazorWebView`
- `MauiAssetReader.cs` — implements `IAssetReader` via `FileSystem` API

### Test Project: `TestCalc/`

Console app that runs 9 reference tax scenarios and compares results against FOD Tax-Calc / Tax-on-Web official reference values. `test_taxcalc.py` is a Python scraper that pulls reference data from the official Belgian tax calculator.

## Key Conventions

- **Constants belong in `TaxConstants2026.cs`** — never hardcode tax rates, brackets, or thresholds elsewhere.
- **`IAssetReader`** must be used for any file access in shared code so both WPF and MAUI can resolve assets correctly.
- **Platform hosts register services** — DI setup lives in `App.xaml.cs` (WPF) and `MauiProgram.cs` (MAUI); shared services must be registered in both.
- CSV field metadata drives form rendering dynamically — changes to form fields go in both the CSV and the corresponding `VakXXData.cs` model.
- Tax year–specific code uses `2026` suffix in file/class names to avoid confusion if future years are added.
