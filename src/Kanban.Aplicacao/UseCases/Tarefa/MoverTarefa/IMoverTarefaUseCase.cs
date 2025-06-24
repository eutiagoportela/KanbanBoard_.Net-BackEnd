using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;

namespace Kanban.Aplicacao.UseCases.Tarefa.MoverTarefa;

public interface IMoverTarefaUseCase
{
    Task<TarefaResponse> ExecuteAsync(int usuarioId, int tarefaId, MoverTarefaRequest request);
}