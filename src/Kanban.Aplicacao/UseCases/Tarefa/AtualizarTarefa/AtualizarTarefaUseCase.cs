using AutoMapper;
using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;

namespace Kanban.Aplicacao.UseCases.Tarefa.AtualizarTarefa;

public class AtualizarTarefaUseCase : IAtualizarTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IMapper _mapper;

    public AtualizarTarefaUseCase(ITarefaRepository tarefaRepository, IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(int usuarioId, int tarefaId, AtualizarTarefaRequest request)
    {
        // Verificar se a tarefa existe e pertence ao usuário
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
        if (tarefa == null)
            throw new TarefaNaoEncontradaException();

        if (tarefa.UsuarioId != usuarioId)
            throw new AcessoNegadoException("Você não tem permissão para atualizar esta tarefa");

        // Atualizar os dados
        tarefa.Titulo = request.Titulo;
        tarefa.Descricao = request.Descricao;
        tarefa.Status = request.Status;
        tarefa.DataVencimento = request.DataVencimento;
        tarefa.DataAtualizacao = DateTime.UtcNow;

        var tarefaAtualizada = await _tarefaRepository.AtualizarAsync(tarefa);
        return _mapper.Map<TarefaResponse>(tarefaAtualizada);
    }
}