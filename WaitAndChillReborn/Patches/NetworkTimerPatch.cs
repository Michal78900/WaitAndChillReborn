namespace WaitAndChillReborn.Patches
{
    using GameCore;
    using HarmonyLib;

    [HarmonyPatch(typeof(RoundStart), nameof(RoundStart.NetworkTimer), MethodType.Setter)]
    internal static class NetworkTimerPatch
    {
        private static bool Prefix(RoundStart __instance, ref short value)
        {
            if (!API.API.IsLobby)
                return true;

            __instance.Timer = value;
            return false;
        }
    }
}
