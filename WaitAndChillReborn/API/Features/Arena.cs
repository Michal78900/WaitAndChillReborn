namespace WaitAndChillReborn.API.Features
{
    using Exiled.API.Features;
    using MapEditorReborn.API.Features;
    using MapEditorReborn.API.Features.Components.ObjectComponents;
    using Mirror;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class Arena
    {
        public static Vector3 NextArenaSpawnPosition = new Vector3(1000f, 1000f, 0f);

        public static List<Arena> List = new List<Arena>();

        public static Arena Create(SchematicObjectComponent schematic) => new Arena(schematic);

        public static Arena GetEmptyArena()
        {
            Arena arena = List.FirstOrDefault(x => x.IsAvailable);
            return arena ?? Create(ObjectSpawner.SpawnSchematic("ExampleArena", NextArenaSpawnPosition));
        }

        private Arena(SchematicObjectComponent schematic)
        {
            GameObject _ntfSpawnPointObject = schematic.AttachedBlocks.FirstOrDefault(x => x.name == "NTF_SPAWN");
            GameObject _ciSpawnPointOjbect = schematic.AttachedBlocks.FirstOrDefault(x => x.name == "CI_SPAWN");

            if (_ntfSpawnPointObject == null || _ciSpawnPointOjbect == null)
            {
                Log.Error($"One or more of the spawnpoints in \"{schematic.Name}\" arena are missing.");
                return;
            }

            Schematic = schematic;
            NtfSpawnPoint = _ntfSpawnPointObject.transform.position;
            CiSpawnPoint = _ciSpawnPointOjbect.transform.position;

            schematic.AttachedBlocks.Remove(_ntfSpawnPointObject);
            schematic.AttachedBlocks.Remove(_ciSpawnPointOjbect);
            NetworkServer.Destroy(_ntfSpawnPointObject);
            NetworkServer.Destroy(_ciSpawnPointOjbect);

            NextArenaSpawnPosition += Vector3.back * 250f;

            IsAvailable = true;
            List.Add(this);
        }

        public SchematicObjectComponent Schematic { get; }

        public Vector3 NtfSpawnPoint { get; }

        public Vector3 CiSpawnPoint { get; }

        public bool IsAvailable { get; set; }
    }
}
