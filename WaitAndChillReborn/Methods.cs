namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Mirror;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    using Object = UnityEngine.Object;

    public partial class Handler
    {
        private List<GameObject> spawnedDoors = new List<GameObject>();

        private IEnumerator<float> LobbyTimer()
        {
            while (!Round.IsStarted)
            {
                text = string.Empty;

                if (Config.HintVertPos != 0 && Config.HintVertPos < 0)
                {
                    for (int i = Config.HintVertPos; i < 0; i++)
                    {
                        text += "\n";
                    }
                }

                text += Config.Translations.TopMessage;

                text += $"\n{Config.Translations.BottomMessage}";

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                switch (NetworkTimer)
                {
                    case -2: text = text.Replace("{seconds}", Config.Translations.ServerIsPaused); break;

                    case -1: text = text.Replace("{seconds}", Config.Translations.RoundIsBeingStarted); break;

                    case 1: text = text.Replace("{seconds}", $"{NetworkTimer} {Config.Translations.OneSecondRemain}"); break;

                    case 0: text = text.Replace("{seconds}", Config.Translations.RoundIsBeingStarted); break;

                    default: text = text.Replace("{seconds}", $"{NetworkTimer} {Config.Translations.XSecondsRemains}"); break;
                }

                if (Player.List.Count() == 1)
                {
                    text = text.Replace("{players}", $"{Player.List.Count()} {Config.Translations.OnePlayerConnected}");
                }
                else
                {
                    text = text.Replace("{players}", $"{Player.List.Count()} {Config.Translations.XPlayersConnected}");
                }

                if (Config.HintVertPos != 0 && Config.HintVertPos > 0)
                {
                    for (int i = 0; i < Config.HintVertPos; i++)
                    {
                        text += "\n";
                    }
                }

                foreach (Player ply in Player.List)
                {
                    if (Config.UseHints)
                    {
                        ply.ShowHint(text.ToString(), 1f);
                    }
                    else
                    {
                        ply.Broadcast(1, text.ToString());
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal void SpawnManager()
        {
            PossibleSpawnsPos.Clear();

            if (Config.LobbyRoom.Contains("TOWER1")) PossibleSpawnsPos.Add(new Vector3(54.81f, 1019.41f, -44.906f));
            if (Config.LobbyRoom.Contains("TOWER2")) PossibleSpawnsPos.Add(new Vector3(148.6951f, 1019.447f, -19.06371f));
            if (Config.LobbyRoom.Contains("TOWER3")) PossibleSpawnsPos.Add(new Vector3(223.1443f, 1026.775f, -18.15129f));
            if (Config.LobbyRoom.Contains("TOWER4")) PossibleSpawnsPos.Add(new Vector3(-21.81f, 1019.89f, -43.45f));
            if (Config.LobbyRoom.Contains("NUKE_SURFACE")) PossibleSpawnsPos.Add(new Vector3(40.68f, 988.86f, -36.2f));

            if (Config.LobbyRoom.Contains("PARKOUR"))
            {
                PossibleSpawnsPos.Add(new Vector3(249.23f, 978.03f, -45.76f));
                PossibleSpawnsPos.Add(new Vector3(240f, 1001f, -22f));

                SpawnParkour();
            }

            if (Config.LobbyRoom.Contains("TOILET"))
            {
                PossibleSpawnsPos.Add(RandomItemSpawner.singleton.posIds.First(x => x.posID == "toilet_keycard" && x.position.position.y > 1.25f && x.position.position.y < 1.35f).position.position);
            }

            if (Config.LobbyRoom.Contains("GR18"))
            {
                PossibleSpawnsPos.Add(RandomItemSpawner.singleton.posIds.First(x => x.posID == "RandomPistol" && x.DoorTriggerName == "372").position.position);
            }

            Dictionary<RoomType, string> RoomToString = new Dictionary<RoomType, string>()
            {
                {RoomType.EzShelter, "SHELTER"},
                {RoomType.EzGateA, "GATE_A" },
                {RoomType.EzGateB, "GATE_B" },
            };

            foreach (Room room in Map.Rooms)
            {
                if (RoomToString.ContainsKey(room.Type) && Config.LobbyRoom.Contains(RoomToString[room.Type]))
                {
                    var roomPos = room.transform.position;
                    PossibleSpawnsPos.Add(new Vector3(roomPos.x, roomPos.y + 2f, roomPos.z));
                }
            }

            if (Config.LobbyRoom.Contains("INTERCOM"))
            {
                PossibleSpawnsPos.Add(Intercom.host._area.position);
            }


            if (Config.LobbyRoom.Contains("079"))
            {
                Vector3 secondDoorPos = Map.GetDoorByName("079_SECOND").transform.position;
                PossibleSpawnsPos.Add(Vector3.MoveTowards(Role.GetRandomSpawnPoint(RoleType.Scp079), secondDoorPos, 7));

                Scp079sDoors(true);
            }

            if (Config.LobbyRoom.Contains("096"))
            {
                PossibleSpawnsPos.Add(RandomItemSpawner.singleton.posIds.First(x => x.posID == "Fireman" && x.DoorTriggerName == "096").position.position);
            }

            Dictionary<string, RoleType> StringToRole = new Dictionary<string, RoleType>()
            {
                {"049", RoleType.Scp049},
                {"106", RoleType.Scp106},
                {"173", RoleType.Scp173},
                {"939", RoleType.Scp93953},
            };

            foreach (var role in StringToRole)
            {
                if (Config.LobbyRoom.Contains(role.Key))
                {
                    PossibleSpawnsPos.Add(Role.GetRandomSpawnPoint(role.Value));
                }
            }

            PossibleSpawnsPos.ShuffleList();
            ChoosedSpawnPos = PossibleSpawnsPos[0];
        }

        internal void SubClassHandler(bool enabled)
        {
            try
            {
                var extensions = WaitAndChillReborn.subclassAssembly.GetType("Subclass.API");
                if (extensions == null) return;

                if (enabled)
                {
                    extensions.GetMethod("EnableAllClasses").Invoke(null, new object[] { });
                    Log.Debug("Enabled all Subclasses", Config.Debug);
                }
                else
                {
                    extensions.GetMethod("DisableAllClasses").Invoke(null, new object[] { });
                    Log.Debug("Disabled all Subclasses", Config.Debug);
                }
            }
            catch (Exception) { }
        }

        internal void Scp079sDoors(bool state)
        {
            Vector3 secondDoorPos = Map.GetDoorByName("079_SECOND").transform.position;

            foreach (DoorVariant controlRoomDoor in Map.Doors.Where(d => Vector3.Distance(d.transform.position, secondDoorPos) < 5f))
            {
                controlRoomDoor.NetworkTargetState = state;
            }
        }

        byte doorType = 0;
        internal void SpawnDoor(Vector3 position, Vector3 rotation, bool big = false)
        {
            // Original code by 初音早猫(sanyae2439)#0001

            MapGeneration.DoorSpawnpoint prefab = null;

            switch (doorType)
            {
                case 0: prefab = Object.FindObjectsOfType<MapGeneration.DoorSpawnpoint>().First(x => x.TargetPrefab.name.Contains("LCZ")); break;
                case 1: prefab = Object.FindObjectsOfType<MapGeneration.DoorSpawnpoint>().First(x => x.TargetPrefab.name.Contains("HCZ")); break;
                case 2: prefab = Object.FindObjectsOfType<MapGeneration.DoorSpawnpoint>().First(x => x.TargetPrefab.name.Contains("EZ")); break;
            }

            doorType++;
            if (doorType > 2)
                doorType = 0;

            var door = Object.Instantiate(prefab.TargetPrefab, position, Quaternion.Euler(rotation));

            if (big)
                door.transform.localScale = new Vector3(4f, 4f, 2.5f);

            spawnedDoors.Add(door.gameObject);
            NetworkServer.Spawn(door.gameObject);
        }

        internal void SpawnParkour()
        {
            foreach (var door in spawnedDoors)
            {
                NetworkServer.Destroy(door);
            }
            spawnedDoors.Clear();

            // Forward stairs
            for (int i = 0; i < 8; i++)
            {
                SpawnDoor(new Vector3(255f + i * 5, 978f + i, -44f), new Vector3(90f, 0f, 0f));
            }

            // Left stairs
            for (int i = 0; i < 35; i++)
            {
                SpawnDoor(new Vector3(288f, 986f + i, -38f + i * 5), new Vector3(90f, 90f, 0f));
            }

            // Middle to up
            doorType = 0;
            for (int i = 0; i < 5; i++)
            {
                SpawnDoor(new Vector3(284f - i * 5, 999f + i, 20f), new Vector3(90f, 0f, 0f));
            }

            // Middle to down
            for (int i = 0; i < 7; i++)
            {
                SpawnDoor(new Vector3(259 - i * 5, 1004f - i, 20f), new Vector3(90f, 0f, 0f));
            }

            // Right stairs on a mountain
            doorType = 0;
            for (int i = 0; i < 5; i++)
            {
                SpawnDoor(new Vector3(238f, 1001f + i, -30f - i * 5), new Vector3(90f, 90f, 0f));
            }

            // Forward stairs on a mountain
            for (int i = 0; i < 8; i++)
            {
                SpawnDoor(new Vector3(244f + i * 5, 1006f + i, -52f), new Vector3(90f, 0f, 0f));
            }

            // Left stairs on a mountain
            for (int i = 0; i < 6; i++)
            {
                SpawnDoor(new Vector3(278f, 1014f + i, -46f + i * 5), new Vector3(90f, 90f, 0f));
            }

            // Backwards stairs on a mountain
            for (int i = 0; i < 9; i++)
            {
                SpawnDoor(new Vector3(274f - i * 5, 1020f + i, -20f), new Vector3(90f, 0f, 0f));
            }

            SpawnDoor(new Vector3(231f, 1029f, -20f), new Vector3(90f, 0f, 0f));

            //Tower floor
            for (int x = 0; x < 6; x++)
            {
                for (int z = 0; z < 4; z++)
                {
                    doorType = 0;
                    SpawnDoor(new Vector3(228f - x * 2f, 1029.5f, -25f + z * 3.4f), new Vector3(90f, 0f, 0f));
                }
            }

            // Tower wall
            for (int z = 0; z < 7; z++)
            {
                doorType = 2;
                SpawnDoor(new Vector3(217f, 1029f, -24 + z * 2f), new Vector3(0f, 90f, 0f));
            }

            // Tower wall
            for (int x = 0; x < 6; x++)
            {
                doorType = 2;
                SpawnDoor(new Vector3(228f - x * 2f, 1029f, -25f), new Vector3(0f, 0f, 0f));
            }

            // Tower wall
            for (int x = 0; x < 6; x++)
            {
                doorType = 2;
                SpawnDoor(new Vector3(228f - x * 2f, 1029f, -12f), new Vector3(0f, 0f, 0f));
            }

            doorType = 1;
            SpawnDoor(new Vector3(233f, 1020f, -12f), new Vector3(0f, 0f, 0f), true);
            doorType = 1;
            SpawnDoor(new Vector3(233f, 1020f, -25f), new Vector3(0f, 0f, 0f), true);

            byte[,] slArray = {
                { 0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 0, 0,1,1,1, 0,0, 1,0,0,0, 0,0 },
                { 0, 1,0,0,0, 0,0, 1,0,0,0, 0,0 },
                { 0, 0,1,1,0, 0,0, 1,0,0,0, 0,0 },
                { 0, 0,0,0,1, 0,0, 1,0,0,0, 0,0 },
                { 0, 1,1,1,0, 0,0, 1,1,1,1, 0,0 },
                { 0,0,0,0,0,0,0,0,0,0,0,0,0 } };

            // Wall with a "SL" word
            for (int y = 0; y < 7; y++)
            {
                for (int z = 0; z < 13; z++)
                {
                    if (slArray[6 - y, z] == 1)
                    {
                        doorType = 0;
                    }
                    else
                    {
                        doorType = 1;
                    }

                    SpawnDoor(new Vector3(227f, 996.5f + y * 3.25f, -54f + z * 2.5f), new Vector3(0f, 90f, 0f));
                }
            }


            byte[,] scpArray = {
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0, 0,1,1,1, 0,0,0, 0,1,1,1, 0,0,0, 1,1,1,0, 0,0,0,0 },
                { 0,0,0,0,0,0,0,0, 1,0,0,0, 0,0,0, 1,0,0,0, 0,0,0, 1,0,0,1, 0,0,0,0 },
                { 0,0,0,0,0,0,0,0, 0,1,1,0, 0,0,0, 1,0,0,0, 0,0,0, 1,1,1,0, 0,0,0,0, },
                { 0,0,0,0,0,0,0,0, 0,0,0,1, 0,0,0, 1,0,0,0, 0,0,0, 1,0,0,0, 0,0,0,0, },
                { 0,0,0,0,0,0,0,0, 1,1,1,0, 0,0,0, 0,1,1,1, 0,0,0, 1,0,0,0, 0,0,0,0, },
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 } };

            // Wall with a "SCP" word
            for (int y = 0; y < 7; y++)
            {
                for (int z = 0; z < 30; z++)
                {
                    if (scpArray[6 - y, z] == 1)
                    {
                        doorType = 0;
                    }
                    else
                    {
                        doorType = 1;
                    }

                    SpawnDoor(new Vector3(227f, 996.5f + y * 3.25f, -12f + z * 2.5f), new Vector3(0f, 90f, 0f));
                }
            }

        }
    }
}
