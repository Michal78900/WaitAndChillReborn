namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    [HarmonyPatch(typeof(TimedGrenadePickup), nameof(TimedGrenadePickup.OnExplosionDetected))]
    internal static class GrenadeExplosionDetectedPatch
    {
        private static bool Prefix(TimedGrenadePickup __instance, Footprinting.Footprint attacker, Vector3 source, float range) => !Handler.IsLobby;
    }
}
