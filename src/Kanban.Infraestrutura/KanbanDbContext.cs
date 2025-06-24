using Microsoft.EntityFrameworkCore;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Enum;

namespace Kanban.Infraestrutura;

public class KanbanDbContext : DbContext
{
    public KanbanDbContext(DbContextOptions<KanbanDbContext> options) : base(options)
    {
    }

    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Tarefas> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // *** CONFIGURAÇÃO PARA CONVERTER DATETIME PARA UTC ***
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v,
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }

        // Configurações da entidade Usuarios
        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).IsRequired();
            entity.Property(u => u.DataCriacao).IsRequired();
            entity.Property(u => u.DataAtualizacao).IsRequired();
        });

        // Configurações da entidade Tarefas
        modelBuilder.Entity<Tarefas>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Descricao).HasMaxLength(1000);
            entity.Property(t => t.Status).IsRequired();
            entity.Property(t => t.UsuarioId).IsRequired();
            entity.Property(t => t.DataCriacao).IsRequired();
            entity.Property(t => t.DataAtualizacao).IsRequired();
            entity.Property(t => t.Ordem).HasDefaultValue(0);

            // Conversão do enum para int
            entity.Property(t => t.Status).HasConversion<int>();

            // Relacionamento com Usuario
            entity.HasOne(t => t.Usuario)
                  .WithMany(u => u.Tarefas)
                  .HasForeignKey(t => t.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Índices para performance
            entity.HasIndex(t => t.UsuarioId);
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.DataCriacao);
            entity.HasIndex(t => new { t.UsuarioId, t.Status });
            entity.HasIndex(t => new { t.UsuarioId, t.DataVencimento });
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // ENTIDADE USUARIOS
        modelBuilder.Entity<Usuarios>().HasData(
            new Usuarios
            {
                Id = 1,
                Nome = "Admin",
                Email = "admin@kanban.com",
                SenhaHash = "HASH_ADMIN", // coloque uma hash segura ou temporária
                DataCriacao = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new Usuarios
            {
                Id = 2,
                Nome = "João",
                Email = "joao@kanban.com",
                SenhaHash = "HASH_JOAO", // coloque uma hash segura ou temporária
                DataCriacao = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc)
            }
        );

        // Tarefas iniciais
        modelBuilder.Entity<Tarefas>().HasData(
            new Tarefas
            {
                Id = 1,
                Titulo = "Configurar ambiente de desenvolvimento",
                Descricao = "Instalar e configurar todas as ferramentas necessárias para desenvolvimento",
                Status = StatusTarefa.Concluido,
                UsuarioId = 1,
                DataCriacao = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                Ordem = 1
            },
            new Tarefas
            {
                Id = 2,
                Titulo = "Implementar autenticação JWT",
                Descricao = "Criar sistema de autenticação com tokens JWT para segurança da API",
                Status = StatusTarefa.EmProgresso,
                UsuarioId = 1,
                DataVencimento = new DateTime(2025, 6, 23, 16, 45, 0, DateTimeKind.Utc),
                DataCriacao = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc),
                Ordem = 1
            },
            new Tarefas
            {
                Id = 3,
                Titulo = "Criar interface do Kanban Board",
                Descricao = "Desenvolver a interface do usuário para o quadro Kanban com drag and drop",
                Status = StatusTarefa.AFazer,
                UsuarioId = 1,
                DataVencimento = new DateTime(2025, 6, 27, 16, 45, 0, DateTimeKind.Utc),
                DataCriacao = new DateTime(2024, 1, 2, 14, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 2, 14, 0, 0, DateTimeKind.Utc),
                Ordem = 1
            },
            new Tarefas
            {
                Id = 4,
                Titulo = "Escrever testes unitários",
                Descricao = "Criar testes automatizados para garantir qualidade do código",
                Status = StatusTarefa.AFazer,
                UsuarioId = 1,
                DataVencimento = new DateTime(2025, 6, 25, 16, 45, 0, DateTimeKind.Utc),
                DataCriacao = new DateTime(2024, 1, 3, 11, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 3, 11, 0, 0, DateTimeKind.Utc),
                Ordem = 2
            },
            new Tarefas
            {
                Id = 5,
                Titulo = "Revisar documentação da API",
                Descricao = "Atualizar e melhorar a documentação técnica da aplicação",
                Status = StatusTarefa.AFazer,
                UsuarioId = 2,
                DataCriacao = new DateTime(2024, 1, 3, 16, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 3, 16, 0, 0, DateTimeKind.Utc),
                Ordem = 1
            }
        );
    }
}