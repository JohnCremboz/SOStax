"""Test Tax-Calc AJ2025 met zelfstandige/bedrijfsleider scenario's.

Scenario A: Bedrijfsleider Brussel (ons scenario 5)
  - Bezoldigingen bedrijfsleider: €60.000 (Code1400)
  - Sociale bijdragen: €4.500 (Code1405)
  - Bedrijfsvoorheffing: €20.000 (Code1421)
  - Pensioensparen: €1.020 (Code1361 — AJ2025 max)
  
Scenario B: Winst zelfstandige Antwerpen
  - Winst: €45.000 (Code1600)
  - Sociale bijdragen: €8.000 (Code1632)
  - Voorafbetalingen: €10.000 (Code1758)
  
Scenario C: Bedrijfsleider + winst + meerwaarde (ons scenario 5 exact)
  - Bezoldigingen bedrijfsleider: €60.000 (Code1400)
  - Sociale bijdragen bedrijfsleider: €4.500 (Code1405)
  - BV bedrijfsleider: €20.000 (Code1421)
  - Winst: €15.000 (Code1600)
  - Sociale bijdragen winst: €3.200 (Code1632)
  - Meerwaarde 16,5%: €5.000 (Code1603)
  - Pensioensparen: €1.020 (Code1361)
"""
import urllib.request, http.cookiejar, re
from urllib.parse import urlencode

BASE = 'https://ccff02.minfin.fgov.be/taxcalc2025/app/anonymous/private/taxform'

class TaxCalc:
    def __init__(self):
        cj = http.cookiejar.CookieJar()
        self.opener = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(cj))
        self.opener.open('https://ccff02.minfin.fgov.be/taxcalc2025/app/anonymous/public/home.do')
        self.opener.open('https://ccff02.minfin.fgov.be/taxcalc2025/app/anonymous/public/common/selectLanguage.do?language=nl')
        r = self.opener.open('https://ccff02.minfin.fgov.be/taxcalc2025/app/anonymous/private/taxcalc/enterTaxCalc.do')
        self.page = r.read().decode('utf-8')
        self.token = re.findall(r'TOKEN"\s*value="([^"]*)"', self.page)[0]

    def post(self, action, data):
        data['org.apache.struts.taglib.html.TOKEN'] = self.token
        data.setdefault('formComplete', '')
        req = urllib.request.Request(f'{BASE}/{action}', urlencode(data).encode(), method='POST')
        req.add_header('Content-Type', 'application/x-www-form-urlencoded')
        r = self.opener.open(req)
        self.page = r.read().decode('utf-8')
        t = re.findall(r'TOKEN"\s*value="([^"]*)"', self.page)
        if t: self.token = t[0]

    def get(self, action):
        r = self.opener.open(f'{BASE}/{action}')
        self.page = r.read().decode('utf-8')
        t = re.findall(r'TOKEN"\s*value="([^"]*)"', self.page)
        if t: self.token = t[0]

    def fill_vak(self, roman, fields):
        self.post(f'displayCadre{roman}.do',
                  {'cadreAnchor': f'cadre{roman}', 'redisplay': f'Cadre_{roman}'})
        submit = {'cadreAnchor': f'cadre{roman}', 'redisplay': f'Cadre_{roman}'}
        for m in re.finditer(r'<input[^>]*type="hidden"[^>]*name="([^"]*)"[^>]*value="([^"]*)"', self.page):
            n, v = m.group(1), m.group(2)
            if 'TOKEN' not in n: submit[n] = v
        for m in re.finditer(r'<input[^>]*type="text"[^>]*name="([^"]*)"', self.page):
            submit[m.group(1)] = ''
        submit.update(fields)
        self.post(f'validateCadre{roman}.do', submit)
        text = re.sub(r'<[^>]+>', ' ', self.page)
        errors = re.findall(r'(Gelieve[^.]*\.|Het gegeven[^.]*\.)', text)
        return errors

    def calculate(self):
        self.post('calculateTaxForm.do', {})
        if 'executeCalculation.do' in self.page:
            self.get('executeCalculation.do')
            return True
        return False

    def get_details(self):
        """Fetch detail calculation page."""
        self.post('showCalculationDetails.do', {})
        return self.page


def fmt(val):
    """Format decimal with comma."""
    return f'{val:,.2f}'.replace(',', 'X').replace('.', ',').replace('X', '.')


def run(label, postcode, geb, couple, geb_p, vakken):
    print(f'\n{"="*70}')
    print(f'  {label}')
    print(f'{"="*70}')
    s = TaxCalc()

    vak1 = {'cadreAnchor': 'cadreI', 'redisplay': 'Cadre_I',
            'text_c01_c': postcode, 'text_c01_e': '', 'text_c02_b': geb}
    if couple:
        vak1['check_c01_h'] = 'checked'
        vak1['text_c02_c'] = geb_p
    s.post('validateCadreI.do', vak1)

    vak2 = {'check_c1002': 'checked'} if couple else {'check_c1001': 'checked'}
    if 'II' in vakken: vak2.update(vakken['II'])
    err = s.fill_vak('II', vak2)
    if err: print(f'  FOUT Vak II: {err}'); return

    for roman, fields in vakken.items():
        if roman == 'II': continue
        err = s.fill_vak(roman, fields)
        if err: print(f'  FOUT Vak {roman}: {err}'); return
        print(f'  Vak {roman} ok')

    if not s.calculate():
        print('  Berekening mislukt!')
        # Save page for debugging
        with open(f'debug_{label.replace(" ","_").lower()}.html', 'w', encoding='utf-8') as f:
            f.write(s.page)
        return

    # Parse summary result
    text = re.sub(r'<(script|style)[^>]*>.*?</\1>', '', s.page, flags=re.DOTALL)
    text = re.sub(r'<[^>]+>', '|', text)
    text = re.sub(r'&nbsp;', ' ', text)
    for line in text.split('|'):
        line = line.strip()
        if re.search(r'[\d.]+,\d{2}', line) and len(line) < 150:
            print(f'  RESULTAAT: {line}')

    # Save result page
    with open(f'res_{label.replace(" ","_").lower()}.html', 'w', encoding='utf-8') as f:
        f.write(s.page)

    # Get details
    detail_page = s.get_details()
    with open(f'detail_{label.replace(" ","_").lower()}.html', 'w', encoding='utf-8') as f:
        f.write(detail_page)
    print(f'  Detail opgeslagen.')
    
    # Parse key detail lines
    text = re.sub(r'<(script|style)[^>]*>.*?</\1>', '', detail_page, flags=re.DOTALL)
    text = re.sub(r'<[^>]+>', '|', text)
    text = re.sub(r'&nbsp;', ' ', text)
    text = re.sub(r'\s+', ' ', text)
    
    # Find amounts with labels
    interesting = [
        'Netto', 'netto', 'beroepskosten', 'forfait', 'Forfait',
        'Basisbelasting', 'belastingvrije', 'Om te slane', 'om te slane',
        'Gereduceerde', 'gereduceerde', 'opcentiemen', 'Opcentiemen',
        'Vermindering', 'vermindering', 'Hoofdsom', 'hoofdsom',
        'Totaal', 'totaal', 'Saldo', 'saldo', 'Te betalen', 'Terug',
        'afzonderlijk', 'Afzonderlijk', 'meerwaarde', 'Meerwaarde',
        'winst', 'Winst', 'bedrijfsleider', 'Bedrijfsleider',
        'sociale bijdrage', 'Sociale bijdrage',
    ]
    
    for segment in text.split('|'):
        segment = segment.strip()
        if not segment: continue
        if re.search(r'[\d.]+,\d{2}', segment) and any(kw in segment for kw in interesting):
            if len(segment) < 200:
                print(f'  DETAIL: {segment}')


if __name__ == '__main__':
    # === Scenario A: Simpele bedrijfsleider ===
    run('Bedrijfsleider Brussel simpel',
        '1000', '15/03/1975', False, '', {
            'XVI': {
                'text_c400b_1': '60000,00',   # bezoldigingen
                'text_c1405': '4500,00',       # sociale bijdragen
                'text_c1421': '20000,00',      # BV
                'text_c407b_1': '0,00',        # VAP (verplicht veld, 0 als niet van toepassing)
                'text_c411b_1': '0,00',        # VAPW totaal
            },
            'X': {
                'text_c1361': '1020,00',       # pensioensparen (AJ2025 max)
            },
        })

    # === Scenario B: Winst zelfstandige ===
    run('Winst zelfstandige Antwerpen',
        '2000', '20/06/1980', False, '', {
            'XVII': {
                'text_c1600': '45000,00',      # winst
                'text_c1632': '8000,00',       # sociale bijdragen
            },
            'XIX': {
                'text_c1758': '10000,00',      # voorafbetalingen zelfstandigen
            },
        })

    # === Scenario C: Combi bedrijfsleider + winst + meerwaarde (= ons scenario 5) ===
    run('Combi bedrijfsleider winst meerwaarde Brussel',
        '1000', '15/03/1975', False, '', {
            'XVI': {
                'text_c400b_1': '60000,00',    # bezoldigingen bedrijfsleider
                'text_c1405': '4500,00',        # sociale bijdragen
                'text_c1421': '20000,00',       # BV
                'text_c407b_1': '0,00',         # VAP
                'text_c411b_1': '0,00',         # VAPW
            },
            'XVII': {
                'text_c1600': '15000,00',      # winst
                'text_c1632': '3200,00',       # sociale bijdragen winst
                'text_c1603': '5000,00',       # meerwaarde 16,5%
            },
            'X': {
                'text_c1361': '1020,00',       # pensioensparen
            },
        })

    print('\nDone!')
