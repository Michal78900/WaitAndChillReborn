namespace WaitAndChillReborn.Configs
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class ArenaConfig 
    {
        [Description("The list of available arenas:")]
        public List<string> ArenaNames { get; private set; } = new List<string>()
        {
            "ExampleArena",
        };

        [Description("The distance between each arena spawned.")]
        public float DistanceBetweenArenas { get; private set; } = 250f;

        [Description("How often (in seconds) is Arena clock updated.")]
        public float ClockUpdateTime { get; private set; } = 1f;

        [Description("The time of respawning player that won in a certain arena.")]
        public float RespawnTime { get; private set; } = 5f;
    }
}
