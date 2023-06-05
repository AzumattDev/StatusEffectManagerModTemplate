using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LocalizationManager;
using StatusEffectManager;
using ServerSync;

namespace StatusEffectManagerModTemplate
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class StatusEffectManagerModTemplatePlugin : BaseUnityPlugin
    {
        internal const string ModName = "StatusEffectManagerModTemplate";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "{azumatt}";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource StatusEffectManagerModTemplateLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public void Awake()
        {
            // Uncomment the line below to use the LocalizationManager for localizing your mod.
            //Localizer.Load(); // Use this to initialize the LocalizationManager (for more information on LocalizationManager, see the LocalizationManager documentation https://github.com/blaxxun-boop/LocalizationManager#example-project).

            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            CustomSE mycooleffect = new("Toxicity");
            mycooleffect.Name.English("Toxicity");
            mycooleffect.Type = EffectType.Consume;
            mycooleffect.IconSprite = null;
            mycooleffect.Name.German("Toxizität"); 
            mycooleffect.Effect.m_startMessageType = MessageHud.MessageType.TopLeft;
            mycooleffect.Effect.m_startMessage = "My Cool Status Effect Started"; 
            mycooleffect.Effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            mycooleffect.Effect.m_stopMessage = "Not cool anymore, ending effect."; 
            mycooleffect.Effect.m_tooltip = "<color=orange>Toxic damage over time</color>"; 
            mycooleffect.AddSEToPrefab(mycooleffect, "SwordIron");
            
            CustomSE drunkeffect = new("se_drunk", "se_drunk_effect");
			drunkeffect.Name.English("Drunk"); // You can use this to fix the display name in code
			drunkeffect.Icon = "DrunkIcon.png"; // Use this to add an icon (64x64) for the status effect. Put your icon in an "icons" folder
			drunkeffect.Name.German("Betrunken"); // Or add translations for other languages
			drunkeffect.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			drunkeffect.Effect.m_startMessage = "I'm drunk!"; // What the start message says
			drunkeffect.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			drunkeffect.Effect.m_stopMessage = "Sober...again."; // What the stop message says
			drunkeffect.Effect.m_tooltip = "<color=red>Your vision is blurry</color>"; // Tooltip that will describe the effect applied to the player
			drunkeffect.AddSEToPrefab(drunkeffect, "TankardAnniversary"); // Adds the status effect to the Anniversary Tankard. Applies when equipped.
			
			// Create a new status effect in code and apply it to a prefab.
			CustomSE codeSE = new("CodeStatusEffect");
			codeSE.Name.English("New Effect");
			codeSE.Type = EffectType.Consume; // Set the type of status effect this should be.
			codeSE.Icon = "ModDevPower.png";
			codeSE.Name.German("Betrunken"); // Or add translations for other languages
			codeSE.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			codeSE.Effect.m_startMessage = "Mod Dev power, granted."; // What the start message says
			codeSE.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			codeSE.Effect.m_stopMessage = "Mod Dev power, removed."; // What the stop message says
			codeSE.Effect.m_tooltip = "<color=green>You now have Mod Dev POWER!</color>"; // Tooltip that will describe the effect applied to the player
			codeSE.AddSEToPrefab(codeSE, "SwordCheat"); // Adds the status effect to the Cheat Sword. Applies when equipped.
		


            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                StatusEffectManagerModTemplateLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                StatusEffectManagerModTemplateLogger.LogError($"There was an issue loading your {ConfigFileName}");
                StatusEffectManagerModTemplateLogger.LogError(
                    "Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<bool>? _serverConfigLocked;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }

        #endregion
    }
}