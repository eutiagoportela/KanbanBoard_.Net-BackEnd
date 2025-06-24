using System.ComponentModel.DataAnnotations;
using Kanban.Dominio.Enum;

namespace Kanban.Comunicacao.Requests.Tarefa;

public class MoverTarefaRequest
{
    [Required(ErrorMessage = "O novo status é obrigatório")]
    public StatusTarefa NovoStatus { get; set; }

    public int? NovaOrdem { get; set; }
}