using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TravelWeather.Api.Models;

namespace TravelWeather.Api.Services;

public sealed class WeatherRepository
{
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, MonthlyWeather>>> _data;

    public WeatherRepository(string dataPath)
    {
        // Se lee una vez al iniciar la aplicación; las solicitudes sólo consultan memoria.
        using var stream = File.OpenRead(dataPath);
        _data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, MonthlyWeather>>>>(
            stream, JsonSerializerOptions.Web)
            ?? throw new InvalidDataException("El archivo de clima no contiene datos.");
    }

    public string[] GetCountries() => _data.Keys.Order(StringComparer.Ordinal).ToArray();

    public bool TryGetCities(string country, [NotNullWhen(true)] out string[]? cities)
    {
        if (_data.TryGetValue(country, out var countryData))
        {
            cities = countryData.Keys.Order(StringComparer.Ordinal).ToArray();
            return true;
        }

        cities = null;
        return false;
    }

    public bool TryGetMonthlyWeather(
        string country,
        string city,
        string month,
        [NotNullWhen(true)] out MonthlyWeather? weather)
    {
        weather = null;
        return _data.TryGetValue(country, out var cities)
            && cities.TryGetValue(city, out var months)
            && months.TryGetValue(month, out weather);
    }
}
