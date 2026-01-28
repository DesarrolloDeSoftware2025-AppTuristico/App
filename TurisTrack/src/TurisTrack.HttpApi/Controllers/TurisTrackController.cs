using TurisTrack.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace TurisTrack.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TurisTrackController : AbpControllerBase
{
    protected TurisTrackController()
    {
        LocalizationResource = typeof(TurisTrackResource);
    }
}
