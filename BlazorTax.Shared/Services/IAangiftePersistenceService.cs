namespace BlazorTax.Services;

using BlazorTax.Belastingen;

public interface IAangiftePersistenceService
{
    /// <summary>Shows save dialog and writes state to chosen file. Also updates auto-save.</summary>
    Task OpslaanAsync(AangifteState state);

    /// <summary>Shows open dialog and returns loaded state, or null when cancelled.</summary>
    Task<AangifteState?> OpenAsync();

    /// <summary>Silently saves state to the default auto-save location.</summary>
    Task AutoOpslaanAsync(AangifteState state);

    /// <summary>Restores state from auto-save location. Returns null when no auto-save exists.</summary>
    Task<AangifteState?> AutoOpenAsync();
}
