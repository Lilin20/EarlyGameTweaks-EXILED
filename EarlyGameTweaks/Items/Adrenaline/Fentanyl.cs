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
            LockerSpawnPoints = new List<LockerSpawnPoint>
            {
                new()
                {
                    Chance = 30,
                    Zone = ZoneType.LightContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 30,
                    Zone = ZoneType.HeavyContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 30,
                    Zone = ZoneType.Entrance,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 100,
                    Zone = ZoneType.Surface,
                    UseChamber = false,
                    Type = LockerType.Misc,
                }
            }
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
                ev.Player.Role.Set(PlayerRoles.RoleTypeId.Scp0492, PlayerRoles.RoleSpawnFlags.None);
                ev.Player.MaxHealth = 50;
                ev.Player.Health = 50;
                ev.Player.EnableEffect(EffectType.Slowness, 60);
                ev.Player.EnableEffect(EffectType.Concussed, 60);
            }
            else if (random <= 0.66f)
            {
                ev.Player.Vaporize();
            }
            else
            {
                ev.Player.EnableEffect(EffectType.Slowness, 200);
                ev.Player.EnableEffect(EffectType.DamageReduction, 100);
                ev.Player.EnableEffect(EffectType.AmnesiaItems, 100);
                ev.Player.EnableEffect(EffectType.Blinded, 90);
            }
        }
    }
}
