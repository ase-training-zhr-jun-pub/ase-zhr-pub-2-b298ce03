using System.Net;
using System.Text.Json.Nodes;
using Xunit;

namespace Calvin.BookingService.IntegrationTests;

public class LocationsEndpointsTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public LocationsEndpointsTests(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLocations_Returns200WithSeededLocations()
    {
        var response = await _client.GetAsync("api/locations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var array = JsonNode.Parse(json)?.AsArray();
        Assert.NotNull(array);
        Assert.NotEmpty(array);

        // Seeded data contains "Köln" (id = "koeln").
        var ids = array.Select(n => n?["id"]?.GetValue<string>()).ToList();
        Assert.Contains("koeln", ids);
    }
}
