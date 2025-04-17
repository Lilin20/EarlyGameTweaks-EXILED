using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class TestGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = true;
        public override float FuseTime { get; set; } = 5f;
        public override uint Id { get; set; } = 203;
        public override string Name { get; set; } = "Test Granate";
        public override string Description { get; set; } = "Experimentelles Doohickey";
        public override float Weight { get; set; } = 1.75f;
        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (ev.Projectile is ExplosionGrenadeProjectile grenade)
            {
                grenade.MaxRadius = 20;
            }
        }
    }
}
