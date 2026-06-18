using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Calvin.BookingService.IntegrationTests;

// Integration tests for the /api/bookings endpoints.
// Each test class creates its own factory (and therefore its own isolated in-memory DB)
// so tests are fully independent.
public class BookingsEndpointsTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    // Shared date far in the future so "date must not be in the past" never fires.
    private const string FutureDate = "2027-06-20";

    // Seed data IDs available after DbSeeder runs.
    private const string RoomId = "KR-KOE-01";
    private const string LocationId = "koeln";
    private const string UserId = "alice";

    public BookingsEndpointsTests(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -------------------------------------------------------------------------
    // GET /api/bookings
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBookings_Returns200WithList()
    {
        var response = await _client.GetAsync("api/bookings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var array = JsonNode.Parse(json)?.AsArray();
        Assert.NotNull(array);
        // Seeded demo bookings (past-dated) may or may not appear depending on filter,
        // but the endpoint must return a JSON array.
    }

    // -------------------------------------------------------------------------
    // POST /api/bookings — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task PostBooking_ValidRequest_Returns201WithLocationHeader()
    {
        var request = new
        {
            type = "ConferenceRoom",
            resourceId = RoomId,
            locationId = LocationId,
            userId = UserId,
            date = FutureDate,
            timeFrom = "09:00",
            timeTo = "10:00",
            title = "Integration Test Meeting"
        };

        var response = await _client.PostAsJsonAsync("api/bookings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        Assert.NotNull(body);
        Assert.Equal("ConferenceRoom", body!["type"]?.GetValue<string>());
        Assert.Equal(FutureDate, body["dateIso"]?.GetValue<string>());
    }

    // -------------------------------------------------------------------------
    // POST /api/bookings — duplicate booking → 409
    // -------------------------------------------------------------------------

    [Fact]
    public async Task PostBooking_DuplicateSlot_Returns409Conflict()
    {
        var request = new
        {
            type = "ConferenceRoom",
            resourceId = "KR-BER-01",
            locationId = "berlin",
            userId = UserId,
            date = "2027-07-10",
            timeFrom = "14:00",
            timeTo = "15:00",
            title = "First booking"
        };

        // First booking must succeed.
        var first = await _client.PostAsJsonAsync("api/bookings", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        // Same resource + overlapping time → conflict.
        var second = await _client.PostAsJsonAsync("api/bookings", request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    // -------------------------------------------------------------------------
    // POST /api/bookings — missing required field → 400
    // -------------------------------------------------------------------------

    [Fact]
    public async Task PostBooking_MissingType_Returns400()
    {
        // 'type' is omitted (empty string triggers the validation guard).
        var request = new
        {
            type = "",          // blank → "Field 'type' is required."
            resourceId = RoomId,
            locationId = LocationId,
            userId = UserId,
            date = FutureDate
        };

        var response = await _client.PostAsJsonAsync("api/bookings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // GET /api/bookings/{id} — existing booking → 200
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBookingById_ExistingId_Returns200()
    {
        // Create a booking first so we have a known ID.
        var create = new
        {
            type = "Workplace",
            resourceId = "AP-01",
            locationId = LocationId,
            userId = UserId,
            date = "2027-08-01",
            title = "Bürotag"
        };
        var created = await _client.PostAsJsonAsync("api/bookings", create);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var body = await created.Content.ReadFromJsonAsync<JsonNode>();
        var id = body!["id"]!.GetValue<string>();

        var response = await _client.GetAsync($"api/bookings/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = await response.Content.ReadFromJsonAsync<JsonNode>();
        Assert.Equal(id, detail!["id"]?.GetValue<string>());
    }

    // -------------------------------------------------------------------------
    // GET /api/bookings/{id} — unknown ID → 404
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBookingById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync("api/bookings/CLVN-B-DOES-NOT-EXIST");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // POST /api/bookings/{id}/cancellation — cancels booking, returns 200
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CancelBooking_ExistingActiveBooking_Returns200WithCancelledStatus()
    {
        // Create a fresh booking to cancel.
        var create = new
        {
            type = "ConferenceRoom",
            resourceId = "KR-HAM-01",
            locationId = "hamburg",
            userId = "bob",
            date = "2027-09-15",
            timeFrom = "11:00",
            timeTo = "12:00",
            title = "Zu stornierende Buchung"
        };
        var created = await _client.PostAsJsonAsync("api/bookings", create);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var body = await created.Content.ReadFromJsonAsync<JsonNode>();
        var id = body!["id"]!.GetValue<string>();

        var response = await _client.PostAsync($"api/bookings/{id}/cancellation", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = await response.Content.ReadFromJsonAsync<JsonNode>();
        Assert.Equal("Cancelled", detail!["status"]?.GetValue<string>());
    }

    // -------------------------------------------------------------------------
    // PUT /api/bookings/{id} — updates booking, returns 200
    // -------------------------------------------------------------------------

    [Fact]
    public async Task PutBooking_ValidUpdate_Returns200WithUpdatedData()
    {
        // Create a booking to update.
        var create = new
        {
            type = "ConferenceRoom",
            resourceId = "KR-MUC-01",
            locationId = "muenchen",
            userId = "charlie",
            date = "2027-10-01",
            timeFrom = "08:00",
            timeTo = "09:00",
            title = "Original Titel"
        };
        var created = await _client.PostAsJsonAsync("api/bookings", create);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var body = await created.Content.ReadFromJsonAsync<JsonNode>();
        var id = body!["id"]!.GetValue<string>();

        // Update to a different time slot on the same resource.
        var update = new
        {
            type = "ConferenceRoom",
            resourceId = "KR-MUC-01",
            locationId = "muenchen",
            date = "2027-10-02",
            timeFrom = "10:00",
            timeTo = "11:30",
            title = "Geänderter Titel"
        };
        var response = await _client.PutAsJsonAsync($"api/bookings/{id}", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<JsonNode>();
        Assert.Equal("2027-10-02", updated!["dateIso"]?.GetValue<string>());
        Assert.Equal("Geänderter Titel", updated["title"]?.GetValue<string>());
    }

    // -------------------------------------------------------------------------
    // DELETE /api/bookings/{id} — deletes booking, returns 204
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeleteBooking_ExistingId_Returns204()
    {
        // Create a booking to delete.
        var create = new
        {
            type = "Workplace",
            resourceId = "AP-04",
            locationId = LocationId,
            userId = "bob",
            date = "2027-11-05",
            title = "Zu löschende Buchung"
        };
        var created = await _client.PostAsJsonAsync("api/bookings", create);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var body = await created.Content.ReadFromJsonAsync<JsonNode>();
        var id = body!["id"]!.GetValue<string>();

        var deleteResponse = await _client.DeleteAsync($"api/bookings/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Confirm it's gone.
        var getResponse = await _client.GetAsync($"api/bookings/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
