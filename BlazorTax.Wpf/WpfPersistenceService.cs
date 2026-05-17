using System.IO;
using System.Text.Json;
using System.Windows;
using BlazorTax.Belastingen;
using BlazorTax.Services;
using Microsoft.Win32;

namespace BlazorTax.Wpf;

public class WpfPersistenceService : IAangiftePersistenceService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static readonly string AutoSavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SOStax",
        "aangifte_aj2026.json");

    public async Task OpslaanAsync(AangifteState state)
    {
        string? path = null;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new SaveFileDialog
            {
                Title = "Aangifte opslaan",
                Filter = "JSON-bestanden (*.json)|*.json|Alle bestanden (*.*)|*.*",
                DefaultExt = ".json",
                FileName = "aangifte_aj2026.json",
            };
            if (dialog.ShowDialog() == true)
                path = dialog.FileName;
        });

        if (path is not null)
            await WriteJsonAsync(path, state);

        await AutoOpslaanAsync(state);
    }

    public async Task<AangifteState?> OpenAsync()
    {
        string? path = null;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new OpenFileDialog
            {
                Title = "Aangifte openen",
                Filter = "JSON-bestanden (*.json)|*.json|Alle bestanden (*.*)|*.*",
                DefaultExt = ".json",
            };
            if (dialog.ShowDialog() == true)
                path = dialog.FileName;
        });

        return path is not null ? await ReadJsonAsync(path) : null;
    }

    public async Task AutoOpslaanAsync(AangifteState state)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(AutoSavePath)!);
        await WriteJsonAsync(AutoSavePath, state);
    }

    public async Task<AangifteState?> AutoOpenAsync()
    {
        if (!File.Exists(AutoSavePath))
            return null;
        return await ReadJsonAsync(AutoSavePath);
    }

    private static async Task WriteJsonAsync(string path, AangifteState state)
    {
        var json = JsonSerializer.Serialize(state, JsonOptions);
        await File.WriteAllTextAsync(path, json);
    }

    private static async Task<AangifteState?> ReadJsonAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<AangifteState>(json);
    }
}
