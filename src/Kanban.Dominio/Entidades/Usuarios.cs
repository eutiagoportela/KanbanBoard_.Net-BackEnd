using System.ComponentModel.DataAnnotations;

namespace Kanban.Dominio.Entidades;

public class Usuarios
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<Tarefas> Tarefas { get; set; } = new List<Tarefas>();
}