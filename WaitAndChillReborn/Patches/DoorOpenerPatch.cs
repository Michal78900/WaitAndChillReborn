namespace WaitAndChillReborn.Patches
{
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;

    [HarmonyPatch(typeof(DoorEventOpenerExtension), nameof(DoorEventOpenerExtension.Trigger))]
    public static class DoorOpenerPatch
    {
        public static void Postfix(DoorEventOpenerExtension __instance, ref DoorEventOpenerExtension.OpenerEventType eventType)
        {
            /*
            if (eventType == DoorEventOpenerExtension.OpenerEventType.WarheadStart && Handler.spawnedDoors.Contains(__instance.gameObject))
            {
                __instance.TargetDoor.NetworkTargetState = false;
                __instance.TargetDoor.ServerChangeLock(DoorLockReason.Warhead, false);
            }
            */
        }
    }
}
