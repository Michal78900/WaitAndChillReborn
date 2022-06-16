namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal static class BulletHolePatch
    {
        private static bool Prefix(StandardHitregBase __instance, Ray ray, RaycastHit hit) => !API.API.IsLobby;
    }
}