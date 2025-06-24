namespace Kanban.Comunicacao.DTOs;

public class RespostaPadrao<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }
    public List<string>? Erros { get; set; }

    public static RespostaPadrao<T> ComSucesso(T dados, string? mensagem = null)
    {
        return new RespostaPadrao<T>
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = dados
        };
    }

    public static RespostaPadrao<T> ComErro(string mensagem)
    {
        return new RespostaPadrao<T>
        {
            Sucesso = false,
            Mensagem = mensagem
        };
    }

    public static RespostaPadrao<T> ComErros(List<string> erros)
    {
        return new RespostaPadrao<T>
        {
            Sucesso = false,
            Mensagem = "Erro de validação",
            Erros = erros
        };
    }
}