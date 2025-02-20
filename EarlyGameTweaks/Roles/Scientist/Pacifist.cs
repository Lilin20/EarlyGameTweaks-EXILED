using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.Scientist
{
    public class Pacifist : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 400;
        public override int MaxHealth { get; set; } = 150;
        public override string Name { get; set; } = "Scientist - Pazifist";
        public override string Description { get; set; } = "Ein simpler Wissenschaftler. Du kannst keine Waffen aufheben.";
        public override string CustomInfo { get; set; } = "Scientist - Pazifist";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public int Chance { get; set; } = 25;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        //public Player pacifistPlayer = null;
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new RestrictedItems()
            {
                Name = "Restricted Items [Passive]",
                Description = "Handles restricted items",
                RestrictedItemList =
                {
                    ItemType.GunA7,
                    ItemType.GunAK,
                    ItemType.GunCOM15,
                    ItemType.GunCOM18,
                    ItemType.GunCom45,
                    ItemType.GunCrossvec,
                    ItemType.GunE11SR,
                    ItemType.GunFRMG0,
                    ItemType.GunFSP9,
                    ItemType.GunLogicer,
                    ItemType.GunRevolver,
                    ItemType.GunRevolver,
                    ItemType.GrenadeFlash,
                    ItemType.GrenadeHE,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor,
                    ItemType.Jailbird,
                },
                RestrictUsingItems = false,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            }
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints =
            [
                new()
                {
                    Role = RoleTypeId.Scientist,
                }
            ]
        };

        protected override void SubscribeEvents()
        {
            //Exiled.Events.Handlers.Player.Dying += OnDeath;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            //Exiled.Events.Handlers.Player.Dying -= OnDeath;
            base.UnsubscribeEvents();
        }

        //public void OnDeath(DyingEventArgs ev)
        //{
        //    if (ev.Player == null)
        //    {
        //        return;
        //    }
        //    if (pacifistPlayer == ev.Player)
        //    {
        //        pacifistPlayer = null;
        //        int limit = 500;
        //        for (int i = 0; i < limit; i++)
        //        {
        //            Scp018 grenade = (Scp018)Item.Create(ItemType.SCP018);
        //            grenade.SpawnActive(ev.Player.Position);
        //        }
        //    }
        //}

        //protected override void RoleAdded(Player player)
        //{
        //pacifistPlayer = player;
        //    base.RoleAdded(player);
        //}

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Lantern.ToString(),
            ItemType.KeycardScientist.ToString(),
        };
    }
}
