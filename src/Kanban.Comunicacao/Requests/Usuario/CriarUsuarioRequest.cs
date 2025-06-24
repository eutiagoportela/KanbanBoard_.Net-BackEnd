using System.ComponentModel.DataAnnotations;

namespace Kanban.Comunicacao.Requests.Usuario;

public class CriarUsuarioRequest
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    public string Senha { get; set; } = string.Empty;
}