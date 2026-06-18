namespace Calvin.BookingService.Endpoints;

// RFC 9457 Problem Details responses (application/problem+json). We also add a
// flat `error` extension member so the existing frontend (which reads `err.error`)
// shows a friendly message.
public static class Problems
{
    public static IResult Validation(string detail) =>
        Build(400, "Validation error", "https://example.com/errors/validation", detail);

    public static IResult NotFound(string detail) =>
        Build(404, "Not found", "https://example.com/errors/not-found", detail);

    public static IResult Conflict(string detail) =>
        Build(409, "Booking conflict", "https://example.com/errors/conflict", detail);

    private static IResult Build(int status, string title, string type, string detail) =>
        Results.Problem(
            detail: detail,
            statusCode: status,
            title: title,
            type: type,
            extensions: new Dictionary<string, object?> { ["error"] = detail });
}
