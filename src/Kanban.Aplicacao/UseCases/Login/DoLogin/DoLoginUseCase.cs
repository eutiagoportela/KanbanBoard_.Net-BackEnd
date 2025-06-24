using AutoMapper;
using Microsoft.Extensions.Configuration;
using Kanban.Comunicacao.Requests.Login;
using Kanban.Comunicacao.Responses.Login;
using Kanban.Comunicacao.Responses.Usuario;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;
using Kanban.Infraestrutura.Security;

namespace Kanban.Aplicacao.UseCases.Login.DoLogin;

public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public DoLoginUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
    {
        // Buscar usuário pelo email
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException("Email ou senha inválidos");

        // Verificar senha
        var senhaValida = _passwordHasher.VerifyPassword(request.Senha, usuario.SenhaHash);
        if (!senhaValida)
            throw new ValidacaoException("Email ou senha inválidos");

        // Gerar token JWT
        var token = _jwtTokenGenerator.GenerateToken(usuario);

        // Obter tempo de expiração do token
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes", 60);
        var expiracaoToken = DateTime.UtcNow.AddMinutes(expirationMinutes);

        // Mapear usuário para response
        var usuarioResponse = _mapper.Map<UsuarioResponse>(usuario);

        return new LoginResponse
        {
            Token = token,
            ExpiracaoToken = expiracaoToken,
            Usuario = usuarioResponse
        };
    }
}