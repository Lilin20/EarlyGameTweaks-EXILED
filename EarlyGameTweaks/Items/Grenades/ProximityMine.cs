using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class ProximityMine : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 900f;
        public override uint Id { get; set; } = 2111;
        public override string Name { get; set; } = "Annäherungs Mine";
        public override string Description { get; set; } = "Explodiert nachdem sich ein Spieler in einen 8 Meter Radius nähert";
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
                    Chance = 30,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 30,
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
            Exiled.Events.Handlers.Player.ThrownProjectile += OnThrow;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ThrownProjectile -= OnThrow;
            base.UnsubscribeEvents();
        }

        public void OnThrow(ThrownProjectileEventArgs ev)
        {
            if (Check(ev.Throwable))
            {
                if (ev.Projectile is ExplosionGrenadeProjectile grenade)
                {
                    Timing.CallDelayed(0.75f, () =>
                    {
                        EarlyGameTweaks.Instance.GrenadeProjectiles.Add(grenade);
                        Log.Info(GrenadeProjectiles);
                        Rigidbody rb = grenade.Rigidbody;
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                        _coroutine = Timing.RunCoroutine(ProximityMineCoroutine());
                    });
                }
            }
        }

        public IEnumerator<float> ProximityMineCoroutine()
        {
            Log.Info("Running coroutine...");
            while (EarlyGameTweaks.Instance.GrenadeProjectiles.Count > 0)
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (Exiled.API.Features.Player singlePlayer in Exiled.API.Features.Player.List)
                {
                    foreach (ExplosionGrenadeProjectile singleGrenade in EarlyGameTweaks.Instance.GrenadeProjectiles)
                    {
                        float distance = Vector3.Distance(singlePlayer.Position, singleGrenade.Position);
                        Log.Info(distance);
                        if (distance <= maxDistance)
                        {
                            singleGrenade.Explode();
                            EarlyGameTweaks.Instance.GrenadeProjectiles.Remove(singleGrenade);
                        }
                    }
                }
            }
            Timing.KillCoroutines(_coroutine);
            yield break;
        }
    }
}
