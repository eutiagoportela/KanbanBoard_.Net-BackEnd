using Kanban.Comunicacao.Requests.Usuario;
using Kanban.Comunicacao.Responses.Usuario;

namespace Kanban.Aplicacao.UseCases.Usuario.CriarUsuario;

/// <summary>
/// Interface para o caso de uso de criação de usuário
/// </summary>
public interface ICriarUsuarioUseCase
{
    /// <summary>
    /// Executa o caso de uso de criação de usuário
    /// </summary>
    /// <param name="request">Dados do usuário a ser criado</param>
    /// <returns>Dados do usuário criado</returns>
    /// <exception cref="EmailJaExisteException">Quando o email já está cadastrado</exception>
    /// <exception cref="ValidacaoException">Quando os dados são inválidos</exception>
    Task<UsuarioResponse> ExecuteAsync(CriarUsuarioRequest request);
}