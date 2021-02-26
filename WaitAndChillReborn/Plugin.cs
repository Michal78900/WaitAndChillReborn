using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using System.Reflection;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;
using Scp106Event = Exiled.Events.Handlers.Scp106;

namespace WaitAndChillReborn
{
    public class WaitAndChillReborn : Plugin<Config>
    {
        public static WaitAndChillReborn Singleton;


        public override string Author => "Michal78900";
        public override Version Version => new Version(2, 2, 0);
        public override Version RequiredExiledVersion => new Version(2, 3, 3);

        private Handler handler;

        public static Assembly subclassAssembly;

        public override void OnEnabled()
        {
            Singleton = this;

            handler = new Handler(this);


            ServerEvent.WaitingForPlayers += handler.OnWatingForPlayers;

            MapEvent.PlacingBlood += handler.OnPlacingBlood;

            PlayerEvent.Verified += handler.OnPlayerJoin;
            PlayerEvent.Hurting += handler.OnHurting;
            PlayerEvent.PickingUpItem += handler.OnItemPickup;
            PlayerEvent.InteractingDoor += handler.OnDoor;
            PlayerEvent.InteractingElevator += handler.OnElevator;

            Scp106Event.CreatingPortal += handler.OnCreatingPortal;
            Scp106Event.Teleporting += handler.OnTeleporting;

            ServerEvent.RoundStarted += handler.OnRoundStarted;


            Log.Debug($"Checking for Subclassing...", Config.ShowDebugMessages);
            try
            {
                subclassAssembly = Loader.Plugins.FirstOrDefault(pl => pl.Name == "Subclass").Assembly;

                Log.Debug("Advanced Subclassing plugin detected!", Config.ShowDebugMessages);
            }
            catch (Exception)
            {
                Log.Debug($"Subclass plugin is not installed", Config.ShowDebugMessages);
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= handler.OnWatingForPlayers;

            MapEvent.PlacingBlood -= handler.OnPlacingBlood;

            PlayerEvent.Verified -= handler.OnPlayerJoin;
            PlayerEvent.Hurting -= handler.OnHurting;
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
