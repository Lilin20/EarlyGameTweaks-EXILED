using System.Collections.Generic;
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
    public class ProximityAlert : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 900f;
        public override uint Id { get; set; } = 2112;
        public override string Name { get; set; } = "Proximity Alarm";
        public override string Description { get; set; } = "Gibt ein Signalton aus wenn sich ein Spieler im Umkreis von 10 Meter nähert.";
        public override float Weight { get; set; } = 1.75f;
        public int maxDuration { get; set; } = 180;
        public List<ExplosionGrenadeProjectile> GrenadeProjectiles { get; set; } = new List<ExplosionGrenadeProjectile>();
        public List<Exiled.API.Features.Player> PlayersWithGodMode = new List<Exiled.API.Features.Player>();
        public float maxDistance = 10f;
        public CoroutineHandle _coroutine;
        public override SpawnProperties SpawnProperties { get; set; }

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
                        EarlyGameTweaks.Instance.AlertGrenadeProjectiles.Add(grenade);
                        Log.Info(EarlyGameTweaks.Instance.AlertGrenadeProjectiles);
                        Rigidbody rb = grenade.Rigidbody;
                        grenade.MaxRadius = 0.1f;
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                        _coroutine = Timing.RunCoroutine(ProximityAlertCoroutine());
                    });
                }
            }
        }

        public IEnumerator<float> ProximityAlertCoroutine()
        {
            Log.Info("Running coroutine...");
            int currentDuration = 0;

            while (EarlyGameTweaks.Instance.AlertGrenadeProjectiles.Count > 0)
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (Exiled.API.Features.Player singlePlayer in Exiled.API.Features.Player.List)
                {
                    foreach (ExplosionGrenadeProjectile singleGrenade in EarlyGameTweaks.Instance.AlertGrenadeProjectiles)
                    {
                        float distance = Vector3.Distance(singlePlayer.Position, singleGrenade.Position);
                        if (currentDuration >= maxDuration)
                        {
                            singleGrenade.Explode();
                        }

                        if (distance <= maxDistance)
                        {
                            continue;
                        }
                    }
                }
                currentDuration++;
            }
            Timing.KillCoroutines(_coroutine);
            yield break;
        }
    }
}
