using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TravelWeather.Api.Models;
using Xunit;

namespace TravelWeather.Api.Tests;

public sealed class BaselineApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BaselineApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Root_RedirectsToLocalDocumentation()
    {
        using var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/docs", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Countries_ReturnsExactlyTheSevenCountriesInOrdinalOrder()
    {
        using var response = await _client.GetAsync("/countries");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(
            new[] { "England", "France", "Germany", "Italy", "Peru", "Portugal", "Spain" },
            await response.Content.ReadFromJsonAsync<string[]>());
    }

    [Fact]
    public async Task MonthlyWeather_ReturnsUnchangedValuesInterpretedAsFahrenheit()
    {
        using var response = await _client.GetAsync("/countries/Spain/Seville/January");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new MonthlyWeather(61, 41), await response.Content.ReadFromJsonAsync<MonthlyWeather>());

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(61, json.RootElement.GetProperty("high").GetInt32());
        Assert.Equal(41, json.RootElement.GetProperty("low").GetInt32());
    }

    [Theory]
    [InlineData("/countries/Atlantis/Seville/January")]
    [InlineData("/countries/Spain/Madrid/January")]
    [InlineData("/countries/Spain/Seville/NotAMonth")]
    [InlineData("/countries/spain/Seville/January")]
    [InlineData("/countries/Spain/seville/January")]
    [InlineData("/countries/Spain/Seville/january")]
    public async Task MonthlyWeather_UnknownKeysReturn404ProblemDetails(string path)
    {
        using var response = await _client.GetAsync(path);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(404, json.RootElement.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(json.RootElement.GetProperty("title").GetString()));
    }

    [Fact]
    public async Task Documentation_IsAvailableLocally()
    {
        using var response = await _client.GetAsync("/docs/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("openapi/v1.json", html);
        Assert.Contains("scalar.js", html);
    }

    [Fact]
    public async Task ScalarJavaScript_IsServedByTheLocalApplication()
    {
        using var response = await _client.GetAsync("/docs/scalar.js");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("javascript", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApi_ProvidesInputAndResponseExamplesForTheRequestClient()
    {
        using var response = await _client.GetAsync("/openapi/v1.json");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var operation = json.RootElement.GetProperty("paths")
            .GetProperty("/countries/{country}/{city}/{month}").GetProperty("get");
        var examples = operation.GetProperty("parameters").EnumerateArray()
            .ToDictionary(parameter => parameter.GetProperty("name").GetString()!,
                parameter => parameter.GetProperty("example").GetString());

        Assert.Equal("Spain", examples["country"]);
        Assert.Equal("Seville", examples["city"]);
        Assert.Equal("January", examples["month"]);
        Assert.False(operation.TryGetProperty("requestBody", out _));
        var weather = operation.GetProperty("responses").GetProperty("200")
            .GetProperty("content").GetProperty("application/json").GetProperty("example");
        Assert.Equal(61, weather.GetProperty("high").GetInt32());
        Assert.Equal(41, weather.GetProperty("low").GetInt32());
    }

    [Fact]
    public async Task OpenApi_DescribesTheBaselineRoutes()
    {
        using var response = await _client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.StartsWith("3.", json.RootElement.GetProperty("openapi").GetString());
        var paths = json.RootElement.GetProperty("paths");
        Assert.True(paths.GetProperty("/countries").TryGetProperty("get", out _));
        Assert.True(paths.GetProperty("/countries/{country}/{city}/{month}").TryGetProperty("get", out _));
    }
}
