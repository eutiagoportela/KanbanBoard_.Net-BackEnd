using Microsoft.AspNetCore.Mvc;
using Kanban.Aplicacao.UseCases.Login.DoLogin;
using Kanban.Comunicacao.DTOs;
using Kanban.Comunicacao.Requests.Login;
using Kanban.Comunicacao.Responses.Login;
using Kanban.Exceptions;

namespace Kanban.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IDoLoginUseCase _doLoginUseCase;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IDoLoginUseCase doLoginUseCase,
        ILogger<AuthController> logger)
    {
        _doLoginUseCase = doLoginUseCase;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

                _logger.LogWarning("Dados inválidos no login: {Erros}", string.Join(", ", erros));
                var respostaValidacao = RespostaPadrao<LoginResponse>.ComErros(erros);
                return BadRequest(respostaValidacao);
            }

            _logger.LogInformation("Tentativa de login para email: {Email}", request.Email);

            var loginResponse = await _doLoginUseCase.ExecuteAsync(request);

            _logger.LogInformation("Login realizado com sucesso para usuário: {UsuarioId} - {Email}",
                loginResponse.Usuario.Id, loginResponse.Usuario.Email);

            var resposta = RespostaPadrao<LoginResponse>.ComSucesso(loginResponse, "Login realizado com sucesso");
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Tentativa de login com credenciais inválidas para email: {Email}", request.Email);
            var resposta = RespostaPadrao<LoginResponse>.ComErro(ex.Message);
            return Unauthorized(resposta);
        }
        catch (ValidacaoException ex)
        {
            _logger.LogWarning("Tentativa de login com credenciais inválidas para email: {Email}", request.Email);
            var resposta = RespostaPadrao<LoginResponse>.ComErro(ex.Message);
            return Unauthorized(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos no login: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<LoginResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados durante login");
            var resposta = RespostaPadrao<LoginResponse>.ComErro("Erro no sistema. Tente novamente mais tarde.");
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante login para email: {Email}", request.Email);
            var resposta = RespostaPadrao<LoginResponse>.ComErro("Erro interno do servidor. Tente novamente mais tarde.");
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }
}