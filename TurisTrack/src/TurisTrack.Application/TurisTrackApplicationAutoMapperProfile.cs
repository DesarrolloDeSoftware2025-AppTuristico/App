using AutoMapper;
using TurisTrack.DestinosTuristicos;

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
    }
}
