using Kanban.Dominio.Entidades;

namespace Kanban.Dominio.Repositorios.Interfaces;

/// <summary>
/// Interface para repositório de usuários
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Obtém um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<Usuarios?> ObterPorIdAsync(int id);

    /// <summary>
    /// Obtém um usuário pelo email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<Usuarios?> ObterPorEmailAsync(string email);

    /// <summary>
    /// Cria um novo usuário no banco de dados
    /// </summary>
    /// <param name="usuario">Usuário a ser criado</param>
    /// <returns>Usuário criado com ID gerado</returns>
    Task<Usuarios> CriarAsync(Usuarios usuario);

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="usuario">Usuário a ser atualizado</param>
    /// <returns>Usuário atualizado</returns>
    Task<Usuarios> AtualizarAsync(Usuarios usuario);

    /// <summary>
    /// Remove um usuário (soft delete - marca como inativo)
    /// </summary>
    /// <param name="id">ID do usuário a ser removido</param>
    /// <returns>True se removido com sucesso</returns>
    Task<bool> RemoverAsync(int id);

    /// <summary>
    /// Verifica se um email já está em uso
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <returns>True se email já existe</returns>
    Task<bool> EmailExisteAsync(string email);

    /// <summary>
    /// Lista todos os usuários
    /// </summary>
    /// <returns>Lista de usuários</returns>
    Task<IEnumerable<Usuarios>> ListarTodosAsync();
}