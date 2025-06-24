using System.ComponentModel.DataAnnotations;
using Kanban.Dominio.Enum;

namespace Kanban.Dominio.Entidades;

public class Tarefas
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descricao { get; set; }

    [Required]
    public StatusTarefa Status { get; set; } = StatusTarefa.AFazer;

    public DateTime? DataVencimento { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public int Ordem { get; set; } = 0;

    // Relacionamentos
    public Usuarios Usuario { get; set; } = null!;
}