using System.ComponentModel.DataAnnotations;
using Kanban.Dominio.Enum;

namespace Kanban.Comunicacao.Requests.Tarefa;

public class CriarTarefaRequest
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O status é obrigatório")]
    public StatusTarefa Status { get; set; } = StatusTarefa.AFazer;

    public DateTime? DataVencimento { get; set; }
}