using Kanban.Dominio.Entidades;
using Kanban.Dominio.Enum;

namespace Kanban.Dominio.Repositorios.Interfaces;

public interface ITarefaRepository
{
    Task<Tarefas?> ObterPorIdAsync(int id);
    Task<Tarefas> CriarAsync(Tarefas tarefa);
    Task<Tarefas> AtualizarAsync(Tarefas tarefa);
    Task<bool> ExcluirAsync(int id);
    Task<List<Tarefas>> ListarPorUsuarioAsync(int usuarioId);
    Task<List<Tarefas>> ListarPorStatusAsync(int usuarioId, StatusTarefa status);
    Task<List<Tarefas>> ListarComFiltroAsync(int usuarioId, string? termoBusca);
    Task<bool> TarefaExisteAsync(int id);
    Task<bool> UsuarioPossuiTarefaAsync(int usuarioId, int tarefaId);
}