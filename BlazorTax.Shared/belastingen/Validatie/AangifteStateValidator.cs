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
}
