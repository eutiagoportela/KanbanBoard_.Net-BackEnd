using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kanban.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "DataAtualizacao", "DataCriacao", "Email", "Nome", "SenhaHash" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), "admin@kanban.com", "Admin", "HASH_ADMIN" },
                    { 2, new DateTime(2024, 1, 2, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 2, 9, 0, 0, 0, DateTimeKind.Utc), "joao@kanban.com", "João", "HASH_JOAO" }
                });

            migrationBuilder.InsertData(
                table: "Tarefas",
                columns: new[] { "Id", "DataAtualizacao", "DataCriacao", "DataVencimento", "Descricao", "Ordem", "Status", "Titulo", "UsuarioId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, "Instalar e configurar todas as ferramentas necessárias para desenvolvimento", 1, 3, "Configurar ambiente de desenvolvimento", 1 },
                    { 2, new DateTime(2024, 1, 2, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 2, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 23, 16, 45, 0, 0, DateTimeKind.Utc), "Criar sistema de autenticação com tokens JWT para segurança da API", 1, 2, "Implementar autenticação JWT", 1 },
                    { 3, new DateTime(2024, 1, 2, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 2, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 27, 16, 45, 0, 0, DateTimeKind.Utc), "Desenvolver a interface do usuário para o quadro Kanban com drag and drop", 1, 1, "Criar interface do Kanban Board", 1 },
                    { 4, new DateTime(2024, 1, 3, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 3, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 25, 16, 45, 0, 0, DateTimeKind.Utc), "Criar testes automatizados para garantir qualidade do código", 2, 1, "Escrever testes unitários", 1 },
                    { 5, new DateTime(2024, 1, 3, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 3, 16, 0, 0, 0, DateTimeKind.Utc), null, "Atualizar e melhorar a documentação técnica da aplicação", 1, 1, "Revisar documentação da API", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_DataCriacao",
                table: "Tarefas",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Status",
                table: "Tarefas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId",
                table: "Tarefas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId_DataVencimento",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "DataVencimento" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId_Status",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
