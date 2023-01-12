namespace WaitAndChillReborn.Configs
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    public sealed class Config : IConfig
    {
        [Description("Whether the plugin enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether debug messages should be shown in a server console.")]
        public bool Debug { get; set; }

        [Description("Whether the wait message should be displayed.")]
        public bool DisplayWaitMessage { get; private set; } = true;

        [Description("Whether the hints should be used instead of broadcasts for the wait message. (broadcasts aren't recommended)")]
        public bool UseHints { get; private set; } = true;

        [Description("Determines the position of the Hint on the users screen (32 = Top, 0 = Middle, -15 = Below)")]
        public int HintVertPos { get; private set; } = 25;

        public LobbyConfig LobbyConfig { get; private set; } = new LobbyConfig();
    }
}
