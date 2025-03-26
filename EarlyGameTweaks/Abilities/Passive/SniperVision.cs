using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Abilities.Passive
{
    public class SniperVision : PassiveAbility
    {
        public override string Name { get; set; } = "Sniper Vision";
        public override string Description { get; set; } = "Sorgt dafür das du weiter gucken kannst.";

        private readonly List<Player> playersWithSVisionAbility = new();
        public Vector3 ScaleForPlayers { get; set; } = new(1f, 1f, 1f);

        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"Adding Sniper Vision to {player.Nickname}");
            playersWithSVisionAbility.Add(player);

            Timing.CallDelayed(1.5f, () => 
                player.EnableEffect(Exiled.API.Enums.EffectType.FogControl, 1));
        }

        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"Removing Sniper Vision from {player.Nickname}");
            playersWithSVisionAbility.Remove(player);
            player.DisableAllEffects();
        }
    }
}
