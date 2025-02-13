using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using PlayerRoles.Subroutines;
using PluginAPI.Events;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Jailbird)]
    public class BerserkerJB : CustomItem
    {
        public override uint Id { get; set; } = 2117;
        public override string Name { get; set; } = "Test JB";
        public override string Description { get; set; } = "WIP";
        public override float Weight { get; set; } = 1.5f;

        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Item.ChargingJailbird += OnJBCharge;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Item.ChargingJailbird -= OnJBCharge;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnsubscribeEvents();
        }

        public void OnJBCharge(ChargingJailbirdEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            ev.IsAllowed = false;
            ev.Jailbird.ChargeDamage = 25;
            ev.Jailbird.FlashDuration = 0.1f;
            ev.Jailbird.ConcussionDuration = 0.1f;
            ev.Jailbird.WearState = InventorySystem.Items.Jailbird.JailbirdWearState.Healthy;
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 40, 2f, false);
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is null) return;
            if (!Check(ev.Attacker.CurrentItem))
                return;
            if (ev.Player is null) return;
            if (ev.DamageHandler is null) return;

            ev.DamageHandler.Damage = 25;
            ev.Attacker.CurrentItem.As<Jailbird>().WearState = InventorySystem.Items.Jailbird.JailbirdWearState.Healthy;
            ev.Attacker.Heal(20);
        }
    }
}
