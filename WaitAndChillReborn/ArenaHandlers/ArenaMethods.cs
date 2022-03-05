namespace WaitAndChillReborn.Methods
{
    using Exiled.API.Features;
    using MEC;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using API.Features;
    using Configs;

    using static API.API;

    internal static class ArenaMethods
    {
        internal static IEnumerator<float> ArenaClock()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(Config.ClockUpdateTime);
                try
                {
                    Player ntf;
                    Player ci;
                    List<Player> availablePlayers = Player.Get(Team.RIP).ToList();

                    if (availablePlayers.Count < 2)
                        continue;

                    ntf = availablePlayers[Random.Range(0, availablePlayers.Count)];
                    availablePlayers.Remove(ntf);

                    if (availablePlayers.Count < 1)
                        continue;

                    ci = availablePlayers[Random.Range(0, availablePlayers.Count)];

                    Arena arena = Arena.GetEmptyArena();
                    arena.IsAvailable = false;

                    if (!ntf.SessionVariables.ContainsKey(AtachedArenaSessionVarName))
                        ntf.SessionVariables.Add(AtachedArenaSessionVarName, null);

                    if (!ci.SessionVariables.ContainsKey(AtachedArenaSessionVarName))
                        ci.SessionVariables.Add(AtachedArenaSessionVarName, null);

                    ntf.SessionVariables[AtachedArenaSessionVarName] = arena;
                    ci.SessionVariables[AtachedArenaSessionVarName] = arena;

                    SetupPlayer(ntf, RoleType.NtfCaptain);
                    SetupPlayer(ci, RoleType.ChaosMarauder);
                }
                catch (System.Exception e)
                {
                    Log.Error(e);
                }
            }

        }

        internal static void SetupPlayer(Player player, RoleType roleType)
        {
            Timing.CallDelayed(0.25f, () =>
            {
                if (IsLobby)
                    player.Role.Type = roleType;
            });

            Timing.CallDelayed(0.3f, () =>
            {
                if (IsLobby)
                    player.ClearInventory();
            });

            Timing.CallDelayed(0.5f, () =>
            {
                if (IsLobby)
                    player.AddItem(ItemType.ArmorHeavy);
            });
        }

        private static readonly ArenaConfig Config = WaitAndChillReborn.Singleton.Config.ArenaConfig;
    }
}
