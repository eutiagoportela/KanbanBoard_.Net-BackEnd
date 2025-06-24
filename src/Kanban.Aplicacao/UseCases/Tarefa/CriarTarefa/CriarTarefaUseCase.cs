using AutoMapper;
using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;

namespace Kanban.Aplicacao.UseCases.Tarefa.CriarTarefa;

public class CriarTarefaUseCase : ICriarTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public CriarTarefaUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(int usuarioId, CriarTarefaRequest request)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // Criar a tarefa
        var tarefa = new Tarefas
        {
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            Status = request.Status,
            DataVencimento = request.DataVencimento,
            UsuarioId = usuarioId,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow,
            Ordem = 0 // Será ajustado se necessário
        };

        var tarefaCriada = await _tarefaRepository.CriarAsync(tarefa);
        return _mapper.Map<TarefaResponse>(tarefaCriada);
    }
}