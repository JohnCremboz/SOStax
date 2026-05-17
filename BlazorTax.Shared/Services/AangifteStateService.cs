namespace BlazorTax.Services;

using BlazorTax.Belastingen;
using BlazorTax.Belastingen.Berekening;
using FluentValidation;

public class AangifteStateService
{
    private readonly IValidator<AangifteState> _validator;
    private readonly GezamenlijkeBerekeningCalculator _calculator = new();
    private readonly IAangiftePersistenceService _persistence;

    public AangifteStateService(IValidator<AangifteState> validator, IAangiftePersistenceService persistence)
    {
        _validator = validator;
        _persistence = persistence;
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
            VakVI = State.VakVI,
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
            TypeBeroepPartner = State.TypeBeroepPartner,
            NettoInkomenPartner = State.NettoInkomenPartner,
            GemiddeldeAanslagvoetVorigJaar = State.GemiddeldeAanslagvoetVorigJaar,
        };

        LaatsteResultaat = _calculator.Bereken(input);
        _ = _persistence.AutoOpslaanAsync(State);
        return true;
    }

    public void NieuweAangifte()
    {
        State = new AangifteState();
        LaatsteResultaat = null;
        ValidatieFouten = [];
    }

    public Task OpslaanAsync() => _persistence.OpslaanAsync(State);

    public async Task<bool> OpenAsync()
    {
        var geladen = await _persistence.OpenAsync();
        if (geladen is null)
            return false;
        State = geladen;
        LaatsteResultaat = null;
        ValidatieFouten = [];
        return true;
    }

    public async Task AutoOpslaanAsync() => await _persistence.AutoOpslaanAsync(State);

    public async Task<bool> AutoOpenAsync()
    {
        var geladen = await _persistence.AutoOpenAsync();
        if (geladen is null)
            return false;
        State = geladen;
        return true;
    }
}
