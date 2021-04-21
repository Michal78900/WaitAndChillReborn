namespace WaitAndChillReborn
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using CustomPlayerEffects;
    using Mirror;
    using System.Linq;

    public partial class Handler
    {
        private readonly Config Config = WaitAndChillReborn.Singleton.Config;

        private readonly System.Random rng = new System.Random();

        private string text;

        private List<Vector3> PossibleSpawnsPos = new List<Vector3>();

        private Vector3 ChoosedSpawnPos;

        private CoroutineHandle lobbyTimer;

        internal void OnWaitingForPlayers()
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

            if (Config.DisplayWaitMessage)
                lobbyTimer = Timing.RunCoroutine(LobbyTimer());
        }

        internal void OnVerified(VerifiedEventArgs ev)
        {
            if (IsLobby && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.Role = Config.RolesToChoose[rng.Next(Config.RolesToChoose.Count)];

                    if (!Config.AllowDamage)
                    {
                        ev.Player.IsGodModeEnabled = true;
                    }

                    if (Config.TurnedPlayers)
                    {
                        Scp096.TurnedPlayers.Add(ev.Player);
                        Scp173.TurnedPlayers.Add(ev.Player);
                    }
                });

                Timing.CallDelayed(0.5f, () =>
                {
                    if (!Config.MultipleRooms)
                    {
                        ev.Player.Position = ChoosedSpawnPos;
                    }
                    else
                    {
                        ev.Player.Position = PossibleSpawnsPos[rng.Next(PossibleSpawnsPos.Count)];
                    }

                    if (Config.ColaMultiplier != 0)
                    {
                        ev.Player.EnableEffect<Scp207>();
                        ev.Player.ChangeEffectIntensity<Scp207>(Config.ColaMultiplier);
                    }
                });
            }
        }

        public static bool IsLobby => !Round.IsStarted && !RoundSummary.singleton._roundEnded;

        internal void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnIntercom(IntercomSpeakingEventArgs ev)
        {
            if (IsLobby && !Config.AllowIntercom)
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

        internal void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            if (IsLobby && ev.Name == "destroy" && ev.Arguments[0] == "**")
            {
                ev.IsAllowed = false;
                ev.Success = false;

                ev.ReplyMessage = "You can't destroy all doors, while parkour is present because it may crash your server and give people an earrape!";
            }
        }

        internal void OnRoundStarted()
        {
            Intercom.host.CustomContent = string.Empty;
            Intercom.host.Network_state = Intercom.State.Ready;

            SubClassHandler(true);

            Timing.CallDelayed(0.25f, () =>
            {
                foreach (Player ply in Player.List)
                {
                    if (!Config.AllowDamage)
                    {
                        ply.IsGodModeEnabled = false;
                    }

                    if (Config.ColaMultiplier != 0)
                    {
                        ply.DisableEffect<Scp207>();
                    }
                }

                if (Config.TurnedPlayers)
                {
                    Scp096.TurnedPlayers.Clear();
                    Scp173.TurnedPlayers.Clear();
                }

                foreach (var door in spawnedDoors)
                {
                    NetworkServer.Destroy(door.gameObject);
                }
                spawnedDoors.Clear();

                Scp079sDoors(false);

                if (lobbyTimer.IsRunning)
                {
                    Timing.KillCoroutines(lobbyTimer);
                }
            });
        }
    }
}
