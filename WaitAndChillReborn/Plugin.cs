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

            API.API.MapEditorRebornInstalled = Loader.Plugins.FirstOrDefault(x => x.Name == "MapEditorReborn" && x.Config.IsEnabled) != null;

            if (Config.ArenaMode)
            {
                if (!API.API.MapEditorRebornInstalled)
                {
                    Log.Error("You are trying to use Arena mode, but MapEditorReborn isn't installed or enabled!\nEnabling Lobby mode instead...");
                    LobbyEventHandler.RegisterEvents();
                }
                else
                {
                    ArenaEventHandler.RegisterEvents();
                }
            }
            else
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
