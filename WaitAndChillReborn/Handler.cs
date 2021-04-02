namespace WaitAndChillReborn
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using System.Text;
    using Exiled.API.Enums;
    using CustomPlayerEffects;

    public partial class Handler
    {
        private readonly WaitAndChillReborn plugin;
        public Handler(WaitAndChillReborn plugin) => this.plugin = plugin;

        System.Random rng = new System.Random();

        string text;

        List<Vector3> PossibleSpawnsPos = new List<Vector3>();

        Vector3 ChoosedSpawnPos;

        CoroutineHandle lobbyTimer;

        internal void OnWatingForPlayers()
        {
            SpawnManager();

            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();

            SubClassHandler(false);

            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (lobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(lobbyTimer);
            }

            if (plugin.Config.DisplayWaitMessage)
                lobbyTimer = Timing.RunCoroutine(LobbyTimer());
        }

        internal void OnVerified(VerifiedEventArgs ev)
        {
            if (IsLobby && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
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
                    if (!plugin.Config.MultipleRooms)
                    {
                        ev.Player.Position = ChoosedSpawnPos;
                    }

                    else
                    {
                        ev.Player.Position = PossibleSpawnsPos[rng.Next(PossibleSpawnsPos.Count)];
                    }

                    if (plugin.Config.ColaMultiplier != 0)
                    {
                        //ev.Player.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Scp207>(999f, false);
                        //ev.Player.ReferenceHub.playerEffectsController.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(plugin.Config.ColaMultiplier);

                        ev.Player.EnableEffect<Scp207>();
                        ev.Player.ChangeEffectIntensity<Scp207>(plugin.Config.ColaMultiplier);
                    }
                });

            }
        }


        bool IsLobby => !Round.IsStarted && !RoundSummary.singleton._roundEnded;

        internal void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnIntercom(IntercomSpeakingEventArgs ev)
        {
            if (IsLobby && !plugin.Config.AllowIntercom)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnHurting(HurtingEventArgs ev)
        {
            if (IsLobby && ev.DamageType == DamageTypes.Scp207)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnItemPickup(PickingUpItemEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnDoor(InteractingDoorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnElevator(InteractingElevatorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnCreatingPortal(CreatingPortalEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnTeleporting(TeleportingEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnRoundStarted()
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

                if (lobbyTimer.IsRunning)
                {
                    Timing.KillCoroutines(lobbyTimer);
                }
            });
        }
    }
}
