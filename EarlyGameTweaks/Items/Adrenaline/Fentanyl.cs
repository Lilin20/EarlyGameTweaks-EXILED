using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class Fentanyl : CustomItem
    {
        public override uint Id { get; set; } = 100;
        public override string Name { get; set; } = "Fentanyl";
        public override string Description { get; set; } = "Absturz";
        public override float Weight { get; set; } = 0.5f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 25,
            LockerSpawnPoints = GetLockerSpawnPoints()
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingInjection;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingInjection;
            base.UnsubscribeEvents();
        }

        private void OnUsingInjection(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            float random = UnityEngine.Random.value;
            if (random <= 0.33f)
            {
                ApplyZombieTransformation(ev.Player);
            }
            else if (random <= 0.66f)
            {
                ev.Player.Vaporize();
            }
            else
            {
                ApplyNegativeEffects(ev.Player);
            }
        }

        private static List<LockerSpawnPoint> GetLockerSpawnPoints() => new()
        {
            new() { Chance = 30, Zone = ZoneType.LightContainment, UseChamber = false, Type = LockerType.Misc },
            new() { Chance = 30, Zone = ZoneType.HeavyContainment, UseChamber = false, Type = LockerType.Misc },
            new() { Chance = 30, Zone = ZoneType.Entrance, UseChamber = false, Type = LockerType.Misc },
            new() { Chance = 100, Zone = ZoneType.Surface, UseChamber = false, Type = LockerType.Misc }
        };

        private static void ApplyZombieTransformation(Player player)
        {
            player.Role.Set(PlayerRoles.RoleTypeId.Scp0492, PlayerRoles.RoleSpawnFlags.None);
            player.MaxHealth = 100;
            player.Health = 100;
            player.EnableEffect(EffectType.Slowness, 200);
            player.EnableEffect(EffectType.Concussed, 60);
        }

        private static void ApplyNegativeEffects(Player player)
        {
            player.EnableEffect(EffectType.Slowness, 200);
            player.EnableEffect(EffectType.DamageReduction, 150);
            player.EnableEffect(EffectType.AmnesiaItems, 100);
            player.EnableEffect(EffectType.Blinded, 90);
        }
    }
}
