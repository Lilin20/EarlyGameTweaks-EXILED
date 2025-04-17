using System.Collections.Generic;
using EarlyGameTweaks.API;
using EarlyGameTweaks.Roles.ChaosInsurgency;
using EarlyGameTweaks.Roles.ClassD;
using EarlyGameTweaks.Roles.NTF;
using EarlyGameTweaks.Roles.Scientist;
using EarlyGameTweaks.Roles.SCP;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using HarmonyLib;
using MEC;
using UserSettings.ServerSpecific;

namespace EarlyGameTweaks
{
    public class EarlyGameTweaks : Plugin<Config>
    {
        public static EarlyGameTweaks Instance;
        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public EventHandlers EventHandlers;
        public Harmony harmony = new Harmony("test.patch.gigachad");

        public Lockpicker tc = new Lockpicker();
        public Dwarf dc = new Dwarf();
        public DeltaAgent da = new DeltaAgent();
        public Breacher bc = new Breacher();
        public Pacifist pc = new Pacifist();
        public ChaosMedic cm = new ChaosMedic();
        public NtfMedic ntfm = new NtfMedic();
        public LuckyMan lm = new LuckyMan();
        public Sniper sc = new Sniper();
        public Berserker berserkerc = new Berserker();
        public SpecialAgentGuard sagc = new SpecialAgentGuard();
        public ZombieJuggernaut zjc = new ZombieJuggernaut();
        public MedicZombie zmc = new MedicZombie();
        public Thief thiefc = new Thief();
        public DwarfZombie dwz = new DwarfZombie();

        public List<ExplosionGrenadeProjectile> GrenadeProjectiles { get; set; } = new List<ExplosionGrenadeProjectile>();
        public List<ExplosionGrenadeProjectile> AlertGrenadeProjectiles { get; set; } = new List<ExplosionGrenadeProjectile>();

        public Dictionary<StartTeam, List<ICustomRole>> Roles { get; } = new();
        public CustomRoleEventHandler CustomRoleEventHandler;

        public override void OnEnabled()
        {
            EventHandlers = new EventHandlers();
            Instance = this;

            Exiled.Events.Handlers.Player.Shot += EventHandlers.OnShot;

            Exiled.Events.Handlers.Player.Verified += EventHandlers.OnVerify;
            Exiled.Events.Handlers.Player.PickingUpItem += EventHandlers.OnPickupArmor;

            Exiled.Events.Handlers.Scp096.AddingTarget += EventHandlers.OnRageStart;
            Exiled.Events.Handlers.Player.Hurting += EventHandlers.OnSCPVoid;

            //Exiled.Events.Handlers.Scp1344.ChangedStatus += EventHandlers.OnWearingGlasses;

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += EventHandlers.OnSettingValueReceived;

            CustomWeapon.RegisterItems();

            CustomRoleEventHandler = new CustomRoleEventHandler(this);
            AudioClipStorage.LoadClip("C:\\Users\\Administrator\\AppData\\Roaming\\EXILED\\Audio\\test.ogg", "test");
            AudioClipStorage.LoadClip("C:\\Users\\Administrator\\AppData\\Roaming\\EXILED\\Audio\\alarm.ogg", "alarmSound");
            AudioClipStorage.LoadClip("C:\\Users\\Administrator\\AppData\\Roaming\\EXILED\\Audio\\berserker2.ogg", "berserker2");

            tc.Register();
            dc.Register();
            da.Register();
            bc.Register();
            pc.Register();
            cm.Register();
            ntfm.Register();
            lm.Register();
            sc.Register();
            berserkerc.Register();
            sagc.Register();
            zjc.Register();
            zmc.Register();
            thiefc.Register();
            dwz.Register();

            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role is ICustomRole custom)
                {
                    Log.Debug($"Adding {role.Name} to dictionary...");
                    StartTeam team;
                    if (custom.StartTeam.HasFlag(StartTeam.Chaos))
                        team = StartTeam.Chaos;
                    else if (custom.StartTeam.HasFlag(StartTeam.Guard))
                        team = StartTeam.Guard;
                    else if (custom.StartTeam.HasFlag(StartTeam.Ntf))
                        team = StartTeam.Ntf;
                    else if (custom.StartTeam.HasFlag(StartTeam.Scientist))
                        team = StartTeam.Scientist;
                    else if (custom.StartTeam.HasFlag(StartTeam.ClassD))
                        team = StartTeam.ClassD;
                    else if (custom.StartTeam.HasFlag(StartTeam.Scp))
                        team = StartTeam.Scp;
                    else
                        team = StartTeam.Other;

                    if (!Roles.ContainsKey(team))
                        Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        Roles[team].Add(custom);
                    Log.Debug($"Roles {team} now has {Roles[team].Count} elements.");
                }
            }

            Exiled.Events.Handlers.Server.RoundStarted += CustomRoleEventHandler.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStartSendHint;
            Exiled.Events.Handlers.Server.RespawningTeam += CustomRoleEventHandler.OnRespawningTeam;
            Exiled.Events.Handlers.Map.Generated += EventHandlers.OnMapGeneration;
            Exiled.Events.Handlers.Scp049.FinishingRecall += CustomRoleEventHandler.FinishingRecall;
            //Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStartHintSetup;

            CustomAbility.RegisterAbilities();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Shot -= EventHandlers.OnShot;
            Exiled.Events.Handlers.Player.Verified -= EventHandlers.OnVerify;
            Exiled.Events.Handlers.Player.PickingUpItem -= EventHandlers.OnPickupArmor;

            Exiled.Events.Handlers.Scp096.AddingTarget -= EventHandlers.OnRageStart;
            Exiled.Events.Handlers.Player.Hurting -= EventHandlers.OnSCPVoid;

            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= EventHandlers.OnSettingValueReceived;

            CustomWeapon.UnregisterItems();

            tc.Unregister();
            dc.Unregister();
            da.Unregister();
            bc.Unregister();
            pc.Unregister();
            cm.Unregister();
            ntfm.Unregister();
            lm.Unregister();
            sc.Unregister();
            berserkerc.Unregister();
            sagc.Unregister();
            zjc.Unregister();
            zmc.Unregister();
            thiefc.Unregister();
            dwz.Unregister();

            //harmony.UnpatchAll();
            CustomAbility.UnregisterAbilities();

            Exiled.Events.Handlers.Server.RoundStarted -= CustomRoleEventHandler.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStartSendHint;
            Exiled.Events.Handlers.Server.RespawningTeam -= CustomRoleEventHandler.OnRespawningTeam;
            Exiled.Events.Handlers.Map.Generated -= EventHandlers.OnMapGeneration;
            Exiled.Events.Handlers.Scp049.FinishingRecall -= CustomRoleEventHandler.FinishingRecall;
            //Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStartHintSetup;

            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
