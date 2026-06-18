using System.Net;
using System.Text;
using System.Text.Json;
using NBomber.CSharp;
using NBomber.Http.CSharp;

// Base URL can be overridden via env var (default: local dev server)
var baseUrl = Environment.GetEnvironmentVariable("CALVIN_BASE_URL") ?? "http://localhost:5000";

using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

// Seed data resource IDs from backend/Data/SeedData.cs
// Conference rooms: KR-KOE-01, KR-KOE-02, KR-BER-01, KR-BER-02, KR-HAM-01, KR-MUC-01
// Users: alice, bob, charlie
// Locations: koeln, berlin, hamburg, muenchen, ...
const string RoomId = "KR-KOE-01";
const string LocationId = "koeln";
const string ConflictRoomId = "KR-BER-01";
const string ConflictLocationId = "berlin";
const string ConflictDate = "2027-09-01";

// Helper: extract the actual HTTP status code from an NBomber response payload.
// FSharpOption<T>.None is represented as null in C#, so a null-check suffices.
static HttpStatusCode? GetHttpStatus(NBomber.Contracts.Response<System.Net.Http.HttpResponseMessage> response)
{
    var payload = response.Payload;
    return payload != null ? payload.Value.StatusCode : null;
}

// ─── Scenario: list-bookings ─────────────────────────────────────────────────
var listBookingsScenario = Scenario.Create("list-bookings", async context =>
{
    var request = Http.CreateRequest("GET", $"{baseUrl}/api/bookings");
    var response = await Http.Send(httpClient, request);
    var httpStatus = GetHttpStatus(response);

    return httpStatus == HttpStatusCode.OK
        ? Response.Ok(statusCode: ((int)HttpStatusCode.OK).ToString())
        : Response.Fail(statusCode: ((int)(httpStatus ?? HttpStatusCode.InternalServerError)).ToString(),
                        message: $"Expected 200, got {httpStatus}");
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(30))
);

// ─── Scenario: list-rooms ────────────────────────────────────────────────────
var listRoomsScenario = Scenario.Create("list-rooms", async context =>
{
    var request = Http.CreateRequest("GET", $"{baseUrl}/api/conference-rooms");
    var response = await Http.Send(httpClient, request);
    var httpStatus = GetHttpStatus(response);

    return httpStatus == HttpStatusCode.OK
        ? Response.Ok(statusCode: ((int)HttpStatusCode.OK).ToString())
        : Response.Fail(statusCode: ((int)(httpStatus ?? HttpStatusCode.InternalServerError)).ToString(),
                        message: $"Expected 200, got {httpStatus}");
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(30))
);

// ─── Scenario: create-booking ────────────────────────────────────────────────
// Each iteration uses a unique date (per virtual user + iteration counter) to
// avoid conflicts, ensuring all requests succeed with 201 Created.
// Dates start 2027-01-01 and advance, far enough into the future to be valid.
var createBookingScenario = Scenario.Create("create-booking", async context =>
{
    // Unique date per virtual user instance and iteration — multiplied by a large
    // offset so different VUs never land on the same date.
    var dayOffset = (long)context.ScenarioInfo.InstanceNumber * 10_000L + context.InvocationNumber;
    var date = new DateOnly(2027, 1, 1).AddDays((int)dayOffset).ToString("yyyy-MM-dd");

    var body = JsonSerializer.Serialize(new
    {
        type = "ConferenceRoom",
        resourceId = RoomId,
        locationId = LocationId,
        userId = "alice",
        date,
        timeFrom = "09:00",
        timeTo = "10:00",
        title = $"Load Test Booking {date}",
    });

    var request = Http.CreateRequest("POST", $"{baseUrl}/api/bookings")
        .WithHeader("Content-Type", "application/json")
        .WithBody(new StringContent(body, Encoding.UTF8, "application/json"));

    var response = await Http.Send(httpClient, request);
    var httpStatus = GetHttpStatus(response);

    return httpStatus == HttpStatusCode.Created
        ? Response.Ok(statusCode: ((int)HttpStatusCode.Created).ToString())
        : Response.Fail(statusCode: ((int)(httpStatus ?? HttpStatusCode.InternalServerError)).ToString(),
                        message: $"Expected 201, got {httpStatus}");
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(30))
);

// ─── Scenario: conflict-detection ────────────────────────────────────────────
// Multiple virtual users attempt to book the same resource on the same date
// and time. The first one wins (201 Created); the rest receive 409 Conflict.
// Both outcomes are valid — we verify the server handles contention correctly
// under load and never returns a 5xx error.
var conflictDetectionScenario = Scenario.Create("conflict-detection", async context =>
{
    var body = JsonSerializer.Serialize(new
    {
        type = "ConferenceRoom",
        resourceId = ConflictRoomId,
        locationId = ConflictLocationId,
        userId = "bob",
        date = ConflictDate,
        timeFrom = "14:00",
        timeTo = "15:00",
        title = "Conflict Load Test",
    });

    var request = Http.CreateRequest("POST", $"{baseUrl}/api/bookings")
        .WithHeader("Content-Type", "application/json")
        .WithBody(new StringContent(body, Encoding.UTF8, "application/json"));

    var response = await Http.Send(httpClient, request);
    var httpStatus = GetHttpStatus(response);

    // 201 (first writer wins) and 409 (conflict) are both acceptable outcomes.
    var isExpected = httpStatus is HttpStatusCode.Created or HttpStatusCode.Conflict;

    return isExpected
        ? Response.Ok(statusCode: ((int)httpStatus!).ToString())
        : Response.Fail(statusCode: ((int)(httpStatus ?? HttpStatusCode.InternalServerError)).ToString(),
                        message: $"Unexpected status: {httpStatus}");
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(30))
);

// ─── Run all scenarios ───────────────────────────────────────────────────────
NBomberRunner
    .RegisterScenarios(
        listBookingsScenario,
        listRoomsScenario,
        createBookingScenario,
        conflictDetectionScenario
    )
    .WithReportFileName("calvin-load-test-report")
    .WithReportFolder("load-test-results")
    .Run();
