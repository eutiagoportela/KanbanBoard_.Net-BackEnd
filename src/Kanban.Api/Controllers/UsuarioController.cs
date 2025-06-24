using Microsoft.AspNetCore.Mvc;
using Kanban.Aplicacao.UseCases.Usuario.CriarUsuario;
using Kanban.Comunicacao.DTOs;
using Kanban.Comunicacao.Requests.Usuario;
using Kanban.Comunicacao.Responses.Usuario;
using Kanban.Exceptions;

namespace Kanban.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(
        ICriarUsuarioUseCase criarUsuarioUseCase,
        ILogger<UsuarioController> logger)
    {
        _criarUsuarioUseCase = criarUsuarioUseCase;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioRequest request)
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

                _logger.LogWarning("Dados inválidos ao criar usuário: {Erros}", string.Join(", ", erros));
                var respostaValidacao = RespostaPadrao<UsuarioResponse>.ComErros(erros);
                return BadRequest(respostaValidacao);
            }

            _logger.LogInformation("Criando novo usuário: {Email}", request.Email);

            var usuario = await _criarUsuarioUseCase.ExecuteAsync(request);

            _logger.LogInformation("Usuário criado com sucesso: {UsuarioId} - {Email}", usuario.Id, usuario.Email);
            var resposta = RespostaPadrao<UsuarioResponse>.ComSucesso(usuario, "Usuário criado com sucesso");
            return CreatedAtAction(nameof(CriarUsuario), new { id = usuario.Id }, resposta);
        }
        catch (EmailJaExisteException ex)
        {
            _logger.LogWarning("Tentativa de criar usuário com email já existente: {Email}", request.Email);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return Conflict(resposta);
        }
        catch (ValidacaoException ex)
        {
            _logger.LogWarning("Dados inválidos ao criar usuário: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErros(ex.Erros);
            return BadRequest(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Argumentos inválidos ao criar usuário: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao criar usuário");
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro("Erro no banco de dados: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar usuário");
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }
}