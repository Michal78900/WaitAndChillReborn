namespace WaitAndChillReborn.Handlers
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;
    using API.Features;
    using MEC;
    using System.Linq;
    using Exiled.API.Extensions;
    using Methods;
    using UnityEngine;
    using Exiled.API.Features;
    using Configs;
    using GameCore;

    using static API.API;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;

    internal static class ArenaEventHandler
    {
        internal static void RegisterEvents()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;
            PlayerEvent.Verified += OnVerified;

            PlayerEvent.Spawning += OnSpawning;
            MapEditorReborn.Events.Handlers.Schematic.ButtonInteracted += OnButtonInteracted;

            PlayerEvent.ReloadingWeapon += OnReloading;
            PlayerEvent.Dying += OnDying;

            PlayerEvent.SpawningRagdoll += OnSpawningRagdoll;
            PlayerEvent.DroppingItem += OnDroppingItem;
            PlayerEvent.DroppingAmmo += OnDroppingAmmo;

            ServerEvent.RoundStarted += OnRoundStarted;
        }

        internal static void UnRegisterEvents()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;
            PlayerEvent.Verified -= OnVerified;

            PlayerEvent.Spawning -= OnSpawning;
            MapEditorReborn.Events.Handlers.Schematic.ButtonInteracted += OnButtonInteracted;

            PlayerEvent.ReloadingWeapon -= OnReloading;
            PlayerEvent.Dying -= OnDying;

            PlayerEvent.SpawningRagdoll -= OnSpawningRagdoll;
            PlayerEvent.DroppingItem -= OnDroppingItem;
            PlayerEvent.DroppingAmmo -= OnDroppingAmmo;

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

            Arena.List.Clear();
            ArenaClock = Timing.RunCoroutine(ArenaMethods.ArenaClock());
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            // if (IsLobby && !WaitAndChillReborn.Singleton.Config.GlobalVoiceChat)
                // ev.Player.SendFakeSyncVar(RoundStart.singleton.netIdentity, typeof(RoundStart), nameof(RoundStart.NetworkTimer), (short)-1);
        }

        private static void OnSpawning(SpawningEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (ev.Player.TryGetSessionVariable(AtachedArenaSessionVarName, out Arena arena))
                ev.Position = ev.RoleType == RoleType.NtfCaptain ? arena.NtfSpawnPoints[Random.Range(0, arena.NtfSpawnPoints.Count)] : arena.CiSpawnPoints[Random.Range(0, arena.CiSpawnPoints.Count)];

            ev.Player.ClearInventory();
            ev.Player.AddItem(ItemType.ArmorHeavy);

            ev.Player.Ammo.Clear();
            ev.Player.Ammo.Add(ItemType.Ammo556x45, 1);
            ev.Player.Ammo.Add(ItemType.Ammo762x39, 1);
            ev.Player.Ammo.Add(ItemType.Ammo9x19, 1);
            ev.Player.Ammo.Add(ItemType.Ammo44cal, 1);
            ev.Player.Ammo.Add(ItemType.Ammo12gauge, 1);
        }

        private static void OnButtonInteracted(MapEditorReborn.Events.EventArgs.ButtonInteractedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (!Config.ArenaNames.Contains(ev.Schematic.Name))
                return;

            Item weapon = ev.Player.Items.FirstOrDefault(x => x.IsWeapon);
            if (weapon != null)
                ev.Player.RemoveItem(weapon);

            Item item = ev.Player.AddItem(ev.Button.Type);
            ev.Player.CurrentItem = item;
        }

        private static void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!IsLobby)
                return;

            ushort num = (ushort)(ev.Firearm.MaxAmmo - ev.Firearm.Ammo - ev.Player.Ammo[ev.Firearm.AmmoType.GetItemType()] + 1);

            if (num == 0)
                return;

            ev.Player.AddAmmo(ev.Firearm.AmmoType, num);
        }

        private static void OnDying(DyingEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (ev.Killer == null)
                return;

            ev.Target.ClearInventory();
            ev.Target.Ammo.Clear();

            Timing.CallDelayed(Config.RespawnTime, () =>
            {
                if (!IsLobby)
                    return;

                ev.Killer.ClearInventory();
                ev.Killer.Ammo.Clear();

                ev.Killer.Role.Type = RoleType.Spectator;

                if (ev.Killer.TryGetSessionVariable(AtachedArenaSessionVarName, out Arena arena))
                    arena.IsAvailable = true;
            });
        }
        private static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (IsLobby)
            {
                ev.IsAllowed = false;
                Ragdoll ragdoll = new Ragdoll(ev.Info, true);

                Timing.CallDelayed(Config.RespawnTime, () => ragdoll.Delete());
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

        private static void OnRoundStarted()
        {
            Timing.KillCoroutines(ArenaClock);

            foreach (Player player in Player.List)
            {
                player.ClearInventory();
                player.Role.Type = RoleType.Spectator;
            }

            foreach (Arena arena in Arena.List)
            {
                arena.Schematic.Destroy();
            }
            Arena.List.Clear();
        }

        private static CoroutineHandle ArenaClock;

        private static readonly ArenaConfig Config = WaitAndChillReborn.Singleton.Config.ArenaConfig;
    }
}
