namespace WaitAndChillReborn.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using System.Linq;

    [HarmonyPatch(typeof(Intercom), nameof(Intercom.Start))]
    public static class IntercomStartPatch
    {
        internal static bool first = true;

        public static void Postfix(Intercom __instance)
        {
            if (!string.IsNullOrEmpty(WaitAndChillReborn.Singleton.Config.Translations.Intercom))
            {
                first = true;
                __instance.Network_state = Intercom.State.Custom;
            }
        }
    }

    [HarmonyPatch(typeof(Intercom), nameof(Intercom.Update))]
    public static class IntercomUpdatePatch
    {
        private readonly static Config Config = WaitAndChillReborn.Singleton.Config;

        public static bool Prefix(Intercom __instance)
        {
            if (Handler.IsLobby && !string.IsNullOrEmpty(WaitAndChillReborn.Singleton.Config.Translations.Intercom))
            {
                string intercomText = WaitAndChillReborn.Singleton.Config.Translations.Intercom;
                intercomText = intercomText.Replace("{servername}", Server.Name).Replace("{playercount}", Player.List.Count().ToString()).Replace("{maxplayers}", CustomNetworkManager.slots.ToString());

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                switch (NetworkTimer)
                {
                    case -2: intercomText = intercomText.Replace("{seconds}", Config.Translations.ServerIsPaused); break;

                    case -1: intercomText = intercomText.Replace("{seconds}", Config.Translations.RoundIsBeingStarted); break;

                    case 1: intercomText = intercomText.Replace("{seconds}", $"{NetworkTimer} {Config.Translations.OneSecondRemain}"); break;

                    case 0: intercomText = intercomText.Replace("{seconds}", Config.Translations.RoundIsBeingStarted); break;

                    default: intercomText = intercomText.Replace("{seconds}", $"{NetworkTimer} {Config.Translations.XSecondsRemains}"); break;
                }

                __instance.CustomContent = intercomText;
                __instance.UpdateText();

                return false;
            }
            else
            {
                if (IntercomStartPatch.first)
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        __instance.CustomContent = string.Empty;
                        __instance.Network_intercomText = string.Empty;
                        __instance.UpdateText();

                        IntercomStartPatch.first = false;
                    });
                }
                return true;
            }
        }
    }
}
