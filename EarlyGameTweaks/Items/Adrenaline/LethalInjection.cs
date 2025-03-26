using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerStatsSystem;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class LethalInjection : CustomItem
    {
        public override uint Id { get; set; } = 101;
        public override string Name { get; set; } = "Gesichtsfressende Chemikalie";
        public override string Description { get; set; } = "Wird direkt in den Kopf injeziert. Entstellt das Gesicht des Anwenders und bringt SCP-096 in einen ruhigen Zustand. Der Anwender stirbt.";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 100,
                    Location = SpawnLocationType.Inside096,
                },
            },
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

            var scp096 = Exiled.API.Features.Player.List
                .FirstOrDefault(player => player.Role == RoleTypeId.Scp096)?
                .Role as Exiled.API.Features.Roles.Scp096Role;

            if (scp096 == null || !scp096.HasTarget(ev.Player))
                return;

            scp096.Calm();
            ev.Player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Poisoned));
        }
    }
}
