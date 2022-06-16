namespace WaitAndChillReborn.Patches
{
    using GameCore;
    using HarmonyLib;

    // [HarmonyPatch(typeof(RoundStart), nameof(RoundStart.NetworkTimer), MethodType.Setter)]
    internal static class NetworkTimerPatch
    {
        private static void Postfix(RoundStart __instance, ref short value)
        {
            if (value == 1)
            {
                foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
                {
                    player.Role.Type = RoleType.Spectator;
                }
            }
        }
    }
}
