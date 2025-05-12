using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.API.Structs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.ArmorCombat)]
    public class AntiGravity : CustomArmor
    {
        public override uint Id { get; set; } = 902;
        public override string Name { get; set; } = "AntiGRAV";
        public override string Description { get; set; } = "idk man noch in arbeit";
        public override float Weight { get; set; } = 0.1f;
        public override float StaminaUseMultiplier { get; set; } = 1f;
        public override int HelmetEfficacy { get; set; } = 0;
        public override int VestEfficacy { get; set; } = 0;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside049Armory,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Player.ItemAdded += OnEquipping;
            Player.DroppingItem += OnRemoving;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.ItemAdded -= OnEquipping;
            Player.DroppingItem -= OnRemoving;
            base.UnsubscribeEvents();
        }

        public void OnEquipping(ItemAddedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Player.Role.Type is RoleTypeId.Scp3114)
                return;

            if (ev.Player.Role is FpcRole fpc)
            {
                //Default gravity = -19.6
                Vector3 gravity = new Vector3(0, 2f, 0);
                fpc.Gravity = gravity;
                ev.Player.Position += new Vector3(0, 0.25f, 0);
                ev.Player.Scale = new Vector3(-1f, -1f, 1f);
            }
        }

        public void OnRemoving(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Player.Role is FpcRole fpc)
            {
                //Default gravity = -19.6
                Vector3 gravity = new Vector3(0, -19.6f, 0);
                fpc.Gravity = gravity;
                ev.Player.Scale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
