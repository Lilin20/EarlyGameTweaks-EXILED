using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Abilities.Passive
{
    public class ScaleAbility : PassiveAbility
    {
        public override string Name { get; set; } = "Scale Ability";
        public override string Description { get; set; } = "Handles everything in regards to custom roles with scaling";

        private readonly List<Player> playersWithScaleAbility = new();
        public Vector3 ScaleForPlayers { get; set; } = new Vector3(1f, 1f, 1f);

        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"Adding Scale Ability to {player.Nickname}");
            playersWithScaleAbility.Add(player);
            ApplyScaleAndEffects(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"Removing Scale Ability from {player.Nickname}");
            playersWithScaleAbility.Remove(player);
            ResetPlayerScaleAndEffects(player);
        }

        private void ApplyScaleAndEffects(Player player)
        {
            Timing.CallDelayed(1.5f, () =>
            {
                player.Scale = ScaleForPlayers;
                player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 10);
            });
        }

        private void ResetPlayerScaleAndEffects(Player player)
        {
            player.Scale = Vector3.one;
            player.DisableAllEffects();
        }
    }
}
