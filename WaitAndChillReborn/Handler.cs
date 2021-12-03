namespace WaitAndChillReborn
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API;
    using Mirror;
    using InventorySystem.Items.ThrowableProjectiles;
    using Exiled.API.Extensions;
    using System.Linq;
    using CustomPlayerEffects;

    public partial class Handler
    {
        internal void OnWaitingForPlayers()
        {
            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();

            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (lobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(lobbyTimer);
            }

            if (Config.DisplayWaitMessage)
                lobbyTimer = Timing.RunCoroutine(LobbyTimer());

            Timing.CallDelayed(0.1f, () => SpawnManager());

            Timing.CallDelayed(1f, () =>
            {
                foreach (Pickup pickup in Map.Pickups)
                {
                    try
                    {
                        if (!pickup.Spawned)
                        {
                            if (pickup.Base.name.Contains("CustomSchematic"))
                                continue;

                            pickup.Locked = true;
                            pickup.Base.GetComponent<Rigidbody>().isKinematic = true;
                            unspawnedPickups.Add(pickup);
                        }
                    }
                    catch (System.Exception)
                    {
                        continue;
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
                        ev.Player.Position = choosedSpawnPos;
                    }
                    else
                    {
                        ev.Player.Position = possibleSpawnPoses[Random.Range(0, possibleSpawnPoses.Count)];
                    }

                    if (Config.MovementBoost != 0)
                    {
                        ev.Player.EnableEffect<MovementBoost>();
                        ev.Player.ChangeEffectIntensity<MovementBoost>(50);
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
                        ev.Target.Position = choosedSpawnPos;
                    }
                    else
                    {
                        ev.Target.Position = possibleSpawnPoses[Random.Range(0, possibleSpawnPoses.Count)];
                    }

                    if (Config.MovementBoost != 0)
                    {
                        ev.Target.EnableEffect<MovementBoost>();
                        ev.Target.ChangeEffectIntensity<MovementBoost>(50);
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

        internal void OnDroppingAmmo(DroppingAmmoEventArgs ev)
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

        internal void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        #endregion

        internal void OnRoundStarted()
        {
            foreach (Player player in Player.List)
            {
                player.ClearInventory();
                player.Role = RoleType.Spectator;
            }

            if (Config.AllowFriendlyFire)
            {
                Server.FriendlyFire = ffPrevValue.Value;
            }

            foreach (ThrownProjectile throwable in Object.FindObjectsOfType<ThrownProjectile>())
            {
                if (throwable.Rb.velocity.sqrMagnitude <= 1f)
                    continue;

                throwable.transform.position = Vector3.zero;
                Timing.CallDelayed(1f, () => NetworkServer.Destroy(throwable?.gameObject));
            }

            foreach (Player player in Player.List)
            {
                if (!Config.AllowDamage)
                {
                    player.IsGodModeEnabled = false;
                }

                if (Config.MovementBoost != 0)
                {
                    player.DisableEffect<MovementBoost>();
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

            foreach (Pickup pickup in unspawnedPickups.ToList())
            {
                try
                {
                    pickup.Locked = false;
                    pickup.Base.GetComponent<Rigidbody>().isKinematic = false;
                }
                catch (System.Exception)
                {
                    continue;
                }

                unspawnedPickups.Clear();
            }
        }

        #region Variables

        public static bool IsLobby => !Round.IsStarted && !RoundSummary.singleton.RoundEnded;

        private bool? ffPrevValue = null;

        private string text;

        private List<Vector3> possibleSpawnPoses = new List<Vector3>();

        private Vector3 choosedSpawnPos;

        private CoroutineHandle lobbyTimer;

        private List<Pickup> unspawnedPickups = new List<Pickup>();

        private static readonly Translation Translation = WaitAndChillReborn.Singleton.Translation;
        private readonly Config Config = WaitAndChillReborn.Singleton.Config;

        #endregion
    }
}
