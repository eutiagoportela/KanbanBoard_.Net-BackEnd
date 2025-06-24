using Kanban.Aplicacao.AutoMapper;

// USE CASES DE TAREFA
using Kanban.Aplicacao.UseCases.Tarefa.CriarTarefa;
using Kanban.Aplicacao.UseCases.Tarefa.ListarTarefas;
using Kanban.Aplicacao.UseCases.Tarefa.AtualizarTarefa;
using Kanban.Aplicacao.UseCases.Tarefa.ExcluirTarefa;
using Kanban.Aplicacao.UseCases.Tarefa.MoverTarefa;

// USE CASES DE USUÁRIO
using Kanban.Aplicacao.UseCases.Usuario.CriarUsuario;
using Kanban.Aplicacao.UseCases.Login.DoLogin;

using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Infraestrutura;
using Kanban.Infraestrutura.Repositorios.Implementacoes;
using Kanban.Infraestrutura.Security;

namespace Kanban.Api.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registra o banco de dados PostgreSQL
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgreSqlConfiguration(configuration);
        return services;
    }

    /// <summary>
    /// Registra o AutoMapper
    /// </summary>
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        // Validação do AutoMapper (recomendado em desenvolvimento)
        var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>());
        mapperConfig.AssertConfigurationIsValid();

        return services;
    }

    /// <summary>
    /// Registra todos os repositórios
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITarefaRepository, TarefaRepository>();

        return services;
    }

    /// <summary>
    /// Registra serviços de segurança e infraestrutura
    /// </summary>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        // Registrar serviços de segurança
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Cache para otimização
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Registra todos os Use Cases de Usuário
    /// </summary>
    public static IServiceCollection AddUsuarioUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
        // Adicione outros use cases de usuário conforme necessário

        return services;
    }

    /// <summary>
    /// Registra todos os Use Cases de Tarefa
    /// </summary>
    public static IServiceCollection AddTarefaUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICriarTarefaUseCase, CriarTarefaUseCase>();
        services.AddScoped<IListarTarefasUseCase, ListarTarefasUseCase>();
        services.AddScoped<IAtualizarTarefaUseCase, AtualizarTarefaUseCase>();
        services.AddScoped<IExcluirTarefaUseCase, ExcluirTarefaUseCase>();
        services.AddScoped<IMoverTarefaUseCase, MoverTarefaUseCase>();

        return services;
    }

    /// <summary>
    /// Registra Use Cases de Autenticação
    /// </summary>
    public static IServiceCollection AddAuthUseCases(this IServiceCollection services)
    {
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();

        return services;
    }

    /// <summary>
    /// Registra todos os Use Cases
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddUsuarioUseCases();
        services.AddTarefaUseCases();
        services.AddAuthUseCases();

        return services;
    }

    /// <summary>
    /// Configura CORS para desenvolvimento e produção
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            // DESENVOLVIMENTO
            options.AddPolicy("Development", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            // PRODUÇÃO
            options.AddPolicy("Production", builder =>
            {
                builder.WithOrigins(
                    // Local development
                    "http://localhost:3000",    // React
                    "http://localhost:5173",    // Vite (React/Vue)
                    "http://localhost:8080",    // Vue CLI
                    "http://localhost:4200",    // Angular

                    // Production hosting
                    "https://kanban-app.vercel.app",
                    "https://kanban-board.netlify.app"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Registra todas as dependências da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // 1. PERSISTÊNCIA
        services.AddDatabase(configuration);
        services.AddRepositories();

        // 2. MAPEAMENTO
        services.AddAutoMapperConfiguration();

        // 3. SEGURANÇA
        services.AddSecurityServices();
        services.AddJwtAuthentication(configuration, environment);

        // 4. REGRAS DE NEGÓCIO (USE CASES)
        services.AddUseCases();

        // 5. CORS
        services.AddCorsConfiguration();

        return services;
    }

    /// <summary>
    /// Validar se todas as dependências estão registradas
    /// </summary>
    public static void ValidateDependencies(this IServiceProvider serviceProvider)
    {
        try
        {
            // TESTAR REPOSITÓRIOS
            serviceProvider.GetRequiredService<IUsuarioRepository>();
            serviceProvider.GetRequiredService<ITarefaRepository>();

            // TESTAR SEGURANÇA
            serviceProvider.GetRequiredService<IPasswordHasher>();
            serviceProvider.GetRequiredService<IJwtTokenGenerator>();

            // TESTAR USE CASES ESSENCIAIS
            serviceProvider.GetRequiredService<ICriarUsuarioUseCase>();
            serviceProvider.GetRequiredService<IDoLoginUseCase>();
            serviceProvider.GetRequiredService<ICriarTarefaUseCase>();
            serviceProvider.GetRequiredService<IListarTarefasUseCase>();

            Console.WriteLine("✅ Todas as dependências estão registradas corretamente!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro na validação de dependências: {ex.Message}");
            throw;
        }
    }

}