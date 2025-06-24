namespace Kanban.Comunicacao.Responses.Tarefa;

public class KanbanBoardResponse
{
    public List<TarefaResponse> AFazer { get; set; } = new();
    public List<TarefaResponse> EmProgresso { get; set; } = new();
    public List<TarefaResponse> Concluido { get; set; } = new();
    public int TotalTarefas { get; set; }
    public int TarefasVencidas { get; set; }
}