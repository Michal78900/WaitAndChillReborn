namespace WaitAndChillReborn
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using Scp106Event = Exiled.Events.Handlers.Scp106;

    public class WaitAndChillReborn : Plugin<Config, Translation>
    {
        public static WaitAndChillReborn Singleton;

        private Handler handler;
        private Harmony harmony;

        public override void OnEnabled()
        {
            Singleton = this;

            harmony = new Harmony($"michal78900.wacr-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            handler = new Handler();

            ServerEvent.WaitingForPlayers += handler.OnWaitingForPlayers;

            PlayerEvent.Verified += handler.OnVerified;
            PlayerEvent.Dying += handler.OnDying;
            PlayerEvent.Died += handler.OnDied;

            MapEvent.PlacingBlood += handler.OnPlacingBlood;
            PlayerEvent.Hurting += handler.OnHurting;
            PlayerEvent.SpawningRagdoll += handler.OnSpawningRagdoll;
            PlayerEvent.IntercomSpeaking += handler.OnIntercom;
            PlayerEvent.DroppingItem += handler.OnDroppingItem;
            PlayerEvent.InteractingDoor += handler.OnInteractingDoor;
            PlayerEvent.InteractingElevator += handler.OnInteractingElevator;
            PlayerEvent.InteractingLocker += handler.OnInteractingLocker;

            Scp106Event.CreatingPortal += handler.OnCreatingPortal;
            Scp106Event.Teleporting += handler.OnTeleporting;

            ServerEvent.RoundStarted += handler.OnRoundStarted;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= handler.OnWaitingForPlayers;

            PlayerEvent.Verified -= handler.OnVerified;
            PlayerEvent.Dying -= handler.OnDying;
            PlayerEvent.Died -= handler.OnDied;

            MapEvent.PlacingBlood -= handler.OnPlacingBlood;
            PlayerEvent.Hurting -= handler.OnHurting;
            PlayerEvent.SpawningRagdoll -= handler.OnSpawningRagdoll;
            PlayerEvent.IntercomSpeaking -= handler.OnIntercom;
            PlayerEvent.DroppingItem -= handler.OnDroppingItem;
            PlayerEvent.InteractingDoor -= handler.OnInteractingDoor;
            PlayerEvent.InteractingElevator -= handler.OnInteractingElevator;
            PlayerEvent.InteractingLocker -= handler.OnInteractingLocker;

            Scp106Event.CreatingPortal -= handler.OnCreatingPortal;
            Scp106Event.Teleporting -= handler.OnTeleporting;

            ServerEvent.RoundStarted -= handler.OnRoundStarted;

            handler = null;
            Singleton = null;

            base.OnDisabled();
        }

        public override string Name => "WaitAndChillReborn";
        public override string Author => "Michal78900";
        public override Version Version => new Version(3, 0, 1);
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
    }
}
