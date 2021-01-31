using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using System.Reflection;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using Scp106Event = Exiled.Events.Handlers.Scp106;

namespace WaitAndChillReborn
{
    public class WaitAndChillReborn : Plugin<Config>
    {
        public static WaitAndChillReborn Singleton;

        public override PluginPriority Priority => PluginPriority.Medium;

        public override string Author => "Michal78900";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(2, 1, 29);



        private Handler handler;

        public static Assembly subclassAssembly;

        public override void OnEnabled()
        {
            base.OnEnabled();

            Singleton = this;

            handler = new Handler(this);


            ServerEvent.WaitingForPlayers += handler.OnWatingForPlayers;

            PlayerEvent.Verified += handler.OnPlayerJoin;
            PlayerEvent.Hurting += handler.OnHurting;
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
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            ServerEvent.WaitingForPlayers -= handler.OnWatingForPlayers;

            PlayerEvent.Verified -= handler.OnPlayerJoin;
            PlayerEvent.Hurting -= handler.OnHurting;
            Scp106Event.CreatingPortal -= handler.OnCreatingPortal;
            Scp106Event.Teleporting -= handler.OnTeleporting;

            ServerEvent.RoundStarted -= handler.OnRoundStarted;

            handler = null;
        }
    }
}
