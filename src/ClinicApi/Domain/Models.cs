namespace ClinicApi.Domain;

public sealed class User
{
    public int Id { get; set; }
    public required string Email { get; init; }
    public required string PasswordHash { get; set; }
    public required string Role { get; init; }
    public bool IsActive { get; set; } = true;
}

public sealed record LoginResult(int UserId, string Email, string Role, string Token);

public sealed record PasswordResetResult(int UserId, string Email, string TemporaryPassword);

public sealed class AccountDeactivatedException(string message) : Exception(message);