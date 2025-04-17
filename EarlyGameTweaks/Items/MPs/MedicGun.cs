using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;
using Light = Exiled.API.Features.Toys.Light;
using MEC;
using Exiled.CustomRoles.API;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunCrossvec)]
    public class MedicGun : CustomWeapon
    {
        public override uint Id { get; set; } = 400;
        public override float Damage { get; set; } = 0;
        public override string Name { get; set; } = "MS9K - MedShot 9000";
        public override string Description { get; set; } = "Ein Werkzeug welches Menschen heilt.";
        public override byte ClipSize { get; set; } = 60;
        public override float Weight { get; set; } = 0.5f;
        

        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            Player.Shot += OnMedicShot;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.Shot -= OnMedicShot;
            base.UnsubscribeEvents();
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (!CustomRole.Get(60).Check(ev.Player) && !CustomRole.Get(61).Check(ev.Player))
            {
                ev.IsAllowed = false;
                ev.Player.Hurt(1);

                Light light = Light.Create(Vector3.zero, Vector3.zero, Vector3.one, true, new Color(0.7f, 0.8f, 1));
                light.Intensity = 50;
                light.Range = 50;
                light.ShadowStrength = 0;
                light.ShadowType = LightShadows.None;

                light.Base.transform.parent = ev.Pickup.Transform;
                light.Position = ev.Pickup.Position;

                ev.Player.ShowHint("Ein Sicherheitsmechanismus greift ein und gibt dir einen Stromschlag.");
                Timing.CallDelayed(0.05f, () =>
                {
                    light.Destroy();
                });
            }
        }

        public void OnMedicShot(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem)) return;
            if (ev.Target is null) return;

            ev.CanHurt = false;

            if (ev.Target.Role.Team == PlayerRoles.Team.ClassD
                | ev.Target.Role.Team == PlayerRoles.Team.Scientists
                | ev.Target.Role.Team == PlayerRoles.Team.FoundationForces
                | ev.Target.Role.Team == PlayerRoles.Team.ChaosInsurgency)
            {
                ev.Target.Heal(5);
            }
            else if (ev.Target.Role == RoleTypeId.Scp0492)
            {
                ev.Target.HumeShield += 20;

                if (ev.Target.HumeShield >= 300)
                {
                    ev.Target.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                }
            }
            else
            {
                return;
            }
        }
    }
}