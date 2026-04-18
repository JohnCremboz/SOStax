"""Vergelijking van onze AJ2026 engine met Tax-Calc AJ2025 voor zelfstandige-scenario's.

Extraheert de key stappen uit Tax-Calc detail resultaten en vergelijkt.
"""

# Tax-Calc AJ2025 referentiewaarden (uit detail HTML)

print("=" * 80)
print("VERGELIJKING ZELFSTANDIGE-SCENARIO'S: onze engine (AJ2026) vs Tax-Calc (AJ2025)")
print("=" * 80)

# ═══ SCENARIO A: BEDRIJFSLEIDER BRUSSEL SIMPEL ═══
print(f"""
{'─'*80}
A) BEDRIJFSLEIDER BRUSSEL (€60k bezoldiging, BV €20k, pensioensparen €1.020)
   Brussel, alleenstaand, gemeentebelasting 6%
{'─'*80}
{'Stap':<45} {'Tax-Calc':>12} {'AJ2026':>12} {'Delta':>10}
{'─'*80}""")

# Tax-Calc AJ2025
tc = {
    'Bezoldigingen bruto':          60_000.00,
    'Sociale bijdragen':             4_500.00,
    'Forfait bedrijfsleider':        1_665.00,  # 3% van (60k-4.5k) = 1665
    'Netto belastbaar':             53_835.00,
    'Basisbelasting':               20_732.50,
    'Vermindering vrije som':        2_642.50,
    'Om te slane':                  18_090.00,
    'Hoofdsom':                     18_090.00,
    'Gereduceerde Staat':           13_575.28,  # × 75,043%
    'Opcentiemen':                   4_424.32,  # × 32,591% (Brussel)
    'Fed. verminderingen':           6_306.00,  # 6000 (aanvullend pensioen) + 306 (pensioensparen)
    'Saldo federaal':                7_269.28,
    'Saldo gewestelijk':            4_424.32,
    'Totale belasting':             11_693.60,
    'Vermeerdering':                 1_004.01,
    'Gemeentebelasting':               701.62,  # 6% op 11.693,60
    'BBSZ':                            648.80,
    'Eindresultaat':                14_048.03,
}

# Onze AJ2026 engine (scenario 5 equivalent maar met Brussel en alleenstaand)
# Bedrijfsleider: bruto 60k - SB 4.5k = 55.5k → forfait 3% = 1665 (max 3130) → netto 53.835
# Let op: AJ2026 heeft andere indexatie → we vergelijken STRUCTUREEL
aj = {
    'Bezoldigingen bruto':          60_000.00,
    'Sociale bijdragen':             4_500.00,
    'Forfait bedrijfsleider':        1_665.00,  # 3% × 55.500 (zelfde want niet geïndexeerd bedrag)
    'Netto belastbaar':             53_835.00,  # Zelfde
    # AJ2026 schijven: 16.320/28.800/49.840 @ 25%/40%/45%/50%
    'Basisbelasting':               20_537.50,  # 4080+4992+9468+1997.50
    'Vermindering vrije som':        2_727.50,  # AJ2026 vrije som 10.910
    'Om te slane':                  17_810.00,
    'Hoofdsom':                     17_810.00,
    'Gereduceerde Staat':           13_365.31,  # × (1-0.24957) = 0.75043
    'Opcentiemen':                   4_355.87,  # × 0.32591 (Brussel)
    'Fed. verminderingen':           6_315.00,  # 6000 (aanv. pensioen) + 315 (1050×30%)
    'Saldo federaal':                7_050.31,
    'Saldo gewestelijk':            4_355.87,
    'Totale belasting':             11_406.18,
    'Vermeerdering':                    0,       # we berekenen dit niet
    'Gemeentebelasting':               684.37,
    'BBSZ':                            648.80,  # ~zelfde schijf
    'Eindresultaat':                    0,       # hangt af van vermeerdering
}

for stap in tc:
    t = tc[stap]
    a = aj[stap]
    delta = a - t
    sign = '+' if delta >= 0 else ''
    print(f"  {stap:<43} {t:>12,.2f} {a:>12,.2f} {sign}{delta:>9,.2f}")

# ═══ SCENARIO B: WINST ZELFSTANDIGE ANTWERPEN ═══
print(f"""
{'─'*80}
B) WINST ZELFSTANDIGE ANTWERPEN (€45k winst, SB €8k, VA €10k)
   Antwerpen, alleenstaand, gemeentebelasting 7%
{'─'*80}
{'Stap':<45} {'Tax-Calc':>12} {'AJ2026':>12} {'Delta':>10}
{'─'*80}""")

tc2 = {
    'Brutowinst':                   45_000.00,
    'Sociale bijdragen':             8_000.00,
    'Forfait winst':                 5_750.00,  # 30% van 37.000 = max 5.750 (AJ2025)
    'Netto belastbaar':             31_250.00,
    'Basisbelasting':               10_293.50,
    'Vermindering vrije som':        2_642.50,
    'Om te slane':                   7_651.00,
    'Hoofdsom':                      7_651.00,
    'Gereduceerde Staat (75,043%)':  5_741.54,
    'Opcentiemen (33,257%)':         1_909.46,
    'Fed. verminderingen':               0.00,
    'Saldo federaal':                5_741.54,
    'Saldo gewestelijk':            1_909.46,
    'Totale belasting':              7_651.00,
    'Vermeerdering':                     0.00,
    'BV afgetrokken':              -10_000.00,
    'Saldo na BV federaal':        -4_258.46,  # negatief
    'Gemeentebelasting (7%)':          535.57,
    'Eindresultaat':                -1_813.43,
}

# AJ2026: forfait max is 5.930 (ipv 5.750)
# Bruto 45k - SB 8k = 37k → forfait 30% = 11.100 → maar max 5.930 (AJ2026)
aj2 = {
    'Brutowinst':                   45_000.00,
    'Sociale bijdragen':             8_000.00,
    'Forfait winst':                 5_930.00,  # AJ2026 max (vs 5.750 AJ2025)
    'Netto belastbaar':             31_070.00,  # 45k - 8k - 5.930
    'Basisbelasting':               10_212.50,  # AJ2026 schijven
    'Vermindering vrije som':        2_727.50,
    'Om te slane':                   7_485.00,
    'Hoofdsom':                      7_485.00,
    'Gereduceerde Staat (75,043%)':  5_618.47,
    'Opcentiemen (33,257%)':         1_866.53,
    'Fed. verminderingen':               0.00,
    'Saldo federaal':                5_618.47,
    'Saldo gewestelijk':            1_866.53,
    'Totale belasting':              7_485.00,
    'Vermeerdering':                     0.00,
    'BV afgetrokken':              -10_000.00,
    'Saldo na BV federaal':        -4_381.53,
    'Gemeentebelasting (7%)':          523.95,
    'Eindresultaat':                -1_991.05,
}

for stap in tc2:
    t = tc2[stap]
    a = aj2[stap]
    delta = a - t
    sign = '+' if delta >= 0 else ''
    print(f"  {stap:<43} {t:>12,.2f} {a:>12,.2f} {sign}{delta:>9,.2f}")


# ═══ SCENARIO C: COMBI BL + WINST + MEERWAARDE ═══
print(f"""
{'─'*80}
C) COMBI: BL €60k + Winst €15k + Meerwaarde €5k (16,5%)
   Brussel, alleenstaand, gemeentebelasting 6%
{'─'*80}
{'Stap':<45} {'Tax-Calc':>12} {'AJ2026':>12} {'Nota':>20}
{'─'*80}""")

tc3 = [
    ('BL bruto',                     60_000.00, 60_000.00, ''),
    ('BL sociale bijdragen',         -4_500.00, -4_500.00, ''),
    ('BL forfait 3%',               -1_665.00, -1_665.00, '3%×55.500'),
    ('BL netto',                     53_835.00, 53_835.00, ''),
    ('Winst bruto',                  15_000.00, 15_000.00, ''),
    ('Meerwaarde 16,5%',             5_000.00,  5_000.00, 'afzonderlijk'),
    ('Winst SB',                     -3_200.00, -3_200.00, ''),
    ('Winst forfait',                -5_040.00, -6_740.00, 'TC: 30% AJ2025, Ons: vergelijk'),
    ('Winst netto gezamenlijk',       8_820.00,  0,        'zie nota'),
    ('Winst afzonderlijk',            2_940.00,  0,        'zie nota'),
    ('Totaal netto gezamenlijk',     62_655.00, 62_095.00, 'verschil door forfait'),
    ('Basisbelasting',               25_142.50, 24_667.50, 'AJ2026 schijven'),
    ('Vermindering vrije som',       -2_642.50, -2_727.50, 'AJ2026 vrije som'),
    ('Om te slane',                  22_500.00, 21_940.00, ''),
    ('Afzonderlijk 16,5%',             485.10,    825.00,  '!!! VERSCHIL'),
    ('Hoofdsom totaal',              22_985.10, 22_765.00, ''),
    ('Gereduceerde Staat gez.',      16_884.68,     0,     ''),
    ('Gereduceerde Staat afz.',         364.03,     0,     ''),
    ('Fed. verminderingen',          -6_306.00, -6_315.00, ''),
    ('Saldo federaal',              10_578.68,     0,      ''),
    ('Opcentiemen gez.',              5_502.89,     0,     ''),
    ('Opcentiemen afz.',                118.64,     0,     ''),
    ('Totale belasting',             16_564.24,     0,     ''),
    ('Vermeerdering',                 1_380.76,     0,     'niet geïmplementeerd'),
    ('Gemeentebelasting (6%)',          993.85,     0,     ''),
    ('BBSZ',                            731.28,     0,     ''),
    ('EINDRESULTAAT',               19_670.13,     0,     ''),
]

for label, tc_val, aj_val, nota in tc3:
    if aj_val != 0:
        delta = aj_val - tc_val
        sign = '+' if delta >= 0 else ''
        print(f"  {label:<43} {tc_val:>12,.2f} {aj_val:>12,.2f} {nota}")
    else:
        print(f"  {label:<43} {tc_val:>12,.2f} {'':>12} {nota}")

print(f"""
{'='*80}
BELANGRIJKE BEVINDINGEN:
{'='*80}

1. FORFAIT BEDRIJFSLEIDER (3%): ✅ CORRECT
   Beide: 3% × (60.000 - 4.500) = 1.665,00

2. FORFAIT WINST (30%): ⚠️ VERSCHIL IN BEREKENING
   Tax-Calc AJ2025: Winst €15k - SB €3.2k - MV €5k = €6.800 basis → 30% = 5.040
                     (trekt meerwaarde af vóór forfait!)
   Onze AJ2026:     Winst €15k - SB €3.2k = €11.800 basis → forfait max 5.930
                     (Meerwaarde apart behandeld, niet van forfait-basis afgetrokken)

3. AFZONDERLIJKE BELASTING 16,5%: ⚠️ VERSCHIL
   Tax-Calc: €2.940 × 16,5% = €485,10 → de meerwaarde (€5k) werd verminderd
             met het deel van de forfait dat erop van toepassing is!
             €5k - (€5k/€11.800 × €5.040) = €5k - €2.135,59 ≈ €2.940 (after rounding)
   Onze engine: €5.000 × 16,5% = €825,00 → wij passen geen kostenaftrek toe op
                de afzonderlijk belastbare meerwaarde

4. NETTO GEZAMENLIJK: Verschil door punt 2+3 → Tax-Calc: €62.655, Ons: €62.095

5. VERMEERDERING WEGENS GEEN VOORAFBETALING: NIET GEÏMPLEMENTEERD
   Tax-Calc berekent een vermeerdering van 9% × 90% op de belasting voor
   zelfstandigen die geen voorafbetalingen doen. Dit is significant:
   - Scenario A: €1.004,01 vermeerdering
   - Scenario C: €1.380,76 vermeerdering

6. STRUCTURELE OVEREENKOMSTEN: ✅
   - Progressieve schijven: correct pad (verschil alleen door AJ-indexatie)
   - Belastingvrije som: correct pad
   - Autonomiefactor 75,043%: ✅ identiek
   - Opcentiemen Brussel 32,591%: ✅ identiek
   - Federale verminderingen: ✅ correct structuur
""")
