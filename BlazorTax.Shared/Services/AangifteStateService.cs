namespace BlazorTax.Services;

using BlazorTax.Belastingen;
using BlazorTax.Belastingen.Berekening;
using FluentValidation;

public class AangifteStateService
{
    private readonly IValidator<AangifteState> _validator;
    private readonly GezamenlijkeBerekeningCalculator _calculator = new();

    public AangifteStateService(IValidator<AangifteState> validator)
    {
        _validator = validator;
    }

    public AangifteState State { get; private set; } = new();

    public GezamenlijkResultaat? LaatsteResultaat { get; private set; }
    public IReadOnlyList<string> ValidatieFouten { get; private set; } = [];

    public bool Bereken()
    {
        var validationResult = _validator.Validate(State);
        if (!validationResult.IsValid)
        {
            ValidatieFouten = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .Distinct()
                .ToArray();
            LaatsteResultaat = null;
            return false;
        }

        ValidatieFouten = [];

        var input = new BerekeningInput
        {
            VakII = State.VakII,
            VakIII = State.VakIII,
            VakIV = State.VakIV,
            VakV = State.VakV,
            VakVIII = State.VakVIII,
            VakIX = State.VakIX,
            VakX = State.VakX,
            VakXII = State.VakXII,
            VakXV = State.VakXV,
            VakXVI = State.VakXVI,
            VakXVII = State.VakXVII,
            VakXVIII = State.VakXVIII,
            VakXIX = State.VakXIX,
            VakXX = State.VakXX,
            VakXXI = State.VakXXI,
            Gewest = State.Gewest,
            GemeentebelastingPercentage = State.GemeentebelastingPercentage,
            TypeBeroep = State.TypeBeroep,
            NettoInkomenPartner = State.NettoInkomenPartner,
        };

        LaatsteResultaat = _calculator.Bereken(input);
        return true;
    }

    public void NieuweAangifte()
    {
        State = new AangifteState();
        LaatsteResultaat = null;
        ValidatieFouten = [];
    }
}
