using Kanban.Dominio.Enum;

namespace Kanban.Comunicacao.Responses.Tarefa;

public class TarefaResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusTarefa Status { get; set; }
    public string StatusDescricao => Status switch
    {
        StatusTarefa.AFazer => "A Fazer",
        StatusTarefa.EmProgresso => "Em Progresso",
        StatusTarefa.Concluido => "Concluído",
        _ => "Desconhecido"
    };
    public DateTime? DataVencimento { get; set; }
    public int UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public int Ordem { get; set; }
    public bool Vencida => DataVencimento.HasValue && DataVencimento.Value < DateTime.UtcNow && Status != StatusTarefa.Concluido;

    public object Should()
    {
        throw new NotImplementedException();
    }
}