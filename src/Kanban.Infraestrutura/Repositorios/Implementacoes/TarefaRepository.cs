using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Enum;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;

namespace Kanban.Infraestrutura.Repositorios.Implementacoes;

public class TarefaRepository : ITarefaRepository
{
    private readonly KanbanDbContext _context;
    private readonly ILogger<TarefaRepository> _logger;

    public TarefaRepository(KanbanDbContext context, ILogger<TarefaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Tarefas?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _context.Tarefas
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tarefa por ID: {TarefaId}", id);
            throw new DatabaseException($"Erro ao obter tarefa por ID: {id}", ex);
        }
    }

    public async Task<Tarefas> CriarAsync(Tarefas tarefa)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        try
        {
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Tarefas.Add(tarefa);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return tarefa;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao criar tarefa: {TarefaTitulo}", tarefa.Titulo);
                    throw new DatabaseException("Erro ao criar tarefa no banco de dados.", ex);
                }
            });
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao criar tarefa: {TarefaTitulo}", tarefa.Titulo);
            throw new DatabaseException("Erro não esperado ao criar tarefa.", ex);
        }
    }

    public async Task<Tarefas> AtualizarAsync(Tarefas tarefa)
    {
        try
        {
            _context.Tarefas.Update(tarefa);
            await _context.SaveChangesAsync();
            return tarefa;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar tarefa: {TarefaId}", tarefa.Id);
            throw new DatabaseException("Erro ao atualizar tarefa no banco de dados.", ex);
        }
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        try
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null) return false;

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir tarefa: {TarefaId}", id);
            throw new DatabaseException("Erro ao excluir tarefa do banco de dados.", ex);
        }
    }

    public async Task<List<Tarefas>> ListarPorUsuarioAsync(int usuarioId)
    {
        try
        {
            return await _context.Tarefas
                .Where(t => t.UsuarioId == usuarioId)
                .OrderBy(t => t.Ordem)
                .ThenBy(t => t.DataCriacao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tarefas do usuário: {UsuarioId}", usuarioId);
            throw new DatabaseException("Erro ao listar tarefas do banco de dados.", ex);
        }
    }

    public async Task<List<Tarefas>> ListarPorStatusAsync(int usuarioId, StatusTarefa status)
    {
        try
        {
            return await _context.Tarefas
                .Where(t => t.UsuarioId == usuarioId && t.Status == status)
                .OrderBy(t => t.Ordem)
                .ThenBy(t => t.DataCriacao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tarefas por status para usuário: {UsuarioId}", usuarioId);
            throw new DatabaseException("Erro ao listar tarefas por status do banco de dados.", ex);
        }
    }

    public async Task<List<Tarefas>> ListarComFiltroAsync(int usuarioId, string? termoBusca)
    {
        try
        {
            var query = _context.Tarefas.Where(t => t.UsuarioId == usuarioId);

            if (!string.IsNullOrWhiteSpace(termoBusca))
            {
                query = query.Where(t =>
                    t.Titulo.Contains(termoBusca) ||
                    (t.Descricao != null && t.Descricao.Contains(termoBusca)));
            }

            return await query
                .OrderBy(t => t.Status)
                .ThenBy(t => t.Ordem)
                .ThenBy(t => t.DataCriacao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tarefas com filtro para usuário: {UsuarioId}", usuarioId);
            throw new DatabaseException("Erro ao filtrar tarefas do banco de dados.", ex);
        }
    }

    public async Task<bool> TarefaExisteAsync(int id)
    {
        try
        {
            return await _context.Tarefas.AnyAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se tarefa existe: {TarefaId}", id);
            throw new DatabaseException("Erro ao verificar existência da tarefa.", ex);
        }
    }

    public async Task<bool> UsuarioPossuiTarefaAsync(int usuarioId, int tarefaId)
    {
        try
        {
            return await _context.Tarefas
                .AnyAsync(t => t.Id == tarefaId && t.UsuarioId == usuarioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar propriedade da tarefa: {TarefaId} - {UsuarioId}", tarefaId, usuarioId);
            throw new DatabaseException("Erro ao verificar propriedade da tarefa.", ex);
        }
    }
}