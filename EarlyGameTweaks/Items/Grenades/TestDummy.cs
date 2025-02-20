using System.Collections.Generic;
using AdminToys;
using EarlyGameTweaks.Abilities;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class TestDummy : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 10f;
        public override uint Id { get; set; } = 8888;
        public override string Name { get; set; } = "TG 5 - Signalgranate";
        public override string Description { get; set; } = "Ein Signalerzeuger. Fordert aus Site-[REDACTED] einen Hume Breaker an.";
        public override float Weight { get; set; } = 1.75f;
        public AirdropManager am = new AirdropManager();
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 100,
                    Location = SpawnLocationType.Inside173Gate,
                }
            },
        };
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;

            ZoneType zone = ev.Projectile.Room.Zone;

            if (zone == ZoneType.Surface)
            {
                ev.Player.ShowHint("Der Hume Breaker wird in kürze geliefert! Das kann einen Moment dauern...");
                Timing.CallDelayed(30, () =>
                {
                    am.HumeDrop();
                });
            }
            else
            {
                ev.Player.ShowHint("Das Signal der Granate konnte nicht empfangen werden.");
            }
        }
    }
}
