using System.Net;
using System.Text.Json;
using Kanban.Comunicacao.DTOs;
using Kanban.Exceptions;

namespace Kanban.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado capturado pelo middleware global");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            // Exceções de negócio - 400 Bad Request
            ValidacaoException validationEx => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = RespostaPadrao<object>.ComErros(validationEx.Erros)
            },

            // Exceções de entidade não encontrada - 404 Not Found
            TarefaNaoEncontradaException or UsuarioNaoEncontradoException => new
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Response = RespostaPadrao<object>.ComErro(exception.Message)
            },

            // Exceções de acesso negado - 403 Forbidden
            AcessoNegadoException => new
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Response = RespostaPadrao<object>.ComErro(exception.Message)
            },

            // Exceções de email já existe - 409 Conflict
            EmailJaExisteException => new
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Response = RespostaPadrao<object>.ComErro(exception.Message)
            },

            // Exceções de status inválido - 400 Bad Request
            StatusInvalidoException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = RespostaPadrao<object>.ComErro(exception.Message)
            },

            // Exceções de banco de dados - 500 Internal Server Error
            DatabaseException dbEx => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Response = RespostaPadrao<object>.ComErro($"Erro no banco de dados: {dbEx.GetFriendlyMessage()}")
            },

            // Exceções de autorização - 401 Unauthorized
            UnauthorizedAccessException => new
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Response = RespostaPadrao<object>.ComErro("Acesso não autorizado")
            },

            // Exceções de argumento - 400 Bad Request
            ArgumentException or ArgumentNullException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = RespostaPadrao<object>.ComErro("Dados inválidos: " + exception.Message)
            },

            // Exceção genérica - 500 Internal Server Error
            _ => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Response = RespostaPadrao<object>.ComErro("Erro interno do servidor")
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response.Response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}