using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using UnityEngine;
using Random = System.Random;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class ClusterGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 8f;
        public override uint Id { get; set; } = 1115;
        public override string Name { get; set; } = "Cluster Granate";
        public override string Description { get; set; } = "Eine erweiterte Granate die nach der initialen Explosion weitere Granaten losfeuert.";
        public override float Weight { get; set; } = 1.75f;
        public float ClusterGrenadeFuseTime { get; set; } = 1.5f;
        public int ClusterGrenadeCount { get; set; } = 4;
        public bool ClusterGrenadeRandomSpread { get; set; } = true;
        public override SpawnProperties SpawnProperties { get; set; }

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
            Timing.CallDelayed(0.1f, () =>
            {
                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                grenade.FuseTime = 0.25f;
                grenade.ScpDamageMultiplier = 0.5f;
                grenade.ChangeItemOwner(null, ev.Player);
                grenade.SpawnActive(ev.Position, ev.Player);
                grenade.FuseTime = ClusterGrenadeFuseTime;
                grenade.ScpDamageMultiplier = 0.5f;

                for (int i = 0; i <= ClusterGrenadeCount; i++)
                {
                    grenade.ChangeItemOwner(null, ev.Player);
                    if (ClusterGrenadeRandomSpread)
                    {
                        grenade.SpawnActive(GrenadeOffset(ev.Position), owner: ev.Player);
                    }
                    else
                    {
                        grenade.SpawnActive(ev.Position, owner: ev.Player);
                    }
                }
            });
        }

        private Vector3 GrenadeOffset(Vector3 position)
        {
            Random random = new Random();
            float x = position.x - 1 + ((float)random.NextDouble() * random.Next(0, 3));
            float y = position.y;
            float z = position.z - 1 + ((float)random.NextDouble() * random.Next(0, 3));
            return new Vector3(x, y, z);
        }
    }
}
