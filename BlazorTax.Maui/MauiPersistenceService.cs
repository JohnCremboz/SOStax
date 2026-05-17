using System.IO;
using System.Text.Json;
using BlazorTax.Belastingen;
using BlazorTax.Services;

namespace BlazorTax.Maui;

public class MauiPersistenceService : IAangiftePersistenceService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static string AutoSavePath => Path.Combine(
        FileSystem.AppDataDirectory,
        "aangifte_aj2026.json");

    public async Task OpslaanAsync(AangifteState state)
    {
        // On MAUI, save directly to AppDataDirectory (no file picker for MVP)
        await AutoOpslaanAsync(state);

        // Optionally export to a user-visible location via Share (future enhancement)
    }

    public Task<AangifteState?> OpenAsync()
    {
        // On MAUI, load from AppDataDirectory (no file picker for MVP)
        return AutoOpenAsync();
    }

    public async Task AutoOpslaanAsync(AangifteState state)
    {
        var json = JsonSerializer.Serialize(state, JsonOptions);
        await File.WriteAllTextAsync(AutoSavePath, json);
    }

    public async Task<AangifteState?> AutoOpenAsync()
    {
        if (!File.Exists(AutoSavePath))
            return null;
        var json = await File.ReadAllTextAsync(AutoSavePath);
        return JsonSerializer.Deserialize<AangifteState>(json);
    }
}
