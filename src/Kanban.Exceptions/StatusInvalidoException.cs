namespace Kanban.Exceptions;

public class StatusInvalidoException : Exception
{
    public StatusInvalidoException() : base("Status da tarefa inválido")
    {
    }

    public StatusInvalidoException(string message) : base(message)
    {
    }

    public StatusInvalidoException(string message, Exception innerException) : base(message, innerException)
    {
    }
}