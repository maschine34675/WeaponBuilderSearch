using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using WeaponBuilderSearch.Patches;

namespace WeaponBuilderSearch
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.maschine.WeaponBuilderSearch";
        public const string PluginName = "maschine-WeaponBuilderSearch";
        public const string PluginVersion = "1.0.0";

        public static ManualLogSource Log;
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> MinItemsForSearch;

        private void Awake()
        {
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true,
                "Enable attachment search in the weapon builder dropdown.");
            MinItemsForSearch = Config.Bind("General", "MinItemsForSearch", 6,
                "Show the search field only when at least this many attachments are listed.");

            if (Enabled.Value)
            {
                new DropDownMenuShowPatch().Enable();
                new DropDownMenuClosePatch().Enable();
            }

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }
    }
}
