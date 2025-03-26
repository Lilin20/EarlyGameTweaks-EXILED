using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace EarlyGameTweaks.Abilities.Passive
{
    public class RestrictedItems : PassiveAbility
    {
        public override string Name { get; set; } = "Restricted Items";
        public override string Description { get; set; } = "Handles restricted items";

        public List<ItemType> RestrictedItemList { get; set; } = new List<ItemType>();
        public List<Player> PlayersWithRestrictedItemsEffect { get; set; } = new List<Player>();
        public bool RestrictUsingItems { get; set; } = true;
        public bool RestrictPickingUpItems { get; set; } = true;
        public bool RestrictDroppingItems { get; set; } = false;

        protected override void AbilityAdded(Player player)
        {
            PlayersWithRestrictedItemsEffect.Add(player);
            Exiled.Events.Handlers.Player.UsingItem += OnItemEvent;
            Exiled.Events.Handlers.Player.PickingUpItem += OnItemEvent;
            Exiled.Events.Handlers.Player.DroppingItem += OnItemEvent;
        }

        protected override void AbilityRemoved(Player player)
        {
            PlayersWithRestrictedItemsEffect.Remove(player);
            Exiled.Events.Handlers.Player.UsingItem -= OnItemEvent;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnItemEvent;
            Exiled.Events.Handlers.Player.DroppingItem -= OnItemEvent;
            player.DisableAllEffects();
        }

        private void OnItemEvent(object sender, System.EventArgs e)
        {
            if (e is UsingItemEventArgs usingItemEvent)
            {
                if (RestrictUsingItems && ShouldRestrict(usingItemEvent.Player, usingItemEvent.Item.Type))
                {
                    usingItemEvent.IsAllowed = false;
                }
            }
            else if (e is PickingUpItemEventArgs pickingUpItemEvent)
            {
                if (RestrictPickingUpItems && ShouldRestrict(pickingUpItemEvent.Player, pickingUpItemEvent.Pickup.Type))
                {
                    pickingUpItemEvent.IsAllowed = false;
                }
            }
            else if (e is DroppingItemEventArgs droppingItemEvent)
            {
                if (RestrictDroppingItems && ShouldRestrict(droppingItemEvent.Player, droppingItemEvent.Item.Type))
                {
                    droppingItemEvent.IsAllowed = false;
                }
            }
        }

        private bool ShouldRestrict(Player player, ItemType itemType)
        {
            return PlayersWithRestrictedItemsEffect.Contains(player) && RestrictedItemList != null && RestrictedItemList.Contains(itemType);
        }
    }
}
