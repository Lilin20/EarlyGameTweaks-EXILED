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
    public class DefiChaos : CustomItem
    {
        public override uint Id { get; set; } = 13552;
        public override string Name { get; set; } = "Vitalisator";
        public override string Description { get; set; } = "Ein speziell für Chaos-Insurgency angefertigter Defibrilator.";
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            PlayerHandler.UsingItemCompleted += OnUsingDefiChaos;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerHandler.UsingItemCompleted -= OnUsingDefiChaos;

            base.UnsubscribeEvents();
        }

        private void OnUsingDefiChaos(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.Player.Role.Team != Team.ChaosInsurgency)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint("Du bist nicht autorisiert diesen Vitalisator zu benutzen.", 5);
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