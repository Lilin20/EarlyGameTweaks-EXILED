using System.Collections.Generic;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace EarlyGameTweaks
{
    public class CustomRoleEventHandler
    {
        private readonly EarlyGameTweaks Plugin;
        public CustomRoleEventHandler(EarlyGameTweaks plugin) => Plugin = plugin;

        public void OnRoundStarted()
        {
            var roleEnumerators = new Dictionary<StartTeam, List<ICustomRole>.Enumerator>();

            Log.Debug($"Found Roles: {Plugin.Roles}");
            foreach (var kvp in Plugin.Roles)
            {
                Log.Debug($"Setting enumerator for {kvp.Key} - {kvp.Value.Count}");
                roleEnumerators[kvp.Key] = kvp.Value.GetEnumerator();
            }

            foreach (var player in Player.List)
            {
                Log.Debug($"Trying to give {player.Nickname} a role | {player.Role.Type}");
                CustomRole? role = player.Role.Type switch
                {
                    RoleTypeId.FacilityGuard => CustomRoleMethods.GetCustomRole(ref roleEnumerators.GetValueOrDefault(StartTeam.Guard)),
                    RoleTypeId.Scientist => CustomRoleMethods.GetCustomRole(ref roleEnumerators.GetValueOrDefault(StartTeam.Scientist)),
                    RoleTypeId.ClassD => CustomRoleMethods.GetCustomRole(ref roleEnumerators.GetValueOrDefault(StartTeam.ClassD)),
                    _ when player.Role.Side == Side.Scp => CustomRoleMethods.GetCustomRole(ref roleEnumerators.GetValueOrDefault(StartTeam.Scp)),
                    _ => null
                };

                role?.AddRole(player);
            }

            foreach (var enumerator in roleEnumerators.Values)
            {
                enumerator.Dispose();
            }
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.Players.Count == 0)
            {
                Log.Warn(
                    $"{nameof(OnRespawningTeam)}: The respawn list is empty ?!? -- {ev.NextKnownTeam} / {ev.MaximumRespawnAmount}");

                foreach (var player in Player.Get(RoleTypeId.Spectator))
                    ev.Players.Add(player);
                ev.MaximumRespawnAmount = ev.Players.Count;
            }

            using var roles = ev.NextKnownTeam switch
            {
                (Faction)SpawnableFaction.ChaosWave or (Faction)SpawnableFaction.ChaosMiniWave
                    when Plugin.Roles.TryGetValue(StartTeam.Chaos, out var chaosRoles) => chaosRoles.GetEnumerator(),
                (Faction)SpawnableFaction.NtfWave or (Faction)SpawnableFaction.NtfMiniWave
                    when Plugin.Roles.TryGetValue(StartTeam.Ntf, out var ntfRoles) => ntfRoles.GetEnumerator(),
                _ => new List<ICustomRole>.Enumerator()
            };

            foreach (var player in ev.Players)
            {
                var role = CustomRoleMethods.GetCustomRole(ref roles);
                role?.AddRole(player);
            }
        }

        public void FinishingRecall(FinishingRecallEventArgs ev)
        {
            Log.Debug("VVUP Custom Roles: FinishingRecall: Selecting random zombie role.");
            if (Plugin.Roles.TryGetValue(StartTeam.Scp, out var scpRoles) && ev.Target != null)
            {
                Log.Debug($"VVUP Custom Roles: FinishingRecall: List count {scpRoles.Count}");
                using var enumerator = scpRoles.GetEnumerator();
                var customRole = CustomRoleMethods.GetCustomRole(ref enumerator, false, true);
                Log.Debug($"VVUP Custom Roles: Got custom role {customRole?.Name}");

                if (customRole != null && customRole.TrackedPlayers.Count < customRole.SpawnProperties.Limit)
                {
                    if (Extensions.GetCustomRoles(ev.Target).Count == 0)
                    {
                        customRole.AddRole(ev.Target);
                    }
                }
                else
                {
                    Log.Debug($"VVUP Custom Roles: Role {customRole?.Name} has reached its spawn limit. Not Spawning");
                }
            }
        }
    }
}
