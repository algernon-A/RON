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
        public static string Version => "0.7.0";

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

            // Add the options panel event handler for the start screen (to enable/disable options panel based on visibility).
            // First, check to see if UIView is ready.
            if (UIView.GetAView() != null)
            {
                // It's ready - attach the hook now.
                OptionsPanelManager.OptionsEventHook();
            }
            else
            {
                // Otherwise, queue the hook for when the intro's finished loading.
                LoadingManager.instance.m_introLoaded += OptionsPanelManager.OptionsEventHook;
            }

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
            // Create options panel.
            OptionsPanelManager.Setup(helper);

            /*
            UIComponent panel = helper as UIComponent;
            //if (helper is UIHelper uiHelper && uiHelper.self is UIComponent panel)
            {
                // Y position indicator.
                float currentY = Margin;
                int tabbingIndex = 0;

                //panel.autoLayout = false;

                UIDropDown languageDropDown = UIControls.AddPlainDropDown(panel, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
                languageDropDown.eventSelectedIndexChanged += (control, index) =>
                {
                    Translations.Index = index;
                    SettingsUtils.SaveSettings();
                };
                languageDropDown.relativePosition = new Vector2(LeftMargin, currentY);
                languageDropDown.tabIndex = ++tabbingIndex;
                currentY += languageDropDown.height + Margin;

                // Hotkey control.
                OptionsKeymapping keyMapping = languageDropDown.parent.parent.gameObject.AddComponent<OptionsKeymapping>();

                UICheckBox advancedCheck = UIControls.AddPlainCheckBox(panel, Translations.Translate("RON_OPT_ADV"));
                advancedCheck.relativePosition = new Vector2(LeftMargin, currentY);
                advancedCheck.tabIndex = ++tabbingIndex;
                advancedCheck.isChecked = ModSettings.enableAdvanced;
                advancedCheck.eventCheckChanged += (control, isChecked) =>
                {
                    ModSettings.enableAdvanced = isChecked;
                    SettingsUtils.SaveSettings();
                };
                currentY += advancedCheck.height + Margin;

                UICheckBox replaceNextCheck = UIControls.AddPlainCheckBox(panel, Translations.Translate("RON_OPT_NEX"));
                replaceNextCheck.relativePosition = new Vector2(LeftMargin, currentY);
                replaceNextCheck.tabIndex = ++tabbingIndex;
                replaceNextCheck.isChecked = ModSettings.replaceNExt2;
                replaceNextCheck.eventCheckChanged += (control, isChecked) =>
                {
                    ModSettings.replaceNExt2 = isChecked;
                    SettingsUtils.SaveSettings();
                };
            }
            else
            {
                Logging.Error("unable to get options panel helper reference");
            }

            



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

            UIComponent component = (helper as UIHelper)?.self as UIComponent;

            if (component != null)
            {
                UIControls.AddLabel(component, 50f, 200f, "MIAOW");
            }*/
        }
    }
}
