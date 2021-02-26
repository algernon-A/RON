using ICities;
using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class RONMod : IUserMod
    {
        public static string ModName => "RON - the network replacer";
        public static string Version => "0.2.1";

        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("RON_DESC");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Load the settings file.
            SettingsUtils.LoadSettings();
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
        }
    }
}
