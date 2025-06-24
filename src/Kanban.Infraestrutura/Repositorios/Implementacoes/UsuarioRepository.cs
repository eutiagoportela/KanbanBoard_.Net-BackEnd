using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;
using Kanban.Infraestrutura;

namespace Kanban.Infraestrutura.Repositorios.Implementacoes;

/// <summary>
/// Implementação do repositório de usuários
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly KanbanDbContext _context;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(KanbanDbContext context, ILogger<UsuarioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Usuarios?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _context.Usuarios
                .Where(u => u.Id == id)  
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {UsuarioId}", id);
            throw new DatabaseException($"Erro ao buscar usuário com ID {id}", ex);
        }
    }

    public async Task<Usuarios?> ObterPorEmailAsync(string email)
    {
        try
        {
            var emailLower = email.ToLowerInvariant();
            return await _context.Usuarios
                .Where(u => u.Email == emailLower)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
            throw new DatabaseException($"Erro ao buscar usuário com email {email}", ex);
        }
    }

    public async Task<Usuarios> CriarAsync(Usuarios usuario)
    {
        try
        {
            usuario.Email = usuario.Email.ToLowerInvariant();
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message?.Contains("IX_Usuarios_Email") == true)
            {
                throw new EmailJaExisteException($"O email '{usuario.Email}' já está cadastrado no sistema");
            }
            throw new DatabaseException($"Erro ao criar usuário {usuario.Email}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar usuário: {Email}", usuario.Email);
            throw new DatabaseException($"Erro inesperado ao criar usuário {usuario.Email}", ex);
        }
    }

    public async Task<Usuarios> AtualizarAsync(Usuarios usuario)
    {
        try
        {
            usuario.Email = usuario.Email.ToLowerInvariant();
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: {UsuarioId}", usuario.Id);
            throw new DatabaseException($"Erro ao atualizar usuário {usuario.Id}", ex);
        }
    }

    public async Task<bool> RemoverAsync(int id)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)  
                .FirstOrDefaultAsync();

            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover usuário: {UsuarioId}", id);
            throw new DatabaseException($"Erro ao remover usuário {id}", ex);
        }
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        try
        {
            var emailLower = email.ToLowerInvariant();
            return await _context.Usuarios.AnyAsync(u => u.Email == emailLower);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se email existe: {Email}", email);
            throw new DatabaseException($"Erro ao verificar email {email}", ex);
        }
    }

    public async Task<IEnumerable<Usuarios>> ListarTodosAsync()
    {
        try
        {
            return await _context.Usuarios
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar usuários");
            throw new DatabaseException("Erro ao listar usuários", ex);
        }
    }
}