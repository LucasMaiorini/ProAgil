using System.Linq;
using AutoMapper;
using ProAgil.Domain;
using ProAgil.WebAPI.Dtos;

namespace ProAgil.WebAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Evento, EventoDto>()
            .ForMember(EventoDto => EventoDto.Palestrantes, opt =>
            {
                opt.MapFrom(EventoDomain => EventoDomain.PalestranteEventos.Select(x => x.Palestrante).ToList());
            }).ReverseMap();

            CreateMap<Palestrante, PalestranteDto>()
            .ForMember(PalestranteDto => PalestranteDto.Eventos, opt =>
            {
                opt.MapFrom(PalestranteDomain => PalestranteDomain.PalestranteEventos.Select(x => x.Evento).ToList());
            }).ReverseMap();

            CreateMap<Lote, LoteDto>().ReverseMap();
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();
        }
    }
}