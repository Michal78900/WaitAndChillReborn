namespace WaitAndChillReborn
{
    using System.Collections.Generic;
    using Configs;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Player;
    using GameCore;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using UnityEngine;
    using static API.API;

    using Object = UnityEngine.Object;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using Player = Exiled.API.Features.Player;
    using Scp106Event = Exiled.Events.Handlers.Scp106;
    using Server = Exiled.API.Features.Server;

    internal static class EventHandlers
    {
        internal static void RegisterEvents()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;

            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Spawned += OnSpawned;
            PlayerEvent.Dying += OnDying;
            PlayerEvent.Died += OnDied;

            MapEvent.PlacingBlood += OnDeniableEvent;
            PlayerEvent.SpawningRagdoll += OnDeniableEvent;
            PlayerEvent.IntercomSpeaking += OnDeniableEvent;
            PlayerEvent.DroppingItem += OnDeniableEvent;
            PlayerEvent.DroppingAmmo += OnDeniableEvent;
            PlayerEvent.InteractingDoor += OnDeniableEvent;
            PlayerEvent.InteractingElevator += OnDeniableEvent;
            PlayerEvent.InteractingLocker += OnDeniableEvent;
            MapEvent.ChangingIntoGrenade += OnDeniableEvent;

            // Scp106Event.CreatingPortal += OnDeniableEvent;
            Scp106Event.Teleporting += OnDeniableEvent;

            ServerEvent.RoundStarted += OnRoundStarted;
        }

        internal static void UnRegisterEvents()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;

            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.Spawned -= OnSpawned;
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.Died -= OnDied;

            MapEvent.PlacingBlood -= OnDeniableEvent;
            PlayerEvent.SpawningRagdoll -= OnDeniableEvent;
            PlayerEvent.IntercomSpeaking -= OnDeniableEvent;
            PlayerEvent.DroppingItem -= OnDeniableEvent;
            PlayerEvent.DroppingAmmo -= OnDeniableEvent;
            PlayerEvent.InteractingDoor -= OnDeniableEvent;
            PlayerEvent.InteractingElevator -= OnDeniableEvent;
            PlayerEvent.InteractingLocker -= OnDeniableEvent;
            MapEvent.ChangingIntoGrenade -= OnDeniableEvent;

            // Scp106Event.CreatingPortal -= OnDeniableEvent;
            Scp106Event.Teleporting -= OnDeniableEvent;

            ServerEvent.RoundStarted -= OnRoundStarted;

        }

        private static void OnWaitingForPlayers()
        {
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (LobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(LobbyTimer);
            }

            if (Server.FriendlyFire)
            {
                FriendlyFireConfig.PauseDetector = true;
            }

            if (WaitAndChillReborn.Singleton.Config.DisplayWaitMessage)
                LobbyTimer = Timing.RunCoroutine(LobbyMethods.LobbyTimer());

            // Scp173.TurnedPlayers.Clear();
            // Scp096.TurnedPlayers.Clear();

            Timing.CallDelayed(0.1f, () => LobbyMethods.SetupAvailablePositions());

            Timing.CallDelayed(1f, () =>
            {
                foreach (Pickup pickup in Pickup.List)
                {
                    try
                    {
                        if (!pickup.IsLocked)
                        {
                            pickup.IsLocked = true;
                            pickup.Base.GetComponent<Rigidbody>().isKinematic = true;
                            _lockedPickups.Add(pickup);
                        }
                    }
                    catch (System.Exception)
                    {
                        
                    }
                }
            });
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            if (!IsLobby)
                return;
            
            if (RoundStart.singleton.NetworkTimer > 1 || RoundStart.singleton.NetworkTimer == -2)
            {
                Timing.CallDelayed(Config.SpawnDelay, () =>
                {
                    ev.Player.Role.Set(Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)]);

                    if (Config.TurnedPlayers)
                    {
                        // Scp096.TurnedPlayers.Add(ev.Player);
                        // Scp173.TurnedPlayers.Add(ev.Player);
                    }
                });
            }
        }

        private static void OnSpawned(SpawnedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2)
                return;

            ev.Player.Position = Config.MultipleRooms switch
            {
                true => LobbyAvailableSpawnPoints[Random.Range(0, LobbyAvailableSpawnPoints.Count)],
                false => LobbyChoosedSpawnPoint
            };
            
            _ = !Config.MultipleRooms ? ev.Player.Position = LobbyChoosedSpawnPoint : ev.Player.Position = LobbyAvailableSpawnPoints[Random.Range(0, LobbyAvailableSpawnPoints.Count)];

            foreach (KeyValuePair<EffectType, byte> effect in Config.LobbyEffects)
            {
                ev.Player.EnableEffect(effect.Key);
                ev.Player.ChangeEffectIntensity(effect.Key, effect.Value);
            }

            Timing.CallDelayed(0.3f, () =>
            {
                Exiled.CustomItems.API.Extensions.ResetInventory(ev.Player, Config.Inventory);

                foreach (KeyValuePair<AmmoType, ushort> ammo in Config.Ammo)
                {
                    ev.Player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                }
            });
        }

        private static void OnDeniableEvent(IExiledEvent ev)
        {
            if (IsLobby && ev is IDeniableEvent deniableEvent)
                deniableEvent.IsAllowed = false;
        }

        private static void OnDying(DyingEventArgs ev)
        {
            if (IsLobby)
                ev.Player.ClearInventory();
        }

        private static void OnDied(DiedEventArgs ev)
        {
            if (!IsLobby || (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2))
                return;
            
            Timing.CallDelayed(Config.SpawnDelay, () => ev.Player.Role.Set(Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)]));

            Timing.CallDelayed(Config.SpawnDelay * 2.5f, () =>
            {
                ev.Player.Position = Config.MultipleRooms switch
                {
                    true => LobbyAvailableSpawnPoints[Random.Range(0, LobbyAvailableSpawnPoints.Count)],
                    false => LobbyChoosedSpawnPoint
                };
                
                foreach (KeyValuePair<EffectType, byte> effect in Config.LobbyEffects)
                {
                    ev.Player.EnableEffect(effect.Key);
                    ev.Player.ChangeEffectIntensity(effect.Key, effect.Value);
                }

                Timing.CallDelayed(0.3f, () =>
                {
                    Exiled.CustomItems.API.Extensions.ResetInventory(ev.Player, Config.Inventory);

                    foreach (KeyValuePair<AmmoType, ushort> ammo in Config.Ammo)
                    {
                        ev.Player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                    }
                });
            });
        }

        private static void OnRoundStarted()
        {
            /*
            foreach (Player player in Player.List)
            {
                player.ClearInventory();
                player.Role.Set(RoleTypeId.Spectator);
            }
            */

            foreach (ThrownProjectile throwable in Object.FindObjectsOfType<ThrownProjectile>())
            {
                if (throwable.Rb.velocity.sqrMagnitude <= 1f)
                    continue;

                throwable.transform.position = Vector3.zero;
                Timing.CallDelayed(1f, () => NetworkServer.Destroy(throwable?.gameObject));
            }

            /*
            foreach (Player player in Player.List)
            {
                foreach (KeyValuePair<System.Type, PlayerEffect> effect in player.ReferenceHub.playerEffectsController.AllEffects)
                {
                    if (effect.Key == null || effect.Value == null)
                    {
                        Log.Error("Effect is null!");
                        continue;
                    }

                    effect.Value.IsEnabled = false;
                }
            }
            */

            if (Config.TurnedPlayers)
            {
                // Scp096.TurnedPlayers.Clear();
                // Scp173.TurnedPlayers.Clear();
            }

            if (Server.FriendlyFire)
            {
                FriendlyFireConfig.PauseDetector = false;
            }

            LobbyMethods.Scp079sDoors(false);

            if (LobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(LobbyTimer);
            }

            foreach (Pickup pickup in _lockedPickups)
            {
                pickup.IsLocked = false;
                pickup.Base.GetComponent<Rigidbody>().isKinematic = false;
            }

            _lockedPickups.Clear();
        }

        private static readonly HashSet<Pickup> _lockedPickups = new();
        private static readonly LobbyConfig Config = WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}
