using TurisTrack.Localization;
using Volo.Abp.Application.Services;

namespace TurisTrack;

/* Inherit your application services from this class.
 */
public abstract class TurisTrackAppService : ApplicationService
{
    protected TurisTrackAppService()
    {
        LocalizationResource = typeof(TurisTrackResource);
    }
}
