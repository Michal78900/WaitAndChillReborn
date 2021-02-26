using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace WaitAndChillReborn
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Should debug messages be shown in a server console?")]
        public bool ShowDebugMessages { get; set; } = false;

        [Description("Determines if any kind of message at all will be displayed")]
        public bool DisplayWaitMessage { get; set; } = true;

        [Description("List of lobbys (rooms) where players can spawn: (TOWER(1-3), SHELTER, GR18, 079, 106, 173, 939, GATE_A, GATE_B)")]
        public List<string> LobbyRoom { get; set; } = new List<string>
        {
            "TOWER1",
            "TOWER2",
            "TOWER3",
            "GATE_A",
            "GATE_B",
            "SHELTER",
            "GR18",
            "079",
            "106",
            "173",
            "939",
        };

        [Description("Instead of choosing one lobby room, should plugin use all of lobby rooms on the list? Player when they join will be teleported to the random lobby room.")]
        public bool MultipleRooms { get; set; } = false;

        [Description("List of roles that players can spawn:")]
        public List<RoleType> RolesToChoose { get; set; } = new List<RoleType>
        {
            RoleType.Tutorial,
        };

        [Description("Allow dealing damage to other players, while in lobby:")]
        public bool AllowDamage { get; set; } = false;

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
