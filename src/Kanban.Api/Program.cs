using Microsoft.OpenApi.Models;
using Kanban.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DOS SERVIÇOS =====

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kanban Board API",
        Version = "v1",
        Description = "API para gerenciamento de quadro Kanban com autenticação JWT",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@kanban.com"
        }
    });

    // Configuração para JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Registrar todas as dependências da aplicação
builder.Services.AddApplicationDependencies(builder.Configuration, builder.Environment);

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddHealthChecks();

var app = builder.Build();

// ===== CONFIGURAÇÃO DO PIPELINE =====

// ===== CONFIGURAR PIPELINE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseCors("Production");
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapHealthChecks("/api/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            service = "KanbanApi",
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});


// Redirecionamento HTTPS (comentado para desenvolvimento)
// app.UseHttpsRedirection();

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapear controllers
app.MapControllers();

// ===== VALIDAÇÃO E INICIALIZAÇÃO =====
try
{
    // ✅ Corrigir: criar escopo antes de validar dependências
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.ValidateDependencies();

    // Executar migrações automaticamente em desenvolvimento
    if (app.Environment.IsDevelopment())
    {
        var context = scope.ServiceProvider.GetRequiredService<Kanban.Infraestrutura.KanbanDbContext>();

        // Aplicar migrações pendentes (se quiser)
        // context.Database.Migrate();

        Console.WriteLine("🏠 Aplicação iniciada com sucesso!");
        Console.WriteLine($"🌐 Swagger UI disponível em: {(app.Environment.IsDevelopment() ? "http://localhost:5000" : "https://sua-api.com")}");
        Console.WriteLine("🔑 Credenciais de teste:");
        Console.WriteLine("   Email: admin@kanban.com | Senha: 123456");
        Console.WriteLine("   Email: joao@kanban.com  | Senha: 123456");
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro fatal na inicialização da aplicação: {ex.Message}");
    Console.WriteLine($"💡 Stack trace: {ex.StackTrace}");
    throw;
}

