using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class CustomJailbird : CustomItem
    {
        public override uint Id { get; set; } = 103;
        public override string Name { get; set; } = "LMD - Neurolink Stimulator";
        public override string Description { get; set; } = "Stimuliert ein Gerät in einem Hirn. Der Stimulator sendet ein Signal zu einem bestimmten Neurolink aus. Aber für welches?";
        public override float Weight { get; set; } = 0.5f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            LockerSpawnPoints = new List<LockerSpawnPoint>
            {
                new()
                {
                    Chance = 11,
                    Zone = ZoneType.LightContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                }
            }
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingInjection;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingInjection;

            base.UnsubscribeEvents();
        }

        private void OnUsingInjection(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Info("Trying to find player for kaboom...");
            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                Log.Info(player.UserId);
                if (player.UserId == "76561199065461828@steam")
                {
                    int limit = 50;
                    for (int i = 0; i < limit; i++)
                    {
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                        grenade.FuseTime = 2f;
                        grenade.MaxRadius = 0.5f;
                        grenade.SpawnActive(player.Position);
                        grenade.ConcussDuration = 5f;
                        grenade.BurnDuration = 5f;
                    }
                }
            }
        }
    }
}
