namespace ClinicApi.Domain;

public sealed class UserAuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IPasswordGenerator passwordGenerator,
    ITokenGenerator tokenGenerator)
{
    private static readonly HashSet<string> AllowedRoles =
        new(StringComparer.OrdinalIgnoreCase) { "Admin", "Doctor", "Nurse", "Receptionist" };

    public User CreateUser(string email, string password, string role)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedEmail) || !normalizedEmail.Contains('@'))
            throw new ArgumentException("A valid email address is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));

        var normalizedRole = AllowedRoles.FirstOrDefault(r =>
            r.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));
        if (normalizedRole is null)
            throw new ArgumentException($"Unsupported role '{role}'.", nameof(role));

        if (users.FindByEmail(normalizedEmail) is not null)
            throw new InvalidOperationException($"A user with email '{normalizedEmail}' already exists.");

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(password),
            Role = normalizedRole,
            IsActive = true
        };

        return users.Add(user);
    }

    public LoginResult Login(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = users.FindByEmail(normalizedEmail)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new AccountDeactivatedException("User account is deactivated.");

        if (!passwordHasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = tokenGenerator.Generate(user);
        return new LoginResult(user.Id, user.Email, user.Role, token);
    }

    public User DeactivateUser(int id)
    {
        var user = users.FindById(id)
            ?? throw new KeyNotFoundException($"User {id} was not found.");

        user.IsActive = false;
        users.Update(user);
        return user;
    }

    public PasswordResetResult ResetPassword(int id)
    {
        var user = users.FindById(id)
            ?? throw new KeyNotFoundException($"User {id} was not found.");

        if (!user.IsActive)
            throw new InvalidOperationException("Cannot reset password for a deactivated account.");

        var tempPassword = passwordGenerator.GenerateTemporaryPassword();
        user.PasswordHash = passwordHasher.Hash(tempPassword);
        users.Update(user);

        return new PasswordResetResult(user.Id, user.Email, tempPassword);
    }
}