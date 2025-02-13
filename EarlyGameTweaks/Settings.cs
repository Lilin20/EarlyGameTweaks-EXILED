using System.Collections.Generic;
using System.Text;
using EarlyGameTweaks.Roles.ChaosInsurgency;
using EarlyGameTweaks.Roles.ClassD;
using EarlyGameTweaks.Roles.NTF;
using EarlyGameTweaks.Roles.Scientist;
using Exiled.CustomRoles.API.Features;
using NorthwoodLib.Pools;
using UserSettings.ServerSpecific;

namespace EarlyGameTweaks
{
    public class SettingHandlers
    {
        public static ServerSpecificSettingBase[] LilinsAIOMenu() => SettingsHelper.GetSettings();
    }

    public class SettingsHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = [];
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();


            var customRoles = new List<CustomRole>
            {
                Breacher.Get(typeof(Breacher)),
                Dwarf.Get(typeof(Dwarf)),
                Lockpicker.Get(typeof(Lockpicker)),
                DeltaAgent.Get(typeof(DeltaAgent)),
                Pacifist.Get(typeof(Pacifist)),
            };

            stringBuilder.AppendLine("Custom Roles - Ausklappen für mehr Infos");

            foreach (var role in customRoles)
            {
                if (role == null || role.CustomAbilities == null) continue;

                stringBuilder.AppendLine($"Role: {role.Name}");
                stringBuilder.AppendLine($"- Description: {role.Description}");
                foreach (var ability in role.CustomAbilities)
                {
                    stringBuilder.AppendLine($"-- Ability: {ability.Name}, {ability.Description}");
                    stringBuilder.AppendLine("-------------------------------");
                }
            }


            settings.Add(new SSGroupHeader("Lilin's AIO - Rollen Info"));
            settings.Add(new SSTextArea(null, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                    SSTextArea.FoldoutMode.CollapsedByDefault));
            stringBuilder.Clear();

            settings.Add(new SSGroupHeader("Lilin's AIO - Ability Keybinds"));
            settings.Add(new SSKeybindSetting(10003, "Lockpicking [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10004, "Zone Blackout [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10005, "Charge [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10006, "Berserker's Fury [Ability]", UnityEngine.KeyCode.B, true, "B"));

            return [.. settings];
        }
    }
}
