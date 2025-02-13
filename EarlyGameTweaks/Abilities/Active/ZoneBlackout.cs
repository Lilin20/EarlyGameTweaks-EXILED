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
        public override float Duration { get; set; } = 4;
        public override float Cooldown { get; set; } = 240;
        public string UsingAbility { get; set; } = "Initiating overload.";
        public override string Name { get; set; } = "Zone Blackoud [Active]";
        public override string Description { get; set; } = "Stört die Lichter in der Zone in der du dich gerade befindest.";
        public List<Player> PlayersWithBlackoutAbility = new List<Player>();

        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(UsingAbility, 5f);
            PlayersWithBlackoutAbility.Add(player);

            ZoneType playerZone = player.Zone;
            float cassieTime = Cassie.CalculateDuration("ZETA 9 UNIT . INITIATED OVERCHARGE", false, 1);
            Cassie.MessageTranslated("ZETA 9 UNIT . INITIATED OVERCHARGE", "Zeta 9 Unit initiated an overcharge.", false, true, true);
            Timing.CallDelayed((cassieTime + 4), () =>
            {
                Map.TurnOffAllLights(180, playerZone);
            });

            //Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        }

        protected override void UnsubscribeEvents()
        {
            //Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            base.UnsubscribeEvents();
        }
    }
}
