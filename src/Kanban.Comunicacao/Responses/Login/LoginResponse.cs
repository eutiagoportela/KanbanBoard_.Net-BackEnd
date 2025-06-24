using Kanban.Comunicacao.Responses.Usuario;

namespace Kanban.Comunicacao.Responses.Login;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiracaoToken { get; set; }
    public UsuarioResponse Usuario { get; set; } = null!;
}