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
            if (hub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Overwatch)
            {
                __result = false;
                return false;
            }

            switch (hub.characterClassManager.InstanceMode)
            {
                case ClientInstanceMode.ReadyClient:
                case ClientInstanceMode.Host:
                    __result = true;
                    break;

                default:
                    __result = false;
                    break;
            }

            return false;
        }
    }
}