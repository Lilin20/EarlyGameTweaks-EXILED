using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PluginAPI.Events;
using UnityEngine;

namespace EarlyGameTweaks.Roles.NTF
{
    public class SpecialAgentGuard : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 303;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Facility Guard - Special Agent";
        public override string Description { get; set; } = "Ein ausgebildeter Guard mit einem experimentellen Cloaking-Device.";
        public override string CustomInfo { get; set; } = "Facility Guard - Special Agent";
        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;
        public int Chance { get; set; } = 25;
        private CoroutineHandle _coroutineHandle;
        public override bool DisplayCustomItemMessages { get; set; } = false;
        public StartTeam StartTeam { get; set; } = StartTeam.Guard;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new() { Chance = 100, Location = SpawnLocationType.InsideHczArmory }
            },
        };

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new EffectEnabler
            {
                Name = "Effect Enabler [Passive]",
                Description = "Gibt diverse Effekte.",
                EffectsToApply = new Dictionary<EffectType, byte>
                {
                    { EffectType.SilentWalk, 8 }
                }
            }
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardGuard.ToString(),
            ItemType.GunCrossvec.ToString(),
            ItemType.GunCOM15.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.ArmorLight.ToString(),
            ItemType.Ammo9x19.ToString(),
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawn;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawn;
            base.UnsubscribeEvents();
        }

        private void OnSpawn(SpawnedEventArgs ev)
        {
            if (!Check(ev.Player)) return;

            Timing.CallDelayed(1f, () =>
            {
                _coroutineHandle = Timing.RunCoroutine(SpecialAgentCoroutine(ev.Player));
            });
        }

        private IEnumerator<float> SpecialAgentCoroutine(Player player)
        {
            while (true)
            {
                if (player.Role == RoleTypeId.Spectator)
                {
                    Timing.KillCoroutines(_coroutineHandle);
                    yield break;
                }

                Vector3 oldPos = player.Position;
                yield return Timing.WaitForSeconds(1f);

                if (player.Role is FpcRole)
                {
                    if (oldPos == player.Position)
                        player.EnableEffect(EffectType.Invisible);
                    else
                        player.DisableEffect(EffectType.Invisible);
                }
            }
        }
    }
}
