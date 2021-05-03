namespace WaitAndChillReborn
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Loader;
    using System.Reflection;
    using HarmonyLib;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using Scp106Event = Exiled.Events.Handlers.Scp106;

    public class WaitAndChillReborn : Plugin<Config>
    {
        public static WaitAndChillReborn Singleton;

        public override string Author => "Michal78900";
        public override Version Version => new Version(2, 4, 1);
        public override Version RequiredExiledVersion => new Version(2, 10, 0);

        private Handler handler;
        private Harmony harmony;

        public static Assembly subclassAssembly;

        public override void OnEnabled()
        {
            Singleton = this;
            handler = new Handler();

            harmony = new Harmony($"wacr-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            ServerEvent.WaitingForPlayers += handler.OnWaitingForPlayers;

            MapEvent.PlacingBlood += handler.OnPlacingBlood;

            PlayerEvent.Verified += handler.OnVerified;
            PlayerEvent.Hurting += handler.OnHurting;
            PlayerEvent.IntercomSpeaking += handler.OnIntercom;
            PlayerEvent.PickingUpItem += handler.OnItemPickup;
            PlayerEvent.InteractingDoor += handler.OnDoor;
            PlayerEvent.InteractingElevator += handler.OnElevator;

            Scp106Event.CreatingPortal += handler.OnCreatingPortal;
            Scp106Event.Teleporting += handler.OnTeleporting;

            ServerEvent.RoundStarted += handler.OnRoundStarted;


            Log.Debug($"Checking for Subclassing...", Config.Debug);
            try
            {
                subclassAssembly = Loader.Plugins.FirstOrDefault(pl => pl.Name == "Subclass").Assembly;

                Log.Debug("Advanced Subclassing plugin detected!", Config.Debug);
            }
            catch (Exception)
            {
                Log.Debug($"Subclass plugin is not installed", Config.Debug);
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= handler.OnWaitingForPlayers;

            MapEvent.PlacingBlood -= handler.OnPlacingBlood;

            PlayerEvent.Verified -= handler.OnVerified;
            PlayerEvent.Hurting -= handler.OnHurting;
            PlayerEvent.IntercomSpeaking -= handler.OnIntercom;
            PlayerEvent.PickingUpItem -= handler.OnItemPickup;
            PlayerEvent.InteractingDoor -= handler.OnDoor;
            PlayerEvent.InteractingElevator -= handler.OnElevator;

            Scp106Event.CreatingPortal -= handler.OnCreatingPortal;
            Scp106Event.Teleporting -= handler.OnTeleporting;

            ServerEvent.RoundStarted -= handler.OnRoundStarted;

            handler = null;
            Singleton = null;

            base.OnDisabled();
        }
    }
}
