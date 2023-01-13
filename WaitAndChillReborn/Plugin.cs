namespace WaitAndChillReborn
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using Configs;
    using Config = global::WaitAndChillReborn.Configs.Config;

    public class WaitAndChillReborn : Plugin<Config, Translation>
    {
        public static WaitAndChillReborn Singleton;

        private Harmony _harmony;

        public override void OnEnabled()
        {
            Singleton = this;
            
            EventHandlers.RegisterEvents();
            
            _harmony = new Harmony($"michal78900.wacr-{DateTime.Now.Ticks}");
            _harmony.PatchAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            EventHandlers.UnRegisterEvents();

            Singleton = null;

            base.OnDisabled();
        }

        public override string Name => "WaitAndChillReborn";
        public override string Author => "Michal78900";
        public override Version Version => new Version(5, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);
    }
}
