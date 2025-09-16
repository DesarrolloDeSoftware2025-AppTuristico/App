using Volo.Abp.Settings;

namespace TurisTrack.Settings;

public class TurisTrackSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TurisTrackSettings.MySetting1));
    }
}
