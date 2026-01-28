using TurisTrack.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace TurisTrack.Permissions;

public class TurisTrackPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TurisTrackPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(TurisTrackPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TurisTrackResource>(name);
    }
}
