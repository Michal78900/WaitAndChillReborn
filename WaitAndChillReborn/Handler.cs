using System;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using MEC;
using UnityEngine;


namespace WaitAndChillReborn
{
    class Handler
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

                    if (!plugin.Config.AlowDamage)
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
                    ev.Player.Position = ChoosedSpawnPos;

                    if (plugin.Config.ColaMultiplier != 0)
                    {
                        ev.Player.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Scp207>(999f, false);
                        ev.Player.ReferenceHub.playerEffectsController.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(plugin.Config.ColaMultiplier);
                    }
                });

            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (!Round.IsStarted && ev.DamageType == DamageTypes.Scp207) ev.Amount = 0f;
        }

        public void OnCreatingPortal(CreatingPortalEventArgs ev)
        {
            if (!Round.IsStarted)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnTeleporting(TeleportingEventArgs ev)
        {
            if (!Round.IsStarted)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnRoundStarted()
        {
            SubClassHandler(true);

            Timing.CallDelayed(0.25f, () =>
            {
                if (!plugin.Config.AlowDamage)
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
                    if (plugin.Config.UseHints) ply.ShowHint(message.ToString(), 1f);
                    else ply.Broadcast(1, message.ToString());
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

            if (plugin.Config.LobbyRoom.Contains("SHELTER"))
            {
                foreach (Room room in Map.Rooms)
                {
                    if (room.Type == Exiled.API.Enums.RoomType.EzShelter)
                    {
                        var BrokenPos = room.transform.position;
                        PossibleSpawnsPos.Add(new Vector3(BrokenPos.x, BrokenPos.y + 2f, BrokenPos.z));
                    }
                }
            }

            if (plugin.Config.LobbyRoom.Contains("173")) PossibleSpawnsPos.Add(Map.GetRandomSpawnPoint(RoleType.Scp173));

            PossibleSpawnsPos.ShuffleList();

            ChoosedSpawnPos = PossibleSpawnsPos[0];
        }
    }
}
