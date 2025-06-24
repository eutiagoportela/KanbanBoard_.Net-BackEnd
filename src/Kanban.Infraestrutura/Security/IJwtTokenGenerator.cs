using Kanban.Dominio.Entidades;

namespace Kanban.Infraestrutura.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(Usuarios usuario);
}