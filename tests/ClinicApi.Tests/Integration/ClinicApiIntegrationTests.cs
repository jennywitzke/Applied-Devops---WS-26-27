using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ClinicApi.Tests.Integration;

[Trait("Category", "Integration")]
public sealed class ClinicApiIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private sealed record CreatedUserDto(int Id, string Email, string Role, bool IsActive);
    private sealed record LoginDto(int UserId, string Email, string Role, string Token);
    private sealed record ResetPasswordDto(int UserId, string Email, string TemporaryPassword);

    private static string UniqueEmail(string prefix) => $"{prefix}-{Guid.NewGuid():N}@clinic.hr";

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_Returns201_AndDuplicateEmailReturns409()
    {
        var email = UniqueEmail("dup");

        var first = await _client.PostAsJsonAsync("/users", new
        {
            email,
            password = "Password123!",
            role = "Doctor"
        });
        var second = await _client.PostAsJsonAsync("/users", new
        {
            email,
            password = "Password123!",
            role = "Nurse"
        });

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithInvalidRole_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/users", new
        {
            email = UniqueEmail("badrole"),
            password = "Password123!",
            role = "UnknownRole"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200_AndWrongPasswordReturns401()
    {
        var email = UniqueEmail("login");
        await _client.PostAsJsonAsync("/users", new
        {
            email,
            password = "Password123!",
            role = "Nurse"
        });

        var okResponse = await _client.PostAsJsonAsync("/auth/login", new { email, password = "Password123!" });
        var login = (await okResponse.Content.ReadFromJsonAsync<LoginDto>())!;
        var badResponse = await _client.PostAsJsonAsync("/auth/login", new { email, password = "WrongPassword!" });

        Assert.Equal(HttpStatusCode.OK, okResponse.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(login.Token));
        Assert.Equal(HttpStatusCode.Unauthorized, badResponse.StatusCode);
    }

    [Fact]
    public async Task DeactivateUser_Returns200_AndBlocksSubsequentLoginWith403()
    {
        var email = UniqueEmail("deact");
        var createdRes = await _client.PostAsJsonAsync("/users", new
        {
            email,
            password = "Password123!",
            role = "Receptionist"
        });
        var created = (await createdRes.Content.ReadFromJsonAsync<CreatedUserDto>())!;

        var deactivateRes = await _client.PostAsync($"/users/{created.Id}/deactivate", null);
        var loginRes = await _client.PostAsJsonAsync("/auth/login", new { email, password = "Password123!" });

        Assert.Equal(HttpStatusCode.OK, deactivateRes.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, loginRes.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_InvalidatesOldPassword_AndAllowsLoginWithTemporaryPassword()
    {
        var email = UniqueEmail("reset");
        var createdRes = await _client.PostAsJsonAsync("/users", new
        {
            email,
            password = "OldPassword1!",
            role = "Admin"
        });
        var created = (await createdRes.Content.ReadFromJsonAsync<CreatedUserDto>())!;

        var resetRes = await _client.PostAsync($"/users/{created.Id}/reset-password", null);
        var reset = (await resetRes.Content.ReadFromJsonAsync<ResetPasswordDto>())!;
        var oldLoginRes = await _client.PostAsJsonAsync("/auth/login", new { email, password = "OldPassword1!" });
        var newLoginRes = await _client.PostAsJsonAsync("/auth/login", new { email, password = reset.TemporaryPassword });

        Assert.Equal(HttpStatusCode.OK, resetRes.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginRes.StatusCode);
        Assert.Equal(HttpStatusCode.OK, newLoginRes.StatusCode);
    }
}