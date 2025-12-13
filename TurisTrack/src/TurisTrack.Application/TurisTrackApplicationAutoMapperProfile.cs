using AutoMapper;
using TurisTrack.CalificacionesDestinos;
using TurisTrack.DestinosTuristicos;
using TurisTrack.Experiencias.Dtos;
using TurisTrack.ExperienciasDeViajes;

namespace TurisTrack;

public class TurisTrackApplicationAutoMapperProfile : Profile
{
    public TurisTrackApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<DestinoTuristico, DestinoTuristicoDto>();
        CreateMap<DestinoTuristicoDto, DestinoTuristico>();
        CreateMap<CreateUpdateDestinoTuristicoDto, DestinoTuristico>();
        CreateMap<CalificacionDestino, CalificacionDestinoDto>();
        CreateMap<CalificacionDestinoDto, CalificacionDestino>();
        CreateMap<ExperienciaDeViaje, ExperienciaDeViajeDto>();
        CreateMap<ExperienciaDeViajeDto, ExperienciaDeViaje>();
    }
}
