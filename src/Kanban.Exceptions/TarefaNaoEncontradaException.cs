namespace Kanban.Exceptions;

public class TarefaNaoEncontradaException : Exception
{
    public TarefaNaoEncontradaException() : base("Tarefa não encontrada")
    {
    }

    public TarefaNaoEncontradaException(string message) : base(message)
    {
    }

    public TarefaNaoEncontradaException(string message, Exception innerException) : base(message, innerException)
    {
    }
}