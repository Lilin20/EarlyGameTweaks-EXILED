using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunLogicer)]
    public class GrenadeLauncher : CustomWeapon
    {
        public override uint Id { get; set; } = 303;
        public override float Damage { get; set; } = 0.1f;
        public override string Name { get; set; } = "Grenade Launcher";
        public override string Description { get; set; } = "...";
        public override byte ClipSize { get; set; } = 3;
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            LockerSpawnPoints = new List<LockerSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Zone = ZoneType.LightContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 25,
                    Zone = ZoneType.HeavyContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
            }
        };

        protected override void SubscribeEvents()
        {
            Player.Shot += OnShotDD;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.Shot -= OnShotDD;
            base.UnsubscribeEvents();
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            ev.IsAllowed = false;
            base.OnPickingUp(ev);
        }

        private void OnShotDD(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            ev.CanHurt = false;

            Vector3 spawnPos = ev.Player.CameraTransform.position + ev.Player.CameraTransform.forward * 1.0f;
            Quaternion rotation = Quaternion.identity;

            Pickup grenade = Pickup.CreateAndSpawn(ItemType.GrenadeHE, spawnPos, rotation);

            if (grenade.Rigidbody is Rigidbody rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                Vector3 launchDirection = ev.Player.CameraTransform.forward + Vector3.up * 0.2f;
                launchDirection.Normalize();

                float launchForce = 20f;
                rb.velocity = launchDirection * launchForce;
            }
            else
            {
                Log.Warn("not a rigidbody (sad emoji).");
            }

            Timing.CallDelayed(5f, () =>
            {
                grenade.As<GrenadePickup>().FuseTime = 1f;
                grenade.As<GrenadePickup>().Explode();
            });
        }
    }
}