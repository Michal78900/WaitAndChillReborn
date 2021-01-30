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
        System.Random rng = new System.Random();

        StringBuilder message = new StringBuilder();

        public List<Vector3> PossibleSpawnsPos = new List<Vector3>();

        Vector3 ChoosedSpawnPos;

        //148.6951f, 1019.447f, -19.06371f third tower
        //223.1443f, 1026.775f, -18,15129f fourth tower
        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public void OnWatingForPlayers()
        {
            SpawnManager();

            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();

            if (WaitAndChillReborn.ThereIsSubClass)
            {
                SubClassHandler(false);
            }

            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            foreach (CoroutineHandle coroutine in coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }
            coroutines.Clear();

            if (WaitAndChillReborn.Instance.Config.DisplayWaitMessage) coroutines.Add(Timing.RunCoroutine(LobbyTimer()));
        }

        public void OnPlayerJoin(JoinedEventArgs ev)
        {
            if (!Round.IsStarted && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.Role = WaitAndChillReborn.Instance.Config.RolesToChoose[rng.Next(WaitAndChillReborn.Instance.Config.RolesToChoose.Count)];

                    if (!WaitAndChillReborn.Instance.Config.AlowDamage)
                    {
                        ev.Player.IsGodModeEnabled = true;
                    }

                    if (WaitAndChillReborn.Instance.Config.TurnedPlayers)
                    {
                        Scp096.TurnedPlayers.Add(ev.Player);
                        Scp173.TurnedPlayers.Add(ev.Player);
                    }
                });

                Timing.CallDelayed(0.5f, () =>
                {
                    ev.Player.Position = ChoosedSpawnPos;

                    if (WaitAndChillReborn.Instance.Config.ColaMultiplier != 0)
                    {
                        ev.Player.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Scp207>(999f, false);
                        ev.Player.ReferenceHub.playerEffectsController.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(WaitAndChillReborn.Instance.Config.ColaMultiplier);
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
            if(!Round.IsStarted)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnTeleporting(TeleportingEventArgs ev)
        {
            if(!Round.IsStarted)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnRoundStarted()
        {
            if (WaitAndChillReborn.ThereIsSubClass)
            {
                SubClassHandler(true);
            }

            Timing.CallDelayed(0.25f, () =>
            {
                if (!WaitAndChillReborn.Instance.Config.AlowDamage)
                {
                    foreach (Player ply in Player.List)
                    {
                        ply.IsGodModeEnabled = false;
                    }
                }

                if (WaitAndChillReborn.Instance.Config.TurnedPlayers)
                {
                    Scp096.TurnedPlayers.Clear();
                    Scp173.TurnedPlayers.Clear();
                }

                if (WaitAndChillReborn.Instance.Config.ColaMultiplier != 0)
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

                if (WaitAndChillReborn.Instance.Config.HintVertPos != 0 && WaitAndChillReborn.Instance.Config.HintVertPos < 0)
                {
                    for (int i = WaitAndChillReborn.Instance.Config.HintVertPos; i < 0; i++)
                    {
                        message.Append("\n");
                    }
                }


                message.Append(WaitAndChillReborn.Instance.Config.TopMessage);

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                switch (NetworkTimer)
                {
                    case -2: message.Replace("%seconds", WaitAndChillReborn.Instance.Config.ServerIsPaused); break;

                    case -1: message.Replace("%seconds", WaitAndChillReborn.Instance.Config.RoundIsBeingStarted); break;

                    case 1: message.Replace("%seconds", $"{NetworkTimer} {WaitAndChillReborn.Instance.Config.OneSecondRemain}"); break;

                    case 0: message.Replace("%seconds", WaitAndChillReborn.Instance.Config.RoundIsBeingStarted); break;

                    default: message.Replace("%seconds", $"{NetworkTimer} {WaitAndChillReborn.Instance.Config.XSecondsRemains}"); break;
                }

                int NumOfPlayers = Player.List.Count();

                message.Append($"\n{WaitAndChillReborn.Instance.Config.BottomMessage}");

                if (NumOfPlayers == 1) message.Replace("%players", $"{NumOfPlayers} {WaitAndChillReborn.Instance.Config.OnePlayerConnected}");
                else message.Replace("%players", $"{NumOfPlayers} {WaitAndChillReborn.Instance.Config.XPlayersConnected}");


                if (WaitAndChillReborn.Instance.Config.HintVertPos != 0 && WaitAndChillReborn.Instance.Config.HintVertPos > 0)
                {
                    for (int i = 0; i < WaitAndChillReborn.Instance.Config.HintVertPos; i++)
                    {
                        message.Append("\n");
                    }
                }


                foreach (Player ply in Player.List)
                {
                    if (WaitAndChillReborn.Instance.Config.UseHints) ply.ShowHint(message.ToString(), 1f);
                    else ply.Broadcast(1, message.ToString());
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }


        public void SubClassHandler(bool enabled)
        {
            var extensions = WaitAndChillReborn.subclassAssembly.GetType("Subclass.API");
            if (extensions == null) return;

            if (enabled)
            {
                extensions.GetMethod("EnableAllClasses").Invoke(null, new object[] { });
            }
            else
            {
                extensions.GetMethod("DisableAllClasses").Invoke(null, new object[] { });
            }

        }

        public void SpawnManager()
        {
            PossibleSpawnsPos.Clear();

            if (WaitAndChillReborn.Instance.Config.LobbyRoom.Contains("TOWER1")) PossibleSpawnsPos.Add(new Vector3(54.81f, 1019.41f, -44.906f));
            if (WaitAndChillReborn.Instance.Config.LobbyRoom.Contains("TOWER2")) PossibleSpawnsPos.Add(new Vector3(148.6951f, 1019.447f, -19.06371f));
            if (WaitAndChillReborn.Instance.Config.LobbyRoom.Contains("TOWER3")) PossibleSpawnsPos.Add(new Vector3(223.1443f, 1026.775f, -18.15129f));

            if (WaitAndChillReborn.Instance.Config.LobbyRoom.Contains("SHELTER"))
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

            if (WaitAndChillReborn.Instance.Config.LobbyRoom.Contains("173")) PossibleSpawnsPos.Add(Map.GetRandomSpawnPoint(RoleType.Scp173));

            PossibleSpawnsPos.ShuffleList();

            ChoosedSpawnPos = PossibleSpawnsPos[0];
        }
    }
}
