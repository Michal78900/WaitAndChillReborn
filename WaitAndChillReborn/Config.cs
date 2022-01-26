namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class Config : IConfig
    {
        [Description("Is the plugin enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Determines if any kind of message at all will be displayed.")]
        public bool DisplayWaitMessage { get; private set; } = true;

        [Description("List of lobbys (rooms) where players can spawn: (list of all possible rooms can be found on plugin's GitHub)")]
        public List<string> LobbyRoom { get; private set; } = new List<string>()
        {
            "TOWER1",
            "TOWER2",
            "TOWER3",
            "TOWER4",
            "GATE_A",
            "GATE_B",
            "SHELTER",
            "GR18",
            "049",
            "079",
            "106",
            "173",
            "939",
        };

        [Description("Instead of choosing one lobby room, should plugin use all of lobby rooms on the list? Player when they join will be teleported to the random lobby room.")]
        public bool MultipleRooms { get; private set; } = false;

        [Description("The time (in seconds) between player joining on the server and him changing role while in lobby (change this number if some players aren't spawned / are spawned as a None class.")]
        public float SpawnDelay { get; private set; } = 0.25f;

        [Description("List of roles which players can spawn as:")]
        public List<RoleType> RolesToChoose { get; private set; } = new List<RoleType>()
        {
            RoleType.Tutorial,
        };

        [Description("List of items given to a player while in lobby: (supports CustomItems)")]
        public List<string> Inventory { get; private set; } = new List<string>()
        {
            "Coin",
        };

        [Description("List of ammo given to a player while in lobby:")]
        public Dictionary<AmmoType, ushort> Ammo { get; private set; } = new Dictionary<AmmoType, ushort>()
        {
            { AmmoType.Nato556, 0 },
            { AmmoType.Nato762, 0 },
            { AmmoType.Nato9, 0 },
            { AmmoType.Ammo12Gauge, 0 },
            { AmmoType.Ammo44Cal, 0 },
        };

        [Description("Allow dealing damage to other players, while in lobby.")]
        public bool AllowDamage { get; private set; } = true;

        [Description("Allow friendly fire, while in lobby.")]
        public bool AllowFriendlyFire { get; private set; } = true;

        [Description("Allow using Intercom by players, while in lobby.")]
        public bool AllowIntercom { get; private set; } = true;

        [Description("Disallow players triggering SCP-096 and stopping from moving SCP-173, while in lobby.")]
        public bool TurnedPlayers { get; private set; } = true;

        [Description("Effects that will be enabled, while in lobby. The number if the effect intensity.")]
        public Dictionary<EffectType, byte> LobbyEffects { get; private set; } = new Dictionary<EffectType, byte>()
        {
            { EffectType.MovementBoost, 50 },
        };

        [Description("Use hints instead of broadcasts for text stuff. (broadcasts are not recommended)")]
        public bool UseHints { get; private set; } = true;

        [Description("Determines the position of the Hint on the users screen (32 = Top, 0 = Middle, -15 = Below)")]
        public int HintVertPos { get; private set; } = 25;
    }
}
