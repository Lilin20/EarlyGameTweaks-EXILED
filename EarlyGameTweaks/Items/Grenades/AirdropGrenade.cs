using System.Collections.Generic;
using EarlyGameTweaks.Abilities;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class AirdropGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 5f;
        public override uint Id { get; set; } = 204;
        public override string Name { get; set; } = "AS - Signalgranate";
        public override string Description { get; set; } = "Ein Signalerzeuger. Fordert Nachschub von Site-[REDACTED] an.";
        public override float Weight { get; set; } = 1.75f;
        public AirdropManager am = new AirdropManager();
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideIntercom,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside173Armory,
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
                Map.Broadcast(message: "FREQUENCY: [REDACTED] - Airdrop inbound.", duration: 10);

                Timing.CallDelayed(15, () =>
                {
                    am.DropToPlayer();
                });
            }
            else
            {
                ev.Player.ShowHint("Das Signal der Granate konnte nicht empfangen werden.");
            }
        }
    }
}
