﻿using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunShotgun)]
    public class BreachShotgun : CustomWeapon
    {
        public override uint Id { get; set; } = 600;
        public override string Name { get; set; } = "Kerberos-12";
        public override string Description { get; set; } = "Ermöglicht es dir Türen zu zerstören. Teilt keinen Schaden an Spieler aus.";
        public override float Damage { get; set; } = 0;
        public override byte ClipSize { get; set; } = 14;
        public override float Weight { get; set; } = 1.5f;

        public override SpawnProperties SpawnProperties { get; set; }
        public override AttachmentName[] Attachments { get; set; } = new[]
        {
            AttachmentName.ShotgunSingleShot,
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        protected override void OnShot(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            ev.CanHurt = false;
            try
            {
                if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit,
                    20, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                    return;

                if (raycastHit.collider is null)
                    return;

                DoorVariant dv = raycastHit.collider.gameObject.GetComponentInParent<DoorVariant>();
                if (dv is null)
                {
                    return;
                }

                var d = Door.Get(raycastHit.collider.gameObject.GetComponentInParent<DoorVariant>());

                d.As<BreakableDoor>().Break();
            }
            catch
            {
                return;
            }
        }
    }
}
