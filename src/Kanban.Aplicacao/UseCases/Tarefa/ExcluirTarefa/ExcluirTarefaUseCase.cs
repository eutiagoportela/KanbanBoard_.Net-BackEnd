using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;

namespace Kanban.Aplicacao.UseCases.Tarefa.ExcluirTarefa;

public class ExcluirTarefaUseCase : IExcluirTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;

    public ExcluirTarefaUseCase(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<bool> ExecuteAsync(int usuarioId, int tarefaId)
    {
        // Verificar se a tarefa existe e pertence ao usuário
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
        if (tarefa == null)
            throw new TarefaNaoEncontradaException();

        if (tarefa.UsuarioId != usuarioId)
            throw new AcessoNegadoException("Você não tem permissão para excluir esta tarefa");

        return await _tarefaRepository.ExcluirAsync(tarefaId);
    }
}