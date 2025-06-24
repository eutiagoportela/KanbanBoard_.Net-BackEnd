using Kanban.Comunicacao.Requests.Login;
using Kanban.Comunicacao.Responses.Login;

namespace Kanban.Aplicacao.UseCases.Login.DoLogin;

public interface IDoLoginUseCase
{
    Task<LoginResponse> ExecuteAsync(LoginRequest request);
}