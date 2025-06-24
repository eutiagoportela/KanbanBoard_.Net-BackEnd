using AutoMapper;
using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Enum;
using Kanban.Dominio.Repositorios.Interfaces;

namespace Kanban.Aplicacao.UseCases.Tarefa.ListarTarefas;

public class ListarTarefasUseCase : IListarTarefasUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IMapper _mapper;

    public ListarTarefasUseCase(ITarefaRepository tarefaRepository, IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _mapper = mapper;
    }

    public async Task<List<TarefaResponse>> ExecuteAsync(int usuarioId, FiltrarTarefasRequest? filtro = null)
    {
        List<Tarefas> tarefas;

        if (filtro?.Status.HasValue == true)
        {
            tarefas = await _tarefaRepository.ListarPorStatusAsync(usuarioId, filtro.Status.Value);
        }
        else if (!string.IsNullOrWhiteSpace(filtro?.TermoBusca))
        {
            tarefas = await _tarefaRepository.ListarComFiltroAsync(usuarioId, filtro.TermoBusca);
        }
        else
        {
            tarefas = await _tarefaRepository.ListarPorUsuarioAsync(usuarioId);
        }

        return _mapper.Map<List<TarefaResponse>>(tarefas);
    }

    public async Task<KanbanBoardResponse> ExecuteKanbanAsync(int usuarioId)
    {
        var todasTarefas = await _tarefaRepository.ListarPorUsuarioAsync(usuarioId);
        var tarefasResponse = _mapper.Map<List<TarefaResponse>>(todasTarefas);

        return new KanbanBoardResponse
        {
            AFazer = tarefasResponse.Where(t => t.Status == StatusTarefa.AFazer).ToList(),
            EmProgresso = tarefasResponse.Where(t => t.Status == StatusTarefa.EmProgresso).ToList(),
            Concluido = tarefasResponse.Where(t => t.Status == StatusTarefa.Concluido).ToList(),
            TotalTarefas = tarefasResponse.Count,
            TarefasVencidas = tarefasResponse.Count(t => t.Vencida)
        };
    }
}