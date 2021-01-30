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
        private static readonly Lazy<WaitAndChillReborn> LazyInstance = new Lazy<WaitAndChillReborn>(() => new WaitAndChillReborn());
        public static WaitAndChillReborn Instance => LazyInstance.Value;

        public override PluginPriority Priority => PluginPriority.Medium;

        public override string Author => "Michal78900";
        public override Version Version => new Version(1, 3, 1);

        private WaitAndChillReborn() { }

        private Handler handler;

        public static Assembly subclassAssembly;
        public static bool ThereIsSubClass = false;

        public override void OnEnabled()
        {
            base.OnEnabled();

            handler = new Handler();

            ServerEvent.WaitingForPlayers += handler.OnWatingForPlayers;

            PlayerEvent.Joined += handler.OnPlayerJoin;
            PlayerEvent.Hurting += handler.OnHurting;
            Scp106Event.CreatingPortal += handler.OnCreatingPortal;
            Scp106Event.Teleporting += handler.OnTeleporting;

            ServerEvent.RoundStarted += handler.OnRoundStarted;


            if (Loader.Plugins.FirstOrDefault(pl => pl.Name == "Subclass") == null)
            {
                ThereIsSubClass = false;
                return;
            }

            subclassAssembly = Loader.Plugins.FirstOrDefault(pl => pl.Name == "Subclass").Assembly;

            ThereIsSubClass = true;
            Log.Debug("Advanced Subclassing plugin detected!");

        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            ServerEvent.WaitingForPlayers -= handler.OnWatingForPlayers;

            PlayerEvent.Joined -= handler.OnPlayerJoin;
            PlayerEvent.Hurting -= handler.OnHurting;
            Scp106Event.CreatingPortal -= handler.OnCreatingPortal;
            Scp106Event.Teleporting -= handler.OnTeleporting;

            ServerEvent.RoundStarted -= handler.OnRoundStarted;

            handler = null;
        }
    }
}
