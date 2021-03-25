using System;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.API.Enums;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using Mirror;
using Interactables.Interobjects.DoorUtils;
using Exiled.API.Extensions;
using UnityEngine.PostProcessing;

namespace WaitAndChillReborn
{
    public class Handler
    {
        private readonly WaitAndChillReborn plugin;
        public Handler(WaitAndChillReborn plugin) => this.plugin = plugin;

        System.Random rng = new System.Random();

        StringBuilder message = new StringBuilder();

        public List<Vector3> PossibleSpawnsPos = new List<Vector3>();

        Vector3 ChoosedSpawnPos;


        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public void OnWatingForPlayers()
        {
            SpawnManager();

            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();

            SubClassHandler(false);

            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            foreach (CoroutineHandle coroutine in coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }
            coroutines.Clear();

            if (plugin.Config.DisplayWaitMessage) coroutines.Add(Timing.RunCoroutine(LobbyTimer()));
        }

        public void OnPlayerJoin(VerifiedEventArgs ev)
        {
            if (!Round.IsStarted && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.Role = plugin.Config.RolesToChoose[rng.Next(plugin.Config.RolesToChoose.Count)];

                    if (!plugin.Config.AllowDamage)
                    {
                        ev.Player.IsGodModeEnabled = true;
                    }

                    if (plugin.Config.TurnedPlayers)
                    {
                        Scp096.TurnedPlayers.Add(ev.Player);
                        Scp173.TurnedPlayers.Add(ev.Player);
                    }
                });

                Timing.CallDelayed(0.5f, () =>
                {
                    if (!plugin.Config.MultipleRooms) ev.Player.Position = ChoosedSpawnPos;

                    else
                    {
                        ev.Player.Position = PossibleSpawnsPos[rng.Next(PossibleSpawnsPos.Count)];
                    }

                    if (plugin.Config.ColaMultiplier != 0)
                    {
                        ev.Player.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Scp207>(999f, false);
                        ev.Player.ReferenceHub.playerEffectsController.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(plugin.Config.ColaMultiplier);
                    }
                });

            }
        }


        bool IsLobby => !Round.IsStarted && !RoundSummary.singleton._roundEnded;

        public void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnIntercom(IntercomSpeakingEventArgs ev)
        {
            if (IsLobby && !plugin.Config.AllowIntercom)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (IsLobby && ev.DamageType == DamageTypes.Scp207)
                ev.IsAllowed = false;
        }

        public void OnItemPickup(PickingUpItemEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnDoor(InteractingDoorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnElevator(InteractingElevatorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnCreatingPortal(CreatingPortalEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnTeleporting(TeleportingEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }



        public void OnRoundStarted()
        {
            SubClassHandler(true);

            Timing.CallDelayed(0.25f, () =>
            {
                if (!plugin.Config.AllowDamage)
                {
                    foreach (Player ply in Player.List)
                    {
                        ply.IsGodModeEnabled = false;
                    }
                }

                if (plugin.Config.TurnedPlayers)
                {
                    Scp096.TurnedPlayers.Clear();
                    Scp173.TurnedPlayers.Clear();
                }

                if (plugin.Config.ColaMultiplier != 0)
                {
                    foreach (Player ply in Player.List)
                    {
                        ply.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                }

                Scp079sDoors(false);

                foreach (CoroutineHandle coroutine in coroutines)
                {
                    Timing.KillCoroutines(coroutine);
                }
                coroutines.Clear();
            });
        }

        private IEnumerator<float> LobbyTimer()
        {
            while (!Round.IsStarted)
            {
                message.Clear();

                if (plugin.Config.HintVertPos != 0 && plugin.Config.HintVertPos < 0)
                {
                    for (int i = plugin.Config.HintVertPos; i < 0; i++)
                    {
                        message.Append("\n");
                    }
                }


                message.Append(plugin.Config.TopMessage);

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                switch (NetworkTimer)
                {
                    case -2: message.Replace("%seconds", plugin.Config.ServerIsPaused); break;

                    case -1: message.Replace("%seconds", plugin.Config.RoundIsBeingStarted); break;

                    case 1: message.Replace("%seconds", $"{NetworkTimer} {plugin.Config.OneSecondRemain}"); break;

                    case 0: message.Replace("%seconds", plugin.Config.RoundIsBeingStarted); break;

                    default: message.Replace("%seconds", $"{NetworkTimer} {plugin.Config.XSecondsRemains}"); break;
                }

                message.Append($"\n{plugin.Config.BottomMessage}");

                if (Player.List.Count() == 1) message.Replace("%players", $"{Player.List.Count()} {plugin.Config.OnePlayerConnected}");
                else message.Replace("%players", $"{Player.List.Count()} {plugin.Config.XPlayersConnected}");


                if (plugin.Config.HintVertPos != 0 && plugin.Config.HintVertPos > 0)
                {
                    for (int i = 0; i < plugin.Config.HintVertPos; i++)
                    {
                        message.Append("\n");
                    }
                }


                foreach (Player ply in Player.List)
                {
                    if (plugin.Config.UseHints)
                        ply.ShowHint(message.ToString(), 1f);
                    else
                        ply.Broadcast(1, message.ToString());
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
