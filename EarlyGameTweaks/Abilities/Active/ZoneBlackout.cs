using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;

namespace EarlyGameTweaks.Abilities.Active
{
    [CustomAbility]
    public class ZoneBlackout : ActiveAbility
    {
        private const string CassieMessage = "ZETA 9 UNIT . INITIATED OVERCHARGE";
        private const string CassieTranslation = "Zeta 9 Unit initiated an overcharge.";
        private const float LightOffDuration = 180f;
        private const float HintDuration = 5f;
        private const float CassieDelayBuffer = 4f;

        public override float Duration { get; set; } = 4;
        public override float Cooldown { get; set; } = 240;
        public string UsingAbilityMessage { get; set; } = "Initiating overload.";
        public override string Name { get; set; } = "Zone Blackout [Active]";
        public override string Description { get; set; } = "Stört die Lichter in der Zone in der du dich gerade befindest.";
        private readonly List<Player> playersWithBlackoutAbility = new();

        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(UsingAbilityMessage, HintDuration);
            playersWithBlackoutAbility.Add(player);

            ZoneType playerZone = player.Zone;
            float cassieDuration = Cassie.CalculateDuration(CassieMessage, false, 1);

            Cassie.MessageTranslated(CassieMessage, CassieTranslation, false, true, true);
            Timing.CallDelayed(cassieDuration + CassieDelayBuffer, () =>
            {
                Map.TurnOffAllLights(LightOffDuration, playerZone);
            });
        }

        protected override void UnsubscribeEvents()
        {
            // No events to unsubscribe currently
            base.UnsubscribeEvents();
        }
    }
}
