using ClinicApi.Domain;
using ClinicApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<IPasswordGenerator, RandomPasswordGenerator>();
builder.Services.AddSingleton<ITokenGenerator, SimpleTokenGenerator>();
builder.Services.AddSingleton<UserAuthService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "ClinicApi" }));

app.MapGet("/patients", () => new[]
{
    new { Id = 1, Name = "Ana Horvat", Active = true },
    new { Id = 2, Name = "Marko Kovac", Active = true }
});

app.MapPost("/users", (CreateUserRequest req, UserAuthService auth) =>
{
    try
    {
        var user = auth.CreateUser(req.Email, req.Password, req.Role);
        return Results.Created($"/users/{user.Id}", new
        {
            user.Id,
            user.Email,
            user.Role,
            user.IsActive
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
});

app.MapPost("/auth/login", (LoginRequest req, UserAuthService auth) =>
{
    try
    {
        var result = auth.Login(req.Email, req.Password);
        return Results.Ok(result);
    }
    catch (AccountDeactivatedException ex)
    {
        return Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status403Forbidden);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status401Unauthorized);
    }
});

app.MapPost("/users/{id:int}/deactivate", (int id, UserAuthService auth) =>
{
    try
    {
        var user = auth.DeactivateUser(id);
        return Results.Ok(new { user.Id, user.Email, user.IsActive });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPost("/users/{id:int}/reset-password", (int id, UserAuthService auth) =>
{
    try
    {
        var result = auth.ResetPassword(id);
        return Results.Ok(result);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
});

app.Run();

public sealed record CreateUserRequest(string Email, string Password, string Role);
public sealed record LoginRequest(string Email, string Password);

public partial class Program { }