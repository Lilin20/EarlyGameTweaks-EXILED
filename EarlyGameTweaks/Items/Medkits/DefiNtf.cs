using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using UnityEngine;
using PlayerHandler = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Medkit)]
    public class DefiNtf : CustomItem
    {
        public override uint Id { get; set; } = 109;
        public override string Name { get; set; } = "Trauma Paket";
        public override string Description { get; set; } = "Ein speziell für NTF angefertigter Defibrilator.";
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            PlayerHandler.UsingItemCompleted += OnUsingDefiNtf;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerHandler.UsingItemCompleted -= OnUsingDefiNtf;

            base.UnsubscribeEvents();
        }

        private void OnUsingDefiNtf(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.Player.Role.Team != Team.FoundationForces)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint("Du bist nicht autorisiert dieses Trauma Paket zu benutzen.", 5);
                return;
            }

            if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit,
                       20, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;

            if (raycastHit.collider is null)
                return;


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

            //Log.Debug(ragdoll);
        }
    }
}