using Microsoft.Extensions.Localization;
using TurisTrack.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace TurisTrack;

[Dependency(ReplaceServices = true)]
public class TurisTrackBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TurisTrackResource> _localizer;

    public TurisTrackBrandingProvider(IStringLocalizer<TurisTrackResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
