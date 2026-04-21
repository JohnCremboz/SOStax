# ✅ Implementatie: Meerdere Fiches voor Bedrijfsvoorheffing

## 📊 Overzicht geïmplementeerde wijzigingen

### **3 Rubrieken met fiche-lijst functionaliteit:**

1. ✅ **VAK IV - Code 1250/2250** - Wedden, lonen volgens fiches 281.10
2. ✅ **VAK IV - Code 1286/2286** - Bedrijfsvoorheffing volgens fiches (286)
3. ✅ **VAK V - Code 1225/2225** - Bedrijfsvoorheffing volgens fiches (225)

---

## 🔧 Technische Wijzigingen

### **1. VakIVData.cs**

#### Code 1250/2250 (Wedden/Lonen):
```csharp
// Meerdere fiches 281.10
public List<decimal?> Fiches1250 { get; set; } = new() { null };
public List<decimal?> Fiches2250 { get; set; } = new() { null };

// Computed totals
public decimal Code1250 => Fiches1250.Sum(f => f ?? 0m);
public decimal Code2250 => Fiches2250.Sum(f => f ?? 0m);

// Niet-op-fiche vermeld (1b)
public decimal? Code1250B { get; set; }
public decimal? Code2250B { get; set; }

// Totaal 1250-11 / 2250-78
public decimal Total1250 => Code1250 + (Code1250B ?? 0m);
public decimal Total2250 => Code2250 + (Code2250B ?? 0m);
```

#### Code 1286/2286 (Bedrijfsvoorheffing):
```csharp
// Meerdere fiches bedrijfsvoorheffing (286)
public List<decimal?> Fiches1286 { get; set; } = new() { null };
public List<decimal?> Fiches2286 { get; set; } = new() { null };

// Computed totals
public decimal Code1286 => Fiches1286.Sum(f => f ?? 0m);
public decimal Code2286 => Fiches2286.Sum(f => f ?? 0m);

// Op niet-op-fiche vakantiegeld
public decimal? Code1286B { get; set; }
public decimal? Code2286B { get; set; }

// Totaal 1286-72 / 2286-42
public decimal Total1286 => Code1286 + (Code1286B ?? 0m);
public decimal Total2286 => Code2286 + (Code2286B ?? 0m);
```

### **2. VakVData.cs**

#### Code 1225/2225 (Bedrijfsvoorheffing Pensioenen):
```csharp
// Meerdere fiches bedrijfsvoorheffing (225)
public List<decimal?> Fiches1225 { get; set; } = new() { null };
public List<decimal?> Fiches2225 { get; set; } = new() { null };

// Computed totals (1225-36 / 2225-06)
public decimal Code1225 => Fiches1225.Sum(f => f ?? 0m);
public decimal Code2225 => Fiches2225.Sum(f => f ?? 0m);
```

---

## 🎨 UI Componenten

### **VakIVForm.razor**
- ✅ Dynamische fiche lijst voor 1250/2250
- ✅ Dynamische fiche lijst voor 1286/2286
- ✅ "Fiche toevoegen" knoppen
- ✅ "Verwijderen" knoppen (behalve eerste fiche)
- ✅ Subtotaal weergave
- ✅ Readonly totaalvelden

### **VakVForm.razor**
- ✅ Dynamische fiche lijst voor 1225/2225
- ✅ "Fiche toevoegen" knop
- ✅ "Verwijderen" knoppen
- ✅ Totaal weergave

### **Code-behind methoden:**
```csharp
// VakIVForm.razor
private void AddFiche() { ... }              // Voor 1250/2250
private void RemoveFiche(int index) { ... }
private void AddFiche1286() { ... }          // Voor 1286/2286
private void RemoveFiche1286(int index) { ... }

// VakVForm.razor
private void AddFiche1225() { ... }          // Voor 1225/2225
private void RemoveFiche1225(int index) { ... }
```

---

## ⚡ Snelle Invoer

### **SnelleInvoerForm.razor**

Ondersteunt nu automatisch alle fiche codes:
- **1250/2250** → Voegt toe aan `Fiches1250`/`Fiches2250`
- **1286/2286** → Voegt toe aan `Fiches1286`/`Fiches2286`
- **1225/2225** → Voegt toe aan `Fiches1225`/`Fiches2225`

**Helper methode:**
```csharp
private bool AddToFicheList(string waarde, bool isPrimary, 
    List<decimal?> primaryList, List<decimal?> partnerList)
{
    // Intelligente logica:
    // - Als laatste fiche leeg is → vul in
    // - Als laatste fiche vol is → voeg nieuwe toe
    // - Synchroniseert beide lijsten
}
```

---

## 🔢 Berekeningen

### **Aangepaste calculators:**

#### PersonenbelastingCalculator.cs:
```csharp
// Voor
return (vakIV.Code1286 ?? 0) + (vakIV.Code2286 ?? 0)
     + (vakV.Code1225 ?? 0) + (vakV.Code2225 ?? 0);

// Na
return vakIV.Code1286 + vakIV.Code2286
     + vakV.Code1225 + vakV.Code2225;
```

#### PartnerInkomen.cs:
```csharp
// Belastingplichtige
Bedrijfsvoorheffing = vakIV.Code1286 + vakV.Code1225

// Partner  
Bedrijfsvoorheffing = vakIV.Code2286 + vakV.Code2225
```

**✅ Alle berekeningen blijven correct werken** dankzij de computed properties!

---

## 🎯 Gebruikerservaring

### **Voor de gebruiker:**

1. **Start:** Elke rubriek heeft 1 lege fiche
2. **Invoer:** Vul bedrag in voor eerste fiche
3. **Meer fiches:** Klik "➕ Fiche toevoegen"
4. **Verwijderen:** Klik 🗑️ om fiche te verwijderen (behalve eerste)
5. **Subtotaal:** Wordt automatisch berekend en getoond
6. **Totaal:** Readonly veld met som van alle fiches

### **Via Snelle Invoer:**
```
Code: 1286
Waarde: 5000
→ Voegt €5.000 toe aan Fiche 1 (Bedrijfsvoorheffing belastingplichtige)

Code: 1286
Waarde: 2500
→ Voegt €2.500 toe aan Fiche 2 (automatisch nieuwe fiche)

Code: 2286
Waarde: 3000
→ Voegt €3.000 toe aan Fiche 1 (Bedrijfsvoorheffing partner)
```

---

## 📋 Formulierstructuur

### **VAK IV - Sectie H: Bedrijfsvoorheffing**
```
1. Volgens fiches:
   Fiche 1: (286) [...] (286) [...]  🗑️
   Fiche 2: (286) [...] (286) [...]  🗑️
   ➕ Fiche toevoegen
   Subtotaal: 7.500,00    3.000,00

2. Op niet-op-fiche vakantiegeld:
   (286) [...] (286) [...]

3. Totaal van rubrieken 1 en 2:
   1286-72: 7.500,00   2286-42: 3.000,00
```

### **VAK V - Sectie B: Bedrijfsvoorheffing**
```
1. Volgens fiches:
   Fiche 1: (225) [...] (225) [...]
   Fiche 2: (225) [...] (225) [...]  🗑️
   ➕ Fiche toevoegen
   Totaal: 4.200,00    1.800,00

2. Totaal van rubriek 1:
   1225-36: 4.200,00   2225-06: 1.800,00
```

---

## ✅ Voordelen

1. **Consistente UX**: Alle fiche-rubrieken werken hetzelfde
2. **Flexibiliteit**: Onbeperkt fiches toevoegen
3. **Automatisch**: Berekeningen gebeuren real-time
4. **Backwards compatible**: Bestaande code blijft werken
5. **Type-safe**: Compile-time validatie van alle berekeningen
6. **DRY principe**: Herbruikbare `AddToFicheList` helper methode

---

## 🚀 Status: COMPLEET

Alle drie de rubrieken zijn nu volledig geïmplementeerd met:
- ✅ Data model (lijsten + computed properties)
- ✅ UI componenten (dynamische rijen)
- ✅ Event handlers (toevoegen/verwijderen)
- ✅ Snelle invoer ondersteuning
- ✅ Berekening updates
- ✅ Build succesvol
- ✅ Type-safe implementatie

**Klaar voor gebruik! 🎉**
