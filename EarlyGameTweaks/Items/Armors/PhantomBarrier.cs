using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.ArmorCombat)]
    public class PhantomBarrier : CustomArmor
    {
        public override uint Id { get; set; } = 668;
        public override string Name { get; set; } = "Phantom Barrier Suit";
        public override string Description { get; set; } = "Dieser Anzug bietet dir die möglichkeit durch Türen zu gehen.";
        public override float Weight { get; set; } = 1.5f;
        public override float StaminaUseMultiplier { get; set; } = 2f;
        public override int HelmetEfficacy { get; set; } = 0;
        public override int VestEfficacy { get; set; } = 0;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.InsideLczArmory,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Player.ItemAdded += OnEquipping;
            Player.DroppingItem += OnRemoving;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.ItemAdded -= OnEquipping;
            Player.DroppingItem -= OnRemoving;
            base.UnsubscribeEvents();
        }

        public void OnEquipping(ItemAddedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.Player.EnableEffect(EffectType.Ghostly);
            ev.Player.EnableEffect(EffectType.FogControl, 0);
            ev.Player.EnableEffect(EffectType.Slowness, 15);
            ev.Player.MaxHealth = 75;
            ev.Player.Health = 75;
        }

        public void OnRemoving(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.Player.DisableEffect(EffectType.Ghostly);
            ev.Player.DisableEffect(EffectType.FogControl);
            ev.Player.DisableEffect(EffectType.Slowness);
            ev.Player.MaxHealth = 100;
        }
    }
}
