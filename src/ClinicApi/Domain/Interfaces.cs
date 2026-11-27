namespace ClinicApi.Domain;

public interface IUserRepository
{
    User? FindById(int id);
    User? FindByEmail(string email);
    User Add(User user);
    void Update(User user);
}

public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string passwordHash);
}

public interface IPasswordGenerator
{
    string GenerateTemporaryPassword();
}

public interface ITokenGenerator
{
    string Generate(User user);
}