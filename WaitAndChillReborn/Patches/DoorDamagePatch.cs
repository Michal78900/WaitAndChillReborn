namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
    internal static class DoorDamagePatch
    {
        private static bool Prefix(BreakableDoor __instance, float hp, DoorDamageType type) =>
            !(API.API.IsLobby && type != DoorDamageType.ServerCommand);
    }
}
