namespace BlazorTax.Services;

using BlazorTax.Belastingen;
using BlazorTax.Belastingen.Berekening;

public class AangifteStateService
{
    public AangifteState State { get; } = new();

    public GezamenlijkResultaat? LaatsteResultaat { get; private set; }

    public void Bereken()
    {
        var calculator = new GezamenlijkeBerekeningCalculator();
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

        LaatsteResultaat = calculator.Bereken(input);
    }
}
