"""Ontdek veldnamen voor Deel 2 vakken in Tax-Calc AJ2025."""
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

    def explore_vak(self, roman):
        """Open a vak and print all input fields."""
        self.post(f'displayCadre{roman}.do',
                  {'cadreAnchor': f'cadre{roman}', 'redisplay': f'Cadre_{roman}'})
        
        print(f'\n=== Vak {roman} velden ===')
        # Text fields
        for m in re.finditer(r'<input[^>]*type="text"[^>]*name="([^"]*)"[^>]*', self.page):
            name = m.group(1)
            readonly = 'readonly' in m.group(0).lower()
            tag = ' (READONLY)' if readonly else ''
            print(f'  TEXT: {name}{tag}')
        
        # Checkboxes
        for m in re.finditer(r'<input[^>]*type="checkbox"[^>]*name="([^"]*)"', self.page):
            print(f'  CHECK: {m.group(1)}')
        
        # Hidden fields
        for m in re.finditer(r'<input[^>]*type="hidden"[^>]*name="([^"]*)"[^>]*value="([^"]*)"', self.page):
            n = m.group(1)
            if 'TOKEN' not in n and 'cadre' not in n.lower() and 'redisplay' not in n.lower():
                print(f'  HIDDEN: {n} = {m.group(2)}')

        # Also extract labels near fields for context
        # Look for code numbers in surrounding text
        for m in re.finditer(r'(\d{4})[^<]*<input[^>]*name="(text_c[^"]*)"', self.page):
            print(f'  LABEL: code {m.group(1)} -> {m.group(2)}')

if __name__ == '__main__':
    s = TaxCalc()
    
    # Setup: Vak I (postcode + geboortedatum)
    vak1 = {'cadreAnchor': 'cadreI', 'redisplay': 'Cadre_I',
            'text_c01_c': '2000', 'text_c01_e': '', 'text_c02_b': '15/03/1975'}
    s.post('validateCadreI.do', vak1)
    
    # Vak II: alleenstaande
    s.post('displayCadreII.do', {'cadreAnchor': 'cadreII', 'redisplay': 'Cadre_II'})
    submit_ii = {'cadreAnchor': 'cadreII', 'redisplay': 'Cadre_II'}
    for m in re.finditer(r'<input[^>]*type="hidden"[^>]*name="([^"]*)"[^>]*value="([^"]*)"', s.page):
        n, v = m.group(1), m.group(2)
        if 'TOKEN' not in n: submit_ii[n] = v
    for m in re.finditer(r'<input[^>]*type="text"[^>]*name="([^"]*)"', s.page):
        submit_ii[m.group(1)] = ''
    submit_ii['check_c1001'] = 'checked'  # alleenstaande
    s.post('validateCadreII.do', submit_ii)
    
    # Explore Deel 2 vakken
    for roman in ['XVI', 'XVII', 'XVIII', 'XIX']:
        s.explore_vak(roman)
    
    # Also save full HTML for XVI and XVII for detailed analysis
    for roman in ['XVI', 'XVII']:
        s.post(f'displayCadre{roman}.do',
               {'cadreAnchor': f'cadre{roman}', 'redisplay': f'Cadre_{roman}'})
        with open(f'vak_{roman}_fields.html', 'w', encoding='utf-8') as f:
            f.write(s.page)
        print(f'\n  Saved vak_{roman}_fields.html')
    
    print('\nDone!')
