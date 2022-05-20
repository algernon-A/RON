using System.Linq;
using UnityEngine;
using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// RON options panel.
    /// </summary>
    public class RONOptionsPanel : UIPanel
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float SubTitleX = 49f;
        private const float CheckRowHeight = 22f;
        private const float GroupMargin = 40f;


        /// <summary>
        /// Performs initial setup for the panel; we don't use Start() as that's not sufficiently reliable (race conditions), and is not needed with the dynamic create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Size and placement.
            this.autoLayout = false;

            // Add controls.
            // Y position indicator.
            float currentY = Margin;

            UIDropDown languageDropDown = UIControls.AddPlainDropDown(this, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager.LocaleChanged();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + GroupMargin;

            // Hotkey control.
            OptionsKeymapping keyMapping = languageDropDown.parent.parent.gameObject.AddComponent<OptionsKeymapping>();
            keyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += keyMapping.uIPanel.height + GroupMargin;

            // Show Railway Repalcer checkbox.
            UICheckBox railwayReplacerCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("RON_OPT_RRP"));
            railwayReplacerCheck.relativePosition = new Vector2(LeftMargin, currentY);
            railwayReplacerCheck.isChecked = ModSettings.ShowRailwayReplacer;
            railwayReplacerCheck.eventCheckChanged += (control, isChecked) => ModSettings.ShowRailwayReplacer = isChecked;
            currentY += CheckRowHeight + GroupMargin;

            // Advanced mode checkbox.
            UICheckBox advancedCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("RON_OPT_ADV"));
            advancedCheck.relativePosition = new Vector2(LeftMargin, currentY);
            advancedCheck.isChecked = ModSettings.EnableAdvanced;
            advancedCheck.eventCheckChanged += (control, isChecked) => ModSettings.EnableAdvanced = isChecked;
            currentY += CheckRowHeight + Margin;

            // Advanced mode sub-label.
            UILabel advancedCheckSubLabel = UIControls.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_ADV2"), textScale: 1.125f);
            advancedCheckSubLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");
            currentY += CheckRowHeight + GroupMargin;

            // Replace NExt2 roads on load checkbox.
            UICheckBox replaceNextCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("RON_OPT_NEX"));
            replaceNextCheck.relativePosition = new Vector2(LeftMargin, currentY);
            replaceNextCheck.isChecked = ModSettings.ReplaceNExt2;
            replaceNextCheck.eventCheckChanged += (control, isChecked) => ModSettings.ReplaceNExt2 = isChecked;
            currentY += CheckRowHeight + Margin;

            // Replace NExt2 roads on load sub-label.
            UILabel replaceCheckSubLabel = UIControls.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NEX2"), textScale: 1.125f);
            replaceCheckSubLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");
            currentY += CheckRowHeight + GroupMargin;
        }
    }
}