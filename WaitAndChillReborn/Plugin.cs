namespace WaitAndChillReborn
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using Exiled.Loader;
    using System.Linq;
    using Handlers;

    public class WaitAndChillReborn : Plugin<Config, Translation>
    {
        public static WaitAndChillReborn Singleton;

        private Harmony harmony;

        public override void OnEnabled()
        {
            Singleton = this;

            if (Loader.Plugins.FirstOrDefault(x => x.Name == "MapEditorReborn") != null)
                API.API.MapEditorRebornInstalled = true;

            if (Config.ArenaMode)
            {
                ArenaEventHandler.RegisterEvents();
            }
            else if (API.API.MapEditorRebornInstalled)
            {
                LobbyEventHandler.RegisterEvents();
            }

            harmony = new Harmony($"michal78900.wacr-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            LobbyEventHandler.UnRegisterEvents();
            ArenaEventHandler.UnRegisterEvents();

            Singleton = null;

            base.OnDisabled();
        }

        public override string Name => "WaitAndChillReborn";
        public override string Author => "Michal78900";
        public override Version Version => new Version(5, 0, 0);
        public override Version RequiredExiledVersion => new Version(5, 0, 0);
    }
}
