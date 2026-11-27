using ClinicApi.Domain;
using Xunit;

namespace ClinicApi.Tests.Unit;

[Trait("Category", "Unit")]
public sealed class UserAuthServiceTests
{
    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly Dictionary<int, User> _store = new();
        private int _nextId;

        public User? FindById(int id) => _store.GetValueOrDefault(id);

        public User? FindByEmail(string email) =>
            _store.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public User Add(User user)
        {
            user.Id = ++_nextId;
            _store[user.Id] = user;
            return user;
        }

        public void Update(User user) => _store[user.Id] = user;
    }

    private sealed class StubPasswordHasher : IPasswordHasher
    {
        public string Hash(string plainPassword) => $"HASH:{plainPassword}";
        public bool Verify(string plainPassword, string passwordHash) => passwordHash == $"HASH:{plainPassword}";
    }

    private sealed class StubPasswordGenerator : IPasswordGenerator
    {
        public string GenerateTemporaryPassword() => "Tmp-Fixed123";
    }

    private sealed class StubTokenGenerator : ITokenGenerator
    {
        public string Generate(User user) => $"token-{user.Id}-{user.Role}";
    }

    private static UserAuthService CreateService() => new(
        new FakeUserRepository(),
        new StubPasswordHasher(),
        new StubPasswordGenerator(),
        new StubTokenGenerator());

    [Fact]
    public void CreateUser_ValidInput_NormalizesEmailAndHashesPassword()
    {
        var sut = CreateService();

        var user = sut.CreateUser("  Ana.Horvat@Clinic.HR ", "StrongPass1!", "doctor");

        Assert.Equal(1, user.Id);
        Assert.Equal("ana.horvat@clinic.hr", user.Email);
        Assert.Equal("Doctor", user.Role);
        Assert.Equal("HASH:StrongPass1!", user.PasswordHash);
        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void CreateUser_InvalidEmail_ThrowsArgumentException(string email)
    {
        var sut = CreateService();

        Assert.Throws<ArgumentException>(() => sut.CreateUser(email, "StrongPass1!", "Nurse"));
    }

    [Fact]
    public void CreateUser_PasswordWithSevenChars_ThrowsArgumentException()
    {
        var sut = CreateService();

        Assert.Throws<ArgumentException>(() => sut.CreateUser("nurse@clinic.hr", "1234567", "Nurse"));
    }

    [Fact]
    public void CreateUser_PasswordWithExactEightChars_Succeeds()
    {
        var sut = CreateService();

        var user = sut.CreateUser("nurse@clinic.hr", "12345678", "Nurse");

        Assert.Equal("nurse@clinic.hr", user.Email);
    }

    [Fact]
    public void CreateUser_UnsupportedRole_ThrowsArgumentException()
    {
        var sut = CreateService();

        Assert.Throws<ArgumentException>(() => sut.CreateUser("user@clinic.hr", "StrongPass1!", "SuperHacker"));
    }

    [Fact]
    public void CreateUser_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var sut = CreateService();
        sut.CreateUser("doc@clinic.hr", "StrongPass1!", "Doctor");

        Assert.Throws<InvalidOperationException>(() => sut.CreateUser("DOC@clinic.hr", "AnotherPass1!", "Nurse"));
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsLoginResultWithToken()
    {
        var sut = CreateService();
        var user = sut.CreateUser("admin@clinic.hr", "AdminPass1!", "Admin");

        var result = sut.Login("ADMIN@clinic.hr", "AdminPass1!");

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("admin@clinic.hr", result.Email);
        Assert.Equal("Admin", result.Role);
        Assert.Equal($"token-{user.Id}-Admin", result.Token);
    }

    [Fact]
    public void Login_UnknownEmail_ThrowsUnauthorizedAccessException()
    {
        var sut = CreateService();

        Assert.Throws<UnauthorizedAccessException>(() => sut.Login("missing@clinic.hr", "StrongPass1!"));
    }

    [Fact]
    public void Login_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var sut = CreateService();
        sut.CreateUser("doc@clinic.hr", "StrongPass1!", "Doctor");

        Assert.Throws<UnauthorizedAccessException>(() => sut.Login("doc@clinic.hr", "WrongPass1!"));
    }

    [Fact]
    public void DeactivateUser_ExistingUser_SetsIsActiveFalseAndBlocksLogin()
    {
        var sut = CreateService();
        var user = sut.CreateUser("staff@clinic.hr", "StrongPass1!", "Receptionist");

        var deactivated = sut.DeactivateUser(user.Id);

        Assert.False(deactivated.IsActive);
        Assert.Throws<AccountDeactivatedException>(() => sut.Login("staff@clinic.hr", "StrongPass1!"));
    }

    [Fact]
    public void DeactivateUser_UnknownId_ThrowsKeyNotFoundException()
    {
        var sut = CreateService();

        Assert.Throws<KeyNotFoundException>(() => sut.DeactivateUser(999));
    }

    [Fact]
    public void ResetPassword_ActiveUser_ReplacesHashAndAllowsLoginWithTemporaryPassword()
    {
        var sut = CreateService();
        var user = sut.CreateUser("doc@clinic.hr", "OldPass123!", "Doctor");

        var reset = sut.ResetPassword(user.Id);

        Assert.Equal("Tmp-Fixed123", reset.TemporaryPassword);
        Assert.Throws<UnauthorizedAccessException>(() => sut.Login("doc@clinic.hr", "OldPass123!"));
        var login = sut.Login("doc@clinic.hr", "Tmp-Fixed123");
        Assert.Equal(user.Id, login.UserId);
    }

    [Fact]
    public void ResetPassword_DeactivatedUser_ThrowsInvalidOperationException()
    {
        var sut = CreateService();
        var user = sut.CreateUser("doc@clinic.hr", "OldPass123!", "Doctor");
        sut.DeactivateUser(user.Id);

        Assert.Throws<InvalidOperationException>(() => sut.ResetPassword(user.Id));
    }

    [Fact]
    public void ResetPassword_UnknownId_ThrowsKeyNotFoundException()
    {
        var sut = CreateService();

        Assert.Throws<KeyNotFoundException>(() => sut.ResetPassword(999));
    }
}