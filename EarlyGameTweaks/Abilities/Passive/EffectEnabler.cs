using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;

namespace EarlyGameTweaks.Abilities.Passive
{
    public class EffectEnabler : PassiveAbility
    {
        public override string Name { get; set; } = "Effect Enabler";
        public override string Description { get; set; } = "Enables Effects to the player";

        public Dictionary<EffectType, byte> EffectsToApply { get; set; } = new();

        protected override void AbilityAdded(Player player)
        {
            Timing.CallDelayed(5f, () => ApplyEffects(player));
        }

        protected override void AbilityRemoved(Player player)
        {
            RemoveEffects(player);
        }

        private void ApplyEffects(Player player)
        {
            foreach (var effect in EffectsToApply)
            {
                Log.Debug($"VVUP Custom Abilities: Activating {effect.Key} for {player.Nickname}");
                player.EnableEffect(effect.Key, effect.Value, 0);
            }
        }

        private void RemoveEffects(Player player)
        {
            foreach (var effect in EffectsToApply)
            {
                Log.Debug($"VVUP Custom Abilities: Removing {effect.Key} from {player.Nickname}");
                player.DisableEffect(effect.Key);
            }
        }
    }
}
