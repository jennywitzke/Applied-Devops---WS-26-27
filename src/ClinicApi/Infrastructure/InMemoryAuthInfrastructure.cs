using System.Collections.Concurrent;
using System.Security.Cryptography;
using ClinicApi.Domain;

namespace ClinicApi.Infrastructure;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId;

    public User? FindById(int id) => _users.GetValueOrDefault(id);

    public User? FindByEmail(string email) =>
        _users.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public User Add(User user)
    {
        var id = Interlocked.Increment(ref _nextId);
        user.Id = id;
        _users[id] = user;
        return user;
    }

    public void Update(User user) => _users[user.Id] = user;
}

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;

    public string Hash(string plainPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(plainPassword, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string plainPassword, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(plainPassword, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public sealed class RandomPasswordGenerator : IPasswordGenerator
{
    public string GenerateTemporaryPassword() =>
        $"Tmp-{Convert.ToHexString(RandomNumberGenerator.GetBytes(6))}";
}

public sealed class SimpleTokenGenerator : ITokenGenerator
{
    public string Generate(User user) =>
        $"clinic-{user.Id}-{user.Role.ToLowerInvariant()}-{Guid.NewGuid():N}";
}