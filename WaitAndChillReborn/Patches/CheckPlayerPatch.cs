namespace WaitAndChillReborn.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.RoleAssign;
    using PluginAPI.Core;

    // Credits to Jesus-QC
    [HarmonyPatch(typeof(RoleAssigner), nameof(RoleAssigner.CheckPlayer))]
    public static class CheckPlayerPatch
    {
        private static bool Prefix(ReferenceHub hub, ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}