using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.ArmorHeavy)]
    public class JuggernautArmor : CustomArmor
    {
        public override uint Id { get; set; } = 667;
        public override string Name { get; set; } = "SCP-645643";
        public override string Description { get; set; } = "Eine Panzerung aus biologischen Bestandteilen. Kann nicht mehr abgelegt werden und frisst andere Panzerungen beim versuch diese anzuziehen.";
        public override float Weight { get; set; } = 1.5f;
        public override float StaminaUseMultiplier { get; set; } = 1.75f;
        public override int VestEfficacy { get; set; } = 200;
        public override int HelmetEfficacy { get; set; } = 200;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHidChamber,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Player.ItemAdded += OnEquipping;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.ItemAdded -= OnEquipping;
            base.UnsubscribeEvents();
        }

        public void OnEquipping(ItemAddedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.Player.EnableEffect(EffectType.Slowness, 25);
            ev.Player.EnableEffect(EffectType.DamageReduction, 80);
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
