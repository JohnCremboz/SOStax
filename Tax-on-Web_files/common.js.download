// Tax Online Common JavaScript Functions
function changeValueCaptcha(baseUrl) {
    var captchaText=document.getElementById("captchaText").value;
    document.getElementById("linkDowloadXls").href=baseUrl+"?captchaText="+captchaText;
}

function disableEnter(e){
    var key;

    if(window.event){
        key = window.event.keyCode;
    } else {
        key = e.which;
    }
    if(key == 13){
        return false;
    } else {
        return true;
    }
}

function updateRadio( checkfield, radiofield )
{
    if ( !checkfield.checked )
    {
        // radiofield[0] is the "###"
        // radiofield[1] is the "" (see RADIO002 in CadreII.jsp)
	    radiofield[2].checked = false;
	    radiofield[3].checked = false;
    }
}

function updateCheckbox( radiofield, checkfield )
{
    if ( radiofield.checked )
    {
        // checkfield[0] is the "###"
        checkfield[1].checked = true;
    }
}

function setRubriqueValueForNotes(form, inputField, value)
{
    form[inputField].value =  value;
}

function submitFormInPopup(form, action, tempUrl)
{
    var window_name = "pdfWindow";
    var height = "700";
    var width = "350";
    var hasMenubar = "yes";
    var hasScrollbars = "yes";
	var oldAction=form.action;
    newWindow(tempUrl, window_name, height, width, hasMenubar, hasScrollbars);
    form.target = "pdfWindow";
	form.action=action;
	form.submit();
	form.target = "";
	form.action=oldAction;

}
function submitFormInNewWindow(form)
{
	var window_name = "submitWindow";
	var height = "550";
	var width = "780";
	var hasMenubar = "no";
	var hasScrollbars = "no";
	newWindow('', window_name, width, height, hasMenubar, hasScrollbars);
	form.target = "submitWindow";
	form.submit();
	form.target = "";
}
function trim(str)
{
   return str.replace(/^\s*|\s*$/g,"");
}

function submitForm(form, action)
{
	form.action=action;
	form.submit();
}

function goToAnchor(anchor)
{
  	document.location=anchor;
}

function writeErrorCount(text,id)
{
	if (text != "")	{
		document.getElementById(id).innerHTML = text;
	}
}

function callDocument(form, action, docID)
{
	form.action=action;
	form.documentID.value=docID;
	form.submit();
}

function getPdf(action, docID)
{
	document.forms.docform.method="get";
	document.forms.docform.target = "_blank";
	callDocument(document.forms.docform, action, docID);
}

/**
 * TO DO :
 * 1. Set the value of a hidden input to the value of the counter you wish to  (this value will be passed as a parameter to the function).
 * 2. Submit the form to the increase counter command.
 **/

function setCounterValues( form , action, value)
{
	  form.cadreCounters.value = value;
	  //set value of hidden input
	  submitForm(form, action);
}
function displayContent ( form, value )
{
	form.contentkey.value = value;
	form.submit();
}
// Tax Online Common JavaScript Functions Continued .... - Cadre V Field Totals for Fields 211/231/225 and 245
function getNumberValue( pString )
{
    var s = "";
    if(/^-?\d{1,3}(\.\d{3})+((\.\d{3})|(,\d+))$/g.test(pString)){
        s = "" + trim(pString.split(".").join(""));
    }else if(/^-?\d*[.,]?\d+$/g.test(pString)){
        s = "" + trim(pString.replace("\.",","));
    } else if(/^-?\d+$/g.test(pString)){
        s = pString;
    } else if(pString.length == 0 && pString == ''){
        return "";
    } else {
        return NaN;
    }

	if (s.indexOf(",") !== -1)
	{
		s = s.substring( 0, s.indexOf(",") ) + "." + s.substring( s.indexOf(",") + 1, s.length ) ;
	}

	return s;
}


function setNumberValue( pString )
{
    // when the entered value is not a number (detected by javascript engine which sets the string to NaN) we do not add ,00
    if ( !isNaN(pString) )
    {
        if ( pString.length == 0 && pString == '')
        {
            return '';
        }
        var value = pString - 0;
        value = Math.round( value * 100 ) / 100;
        var s = "" + value;

    // even when the number is negatif the minus is not to be displayed
    //if (s.charAt(0) == "-")
    //{
    //  s = s.substring(1,s.length);
	//}
	if(isMobile()){
	    s = s.replace('.', ',');
	} else {
	    s = addDots(s.replace('.', ','));
	}
	var commaPos = s.indexOf(",");

	if (commaPos == -1)
	{
		return s + ",00";
	}
	// comma at the end
	if ( commaPos == s.length - 1 )
	{
		return s + "00";
	}

	if ( commaPos == s.length - 2 )
	{
		return s + "0";
	}

        s = s.substring( 0 , commaPos + 3 );
        return s;
    }
    return pString; // NaN
}

function isMobile(){
	var nav=navigator.userAgent;
	if(nav!=null){
		if(nav.indexOf("Android") !== -1 || nav.indexOf("iPhone") !== -1
		|| nav.indexOf("iPad") !== -1){
			return true;
		}
	}
	return false;
}

function addDots(n){
    var rx=  /(\d+)(\d{3})/;
    return n.replace(/^\d+/, function(w){
        while(rx.test(w)){
            w= w.replace(rx, '$1.$2');
        }
        return w;
    });
}

function setNumberValueWithoutMinus( pString )
{
  var value=pString+"";
  if ( value.length > 0 && value.charAt(0) == "-"){
      value = value.substring(1,value.length);
  }
  return setNumberValue(value);
}

// POSITIF CONTRIBUTORS ARE ADDED TO THE TOTALSUM
// NEGATIF CONTRIBUTORS ARE DISTRACTED FROM THE TOTALSUM
function calculateSum(Obj, positifContributors, negatifContributors)
{

    var sumValue = 0;
    var setValue = false;
    // initialize Sum_is_NaN
    Sum_is_NaN = false;

      for(i=0;i<positifContributors.length;i++)
      {
        if ( !Sum_is_NaN )
        {
            var contributor = Obj[positifContributors[i]].value;
            var dummy = getNumberValue( contributor );
            if (dummy!="")
              {
                  setValue = true;
                  dummy = dummy - 0;
                  sumValue = sumValue + dummy;

                  // if number is NaN set boolean Sum_is_NaN
                  if ( isNaN(sumValue) )
                  {
                    Sum_is_NaN = true;
                }
            }
        }else{
            sumValue = "NaN";
        }
    }

      for(j=0;j<negatifContributors.length;j++)
      {
        if ( !Sum_is_NaN )
        {
            var contributor = Obj[negatifContributors[j]].value;
            var dummy = getNumberValue( contributor );
            if (dummy!="")
              {
                  setValue = true;
                  dummy = dummy - 0;
                  sumValue = sumValue - dummy;

                  // if number is NaN set boolean Sum_is_NaN
                  if ( isNaN(sumValue) )
                  {
                    Sum_is_NaN = true;
                }
            }
        }else{
            sumValue = "NaN";
        }
    }

      if (setValue)
      {
          return sumValue;
      }
      else
      {
          return "";
      }

}


function calculateSumOfArr(Obj, questionACount, fieldArr)
{
    var sumValue = 0;
    var setValue = false;
    // initialize Sum_is_NaN
    Sum_is_NaN = false;

    if ( !Sum_is_NaN )
    {
        for(i=1;i<=questionACount;i++)
        {
            var dummy = getNumberValue(  Obj[fieldArr + i].value );
            if (dummy!="")
            {
                setValue = true;
                dummy = dummy - 0;
                sumValue = sumValue + dummy;
            }
        }

        if (setValue)
        {
            // set Sum_is_NaN to true if not a number
            if ( isNaN(sumValue) )
            {
                Sum_is_NaN = true;
            }
            return setNumberValue( sumValue );
        }
        else
        {
            return "";
        }
    }
}

function upperCaseBicIban(Obj, fieldName)
{
    upperCaseBic(Obj, fieldName);
    upperCaseIban(Obj, fieldName);
}

function upperCaseBic(Obj, fieldName)
{
    Obj[fieldName].value = Obj[fieldName].value.toUpperCase();
}

function upperCaseIban(Obj, fieldName)
{
    Obj[fieldName].value = Obj[fieldName].value.toUpperCase();
}

function initCadreVA(Obj, questionACount)
{
	calculateTotal1211(Obj, questionACount);
	calculateTotal2211(Obj, questionACount);
	calculateTotal1228(Obj, questionACount);
	calculateTotal2228(Obj, questionACount);
	
}

function initCadreVB(Obj, questionBCount)
{
	calculateTotal1225(Obj, questionBCount);
	calculateTotal2225(Obj, questionBCount);
}


function calculateTotal1228(Obj, questionACount)
{
	Obj.text_c1228.value = calculateSumOfArr(Obj, questionACount, 'text_c228b_');
}


function calculateTotal2228(Obj, questionACount)
{
	if( Obj.text_c2228 )
	{
		Obj.text_c2228.value = calculateSumOfArr(Obj, questionACount, 'text_c228c_');
	}
}

function calculateTotal1211(Obj, questionACount)
{
	Obj.text_c1211.value = calculateSumOfArr(Obj, questionACount, 'text_c211b_');
}


function calculateTotal2211(Obj, questionACount)
{
	if( Obj.text_c2211 )
	{
		Obj.text_c2211.value = calculateSumOfArr(Obj, questionACount, 'text_c211c_');
	}
}


function calculateTotal1225(Obj, questionACount)
{
	Obj.text_c1225.value = calculateSumOfArr(Obj, questionACount, 'text_c225b_');
}


function calculateTotal2225(Obj, questionACount)
{
	if( Obj.text_c2225 )
	{
		Obj.text_c2225.value = calculateSumOfArr(Obj, questionACount, 'text_c225c_');
	}
}

//Cadre IV : 1251
function calculateTotal1251(Obj, questionA2Count)
{
	Obj.text_c1251.value = calculateSumOfArr(Obj, questionA2Count, 'text_c251b_');
}

//Cadre IV : 2251
function calculateTotal2251(Obj, questionA2Count)
{
	if( Obj.text_c2251 )
	{
		Obj.text_c2251.value = calculateSumOfArr(Obj, questionA2Count, 'text_c251c_');
	}
}


function addToField(FieldBase, FieldAdd)
{
	var sum1 = getNumberValue( FieldBase.value );
    var sum2 = getNumberValue( FieldAdd.value );

  // if one number is a NaN, Sum remains NaN (Sum_is_NaN = true)
    if ( !isNaN( sum1 ) && !isNaN( sum2 ) && !Sum_is_NaN )
    {
      if ( ( sum2 == "" ) && ( sum1 == "" ) )
      {
          FieldBase.value = "";
      }
      if ( ( sum2 != "" ) && ( sum1 == "" ) )
      {
          FieldBase.value = setNumberValue( sum2 );
      }
      if ( ( sum2 != "" ) && ( sum1 != "" ) )
      {
          sum1 = sum1 - 0;
          sum2 = sum2 - 0;
          FieldBase.value = setNumberValue( sum1 + sum2 );
      }
    }else{
      FieldBase.value ="NaN";
      Sum_is_NaN = true;
    }
}


function initCadreIV(Obj, questionH1Count, questionE1Count, questionA1Count)
{
	calculateTotal1286(Obj, questionH1Count);
	calculateTotal2286(Obj, questionH1Count);
	calculateTotal1250(Obj, questionA1Count);
	calculateTotal2250(Obj, questionA1Count);
}

//Cadre IV
function calculateTotal1286(Obj, questionH1Count)
{
	Obj.text_c1286.value = calculateSumOfArr(Obj, questionH1Count, 'text_c286b_');
	addToField( Obj.text_c1286, Obj.text_D280 );
}

//Cadre IV
function calculateTotal2286(Obj, questionH1Count)
{
	if( Obj.text_c2286 )
	{
		Obj.text_c2286.value = calculateSumOfArr(Obj, questionH1Count, 'text_c286c_');
		addToField( Obj.text_c2286, Obj.text_D281 );
	}
}

//Cadre IV
function calculateTotal1250(Obj, questionA1Count)
{
	Obj.text_c1250.value = calculateSumOfArr(Obj, questionA1Count, 'text_c250b_');
    addToField( Obj.text_c1250, Obj.text_D210 );
}

//Cadre IV
function calculateTotal2250(Obj, questionA1Count)
{
	if( Obj.text_c2250 )
	{
		Obj.text_c2250.value = calculateSumOfArr(Obj, questionA1Count, 'text_c250c_');
        addToField( Obj.text_c2250, Obj.text_D211 );
	}
}

function calculateTotal1570(Obj)
{
    //initialize the Total to being a number
    Sum_is_NaN = false;
	Obj.text_c1570.value = "";
	addToField( Obj.text_c1570, Obj.text_c1571 );
	addToField( Obj.text_c1570, Obj.text_c1572 );
	addToField( Obj.text_c1570, Obj.text_c1573 );
	addToField( Obj.text_c1570, Obj.text_c1574 );
}

function calculateTotal2570(Obj)
{
	if (Obj.text_c2570 )
	{
    Sum_is_NaN = false;
	Obj.text_c2570.value = "";
	addToField( Obj.text_c2570, Obj.text_c2571 );
	addToField( Obj.text_c2570, Obj.text_c2572 );
	addToField( Obj.text_c2570, Obj.text_c2573 );
	addToField( Obj.text_c2570, Obj.text_c2574 );
	}
}


function showMessage(pLang)
{
	var lang = "" + pLang;
	if(lang == "nl" || lang == "NL")
	{
		myString1 = new String("U KAN UW AANGIFTE INDIENEN TOT EN MET 29.08.2003.\n\nVoor dit jaar, blijft het indienen van een aangifte via Tax-on-web materieel mogelijk tot eind september.\n\nDe aangiften die ons na 29.08.2003 bereiken zullen fiscaal in principe als laattijdig worden beschouwd.");
		window.alert(myString1);
	}
	if(lang == "fr" || lang == "FR")
	{
		myString2 = new String("VOUS AVEZ JUSQU\' AU 29.08.2003 POUR RENTRER VOTRE D\u00C9CLARATION DANS LES D\u00C9LAIS.\n\nPour cette ann\u00E9e, l\'introduction d\'une d\u00E9claration via Tax-on-web demeure mat\u00E9riellement possible jusqu\' \u00E0 la fin du mois de septembre.\n\nLes d\u00E9clarations nous parvenant apr\u00E8s le 29.08.2003 seront fiscalement en principe consid\u00E9r\u00E9es comme tardives.");
		window.alert(myString2);
	}
}

function isReturnConfirm(confirmReturn, alertMessage, URL)
{
    returnValue = '';
	if (confirmReturn != "null" && confirmReturn == "true")
	{
		if (confirm(alertMessage))
		{
		    returnValue = URL;
		}
	}

	return String(returnValue);
}
function isNotConfirmed(confirmReturnTaxBox, alertMessage, taxBoxURL)
{
    returnValue = '';
	if (confirmReturnTaxBox != "null" && confirmReturnTaxBox == "true")
	{
		if (!confirm(alertMessage))
		{
		    returnValue = taxBoxURL;
		}
	}

	return String(returnValue);
}
function continueConfirm(needConfirm, alertMessage, URL_OK, URL_Cancel)
{
    returnValue = '';
    if(needConfirm != "null" && needConfirm == "true")
    {
        if(confirm(alertMessage.replace(/<br>/g, '\n')))
        {
            returnValue = URL_OK;
        }
        else
        {
            returnValue = URL_Cancel;
        }
    }
    return String(returnValue);
}

function isOptimisationResultReady(displayOptimisationResult, popupUrl)
{
    if (displayOptimisationResult != "null" && displayOptimisationResult == "true")
	{
    	//var window_name = "";
        //var height = "550";
        //var width = "780";
        //var hasMenubar = "yes";
        //var hasScrollbars = "yes";
        //newWindow(popupUrl, window_name, width, height, hasMenubar, hasScrollbars);
        window.location = popupUrl;
	}
}

function optimizationResultClose(flag){
	if (flag == "true")
	{
		opener.document.forms.cadreform.submit();
		window.close();
	}
}

function calculateTotal1400(Obj, questionA1Count)
{
	Obj.text_c1400.value = calculateSumOfArr(Obj, questionA1Count, 'text_c400b_');
	addToField( Obj.text_c1400, Obj.text_P210 );
}
function calculateTotal2400(Obj, questionA1Count)
{
	Obj.text_c2400.value = calculateSumOfArr(Obj, questionA1Count, 'text_c400c_');
	addToField( Obj.text_c2400, Obj.text_P211 );
}
function calculateTotal1407(Obj, questionA1Count)
{
	Obj.text_c1407.value = calculateSumOfArr(Obj, questionA1Count, 'text_c407b_');
	addToField( Obj.text_c1407, Obj.text_P300 );
}
function calculateTotal2407(Obj, questionA1Count)
{
	Obj.text_c2407.value = calculateSumOfArr(Obj, questionA1Count, 'text_c407c_');
	addToField( Obj.text_c2407, Obj.text_P301 );
}
function calculateTotal1411(Obj, questionA1Count)
{
	Obj.text_c1411.value = calculateSumOfArr(Obj, questionA1Count, 'text_c411b_');
	addToField( Obj.text_c1411, Obj.text_P350 );
}
function calculateTotal2411(Obj, questionA1Count)
{
	Obj.text_c2411.value = calculateSumOfArr(Obj, questionA1Count, 'text_c411c_');
	addToField( Obj.text_c2411, Obj.text_P351 );
}
function calculateTotal1430(Obj, questionA1Count)
{
	Obj.text_c1430.value = calculateSumOfArr(Obj, questionA1Count, 'text_c430b_');
	addToField( Obj.text_c1430, Obj.text_P230 );
}
function calculateTotal2430(Obj, questionA1Count)
{
	Obj.text_c2430.value = calculateSumOfArr(Obj, questionA1Count, 'text_c430c_');
	addToField( Obj.text_c2430, Obj.text_P231 );
}


function submitCodeForm (form, action, anchor)
{
    form.action=action;
    form.cadreAnchor.value=anchor;
    form.submit();
}

function submitHelpForm (url, contentKey, anchor)
{
	if (anchor) document.helpForm.action = webappRoot + userContext + url + '.do#' + anchor;
	else document.helpForm.action = webappRoot + userContext + url + '.do';
    document.helpForm.elements['contentkey'].value=contentKey;
    document.helpForm.setAttribute("target", "_blank");
    document.helpForm.submit();
}

function submitLanguage(form,language)
{
  form.language.value = language;
  form.submit();
}

function goToHome()
{
	window.location = webappRoot + userContext + '/public/home.do';
}

function newWindow(mypage, myname, width, height, hasMenubar, hasScrollbars)
{
	var winl = (screen.width - width) / 2;
	var wint = (screen.height - height) / 2;
	winprops = 'height=' + height + ',width=' + width + ',top=' + wint + ',left=' + winl
	         + ',menubar=' + hasMenubar + ',scrollbars=' + hasScrollbars + ',resizable';
	win = window.open(mypage, myname, winprops);
	focusWindow(win);
}

function newWindowPAC(mypage, myname, width, height, hasMenubar, hasScrollbars)
{
	var winl = (screen.width - width) / 2;
	var wint = (screen.height - height) / 2;
	winprops = 'height=' + height + ',width=' + width + ',top=' + wint + ',left=' + winl
		+ ',menubar=' + hasMenubar + ',scrollbars=' + hasScrollbars + ',resizable';
	win = window.open(mypage, myname, winprops);
	focusWindow(win);
	return win;
}

function newSizableWindow(mypage,pWidth,pHeight){
    var window_name = "";
    var height = ""+pHeight;
    var width = ""+pWidth;
    var hasMenubar = "yes";
    var hasScrollbars = "yes";
	newWindow(mypage, window_name, width, height, hasMenubar, hasScrollbars);
}

function focusWindow(win)
{
	if (win != null && typeof(win)!='undefined' && parseInt(navigator.appVersion) >= 4)
	{
	    win.window.focus();
	}
}

function newFullScreenWindow(mypage,myname)
{
	var taille='';
	try{
 		taille="height="+screen.availHeight+",width="+(screen.availWidth-10)+",top=0,left=0,scrollbars=yes";
	}catch(error){
   		//nothing to do
	}
	var newwindow;
	try{
   		if(taille!=''){
   			newwindow=window.open(mypage,myname,taille.valueOf());
   		}else{
       		newwindow=window.open(mypage,myname);
   		}
	}catch(error){
   		newwindow=window.open(mypage,myname);
	}
	focusWindow(newwindow);
}

function newCalculWindow(mypage)
{
	newFullScreenWindow(mypage,"Taxonweb");
}

function newDefaultWindow(mypage)
{
    var window_name = "";
    var height = "350";
    var width = "760";
    var hasMenubar = "yes";
    var hasScrollbars = "yes";
    newWindow(mypage, window_name, width, height, hasMenubar, hasScrollbars);
}

function newDefaultWindowIfEmptyField(field,action){
	if(field.value == ''){
		newConfirmationWizardWindow(action);
	}
	
}

function newSizableWindowIfEmptyField(field,action,width,height){
	if(field.value == ''){
		newSizableWindow(action,width,height);
	}
}

function newConfirmationWizardWindow(mypage)
{
	var window_name = "";
    var height = "300";
    var width = "625";
    var hasMenubar = "yes";
    var hasScrollbars = "yes";
	newWindow(mypage, window_name, width, height, hasMenubar, hasScrollbars);
}

function newWizardWindow(mypage)
{
	var window_name = "";
    var height = "700";
    var width = "1000";
    var hasMenubar = "yes";
    var hasScrollbars = "yes";
	newWindow(mypage, window_name, width, height, hasMenubar, hasScrollbars);
}

function newHelpWindow(mypage, myname, width, height, hasMenubar, hasScrollbars)
{
	var winl = (screen.width - width) / 2;
	var wint = (screen.height - height) / 2;
	winprops = 'height=' + height + ',width=' + width + ',top=' + wint + ',left=' + winl
	         + ',menubar=no,toolbar=' + hasMenubar + ',scrollbars=' + hasScrollbars + ',resizable';
	win = window.open(mypage, myname, winprops);
	focusWindow(win);
}
function coupleNoCouple(obj)
{
  if (obj.check_c01_h.checked)
  {
    obj.text_c02_c.disabled = false;
  }
  else
  {
    obj.text_c02_c.value="";
    obj.text_c02_c.disabled = true;
  }
}
function checkCPLength(e,message)
{
  var code;
  if (e.keyCode) code = e.keyCode;
  else if (e.which) code = e.which;
  if (code > 32 && code < 255 && document.forms.cadreform.text_c01_c.value.length == 4)
  {
    getCommuneRate(message);
  }
}
function getCommuneRate(message) {
	if (postCode.containsKey(document.forms.cadreform.text_c01_c.value)) {
		document.forms.cadreform.text_c01_e.value = postCode.get(document.forms.cadreform.text_c01_c.value);
                if(postCodeYear.containsKey(document.forms.cadreform.text_c01_c.value)) {
                  var year=postCodeYear.get(document.forms.cadreform.text_c01_c.value);
                  if(year != '2025'){
                    window.alert(message);
                  }
                }else{
                  window.alert(message);
                }
	}
}

function checkInrIIA1(elem, message)
{
    var check_c1002 = "check_c1002";
    var check_c1012 = "check_c1012";
    var check_c1019 = "check_c1019";

    if (check_c1002 == elem || check_c1012 == elem || check_c1019 == elem)
    {
        if (document.cadreform.check_c1051.checked == 0 && document.cadreform.check_c1052.checked == 0){
            if (document.cadreform.check_c1002.checked == 1 || document.cadreform.check_c1012.checked == 1 || document.cadreform.check_c1019.checked == 1){
                alert(message);
            }
        }
    }
}


function checkInrIIA2(elem, message)
{
  var check5 = "check_c1051";
  var check6 = "check_c1052";

  if (check5 == elem || check6 == elem)
  {
      if (document.cadreform.check_c1051.checked == 1)
        alert(message);
      if (document.cadreform.check_c1052.checked == 1)
        alert(message);
  }

}

function popupOptimization(elem,message){
    if (document.forms["cadreform"].elements[elem] !=null && document.forms["cadreform"].elements[elem].value != "" && document.forms["cadreform"].elements[elem].value != null  ){
     alert(message);
    }
}

function checkIban(field, fieldPrefix) {
	if (document.getElementById(fieldPrefix + field).value.length == 4) {
		nextField = field + 1
		document.getElementById(fieldPrefix + nextField).focus();
	} else document.getElementById(fieldPrefix + field);
}

function documentClicked() {
    if(wizard!==null && !wizard.closed) {
        wizard.focus();
    }
}

function disableButtonSynchroIban() {
    const button = document.getElementById('UPDATE_IBAN_APPEARANCE');
    button.disabled = true;
}

function submitForm0(){
    document.forms[0].submit();
}

function displayInCalcul(calcul){
     if(calcul == null){
        document.getElementById("calcul").innerHTML = document.getElementById("calculButton").innerHTML;
     } else if(calcul === true){
        document.getElementById("calcul").innerHTML = document.getElementById("calculDetails").innerHTML;
        document.location='#calcul';
     }
}

function focusForOption() {
    [].forEach.call(this.options, function(o) {
        if(o.getAttribute('data-descr') != null) {
            o.textContent = o.getAttribute('value') + o.getAttribute('data-descr');
        }
    });
}

function blurForOption(){
    [].forEach.call(this.options, function(o) {
        if(o.getAttribute('data-descr') != null) {
            o.textContent = o.getAttribute('value');
        }
    });
}

function alertCookies(location) {
    if (location.length > 0) newWindow(location, "", 1080, 800, "yes", "yes");
}


function addListenersForSelects(id){
    [].forEach.call(document.querySelectorAll(id), function(s) {
        s.addEventListener('focus', focusForOption);
        s.addEventListener('blur', blurForOption);
        blurForOption.call(s);
    });
}

$(document).ready(function () {
    addListenersForSelects('.country-select');
    addListenersForSelects('.nature-select');
});
