namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.ServerDamageWindow))]
    internal static class BreakableWindowPatch
    {
        private static bool Prefix(BreakableWindow __instance, float damage) =>
            !API.API.IsLobby;
    }
}
