using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;

namespace EarlyGameTweaks.Abilities.Passive
{
    public class Fake : PassiveAbility
    {
        public override string Name { get; set; } = "Faking Ability";

        public override string Description { get; set; } = "test";

        public List<Player> PlayersWithFakeAbility = new List<Player>();

        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: FakeHuman, Adding Fake Ability to {player.Nickname}");
            PlayersWithFakeAbility.Add(player);
            Timing.CallDelayed(1.0f, () =>
            {
                player.ChangeAppearance(PlayerRoles.RoleTypeId.Scientist, false);
                Room room = Room.Get(Exiled.API.Enums.RoomType.LczPlants);
                player.Teleport(room);
                player.EnableEffect(EffectType.Slowness, 15);
            });
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Dwarf, Removing Dwarf Ability from {player.Nickname}");
            PlayersWithFakeAbility.Remove(player);
            player.DisableAllEffects();
        }
    }
}
