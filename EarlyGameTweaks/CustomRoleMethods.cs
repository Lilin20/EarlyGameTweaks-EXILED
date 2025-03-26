using System;
using System.Collections.Generic;
using EarlyGameTweaks.API;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;

namespace EarlyGameTweaks
{
    public class CustomRoleMethods
    {
        private readonly EarlyGameTweaks Plugin;

        public CustomRoleMethods(EarlyGameTweaks plugin) => Plugin = plugin;

        public static CustomRole? GetCustomRole(ref List<ICustomRole>.Enumerator enumerator, bool checkEscape = false,
            bool checkRevive = false)
        {
            try
            {
                Log.Debug("Getting role from enumerator..");

                while (enumerator.MoveNext())
                {
                    var currentRole = enumerator.Current;
                    if (currentRole is null)
                        continue;

                    Log.Debug(currentRole.StartTeam);

                    int randomChance = Loader.Random.Next(100);
                    bool isInvalidRole = currentRole.StartTeam.HasFlag(StartTeam.Other)
                        || (currentRole.StartTeam.HasFlag(StartTeam.Revived) && !checkRevive)
                        || (currentRole.StartTeam.HasFlag(StartTeam.Escape) && !checkEscape)
                        || (!currentRole.StartTeam.HasFlag(StartTeam.Revived) && checkRevive)
                        || (!currentRole.StartTeam.HasFlag(StartTeam.Escape) && checkEscape)
                        || randomChance > currentRole.Chance;

                    if (isInvalidRole)
                    {
                        Log.Debug(
                            $"Validation check failed | {currentRole.StartTeam} {currentRole.Chance}% || {randomChance}");
                        continue;
                    }

                    Log.Debug("Returning a role!");
                    return (CustomRole)currentRole;
                }

                Log.Debug("Cannot move next");
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred in GetCustomRole: {ex.Message}");
                return null;
            }
        }
    }
}
