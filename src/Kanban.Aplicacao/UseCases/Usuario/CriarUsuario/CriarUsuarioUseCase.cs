using AutoMapper;
using Microsoft.Extensions.Logging;
using Kanban.Comunicacao.Requests.Usuario;
using Kanban.Comunicacao.Responses.Usuario;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;
using Kanban.Infraestrutura.Security;

namespace Kanban.Aplicacao.UseCases.Usuario.CriarUsuario;

/// <summary>
/// Implementação do caso de uso de criação de usuário
/// </summary>
public class CriarUsuarioUseCase : ICriarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<CriarUsuarioUseCase> _logger;

    public CriarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<CriarUsuarioUseCase> logger)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de criação de usuário
    /// </summary>
    /// <param name="request">Dados do usuário a ser criado</param>
    /// <returns>Dados do usuário criado</returns>
    /// <exception cref="EmailJaExisteException">Quando o email já está cadastrado</exception>
    /// <exception cref="ValidacaoException">Quando os dados são inválidos</exception>
    public async Task<UsuarioResponse> ExecuteAsync(CriarUsuarioRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de usuário para email: {Email}", request.Email);

            // 1. VALIDAÇÕES DE NEGÓCIO
            await ValidarDadosUsuario(request);

            // 2. VERIFICAR SE EMAIL JÁ EXISTE
            await ValidarEmailUnico(request.Email);

            // 3. CRIAR ENTIDADE USUÁRIO COM SENHA HASHEADA
            var usuario = await CriarNovoUsuario(request);

            // 4. SALVAR NO BANCO DE DADOS
            await _usuarioRepository.CriarAsync(usuario);

            _logger.LogInformation("Usuário criado com sucesso: {UsuarioId} - {Email}", usuario.Id, usuario.Email);

            // 5. MAPEAR PARA RESPONSE (SEM SENHA)
            var response = _mapper.Map<UsuarioResponse>(usuario);

            return response;
        }
        catch (EmailJaExisteException)
        {
            _logger.LogWarning("Tentativa de cadastro com email já existente: {Email}", request.Email);
            throw;
        }
        catch (ValidacaoException)
        {
            _logger.LogWarning("Dados inválidos na criação de usuário: {Email}", request.Email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar usuário: {Email}", request.Email);
            throw new DatabaseException("Erro interno ao criar usuário", ex);
        }
    }

    /// <summary>
    /// Valida os dados básicos do usuário
    /// </summary>
    private static async Task ValidarDadosUsuario(CriarUsuarioRequest request)
    {
        var erros = new List<string>();

        // Validação do email
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            erros.Add("Email é obrigatório");
        }
        else if (!IsEmailValido(request.Email))
        {
            erros.Add("Email deve ter um formato válido");
        }

        // Validação da senha
        if (string.IsNullOrWhiteSpace(request.Senha))
        {
            erros.Add("Senha é obrigatória");
        }
        else if (request.Senha.Length < 6)
        {
            erros.Add("Senha deve ter pelo menos 6 caracteres");
        }

        // Validação do nome
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            erros.Add("Nome é obrigatório");
        }
        else if (request.Nome.Length < 2)
        {
            erros.Add("Nome deve ter pelo menos 2 caracteres");
        }

        if (erros.Any())
        {
            throw new ValidacaoException(erros);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se o email já está cadastrado
    /// </summary>
    private async Task ValidarEmailUnico(string email)
    {
        var emailExiste = await _usuarioRepository.EmailExisteAsync(email);

        if (emailExiste)
        {
            throw new EmailJaExisteException($"O email '{email}' já está cadastrado no sistema");
        }
    }

    /// <summary>
    /// Cria nova entidade usuário com senha hasheada
    /// </summary>
    private async Task<Usuarios> CriarNovoUsuario(CriarUsuarioRequest request)
    {
        // Hash da senha
        var senhaHash = _passwordHasher.HashPassword(request.Senha);

        // Criar entidade
        var usuario = new Usuarios
        {
            Nome = request.Nome.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            SenhaHash = senhaHash,  // ✅ CORRIGIDO: era Senha, agora SenhaHash
            DataCriacao = DateTime.UtcNow
            // ✅ REMOVIDO: Ativo = true (campo não existe)
        };

        _logger.LogDebug("Usuário criado em memória: {Email}", usuario.Email);

        return await Task.FromResult(usuario);
    }

    /// <summary>
    /// Valida formato do email usando regex simples
    /// </summary>
    private static bool IsEmailValido(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}