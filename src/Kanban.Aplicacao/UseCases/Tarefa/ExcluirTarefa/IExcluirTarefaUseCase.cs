namespace Kanban.Aplicacao.UseCases.Tarefa.ExcluirTarefa;

public interface IExcluirTarefaUseCase
{
    Task<bool> ExecuteAsync(int usuarioId, int tarefaId);
}