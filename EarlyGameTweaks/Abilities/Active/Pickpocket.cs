using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Abilities.Active
{
    [CustomAbility]
    public class Pickpocket : ActiveAbility
    {
        public override string Name { get; set; } = "Pickpocket Ability";
        public override string Description { get; set; } = "Raubt einen Spieler aus.";
        public override float Duration { get; set; } = 1f;
        public override float Cooldown { get; set; } = 15f;

        protected override void AbilityUsed(Player player)
        {
            if (!Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit raycastHit,
                       3, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)) || raycastHit.collider is null)
                return;

            Player robbedPlayer = Player.Get(raycastHit.transform.GetComponentInParent<ReferenceHub>());
            if (robbedPlayer == null || robbedPlayer.Role.Team == PlayerRoles.Team.SCPs)
                return;

            if (robbedPlayer.Items.Count == 0)
            {
                player.ShowHint("Dieser Spieler hat keine Items im Inventar.");
                return;
            }

            try
            {
                Item randomItem = robbedPlayer.Items.ToList().RandomItem();
                player.AddItem(randomItem.Type);
                robbedPlayer.RemoveItem(randomItem);
                player.ShowHint("Du hast etwas geklaut...");
            }
            catch (Exception e)
            {
                Log.Error($"Error during Pickpocket ability: {e}");
            }
        }

        protected override void AbilityRemoved(Player player)
        {
            
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
    }
}
