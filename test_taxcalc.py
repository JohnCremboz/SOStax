"""Test Tax-Calc AJ2025 met correcte veldnamen.

Veldnaam-patroon:
  text_c250b_1 = belastingplichtige loon sub-veld 1 (editable)
  text_c250c_1 = partner loon sub-veld 1 (editable)
  text_c1250   = totaal belastingplichtige (READONLY)
  text_c2250   = totaal partner (READONLY)
  text_c1254   = direct veld (niet readonly)
  text_c2254   = partner direct veld
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
        return re.findall(r'(Gelieve[^.]*\.|Het gegeven[^.]*\.)', text)

    def calculate(self):
        self.post('calculateTaxForm.do', {})
        if 'executeCalculation.do' in self.page:
            self.get('executeCalculation.do')
            return True
        return False


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
        return

    # Parse result
    text = re.sub(r'<(script|style)[^>]*>.*?</\1>', '', s.page, flags=re.DOTALL)
    text = re.sub(r'<[^>]+>', '|', text)
    text = re.sub(r'&nbsp;', ' ', text)
    for line in text.split('|'):
        line = line.strip()
        if re.search(r'[\d.]+,\d{2}', line) and len(line) < 150:
            print(f'  RESULTAAT: {line}')

    with open(f'res_{label.replace(" ","_").lower()}.html', 'w', encoding='utf-8') as f:
        f.write(s.page)


if __name__ == '__main__':
    # === Test 1: Pensionaris Brussel (AJ2025) ===
    # Referentie Vb-pen.txt: verschuldigd 2.834,38; BV 2.886,03 → 51,65 terug
    run('Pensionaris Brussel',
        '1000', '15/03/1960', False, '', {
            'V': {
                'text_c228b_1': '25000,00',   # pensioen
                'text_c225b_1': '2886,03',    # BV pensioenen
            },
        })

    # === Test 2: Werknemer Gent alleenstaand ===
    run('Werknemer Gent',
        '9000', '20/06/1985', False, '', {
            'IV': {
                'text_c250b_1': '35000,00',   # loon
                'text_c286b_1': '8500,00',    # BV
            },
        })

    # === Test 3: Gehuwd koppel Menen (AJ2025) ===
    # Referentie aanslagbiljet: -6.534,88 (terug)
    run('Gehuwd Menen',
        '8930', '01/01/1980', True, '01/01/1982', {
            'II': {
                'text_c1030': '3',     # kinderen ten laste
                'text_c1031': '1',     # waarvan gehandicapt
            },
            'IV': {
                # Belastingplichtige (b = sub-veld belastingplichtige)
                'text_c250b_1': '40193,95',    # loon
                'text_c1254': '2243,70',       # wettelijke inhoudingen
                'text_c1255': '2243,70',
                'text_c286b_1': '8948,90',     # BV ingehouden
                'text_c1287': '324,21',        # BBSZ
                # Partner (c = sub-veld partner)
                'text_c250c_1': '21794,78',    # loon partner
                'text_c2254': '57,46',
                'text_c2255': '57,46',
                'text_c2284': '1516,77',       # werkbonus 33,14%
                'text_c286c_1': '184,66',      # BV partner
                'text_c2287': '96,45',         # BBSZ partner
                'text_c2360': '1142,19',       # werkbonus 52,54%
            },
            'X': {
                'text_c1384': '235,10',    # kinderopvang
                'text_c2361': '1020,00',   # giften partner
            },
        })

    # === Test 4: Alleenstaande werknemer Antwerpen ===
    run('Werknemer Antwerpen',
        '2000', '15/06/1990', False, '', {
            'IV': {
                'text_c250b_1': '45000,00',
                'text_c286b_1': '12000,00',
            },
        })

    # === Test 5: Gepensioneerde + zieke partner Heuvelland (AJ2025) ===
    # Referentie 2025-12-09-Aanslagbiljet 2025.txt: te betalen 3.048,77
    # bp: pensioen 43.195,23 / BV 5.803,86 / LT-sparen 1.940,04
    # partner: ziekte 15.430,22 / vorige verliezen 8.127,89 / handicap / giften 1.020,00
    run('Gepensioneerde + zieke partner Heuvelland',
        '8954', '07/03/1957', True, '22/01/1964', {
            'II': {
                'check_c2020': 'checked',   # partner heeft handicap
            },
            'III': {
                'text_c2105': '787,00',     # KI eigen woning partner
            },
            'IV': {
                'text_c266c_1': '15430,22', # ziekte of invaliditeit partner
                'text_c2349':   '8127,89',  # overdraagbare verliezen partner
            },
            'V': {
                'text_c228b_1': '43195,23', # wettelijke pensioenen bp
                'text_c225b_1': '5803,86',  # BV pensioenen bp
            },
            'IX': {
                'text_c1353': '1940,04',    # lange termijnsparen bp
            },
            'X': {
                'text_c2361': '1020,00',    # giften partner
            },
        })

    print('\nDone!')
