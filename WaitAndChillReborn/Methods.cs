namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public partial class Handler
    {
        private IEnumerator<float> LobbyTimer()
        {
            while (!Round.IsStarted)
            {
                text = string.Empty;

                if (plugin.Config.HintVertPos != 0 && plugin.Config.HintVertPos < 0)
                {
                    for (int i = plugin.Config.HintVertPos; i < 0; i++)
                    {
                        //message.Append("\n");
                        text += "\n";
                    }
                }

                //message.Append(plugin.Config.TopMessage);
                text += plugin.Config.TopMessage;

                //message.Append($"\n{plugin.Config.BottomMessage}");
                text += $"\n{plugin.Config.BottomMessage}";

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                switch (NetworkTimer)
                {
                    case -2: text = text.Replace("%seconds", plugin.Config.ServerIsPaused); break;

                    case -1: text = text.Replace("%seconds", plugin.Config.RoundIsBeingStarted); break;

                    case 1: text = text.Replace("%seconds", $"{NetworkTimer} {plugin.Config.OneSecondRemain}"); break;

                    case 0: text = text.Replace("%seconds", plugin.Config.RoundIsBeingStarted); break;

                    default: text = text.Replace("%seconds", $"{NetworkTimer} {plugin.Config.XSecondsRemains}"); break;
                }

                if (Player.List.Count() == 1)
                {
                    text.Replace("%players", $"{Player.List.Count()} {plugin.Config.OnePlayerConnected}");
                }
                else
                {
                    text.Replace("%players", $"{Player.List.Count()} {plugin.Config.XPlayersConnected}");
                }

                if (plugin.Config.HintVertPos != 0 && plugin.Config.HintVertPos > 0)
                {
                    for (int i = 0; i < plugin.Config.HintVertPos; i++)
                    {
                        //message.Append("\n");
                        text += "\n";
                    }
                }

                foreach (Player ply in Player.List)
                {
                    if (plugin.Config.UseHints)
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


        public void SubClassHandler(bool enabled)
        {
            try
            {
                var extensions = WaitAndChillReborn.subclassAssembly.GetType("Subclass.API");
                if (extensions == null) return;

                if (enabled)
                {
                    extensions.GetMethod("EnableAllClasses").Invoke(null, new object[] { });
                    Log.Debug("Enabled all Subclasses", plugin.Config.ShowDebugMessages);
                }
                else
                {
                    extensions.GetMethod("DisableAllClasses").Invoke(null, new object[] { });
                    Log.Debug("Disabled all Subclasses", plugin.Config.ShowDebugMessages);
                }
            }
            catch (Exception) { }
        }

        public void SpawnManager()
        {
            PossibleSpawnsPos.Clear();

            if (plugin.Config.LobbyRoom.Contains("TOWER1")) PossibleSpawnsPos.Add(new Vector3(54.81f, 1019.41f, -44.906f));
            if (plugin.Config.LobbyRoom.Contains("TOWER2")) PossibleSpawnsPos.Add(new Vector3(148.6951f, 1019.447f, -19.06371f));
            if (plugin.Config.LobbyRoom.Contains("TOWER3")) PossibleSpawnsPos.Add(new Vector3(223.1443f, 1026.775f, -18.15129f));
            if (plugin.Config.LobbyRoom.Contains("TOWER4")) PossibleSpawnsPos.Add(new Vector3(-21.81f, 1019.89f, -43.45f));
            if (plugin.Config.LobbyRoom.Contains("NUKE_SURFACE")) PossibleSpawnsPos.Add(new Vector3(40.68f, 988.86f, -36.2f));


            if (plugin.Config.LobbyRoom.Contains("TOILET"))
            {
                PossibleSpawnsPos.Add(RandomItemSpawner.singleton.posIds.First(x => x.posID == "toilet_keycard" && x.position.position.y > 1.25f && x.position.position.y < 1.35f).position.position);
            }

            if (plugin.Config.LobbyRoom.Contains("GR18"))
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
                if (RoomToString.ContainsKey(room.Type) && plugin.Config.LobbyRoom.Contains(RoomToString[room.Type]))
                {
                    var roomPos = room.transform.position;
                    PossibleSpawnsPos.Add(new Vector3(roomPos.x, roomPos.y + 2f, roomPos.z));
                }
            }

            if (plugin.Config.LobbyRoom.Contains("INTERCOM"))
            {
                var doorPos = Map.GetDoorByName("INTERCOM").transform.position;
                var fixedDoorPos = new Vector3(doorPos.x, doorPos.y + 1.5f, doorPos.z);

                PossibleSpawnsPos.Add(fixedDoorPos);
            }


            if (plugin.Config.LobbyRoom.Contains("079"))
            {
                Vector3 secondDoorPos = Map.GetDoorByName("079_SECOND").transform.position;
                PossibleSpawnsPos.Add(Vector3.MoveTowards(Map.GetRandomSpawnPoint(RoleType.Scp079), secondDoorPos, 7));

                Scp079sDoors(true);
            }

            if (plugin.Config.LobbyRoom.Contains("096"))
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
                if (plugin.Config.LobbyRoom.Contains(role.Key))
                {
                    PossibleSpawnsPos.Add(Map.GetRandomSpawnPoint(role.Value));
                }
            }

            PossibleSpawnsPos.ShuffleList();
            ChoosedSpawnPos = PossibleSpawnsPos[0];
        }

        public void Scp079sDoors(bool state)
        {
            Vector3 secondDoorPos = Map.GetDoorByName("079_SECOND").transform.position;

            foreach (DoorVariant controlRoomDoor in Map.Doors.Where(d => Vector3.Distance(d.transform.position, secondDoorPos) < 5f))
            {
                controlRoomDoor.NetworkTargetState = state;
            }
        }
    }
}
