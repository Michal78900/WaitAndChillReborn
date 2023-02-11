namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.RoleAssign;

    [HarmonyPatch(typeof(RoleAssigner), nameof(RoleAssigner.CheckPlayer))]
    public static class CheckPlayerPatch
    {
        private static bool Prefix(ReferenceHub hub, ref bool __result)
        {
            if (hub == ReferenceHub.HostHub || hub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Overwatch)
                return true;
        
            __result = true;
            return false;
        }
    }
}