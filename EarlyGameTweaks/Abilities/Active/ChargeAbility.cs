using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using InventorySystem.Items.Firearms.Modules;
using MEC;
using PlayerStatsSystem;
using UnityEngine;

namespace EarlyGameTweaks.Abilities.Active
{
    [CustomAbility]
    public class ChargeAbility : ActiveAbility
    {
        public override string Name { get; set; } = "Charge";

        public override string Description { get; set; } = "Charges towards the target location.";

        public override float Duration { get; set; } = 0f;

        public override float Cooldown { get; set; } = 45f;

        [Description("The amount of damage inflicted when the player collides with something.")]
        public float ContactDamage { get; set; } = 15f;

        [Description("The bonus multiplier if the target player wasn't moving.")]
        public float AccuracyMultiplier { get; set; } = 2f;

        [Description("How long the ensnare effect lasts.")]
        public float EnsnareDuration { get; set; } = 5f;

        protected override void AbilityUsed(Player player)
        {
            if (!RunRaycast(player, out RaycastHit hit))
                return;

            Log.Debug($"{player.Nickname} -- {player.Position} - {hit.point}");

            if (!IsValidChargeTarget(hit, out RaycastHit lineHit))
            {
                NotifyInvalidCharge(player);
                return;
            }

            Log.Debug($"{player.Nickname} -- {lineHit.point}");
            Timing.RunCoroutine(MovePlayer(player, hit));
        }

        private bool RunRaycast(Player player, out RaycastHit hit)
        {
            Vector3 forward = player.CameraTransform.forward;
            return Physics.Raycast(player.Position + forward, forward, out hit, 200f, HitscanHitregModuleBase.HitregMask);
        }

        private bool IsValidChargeTarget(RaycastHit hit, out RaycastHit lineHit)
        {
            return Physics.Linecast(hit.point, hit.point + (Vector3.down * 5f), out lineHit);
        }

        private void NotifyInvalidCharge(Player player)
        {
            player.ShowHint("You cannot charge straight up walls, silly.\nYour cooldown has been lowered to 5sec.");
            Timing.CallDelayed(0.5f, () => LastUsed[player] = DateTime.Now + TimeSpan.FromSeconds(5));
        }

        private IEnumerator<float> MovePlayer(Player player, RaycastHit hit)
        {
            while (!HasReachedTarget(player.Position, hit.point))
            {
                player.Position = Vector3.MoveTowards(player.Position, hit.point, 0.5f);
                yield return Timing.WaitForSeconds(0.00025f);
            }

            ApplyEffects(player, hit);
            EndAbility(player);
        }

        private bool HasReachedTarget(Vector3 currentPosition, Vector3 targetPosition)
        {
            return (currentPosition - targetPosition).sqrMagnitude < 2.5f;
        }

        private void ApplyEffects(Player player, RaycastHit hit)
        {
            Timing.CallDelayed(0.5f, () => player.EnableEffect(EffectType.Ensnared, EnsnareDuration));

            Player target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());
            if (target != null)
            {
                HandleTargetCollision(player, target, hit.point);
            }
            else
            {
                InflictSelfDamage(player);
            }
        }

        private void HandleTargetCollision(Player player, Player target, Vector3 hitPoint)
        {
            if ((target.Position - hitPoint).sqrMagnitude >= 3f)
            {
                target.Hurt(new ScpDamageHandler(player.ReferenceHub, ContactDamage * AccuracyMultiplier, DeathTranslations.Zombie));
                target.EnableEffect(EffectType.Ensnared, EnsnareDuration);
            }
            else
            {
                InflictSelfDamage(player);
            }
        }

        private void InflictSelfDamage(Player player)
        {
            player.Hurt(new UniversalDamageHandler(ContactDamage, DeathTranslations.Falldown));
        }
    }
}
