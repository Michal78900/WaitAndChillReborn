namespace WaitAndChillReborn
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;
    using Configs;

    public sealed class Config : IConfig
    {
        [Description("Is the plugin enabled.")]
        public bool IsEnabled { get; set; } = true;

        public bool ArenaMode { get; set; } = false;

        [Description("Determines if any kind of message at all will be displayed.")]
        public bool DisplayWaitMessage { get; private set; } = true;

        [Description("Use hints instead of broadcasts for text stuff. (broadcasts are not recommended)")]
        public bool UseHints { get; private set; } = true;

        [Description("Determines the position of the Hint on the users screen (32 = Top, 0 = Middle, -15 = Below)")]
        public int HintVertPos { get; private set; } = 25;

        public LobbyConfig LobbyConfig { get; private set; } = new LobbyConfig();

        public ArenaConfig ArenaConfig { get; private set; } = new ArenaConfig();
    }
}
