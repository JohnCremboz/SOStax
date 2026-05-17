namespace BlazorTax.Belastingen.Validatie;

using System.Collections;
using FluentValidation;

public sealed class AangifteStateValidator : AbstractValidator<AangifteState>
{
    public AangifteStateValidator()
    {
        RuleFor(state => state).Custom((state, context) =>
        {
            ValidateNumericValues(state, context);
            ValidateVakIiCombinaties(state.VakII, context);
            ValidateVakIIIVerdeling(state.VakII, state.VakIII, context);
        });
    }

    private static void ValidateNumericValues(AangifteState state, ValidationContext<AangifteState> context)
    {
        foreach (var vakProperty in typeof(AangifteState).GetProperties())
        {
            var vakObject = vakProperty.GetValue(state);
            if (vakObject is null)
            {
                continue;
            }

            var vakType = vakObject.GetType();
            if (vakType == typeof(string) || vakType.IsEnum)
            {
                continue;
            }

            foreach (var field in vakType.GetProperties())
            {
                var value = field.GetValue(vakObject);
                if (value is null)
                {
                    continue;
                }

                if (value is decimal decimalValue && decimalValue < 0m)
                {
                    context.AddFailure($"{vakProperty.Name}.{field.Name}", "Negatieve bedragen zijn niet toegestaan.");
                    continue;
                }

                if (value is int intValue && intValue < 0)
                {
                    context.AddFailure($"{vakProperty.Name}.{field.Name}", "Negatieve aantallen zijn niet toegestaan.");
                    continue;
                }

                if (value is IList list)
                {
                    ValidateListValues(vakProperty.Name, field.Name, list, context);
                }
            }
        }
    }

    private static void ValidateListValues(
        string vakName,
        string fieldName,
        IList values,
        ValidationContext<AangifteState> context)
    {
        for (var index = 0; index < values.Count; index++)
        {
            var item = values[index];
            if (item is decimal amount && amount < 0m)
            {
                context.AddFailure($"{vakName}.{fieldName}[{index}]", "Negatieve bedragen zijn niet toegestaan.");
            }
        }
    }

    private static void ValidateVakIiCombinaties(VakIIData vakII, ValidationContext<AangifteState> context)
    {
        var isGehuwd = vakII.BurgerlijkeStaat == BurgerlijkeStaatCodes.GehuwdOfWettelijkSamenwonend;
        var isWeduwnaar = vakII.BurgerlijkeStaat == BurgerlijkeStaatCodes.WeduwnaarOfWeduwe;

        if (!isGehuwd && (vakII.Code1003 || vakII.Code1004 || vakII.Code1018 || vakII.Code1019))
        {
            context.AddFailure("VakII.BurgerlijkeStaat", "Codes 1003/1004/1018/1019 zijn enkel geldig bij burgerlijke staat 1002.");
        }

        if (!isWeduwnaar && (vakII.Code1011 || !string.IsNullOrWhiteSpace(vakII.GemeenschappelijkAanslag)))
        {
            context.AddFailure("VakII.BurgerlijkeStaat", "Codes 1011/1012/1013 zijn enkel geldig bij burgerlijke staat 1010.");
        }

        if (isWeduwnaar && vakII.Code1011 &&
            vakII.GemeenschappelijkAanslag is not (GemeenschappelijkAanslagCodes.EenGemeenschappelijkeAanslag or GemeenschappelijkAanslagCodes.TweeAfzonderlijkeAanslagen))
        {
            context.AddFailure("VakII.GemeenschappelijkAanslag", "Kies 1012 of 1013 wanneer code 1011 is aangevinkt.");
        }

        if (!vakII.Code1022 && (vakII.Code1023 || vakII.Code1024 || !string.IsNullOrWhiteSpace(vakII.GemeenschappelijkAanslagOverledene)))
        {
            context.AddFailure("VakII.Code1022", "Codes 1023/1024/1025/1026 zijn enkel geldig wanneer code 1022 is aangevinkt.");
        }

        if (vakII.Code1022 && vakII.Code1023 && vakII.Code1024)
        {
            context.AddFailure("VakII", "Codes 1023 en 1024 kunnen niet tegelijk aangevinkt zijn.");
        }

    }

    /// <summary>
    /// Valideert de verdeling van onroerende inkomsten (Vak III) tussen de twee partners.
    /// Regels op basis van art. 7 WIB92 en de huwelijksvermogensstelsels:
    /// <list type="bullet">
    ///   <item>Partner-codes (2106/2107/…) zijn enkel geldig bij een gemeenschappelijke aanslag.</item>
    ///   <item>Bij huwelijk onder het wettelijk stelsel geldt een 50/50-verdeling van code 1106/2106.</item>
    /// </list>
    /// </summary>
    private static void ValidateVakIIIVerdeling(VakIIData vakII, VakIIIData vakIII, ValidationContext<AangifteState> context)
    {
        bool isGemeenschappelijk = (vakII.BurgerlijkeStaat == BurgerlijkeStaatCodes.GehuwdOfWettelijkSamenwonend
                                        && !vakII.Code1003 && !vakII.Code1018)
                                   || (vakII.BurgerlijkeStaat == BurgerlijkeStaatCodes.WeduwnaarOfWeduwe
                                        && vakII.Code1011);

        // Partner-codes zijn enkel geldig bij een gemeenschappelijke aanslag
        bool heeftPartnerCodes = (vakIII.Code2106 ?? 0) > 0 || (vakIII.Code2107 ?? 0) > 0
                               || (vakIII.Code2108 ?? 0) > 0 || (vakIII.Code2109 ?? 0) > 0
                               || (vakIII.Code2110 ?? 0) > 0 || (vakIII.Code2112 ?? 0) > 0
                               || (vakIII.Code2113 ?? 0) > 0 || (vakIII.Code2115 ?? 0) > 0
                               || (vakIII.Code2116 ?? 0) > 0;

        if (!isGemeenschappelijk && heeftPartnerCodes)
        {
            context.AddFailure("VakIII",
                "Codes 2106/2107/2108/2109/2110/2112/2113/2115/2116 zijn enkel geldig bij een gemeenschappelijke aanslag (gehuwden of wettelijk samenwonenden).");
        }

        // Bij gehuwden onder het wettelijk stelsel geldt een 50/50-verdeling voor code 1106/2106
        if (isGemeenschappelijk)
        {
            decimal ki1 = (vakIII.Code1106 ?? 0) + (vakIII.Code1107 ?? 0) + (vakIII.Code1108 ?? 0);
            decimal ki2 = (vakIII.Code2106 ?? 0) + (vakIII.Code2107 ?? 0) + (vakIII.Code2108 ?? 0);

            if (ki1 > 0 && ki2 == 0)
            {
                context.AddFailure("VakIII",
                    "Enkel partner 1 heeft onroerende inkomsten (codes 1106/1107/1108) ingevuld. "
                    + "Bij huwelijk onder het wettelijk stelsel worden onroerende inkomsten 50/50 verdeeld. "
                    + "Geldt dit niet voor u (scheiding van goederen / wettelijke samenwoning), dan kunt u deze melding negeren.");
            }
            else if (ki2 > 0 && ki1 == 0)
            {
                context.AddFailure("VakIII",
                    "Enkel partner 2 heeft onroerende inkomsten (codes 2106/2107/2108) ingevuld. "
                    + "Bij huwelijk onder het wettelijk stelsel worden onroerende inkomsten 50/50 verdeeld. "
                    + "Geldt dit niet voor u (scheiding van goederen / wettelijke samenwoning), dan kunt u deze melding negeren.");
            }
            else if (ki1 > 0 && ki2 > 0 && ki1 != ki2)
            {
                context.AddFailure("VakIII",
                    $"De onroerende inkomsten zijn ongelijk verdeeld ({ki1:N2} vs {ki2:N2}). "
                    + "Bij huwelijk onder het wettelijk stelsel geldt een gelijke (50/50) verdeling. "
                    + "Geldt dit niet voor u (scheiding van goederen / wettelijke samenwoning), dan kunt u deze melding negeren.");
            }
        }
    }
}
