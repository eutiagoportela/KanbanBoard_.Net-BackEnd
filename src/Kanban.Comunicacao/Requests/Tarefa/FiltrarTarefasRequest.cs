using Kanban.Dominio.Enum;

namespace Kanban.Comunicacao.Requests.Tarefa;

public class FiltrarTarefasRequest
{
    public string? TermoBusca { get; set; }
    public StatusTarefa? Status { get; set; }
    public DateTime? DataVencimentoInicio { get; set; }
    public DateTime? DataVencimentoFim { get; set; }
}