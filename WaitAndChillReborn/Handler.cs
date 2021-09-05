namespace WaitAndChillReborn
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using CustomPlayerEffects;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API;
    using Mirror;
    using InventorySystem.Items.ThrowableProjectiles;
    using Exiled.API.Extensions;

    public partial class Handler
    {
        internal void OnWaitingForPlayers()
        {
            SpawnManager();

            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();

            if (Config.AllowFriendlyFire)
            {
                ffPrevValue = Server.FriendlyFire;
                Server.FriendlyFire = true;
            }

            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (lobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(lobbyTimer);
            }

            if (Config.DisplayWaitMessage)
                lobbyTimer = Timing.RunCoroutine(LobbyTimer());

            Timing.CallDelayed(1f, () =>
            {
                foreach (Pickup pickup in Map.Pickups)
                {
                    if (!pickup.Spawned)
                    {
                        NetworkServer.UnSpawn(pickup.Base.gameObject);
                        unspawnedPickups.Add(pickup);
                    }
                }
            });
        }

        internal void OnVerified(VerifiedEventArgs ev)
        {
            if (IsLobby && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
            {
                Timing.CallDelayed(Config.SpawnDelay, () =>
                {
                    ev.Player.Role = Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)];

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

                Timing.CallDelayed(Config.SpawnDelay * 2f, () =>
                {
                    if (!Config.MultipleRooms)
                    {
                        ev.Player.Position = ChoosedSpawnPos;
                    }
                    else
                    {
                        ev.Player.Position = possibleSpawnPoses[Random.Range(0, possibleSpawnPoses.Count)];
                    }

                    if (Config.ColaMultiplier != 0)
                    {
                        ev.Player.EnableEffect<Scp207>();
                        ev.Player.ChangeEffectIntensity<Scp207>(Config.ColaMultiplier);
                    }

                    Timing.CallDelayed(0.3f, () =>
                    {
                        ev.Player.ResetInventory(Config.Inventory);

                        foreach (var ammo in Config.Ammo)
                        {
                            ev.Player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                        }
                    });
                });
            }
        }

        internal void OnDying(DyingEventArgs ev)
        {
            if (IsLobby)
                ev.Target.ClearInventory();
        }

        internal void OnDied(DiedEventArgs ev)
        {
            if (IsLobby && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
            {
                Timing.CallDelayed(Config.SpawnDelay, () => ev.Target.Role = Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)]);

                Timing.CallDelayed(Config.SpawnDelay * 2.5f, () =>
                {
                    if (!Config.MultipleRooms)
                    {
                        ev.Target.Position = ChoosedSpawnPos;
                    }
                    else
                    {
                        ev.Target.Position = possibleSpawnPoses[Random.Range(0, possibleSpawnPoses.Count)];
                    }

                    if (Config.ColaMultiplier != 0)
                    {
                        ev.Target.EnableEffect<Scp207>();
                        ev.Target.ChangeEffectIntensity<Scp207>(Config.ColaMultiplier);
                    }

                    Timing.CallDelayed(0.3f, () =>
                    {
                        ev.Target.ResetInventory(Config.Inventory);

                        foreach (var ammo in Config.Ammo)
                        {
                            ev.Target.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                        }
                    });
                });
            }
        }

        #region Disallowing Events

        internal void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnIntercom(IntercomSpeakingEventArgs ev)
        {
            if ((IsLobby || Round.ElapsedTime.TotalSeconds <= 5) && !Config.AllowIntercom)
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

        internal void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnInteractingLocker(InteractingLockerEventArgs ev)
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

        #endregion

        internal void OnRoundStarted()
        {
            if (Config.AllowFriendlyFire)
                Server.FriendlyFire = ffPrevValue;

            foreach (ThrownProjectile throwable in Object.FindObjectsOfType<ThrownProjectile>())
            {
                throwable.transform.position = Vector3.zero;
                Timing.CallDelayed(1f, () => NetworkServer.Destroy(throwable?.gameObject));
            }

            foreach (Player player in Player.List)
            {
                if (!Config.AllowDamage)
                {
                    player.IsGodModeEnabled = false;
                }

                if (Config.ColaMultiplier != 0)
                {
                    player.DisableEffect<Scp207>();
                }
            }

            if (Config.TurnedPlayers)
            {
                Scp096.TurnedPlayers.Clear();
                Scp173.TurnedPlayers.Clear();
            }

            Scp079sDoors(false);

            Intercom.host.CustomContent = null;

            if (lobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(lobbyTimer);
            }

            foreach (Pickup pickup in unspawnedPickups)
            {
                NetworkServer.Spawn(pickup.Base.gameObject);
            }
            unspawnedPickups.Clear();
        }

        #region Variables

        public static bool IsLobby => !Round.IsStarted && !RoundSummary.singleton.RoundEnded;

        private bool ffPrevValue;

        private string text;

        private List<Vector3> possibleSpawnPoses = new List<Vector3>();

        private Vector3 ChoosedSpawnPos;

        private CoroutineHandle lobbyTimer;

        private List<Pickup> unspawnedPickups = new List<Pickup>();

        private static readonly Translation Translation = WaitAndChillReborn.Singleton.Translation;
        private readonly Config Config = WaitAndChillReborn.Singleton.Config;

        #endregion
    }
}
