using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;

namespace EarlyGameTweaks.Abilities.Active
{
    [CustomAbility]
    public class HealingMist : ActiveAbility
    {
        private readonly List<CoroutineHandle> coroutines = new();

        public override string Name { get; set; } = "Healing Mist";
        public override string Description { get; set; } =
            "Activates a short-term spray of chemicals which will heal and protect allies for a short duration.";
        public override float Duration { get; set; } = 15f;
        public override float Cooldown { get; set; } = 180f;

        [Description("The amount healed every second the ability is active.")]
        public float HealAmount { get; set; } = 40;

        [Description("The amount of AHP given when the ability ends.")]
        public ushort ProtectionAmount { get; set; } = 75;

        private const float MaxDistanceSquared = 144f;
        private const float HealInterval = 0.75f;

        public string UsingAbilityMessage { get; set; } = "Oozing out some healing chemicals...";

        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(UsingAbilityMessage, 5f);
            StartHealingMist(player);
        }

        protected override void UnsubscribeEvents()
        {
            coroutines.ForEach(Timing.KillCoroutines);
            base.UnsubscribeEvents();
        }

        private void StartHealingMist(Player activator)
        {
            var allies = Player.List.Where(p => p.Role.Side == activator.Role.Side && p != activator);
            foreach (var ally in allies)
            {
                coroutines.Add(Timing.RunCoroutine(ApplyHealingMist(activator, ally)));
            }
        }

        private IEnumerator<float> ApplyHealingMist(Player activator, Player ally)
        {
            for (int i = 0; i < Duration; i++)
            {
                if (ShouldSkipHealing(activator, ally))
                    continue;

                ally.Health = Math.Min(ally.Health + HealAmount, ally.MaxHealth);
                yield return Timing.WaitForSeconds(HealInterval);
            }

            if (IsWithinRange(activator, ally))
                ally.ArtificialHealth += ProtectionAmount;
        }

        private bool ShouldSkipHealing(Player activator, Player ally)
        {
            return ally.Health >= ally.MaxHealth || !IsWithinRange(activator, ally);
        }

        private bool IsWithinRange(Player activator, Player ally)
        {
            return (activator.Position - ally.Position).sqrMagnitude <= MaxDistanceSquared;
        }
    }
}
