using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;

namespace Kanban.Aplicacao.UseCases.Tarefa.ListarTarefas;

public interface IListarTarefasUseCase
{
    Task<List<TarefaResponse>> ExecuteAsync(int usuarioId, FiltrarTarefasRequest? filtro = null);
    Task<KanbanBoardResponse> ExecuteKanbanAsync(int usuarioId);
}