using System;
using Exiled.API.Enums;
using Exiled.API.Features;

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
        public override Version Version => new Version(1, 0, 0);

        private WaitAndChillReborn() { }

        private Handler handler;

        public override void OnEnabled()
        {
            base.OnEnabled();

            handler = new Handler();


            ServerEvent.WaitingForPlayers += handler.OnWatingForPlayers;

            PlayerEvent.Joined += handler.OnPlayerJoin;
            PlayerEvent.Hurting += handler.OnHurting;

            ServerEvent.RoundStarted += handler.OnRoundStarted;

            






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
    }
}
