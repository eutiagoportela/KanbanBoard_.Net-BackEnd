using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;

namespace Kanban.Aplicacao.UseCases.Tarefa.CriarTarefa;

public interface ICriarTarefaUseCase
{
    Task<TarefaResponse> ExecuteAsync(int usuarioId, CriarTarefaRequest request);
}