using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Toys;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace EarlyGameTweaks.Abilities.Active
{
    [CustomAbility]
    public class BombSuicide : ActiveAbility
    {
        public override string Name { get; set; } = "Bloat Buster";
        public override string Description { get; set; } = "Du blähst dich auf und jagst dich in die Luft.";
        public override float Duration { get; set; } = 15f;
        public override float Cooldown { get; set; } = 45f;
        public string UsingAbility { get; set; } = "Charging Bloat Buster...";
        public List<Player> PlayersWithBloatBusterAbility = new List<Player>();

        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(UsingAbility, 5f);
            PlayersWithBloatBusterAbility.Add(player);


        }


    }
}
