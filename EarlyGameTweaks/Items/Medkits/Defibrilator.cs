using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using UnityEngine;
using UnityEngine.Rendering;
using PlayerHandler = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Medkit)]
    public class Defibrilator : CustomItem
    {
        public override uint Id { get; set; } = 107;
        public override string Name { get; set; } = "Defibrilator";
        public override string Description { get; set; } = "Belebt einen Spieler wieder (macht mit dieser Information was Ihr wollt).";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 75,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 75,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideLczCafe,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            PlayerHandler.UsingItemCompleted += OnUsingDefi;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerHandler.UsingItemCompleted -= OnUsingDefi;

            base.UnsubscribeEvents();
        }

        private void OnUsingDefi(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit,
                       20, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;


            if (raycastHit.collider is null)
                return;

            Log.Info(raycastHit.collider);

            if (raycastHit.collider.gameObject.GetComponentInParent<BasicRagdoll>() is BasicRagdoll ragdoll)
            {
                Player aPlayer = Player.Get(ragdoll.Info.OwnerHub);

                // Find exiled ragdoll with BasicRagdoll
                Ragdoll exRagdoll = Ragdoll.Get(ragdoll);

                if (aPlayer.Role == RoleTypeId.Spectator)
                {
                    aPlayer.Role.Set(exRagdoll.Role, RoleSpawnFlags.None);
                    aPlayer.Teleport(exRagdoll.Position + Vector3.up);
                    exRagdoll.Destroy();
                }
            }
        }
    }
}