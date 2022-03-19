namespace WaitAndChillReborn.Handlers
{
    using System.Collections.Generic;
    using Configs;
    using CustomPlayerEffects;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using Mirror;
    using UnityEngine;
    using Methods;
    using GameCore;

    using static API.API;

    using Object = UnityEngine.Object;
    using Log = Exiled.API.Features.Log;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using Scp106Event = Exiled.Events.Handlers.Scp106;

    internal static class LobbyEventHandler
    {
        internal static void RegisterEvents()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;

            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Dying += OnDying;
            PlayerEvent.Died += OnDied;

            MapEvent.PlacingBlood += OnPlacingBlood;
            PlayerEvent.SpawningRagdoll += OnSpawningRagdoll;
            PlayerEvent.IntercomSpeaking += OnIntercom;
            PlayerEvent.DroppingItem += OnDroppingItem;
            PlayerEvent.DroppingAmmo += OnDroppingAmmo;
            PlayerEvent.InteractingDoor += OnInteractingDoor;
            PlayerEvent.InteractingElevator += OnInteractingElevator;
            PlayerEvent.InteractingLocker += OnInteractingLocker;
            MapEvent.ChangingIntoGrenade += OnChangingIntoGrenade;

            Scp106Event.CreatingPortal += OnCreatingPortal;
            Scp106Event.Teleporting += OnTeleporting;

            ServerEvent.RoundStarted += OnRoundStarted;
        }

        internal static void UnRegisterEvents()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;

            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.Died -= OnDied;

            MapEvent.PlacingBlood -= OnPlacingBlood;
            PlayerEvent.SpawningRagdoll -= OnSpawningRagdoll;
            PlayerEvent.IntercomSpeaking -= OnIntercom;
            PlayerEvent.DroppingItem -= OnDroppingItem;
            PlayerEvent.DroppingAmmo -= OnDroppingAmmo;
            PlayerEvent.InteractingDoor -= OnInteractingDoor;
            PlayerEvent.InteractingElevator -= OnInteractingElevator;
            PlayerEvent.InteractingLocker -= OnInteractingLocker;
            MapEvent.ChangingIntoGrenade -= OnChangingIntoGrenade;

            Scp106Event.CreatingPortal -= OnCreatingPortal;
            Scp106Event.Teleporting -= OnTeleporting;

            ServerEvent.RoundStarted -= OnRoundStarted;

        }

        private static void OnWaitingForPlayers()
        {
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (LobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(LobbyTimer);
            }

            if (WaitAndChillReborn.Singleton.Config.DisplayWaitMessage)
                LobbyTimer = Timing.RunCoroutine(LobbyMethods.LobbyTimer());

            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();

            Timing.CallDelayed(0.1f, () => LobbyMethods.SetupAvailablePositions());
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (!WaitAndChillReborn.Singleton.Config.GlobalVoiceChat)
                ev.Player.SendFakeSyncVar(RoundStart.singleton.netIdentity, typeof(RoundStart), nameof(RoundStart.NetworkTimer), (short)-1);

            if (RoundStart.singleton.NetworkTimer > 1 || RoundStart.singleton.NetworkTimer == -2)
            {
                Timing.CallDelayed(Config.SpawnDelay, () =>
                {
                    ev.Player.Role.Type = Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)];

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
                        ev.Player.Position = LobbyChoosedSpawnPoint;
                    }
                    else
                    {
                        ev.Player.Position = LobbyAvailableSpawnPoints[Random.Range(0, LobbyAvailableSpawnPoints.Count)];
                    }

                    foreach (var effect in Config.LobbyEffects)
                    {
                        ev.Player.EnableEffect(effect.Key);
                        ev.Player.ChangeEffectIntensity(effect.Key, effect.Value);
                    }

                    Timing.CallDelayed(0.3f, () =>
                    {
                        Exiled.CustomItems.API.Extensions.ResetInventory(ev.Player, Config.Inventory);
                        // ev.Target.ResetInventory();

                        foreach (var ammo in Config.Ammo)
                        {
                            ev.Player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                        }
                    });
                });
            }
        }

        #region Disallowing Events

        private static void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnIntercom(IntercomSpeakingEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnCreatingPortal(CreatingPortalEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnTeleporting(TeleportingEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
            }
        }

        #endregion

        private static void OnDying(DyingEventArgs ev)
        {
            if (IsLobby)
                ev.Target.ClearInventory();
        }

        private static void OnDied(DiedEventArgs ev)
        {
            if (IsLobby && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
            {
                Timing.CallDelayed(Config.SpawnDelay, () => ev.Target.Role.Type = Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)]);

                Timing.CallDelayed(Config.SpawnDelay * 2.5f, () =>
                {
                    if (!Config.MultipleRooms)
                    {
                        ev.Target.Position = LobbyChoosedSpawnPoint;
                    }
                    else
                    {
                        ev.Target.Position = LobbyAvailableSpawnPoints[Random.Range(0, LobbyAvailableSpawnPoints.Count)];
                    }

                    foreach (var effect in Config.LobbyEffects)
                    {
                        ev.Target.EnableEffect(effect.Key);
                        ev.Target.ChangeEffectIntensity(effect.Key, effect.Value);
                    }

                    Timing.CallDelayed(0.3f, () =>
                    {
                        Exiled.CustomItems.API.Extensions.ResetInventory(ev.Target, Config.Inventory);
                        // ev.Target.ResetInventory();

                        foreach (var ammo in Config.Ammo)
                        {
                            ev.Target.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                        }
                    });
                });
            }
        }

        private static void OnRoundStarted()
        {
            foreach (Player player in Player.List)
            {
                player.ClearInventory();
                player.Role.Type = RoleType.Spectator;
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

            if (Config.TurnedPlayers)
            {
                Scp096.TurnedPlayers.Clear();
                Scp173.TurnedPlayers.Clear();
            }

            LobbyMethods.Scp079sDoors(false);

            if (LobbyTimer.IsRunning)
            {
                Timing.KillCoroutines(LobbyTimer);
            }
        }

        private static readonly LobbyConfig Config = WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}
