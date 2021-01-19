using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace WaitAndChillReborn
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Determines if any kind of message at all will be displayed")]
        public bool DisplayWaitMessage { get; set; } = true;

        [Description("List of lobbys (rooms) where players can spawn: (TOWER(1-3), SHELTER, 173)")]
        public List<string> LobbyRoom { get; set; } = new List<string>
        {
            "TOWER1",
            "TOWER2",
            "TOWER3",
            "SHELTER",
            "173"
        };

        [Description("List of roles that players can spawn:")]
        public List<RoleType> RolesToChoose { get; set; } = new List<RoleType>
        {
            RoleType.Tutorial,
        };

        [Description("Allow dealing damage to other players, while in lobby:")]
        public bool AlowDamage { get; set; } = false;

        [Description("Disallow players triggering SCP-096 and stopping from moving SCP-173, while in lobby:")]
        public bool TurnedPlayers { get; set; } = true;

        [Description("Give players an effect of SCP-207, while in lobby: (set 0 to disable)")]
        public byte ColaMultiplier { get; set; } = 4;

        [Description("Use hints instead of broadcasts for text stuff:")]
        public bool UseHints { get; set; } = true;

        [Description("Determines the position of the Hint on the users screen (32 = Top, 0 = Middle, -15 = Below)")]
        public int HintVertPos { get; set; } = 25;

        [Description("Text traslations:")]
        public string TopMessage { get; set; } = "<size=40><color=yellow><b>The game will be starting soon, %seconds</b></color></size>";
        public string BottomMessage { get; set; } = "<size=30><i>%players</i></size>";
        public string ServerIsPaused { get; set; } = "Server is paused";
        public string RoundIsBeingStarted { get; set; } = "Round is being started";
        public string OneSecondRemain { get; set; } = "second remain";
        public string XSecondsRemains { get; set; } = "seconds remains";
        public string OnePlayerConnected { get; set; } = "player has connected";
        public string XPlayersConnected { get; set; } = "players have connected";
    }
}
