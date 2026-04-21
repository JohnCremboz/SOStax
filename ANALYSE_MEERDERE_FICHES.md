# Analyse: Rubrieken met meerdere fiches in SOStax

## ✅ Al geïmplementeerd met meerdere fiches:

### **VAK IV - Wedden, Lonen**
- **Code 1250/2250** (Rubriek 1a) - volgens fiches 281.10
  - Status: ✅ **Geïmplementeerd** met lijst `Fiches1250` en `Fiches2250`
  - 3 invoerlijnen in het formulier
  - Automatische som in rubriek 1250-11/2250-78

## 🔍 Te implementeren met meerdere fiches:

### **VAK IV - Bedrijfsvoorheffing (Sectie H)**
- **Code 1286/2286** - Volgens fiches (rubriek H1)
  - 3 invoerlijnen in formulier (code 286)
  - Totaal in rubriek H3: 1286-72/2286-42
  - Ook: Code voor niet-op-fiche vakantiegeld (H2)

### **VAK V - Bedrijfsvoorheffing (Sectie B)**  
- **Code 1225/2225** - Volgens fiches (rubriek B1)
  - 3 invoerlijnen in formulier (code 225)
  - Totaal in rubriek B2: 1225-36/2225-06

## 📋 Al geïmplementeerd met lijsten:

### **VAK IV - Sectie N: Helpende Gezinsleden**
- `List<HelpendeGezinsledenRij>` - Dynamische lijst met code + bedrag
- Heeft al "+" knop voor nieuwe rijen

### **VAK IV - Sectie O: Buitenlandse Inkomsten**
- `List<BuitenlandseInkomstenIVRij>` - 4 verschillende categorieën (O1, O2a, O2b, O2c)
- Elk met land, code en bedrag

### **VAK V - Sectie C: Pensioenen Buitenlandse Oorsprong**
- `List<BuitenlandsPensioen>` - Land, code, bedrag, type vermindering

### **VAK VI - Sectie 4: Schuldenaars**
- `List<SchuldenaarItem>` - Rijksinwoners en Niet-rijksinwoners
- Alleen naam veld

### **VAK XX - Buitenlandse Inkomsten**
- `List<BuitenlandInkomen>` - Land, code, bedrag

## 📊 Samenvatting:

**Te implementeren (2 rubrieken):**
1. **VAK IV - Sectie H**: Bedrijfsvoorheffing volgens fiches (1286/2286)
2. **VAK V - Sectie B**: Bedrijfsvoorheffing volgens fiches (1225/2225)

**Patroon:**
- 3 invoerlijnen voor fiches (zoals bij 1250/2250)
- Subtotaal berekening
- Apart veld voor "niet op fiche vermeld" (indien van toepassing)
- Totaal veld (readonly, automatisch berekend)

## 🎯 Aanbeveling:

Implementeer dezelfde aanpak als bij Code 1250/2250:
- `List<decimal?>` voor belastingplichtige en partner
- Computed property voor totaal
- Dynamische UI met "Fiche toevoegen" knop
- Readonly totaalveld

Dit zorgt voor consistentie in de gebruikerservaring en herbruikbare code patterns.
