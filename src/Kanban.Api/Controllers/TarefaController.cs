using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanban.Aplicacao.UseCases.Tarefa.CriarTarefa;
using Kanban.Aplicacao.UseCases.Tarefa.ListarTarefas;
using Kanban.Aplicacao.UseCases.Tarefa.AtualizarTarefa;
using Kanban.Aplicacao.UseCases.Tarefa.ExcluirTarefa;
using Kanban.Aplicacao.UseCases.Tarefa.MoverTarefa;
using Kanban.Comunicacao.DTOs;
using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Exceptions;
using System.Security.Claims;

namespace Kanban.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TarefaController : ControllerBase
{
    private readonly ICriarTarefaUseCase _criarTarefaUseCase;
    private readonly IListarTarefasUseCase _listarTarefasUseCase;
    private readonly IAtualizarTarefaUseCase _atualizarTarefaUseCase;
    private readonly IExcluirTarefaUseCase _excluirTarefaUseCase;
    private readonly IMoverTarefaUseCase _moverTarefaUseCase;
    private readonly ILogger<TarefaController> _logger;

    public TarefaController(
        ICriarTarefaUseCase criarTarefaUseCase,
        IListarTarefasUseCase listarTarefasUseCase,
        IAtualizarTarefaUseCase atualizarTarefaUseCase,
        IExcluirTarefaUseCase excluirTarefaUseCase,
        IMoverTarefaUseCase moverTarefaUseCase,
        ILogger<TarefaController> logger)
    {
        _criarTarefaUseCase = criarTarefaUseCase;
        _listarTarefasUseCase = listarTarefasUseCase;
        _atualizarTarefaUseCase = atualizarTarefaUseCase;
        _excluirTarefaUseCase = excluirTarefaUseCase;
        _moverTarefaUseCase = moverTarefaUseCase;
        _logger = logger;
    }

    private int ObterUsuarioId()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Token inválido ou usuário não identificado");
        }
        return userId;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarTarefa([FromBody] CriarTarefaRequest request)
    {
        try
        {
            // Validação automática com Data Annotations
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Dados inválidos ao criar tarefa: {Erros}", string.Join(", ", erros));
                var respostaValidacao = RespostaPadrao<TarefaResponse>.ComErros(erros);
                return BadRequest(respostaValidacao);
            }

            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} criando nova tarefa: {TarefaTitulo}", usuarioId, request.Titulo);

            var tarefa = await _criarTarefaUseCase.ExecuteAsync(usuarioId, request);

            _logger.LogInformation("Tarefa criada com sucesso para usuário {UsuarioId}: {TarefaId}", usuarioId, tarefa.Id);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Tarefa criada com sucesso");
            return CreatedAtAction(nameof(ObterTarefa), new { id = tarefa.Id }, resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao criar tarefa: {Message}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário não encontrado ao criar tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao criar tarefa: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao criar tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListarTarefas([FromQuery] FiltrarTarefasRequest? filtro)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} listando tarefas", usuarioId);

            var tarefas = await _listarTarefasUseCase.ExecuteAsync(usuarioId, filtro);

            _logger.LogInformation("Tarefas listadas com sucesso para usuário {UsuarioId}: {Quantidade} tarefas", usuarioId, tarefas.Count);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComSucesso(tarefas);
            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao listar tarefas: {Message}", ex.Message);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao listar tarefas");
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao listar tarefas");
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }

    [HttpGet("kanban")]
    [ProducesResponseType(typeof(RespostaPadrao<KanbanBoardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<KanbanBoardResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<KanbanBoardResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterKanbanBoard()
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} obtendo board kanban", usuarioId);

            var kanbanBoard = await _listarTarefasUseCase.ExecuteKanbanAsync(usuarioId);

            _logger.LogInformation("Board kanban obtido com sucesso para usuário {UsuarioId}", usuarioId);
            var resposta = RespostaPadrao<KanbanBoardResponse>.ComSucesso(kanbanBoard);
            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao obter board kanban: {Message}", ex.Message);
            var resposta = RespostaPadrao<KanbanBoardResponse>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao obter board kanban");
            var resposta = RespostaPadrao<KanbanBoardResponse>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao obter board kanban");
            var resposta = RespostaPadrao<KanbanBoardResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTarefa(int id)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} obtendo tarefa {TarefaId}", usuarioId, id);

            var tarefas = await _listarTarefasUseCase.ExecuteAsync(usuarioId);
            var tarefa = tarefas.FirstOrDefault(t => t.Id == id);

            if (tarefa == null)
            {
                _logger.LogWarning("Tarefa {TarefaId} não encontrada para usuário {UsuarioId}", id, usuarioId);
                var respostaNaoEncontrada = RespostaPadrao<TarefaResponse>.ComErro("Tarefa não encontrada");
                return NotFound(respostaNaoEncontrada);
            }

            _logger.LogInformation("Tarefa {TarefaId} obtida com sucesso para usuário {UsuarioId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa);
            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao obter tarefa: {Message}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao obter tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao obter tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] AtualizarTarefaRequest request)
    {
        try
        {
            // Validação automática com Data Annotations
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Dados inválidos ao atualizar tarefa: {Erros}", string.Join(", ", erros));
                var respostaValidacao = RespostaPadrao<TarefaResponse>.ComErros(erros);
                return BadRequest(respostaValidacao);
            }

            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} atualizando tarefa {TarefaId}", usuarioId, id);

            var tarefa = await _atualizarTarefaUseCase.ExecuteAsync(usuarioId, id, request);

            _logger.LogInformation("Tarefa {TarefaId} atualizada com sucesso para usuário {UsuarioId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Tarefa atualizada com sucesso");
            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao atualizar tarefa: {Message}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada para atualização", id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Acesso negado ao atualizar tarefa {TarefaId} para usuário {UsuarioId}", id, ObterUsuarioId());
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao atualizar tarefa: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao atualizar tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao atualizar tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }

    [HttpPatch("{id}/mover")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoverTarefa(int id, [FromBody] MoverTarefaRequest request)
    {
        try
        {
            // Validação automática com Data Annotations
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Dados inválidos ao mover tarefa: {Erros}", string.Join(", ", erros));
                var respostaValidacao = RespostaPadrao<TarefaResponse>.ComErros(erros);
                return BadRequest(respostaValidacao);
            }

            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} movendo tarefa {TarefaId} para status {NovoStatus}", usuarioId, id, request.NovoStatus);

            var tarefa = await _moverTarefaUseCase.ExecuteAsync(usuarioId, id, request);

            _logger.LogInformation("Tarefa {TarefaId} movida com sucesso para usuário {UsuarioId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Tarefa movida com sucesso");
            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao mover tarefa: {Message}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada para mover", id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Acesso negado ao mover tarefa {TarefaId} para usuário {UsuarioId}", id, ObterUsuarioId());
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return Forbid();
        }
        catch (StatusInvalidoException ex)
        {
            _logger.LogWarning("Status inválido ao mover tarefa: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao mover tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao mover tarefa");
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExcluirTarefa(int id)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            _logger.LogInformation("Usuário {UsuarioId} excluindo tarefa {TarefaId}", usuarioId, id);

            var sucesso = await _excluirTarefaUseCase.ExecuteAsync(usuarioId, id);

            if (!sucesso)
            {
                _logger.LogWarning("Falha ao excluir tarefa {TarefaId} para usuário {UsuarioId}", id, usuarioId);
                var respostaFalha = RespostaPadrao<object>.ComErro("Falha ao excluir a tarefa");
                return BadRequest(respostaFalha);
            }

            _logger.LogInformation("Tarefa {TarefaId} excluída com sucesso para usuário {UsuarioId}", id, usuarioId);
            var resposta = RespostaPadrao<object>.ComSucesso(null, "Tarefa excluída com sucesso");
            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado ao excluir tarefa: {Message}", ex.Message);
            var resposta = RespostaPadrao<object>.ComErro("Acesso não autorizado");
            return Unauthorized(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada para exclusão", id);
            var resposta = RespostaPadrao<object>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Acesso negado ao excluir tarefa {TarefaId} para usuário {UsuarioId}", id, ObterUsuarioId());
            var resposta = RespostaPadrao<object>.ComErro(ex.Message);
            return Forbid();
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao excluir tarefa");
            var resposta = RespostaPadrao<object>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao excluir tarefa");
            var resposta = RespostaPadrao<object>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }
}