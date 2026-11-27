using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ClinicApi.E2E.Tests;

[Trait("Category", "E2E")]
public sealed class UserLifecycleE2ETests
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:8081";

    private static readonly HttpClient Client = new() { BaseAddress = new Uri(BaseUrl) };

    private sealed record CreatedUserDto(int Id, string Email, string Role, bool IsActive);
    private sealed record LoginDto(int UserId, string Email, string Role, string Token);
    private sealed record ResetPasswordDto(int UserId, string Email, string TemporaryPassword);

    [Fact]
    public async Task FullUserAuthLifecycle_AgainstRunningContainer_WorksEndToEnd()
    {
        var email = $"e2e-{Guid.NewGuid():N}@clinic.hr";

        // 0. Container health check
        var health = await Client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, health.StatusCode);

        // 1. US-17: Create user with role
        var createRes = await Client.PostAsJsonAsync("/users", new
        {
            email,
            password = "InitialPassword1!",
            role = "Doctor"
        });
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var created = (await createRes.Content.ReadFromJsonAsync<CreatedUserDto>())!;

        // 2. US-20: Log in with initial password
        var loginRes = await Client.PostAsJsonAsync("/auth/login", new
        {
            email,
            password = "InitialPassword1!"
        });
        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);
        var login = (await loginRes.Content.ReadFromJsonAsync<LoginDto>())!;
        Assert.False(string.IsNullOrWhiteSpace(login.Token));

        // 3. US-19: Reset password & log in with temporary password
        var resetRes = await Client.PostAsync($"/users/{created.Id}/reset-password", null);
        Assert.Equal(HttpStatusCode.OK, resetRes.StatusCode);
        var reset = (await resetRes.Content.ReadFromJsonAsync<ResetPasswordDto>())!;

        var oldLoginRes = await Client.PostAsJsonAsync("/auth/login", new
        {
            email,
            password = "InitialPassword1!"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginRes.StatusCode);

        var tempLoginRes = await Client.PostAsJsonAsync("/auth/login", new
        {
            email,
            password = reset.TemporaryPassword
        });
        Assert.Equal(HttpStatusCode.OK, tempLoginRes.StatusCode);

        // 4. US-18: Deactivate user & verify login is blocked
        var deactivateRes = await Client.PostAsync($"/users/{created.Id}/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, deactivateRes.StatusCode);

        var blockedLoginRes = await Client.PostAsJsonAsync("/auth/login", new
        {
            email,
            password = reset.TemporaryPassword
        });
        Assert.Equal(HttpStatusCode.Forbidden, blockedLoginRes.StatusCode);
    }
}