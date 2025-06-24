using AutoMapper;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Comunicacao.Responses.Usuario;
using Kanban.Dominio.Entidades;

namespace Kanban.Aplicacao.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Usuario
        CreateMap<Usuarios, UsuarioResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.DataCriacao));

        // Tarefa
        CreateMap<Tarefas, TarefaResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DataVencimento, opt => opt.MapFrom(src => src.DataVencimento))
            .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.DataCriacao))
            .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(src => src.DataAtualizacao))
            .ForMember(dest => dest.Ordem, opt => opt.MapFrom(src => src.Ordem));
    }
}