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

        public override string Description { get; set; } =
            "Sorgt dafür das du weiter gucken kannst.";

        public List<Player> PlayersWithSVisionAbility = new List<Player>();
        public Vector3 ScaleForPlayers { get; set; } = new Vector3(1f, 1f, 1f);

        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Sniper Vision, Adding Sniper Vision to {player.Nickname}");
            PlayersWithSVisionAbility.Add(player);
            Timing.CallDelayed(1.5f, () =>
            {
                player.EnableEffect(Exiled.API.Enums.EffectType.FogControl, 1);

            });
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Sniper Vision, Removing Sniper Vision from {player.Nickname}");
            PlayersWithSVisionAbility.Remove(player);
            player.DisableAllEffects();
        }
    }
}
