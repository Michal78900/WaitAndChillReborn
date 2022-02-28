namespace WaitAndChillReborn.API.Features
{
    using Exiled.API.Features;
    using MapEditorReborn.API.Features;
    using MapEditorReborn.API.Features.Components.ObjectComponents;
    using Mirror;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Configs;

    public class Arena
    {
        public static Vector3 NextArenaSpawnPosition = new Vector3(1000f, 1000f, 0f);

        public static List<Arena> List = new List<Arena>();

        public static Arena Create(SchematicObjectComponent schematic) => new Arena(schematic);

        public static Arena GetEmptyArena()
        {
            Arena arena = List.FirstOrDefault(x => x.IsAvailable);
            return arena ?? Create(ObjectSpawner.SpawnSchematic(GetRandomArenaName(), NextArenaSpawnPosition));
        }

        private Arena(SchematicObjectComponent schematic)
        {
            List<Vector3> ntfList = new List<Vector3>();
            List<Vector3> ciList = new List<Vector3>();

            foreach (GameObject block in schematic.AttachedBlocks.ToList())
            {
                if (block.name.Contains(NtfSpawnPointName))
                {
                    ntfList.Add(block.transform.position);
                    schematic.AttachedBlocks.Remove(block);
                    NetworkServer.Destroy(block);
                }
                else if (block.name.Contains(CiSpawnPointName))
                {
                    ciList.Add(block.transform.position);
                    schematic.AttachedBlocks.Remove(block);
                    NetworkServer.Destroy(block);
                }
            }

            if (ntfList.Count == 0 || ciList.Count == 0)
            {
                Log.Error($"One or more of the spawnpoints in \"{schematic.Name}\" arena are missing.");
                return;
            }

            Schematic = schematic;
            NtfSpawnPoints = ntfList.AsReadOnly();
            CiSpawnPoints = ciList.AsReadOnly();
            NextArenaSpawnPosition += Vector3.back * Config.DistanceBetweenArenas;

            IsAvailable = true;
            List.Add(this);
        }

        public SchematicObjectComponent Schematic { get; }

        public IReadOnlyList<Vector3> NtfSpawnPoints { get; }

        public IReadOnlyList<Vector3> CiSpawnPoints { get; }

        public bool IsAvailable { get; set; }

        private static string GetRandomArenaName() => Config.ArenaNames[Random.Range(0, Config.ArenaNames.Count)];

        private const string NtfSpawnPointName = "NTF_SPAWN";
        private const string CiSpawnPointName = "CI_SPAWN";

        private static readonly ArenaConfig Config = WaitAndChillReborn.Singleton.Config.ArenaConfig;
    }
}
