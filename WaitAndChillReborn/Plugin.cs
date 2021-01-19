using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using System.Reflection;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;

namespace WaitAndChillReborn
{
    public class WaitAndChillReborn : Plugin<Config>
    {
        private static readonly Lazy<WaitAndChillReborn> LazyInstance = new Lazy<WaitAndChillReborn>(() => new WaitAndChillReborn());
        public static WaitAndChillReborn Instance => LazyInstance.Value;

        public override PluginPriority Priority => PluginPriority.Medium;

        public override string Author => "Michal78900";
        public override Version Version => new Version(1, 2, 0);

        private WaitAndChillReborn() { }

        private Handler handler;

        public static bool ThereIsSubClass;

        public override void OnEnabled()
        {
            base.OnEnabled();

            handler = new Handler();

            ServerEvent.WaitingForPlayers += handler.OnWatingForPlayers;

            PlayerEvent.Joined += handler.OnPlayerJoin;
            PlayerEvent.Hurting += handler.OnHurting;

            ServerEvent.RoundStarted += handler.OnRoundStarted;

            if (IsSubClass())
            {
                ThereIsSubClass = true;
                Log.Debug("Advanced Subclassing plugin detected!");
            }
            else ThereIsSubClass = false;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            ServerEvent.WaitingForPlayers -= handler.OnWatingForPlayers;

            PlayerEvent.Joined -= handler.OnPlayerJoin;
            PlayerEvent.Hurting -= handler.OnHurting;

            ServerEvent.RoundStarted -= handler.OnRoundStarted;

            handler = null;
        }

        private static bool IsSubClass()
        {
            Assembly assembly = Loader.Plugins.FirstOrDefault(pl => pl.Name == "Subclass")?.Assembly;
            if (assembly == null) return false;
            else return true;
        }
    }
}
