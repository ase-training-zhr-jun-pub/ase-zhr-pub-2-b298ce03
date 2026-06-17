using Calvin.BookingService.Data;

namespace Calvin.BookingService.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", (InMemoryStore store) =>
            Results.Ok(store.Users));

        return app;
    }
}
