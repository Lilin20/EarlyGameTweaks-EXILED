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
    public class TetherGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public override uint Id { get; set; } = 202;
        public override string Name { get; set; } = "Tether Granate";
        public override string Description { get; set; } = "Explodiert beim aufprall. Hält Gegner im Wirkungsbereich fest.";
        public override float Weight { get; set; } = 1.75f;
        public List<ExplosionGrenadeProjectile> GrenadeProjectiles { get; set; } = new List<ExplosionGrenadeProjectile>();
        public List<Exiled.API.Features.Player> PlayersWithGodMode = new List<Exiled.API.Features.Player>();
        public float maxDistance = 4f;
        public CoroutineHandle _coroutine;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside049Armory,
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
            var impactPos = ev.Position;
            ev.Projectile.Destroy();

            foreach (Exiled.API.Features.Player player in ev.TargetsToAffect)
            {
                PlayersWithGodMode.Add(player);
                player.IsGodModeEnabled = true;
                player.EnableEffect(EffectType.Ensnared, 5f);
            }
            Timing.CallDelayed(0.25f, () =>
            {
                foreach (Exiled.API.Features.Player player in PlayersWithGodMode)
                {
                    player.IsGodModeEnabled = false;
                }
            });
        }
    }
}
