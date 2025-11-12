using Microsoft.JSInterop;
using System.Text.Json;

namespace MyApp.Client.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage SetItem Hatası: {ex.Message}");
        }
    }

    public void SetItem<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage SetItem Hatası: {ex.Message}");
        }
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage GetItem Hatası: {ex.Message}");
            return default;
        }
    }

    public T? GetItem<T>(string key)
    {
        try
        {
            var json = _jsRuntime.InvokeAsync<string>("localStorage.getItem", key).Result;
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage GetItem Hatası: {ex.Message}");
            return default;
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage RemoveItem Hatası: {ex.Message}");
        }
    }

    public void RemoveItem(string key)
    {
        try
        {
            _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage RemoveItem Hatası: {ex.Message}");
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage Clear Hatası: {ex.Message}");
        }
    }
}
