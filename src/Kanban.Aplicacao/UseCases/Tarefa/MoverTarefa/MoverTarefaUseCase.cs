using AutoMapper;
using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;

namespace Kanban.Aplicacao.UseCases.Tarefa.MoverTarefa;

public class MoverTarefaUseCase : IMoverTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IMapper _mapper;

    public MoverTarefaUseCase(ITarefaRepository tarefaRepository, IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(int usuarioId, int tarefaId, MoverTarefaRequest request)
    {
        // Verificar se a tarefa existe e pertence ao usuário
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
        if (tarefa == null)
            throw new TarefaNaoEncontradaException();

        if (tarefa.UsuarioId != usuarioId)
            throw new AcessoNegadoException("Você não tem permissão para mover esta tarefa");

        if (request.NovaOrdem.HasValue)
            tarefa.Ordem = request.NovaOrdem.Value;
        tarefa.DataAtualizacao = DateTime.UtcNow;

        var tarefaAtualizada = await _tarefaRepository.AtualizarAsync(tarefa);
        return _mapper.Map<TarefaResponse>(tarefaAtualizada);
    }
}