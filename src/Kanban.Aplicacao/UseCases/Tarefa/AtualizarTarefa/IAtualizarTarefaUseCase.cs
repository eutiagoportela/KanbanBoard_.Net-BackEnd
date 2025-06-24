using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;

namespace Kanban.Aplicacao.UseCases.Tarefa.AtualizarTarefa;

public interface IAtualizarTarefaUseCase
{
    Task<TarefaResponse> ExecuteAsync(int usuarioId, int tarefaId, AtualizarTarefaRequest request);
}