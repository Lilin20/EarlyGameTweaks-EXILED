using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace EarlyGameTweaks.Roles.ClassD
{
    public class LuckyMan : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 202;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "SCP-181 - Lucky Man";
        public override string Description { get; set; } = "Du öffnest für die ersten 3 Minuten alle Türen ohne Karte und du kannst Kugeln/Schläge ausweichen.";
        public override string CustomInfo { get; set; } = "SCP-181 - Lucky Man";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override List<CustomAbility> CustomAbilities { get; set; }
        public List<EffectType> GoodEffects { get; set; } = new List<EffectType> { EffectType.MovementBoost, EffectType.Vitality, EffectType.Invigorated };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints =
            [
                new()
                {
                    Role = RoleTypeId.ClassD,
                }
            ]
        };
        public bool _canOpenWithoutPerms = true;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurt;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor += OnDoorOpening;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTeslaTrigger;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurt;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnDoorOpening;
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTeslaTrigger;
            base.UnsubscribeEvents();
        }

        public void OnHurt(HurtingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            if (ev.Player == null)
                return;

            float random = UnityEngine.Random.value;
            if (random >= 0.4f)
            {
                ev.DamageHandler.Damage = 0;
                System.Random randomEffectIndex = new System.Random();

                EffectType randomEffect = GoodEffects[randomEffectIndex.Next(GoodEffects.Count)];
                ev.Player.EnableEffect(randomEffect, 20, 2f);
            }
        }

        public void OnTeslaTrigger(TriggeringTeslaEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.IsAllowed = false;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            _canOpenWithoutPerms = true;

            Timing.CallDelayed(180f, () =>
            {
                _canOpenWithoutPerms = false;
                ev.Player.ShowHint("Du kannst keine versperrten Türen mehr ohne Karte öffnen.");
            });
        }

        public void OnDoorOpening(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            if (_canOpenWithoutPerms && ev.Door.IsKeycardDoor)
            {
                ev.Door.IsOpen = true;
            }
        }

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Lantern.ToString(),
            ItemType.Coin.ToString(),
        };
    }
}
