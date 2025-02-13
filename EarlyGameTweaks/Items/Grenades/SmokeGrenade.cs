using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class SmokeGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public override uint Id { get; set; } = 2119;
        public override string Name { get; set; } = "Rauchgranate";
        public override string Description { get; set; } = "Vernebelt die Umgebung.";
        public override float Weight { get; set; } = 1.15f;
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override SpawnProperties SpawnProperties { get; set; }
        public bool RemoveSmoke { get; set; } = true;
        public float FogTime { get; set; } = 30;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            Vector3 savedGrenadePosition = ev.Position;
            Scp244 scp244 = (Scp244)Item.Create(ItemType.SCP244a);
            Pickup pickup = null;
            scp244.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            scp244.Primed = true;
            scp244.MaxDiameter = 0.0f;
            pickup = scp244.CreatePickup(savedGrenadePosition);
            if (RemoveSmoke)
            {
                Timing.CallDelayed(FogTime, () =>
                {
                    pickup.Position += Vector3.down * 10;

                    Timing.CallDelayed(30, () =>
                    {
                        pickup.Destroy();
                    });
                });
            }
        }
    }
}
