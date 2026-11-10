namespace TravelWeather.Api.Models;

// La adaptación interpreta las cifras originales como grados Fahrenheit.
// El archivo de origen no especifica una unidad; no se convierten los números.
public sealed record MonthlyWeather(int High, int Low);
