namespace WaitAndChillReborn.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using System.Linq;

    [HarmonyPatch(typeof(Intercom), nameof(Intercom.Start))]
    public static class IntercomStartPatch
    {
        private static void Postfix(Intercom __instance)
        {
            if (!string.IsNullOrEmpty(WaitAndChillReborn.Singleton.Config.Translations.Intercom))
            {
                __instance.Network_state = Intercom.State.Custom;
            }
        }
    }

    [HarmonyPatch(typeof(Intercom), nameof(Intercom.Update))]
    public static class IntercomUpdatePatch
    {
        private static bool Prefix(Intercom __instance)
        {
            if (Handler.IsLobby && !string.IsNullOrEmpty(WaitAndChillReborn.Singleton.Config.Translations.Intercom))
            {
                string intercomText = WaitAndChillReborn.Singleton.Config.Translations.Intercom;
                intercomText = intercomText.Replace("{servername}", Server.Name).Replace("{playercount}", Player.List.Count().ToString()).Replace("{maxplayers}", CustomNetworkManager.slots.ToString());

                __instance.CustomContent = intercomText;
                __instance.Network_intercomText = __instance.Network_intercomText;
                __instance.UpdateText();

                return false;
            }
            else
                return true;
        }
    }
}
