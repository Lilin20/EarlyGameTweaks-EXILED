using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Utf8Json.Resolvers.Internal;

namespace EarlyGameTweaks.Items.Unknown
{
    [CustomItem(ItemType.KeycardO5)]
    public class KeycardHB : CustomKeycard
    {
        public override uint Id { get; set; } = 1000;
        public override string Name { get; set; } = "O5-### '[REDACTED]' Access Pass";
        public override string Description { get; set; } = "Ermöglicht Zugang zu [REDACTED]. Schwer beschädigt.";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 100,
                    Location = SpawnLocationType.Inside049Armory,
                }
            },
        };

        public override KeycardPermissions Permissions { get; set; } = KeycardPermissions.None;

        protected override void OnInteractingDoor(Player player, Door door)
        {
            if (door.Type == DoorType.Scp173Gate)
            {
                door.Unlock();
                door.IsOpen = true;
                player.CurrentItem.Destroy();
            }
        }
    }
}
