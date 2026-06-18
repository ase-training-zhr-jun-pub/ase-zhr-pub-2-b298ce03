using Calvin.BookingService.Data;

namespace Calvin.BookingService.Endpoints;

// Prototype identity (CLVN-028/029): the client picks a known test user from
// this list and stores the selection locally — there is no real session/SSO yet
// (production will use Okta, see technical debt T-002). These endpoints provide
// the selectable users and lookup by id.
public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", (CalvinDbContext db) =>
            Results.Ok(db.Users.OrderBy(u => u.Name).ToList()));

        group.MapGet("/{id}", (string id, CalvinDbContext db) =>
            db.Users.FirstOrDefault(u => u.Id == id) is { } user
                ? Results.Ok(user)
                : Problems.NotFound($"User '{id}' not found."));

        return app;
    }
}
