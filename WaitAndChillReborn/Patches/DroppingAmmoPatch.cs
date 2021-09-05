namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;
    using InventorySystem;

    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
    internal static class DroppingAmmoPatch
    {
        private static bool Prefix(Inventory inv, ItemType ammoType, ushort amount, bool checkMinimals) => !Handler.IsLobby;
    }
}