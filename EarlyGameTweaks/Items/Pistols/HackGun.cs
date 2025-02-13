using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunCOM15)]
    public class HackGun : CustomWeapon
    {
        public override uint Id { get; set; } = 555;
        public override string Name { get; set; } = "Lilin's Toolgun";
        public override string Description { get; set; } = "Eine Toolgun welches offene Türen sperrt und geschlossene Türen öffnet. Kann 3 mal benutzt werden.";
        public override float Damage { get; set; } = 1;
        public override byte ClipSize { get; set; } = 3;
        public override float Weight { get; set; } = 0.1f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
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
                }
            }
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            try
            {
                if (Check(ev.Player.CurrentItem))
                {
                    if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit,
                       20, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                        return;

                    if (raycastHit.collider is null)
                        return;

                    //if (!Exiled.API.Features.Camera.TryGet(raycastHit.collider.gameObject.GetComponentInParent<Scp079Camera>(), out Exiled.API.Features.Camera hit))
                    //    return;

                    DoorVariant dv = raycastHit.collider.gameObject.GetComponentInParent<DoorVariant>();
                    if (dv is null)
                    {
                        Scp079Generator generator = raycastHit.collider.gameObject?.GetComponentInParent<Scp079Generator>();
                        if (generator != null)
                        {
                            Cassie.MessageTranslated("GENERATOR DAMAGE DETECTED . REPAIRING GENERATOR", "Generator Malfunction", false, true, true);
                            Map.TurnOffAllLights(10f);
                        }
                    }

                    Door door = Door.Get(dv);

                    if (door.IsLocked)
                    {
                        door.Unlock();
                    }
                    else
                    {
                        door.Lock(5, DoorLockType.Regular079);
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
