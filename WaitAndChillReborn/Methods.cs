namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using MEC;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using MapGeneration.Distributors;

    using Random = UnityEngine.Random;

    public partial class Handler
    {
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

                text += Translation.TopMessage;

                text += $"\n{Translation.BottomMessage}";

                short networkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                switch (networkTimer)
                {
                    case -2: text = text.Replace("{seconds}", Translation.ServerIsPaused); break;

                    case -1: text = text.Replace("{seconds}", Translation.RoundIsBeingStarted); break;

                    case 1: text = text.Replace("{seconds}", $"{networkTimer} {Translation.OneSecondRemain}"); break;

                    case 0: text = text.Replace("{seconds}", Translation.RoundIsBeingStarted); break;

                    default: text = text.Replace("{seconds}", $"{networkTimer} {Translation.XSecondsRemains}"); break;
                }

                if (Player.List.Count() == 1)
                {
                    text = text.Replace("{players}", $"{Player.List.Count()} {Translation.OnePlayerConnected}");
                }
                else
                {
                    text = text.Replace("{players}", $"{Player.List.Count()} {Translation.XPlayersConnected}");
                }

                if (Config.HintVertPos != 0 && Config.HintVertPos > 0)
                {
                    for (int i = 0; i < Config.HintVertPos; i++)
                    {
                        text += "\n";
                    }
                }

                foreach (Player player in Player.List)
                {
                    if (Config.UseHints)
                    {
                        player.ShowHint(text.ToString(), 1.1f);
                    }
                    else
                    {
                        player.Broadcast(1, text.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(Translation.Intercom))
                {
                    string intercomText = Translation.Intercom;
                    intercomText = intercomText.Replace("{servername}", Server.Name).Replace("{playercount}", Player.List.Count().ToString()).Replace("{maxplayers}", CustomNetworkManager.slots.ToString());

                    switch (networkTimer)
                    {
                        case -2: intercomText = intercomText.Replace("{seconds}", Translation.ServerIsPaused); break;

                        case -1: intercomText = intercomText.Replace("{seconds}", Translation.RoundIsBeingStarted); break;

                        case 1: intercomText = intercomText.Replace("{seconds}", $"{networkTimer} {Translation.OneSecondRemain}"); break;

                        case 0: intercomText = intercomText.Replace("{seconds}", Translation.RoundIsBeingStarted); break;

                        default: intercomText = intercomText.Replace("{seconds}", $"{networkTimer} {Translation.XSecondsRemains}"); break;
                    }

                    Intercom.host.CustomContent = intercomText;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal void SpawnManager()
        {
            possibleSpawnPoses.Clear();

            for (int i = 0; i < Config.LobbyRoom.Count; i++)
            {
                Config.LobbyRoom[i] = Config.LobbyRoom[i].ToUpper();
            }

            if (Config.LobbyRoom.Contains("TOWER1")) possibleSpawnPoses.Add(new Vector3(54.81f, 1019.41f, -44.906f));
            if (Config.LobbyRoom.Contains("TOWER2")) possibleSpawnPoses.Add(new Vector3(148.6951f, 1019.447f, -19.06371f));
            if (Config.LobbyRoom.Contains("TOWER3")) possibleSpawnPoses.Add(new Vector3(223.1443f, 1026.775f, -18.15129f));
            if (Config.LobbyRoom.Contains("TOWER4")) possibleSpawnPoses.Add(new Vector3(-21.81f, 1019.89f, -43.45f));
            if (Config.LobbyRoom.Contains("NUKE_SURFACE")) possibleSpawnPoses.Add(new Vector3(40.68f, 988.86f, -36.2f));

            if (Config.LobbyRoom.Contains("WC"))
            {
                foreach (Transform transform in ItemSpawnpoint.RandomInstances.First(x => x.name == "Random Keycard")._positionVariants)
                {
                    possibleSpawnPoses.Add(transform.position + Vector3.up);
                }
            }

            if (Config.LobbyRoom.Contains("GR18"))
            {
                possibleSpawnPoses.Add(ItemSpawnpoint.RandomInstances.First(x => x.name == "COM-15" && x.TriggerDoorName == "GR18")._positionVariants.First().position + Vector3.up);
            }

            Dictionary<RoomType, string> RoomToString = new Dictionary<RoomType, string>()
            {
                { RoomType.EzShelter, "SHELTER" },
                { RoomType.EzGateA, "GATE_A" },
                { RoomType.EzGateB, "GATE_B" },
            };

            foreach (Room room in Map.Rooms)
            {
                if (RoomToString.ContainsKey(room.Type) && Config.LobbyRoom.Contains(RoomToString[room.Type]))
                {
                    var roomPos = room.transform.position;
                    possibleSpawnPoses.Add(new Vector3(roomPos.x, roomPos.y + 2f, roomPos.z));
                }
            }

            if (Config.LobbyRoom.Contains("INTERCOM"))
            {
                possibleSpawnPoses.Add(Intercom.host._area.position);
            }

            if (Config.LobbyRoom.Contains("079"))
            {
                Vector3 secondDoorPos = Map.GetDoorByName("079_SECOND").Base.transform.position;
                possibleSpawnPoses.Add(Vector3.MoveTowards(RoleType.Scp079.GetRandomSpawnProperties().Item1, secondDoorPos, 7f));

                Scp079sDoors(true);
            }

            if (Config.LobbyRoom.Contains("096"))
            {
                possibleSpawnPoses.Add(ItemSpawnpoint.AutospawnInstances.First(x => x.AutospawnItem == ItemType.KeycardNTFLieutenant).transform.position + Vector3.up);
            }

            Dictionary<string, RoleType> stringToRole = new Dictionary<string, RoleType>()
            {
                { "049", RoleType.Scp049 },
                { "106", RoleType.Scp106 },
                { "173", RoleType.Scp173 },
                { "939", RoleType.Scp93953 },
            };

            foreach (var role in stringToRole)
            {
                if (Config.LobbyRoom.Contains(role.Key))
                {
                    possibleSpawnPoses.Add(role.Value.GetRandomSpawnProperties().Item1);
                }
            }

            ChoosedSpawnPos = possibleSpawnPoses[Random.Range(0, possibleSpawnPoses.Count)];
        }

        internal void Scp079sDoors(bool state)
        {
            Vector3 secondDoorPos = Map.GetDoorByName("079_SECOND").Base.transform.position;

            foreach (Door controlRoomDoor in Map.Doors.Where(d => Vector3.Distance(d.Base.transform.position, secondDoorPos) < 5f))
            {
                controlRoomDoor.IsOpen = state;
            }
        }
    }
}
