namespace Kanban.Infraestrutura.Security;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}