using ICities;
using ColossalFramework.UI;
using CitiesHarmony.API;


namespace RON
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class RONMod : IUserMod
    {
        public static string ModName => "RON - the network replacer";
        public static string Version => "0.6.2";

        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("RON_DESC");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());

            // Load the settings file.
            SettingsUtils.LoadSettings();
        }


        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // Language drop down.
            UIDropDown languageDropDown = (UIDropDown)helper.AddDropdown(Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index, (index) => { Translations.Index = index; SettingsUtils.SaveSettings(); });
            languageDropDown.autoSize = false;
            languageDropDown.width = 270f;

            // Hotkey control.
            languageDropDown.parent.parent.gameObject.AddComponent<OptionsKeymapping>();

            // Enable advanced mode.
            helper.AddCheckbox(Translations.Translate("RON_OPT_ADV"), ModSettings.enableAdvanced, (value) => { ModSettings.enableAdvanced = value; SettingsUtils.SaveSettings(); });

            // Auto-replace NExt2 roads.
            helper.AddCheckbox(Translations.Translate("RON_OPT_NEX"), ModSettings.replaceNExt2, (value) => { ModSettings.replaceNExt2 = value; SettingsUtils.SaveSettings(); });
        }
    }
}
