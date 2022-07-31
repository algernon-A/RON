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


        // Panel components.
        UIDropDown narModeDropDown;

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

            // Sub-label font.
            UIFont subLabelFont = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");

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

            // Show Railway Replacer checkbox.
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
            UILabel replaceNext2CheckSubLabel = UIControls.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NEX2"), textScale: 1.125f);
            replaceNext2CheckSubLabel.font = subLabelFont;
            currentY += CheckRowHeight + GroupMargin;

            // Replace MOM tracks on load checkbox.
            UICheckBox replaceMOMCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("RON_OPT_MOM"));
            replaceMOMCheck.relativePosition = new Vector2(LeftMargin, currentY);
            replaceMOMCheck.isChecked = ModSettings.ReplaceMOM;
            replaceMOMCheck.eventCheckChanged += (control, isChecked) => ModSettings.ReplaceMOM = isChecked;
            currentY += CheckRowHeight + Margin;

            // Replace MOM tracks on load sub-label.
            UILabel replaceMOMCheckSubLabel = UIControls.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NEX2"), textScale: 1.125f);
            replaceMOMCheckSubLabel.font = subLabelFont;
            currentY += CheckRowHeight + GroupMargin;

            // Replace NAR tracks on load checkbox.
            UICheckBox replaceNARcheck = UIControls.AddPlainCheckBox(this, Translations.Translate("RON_OPT_NAR"));
            replaceNARcheck.relativePosition = new Vector2(LeftMargin, currentY);
            replaceNARcheck.isChecked = ModSettings.ReplaceNAR;
            replaceNARcheck.eventCheckChanged += NARCheckChanged;
            currentY += CheckRowHeight;

            // NAR replacement mode dropdown - custom sized.
            narModeDropDown = UIControls.AddPlainDropDown(this, string.Empty, new string[] { Translations.Translate("RON_OPT_NAR_R2"), Translations.Translate("RON_OPT_NAR_BP") }, (int)ModSettings.ReplaceNARmode);
            narModeDropDown.parent.relativePosition = new Vector2(LeftMargin + LeftMargin, currentY);
            narModeDropDown.textScale = 1f;
            narModeDropDown.height = 29f;
            narModeDropDown.width = 500f;
            narModeDropDown.autoListWidth = false;
            narModeDropDown.listWidth = 500;
            (narModeDropDown.parent as UIPanel).autoLayout = false;
            narModeDropDown.relativePosition = Vector2.zero;
            narModeDropDown.isEnabled = ModSettings.ReplaceNAR;
            narModeDropDown.eventSelectedIndexChanged += NARModeChanged;
            currentY += narModeDropDown.height + 2f;

            // Replace NAR tracks on load sub-label.
            UILabel replaceNARCheckSubLabel = UIControls.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NAR2"), textScale: 1.125f);
            replaceNARCheckSubLabel.font = subLabelFont;
        }


        /// <summary>
        /// Replace NAR check changed event handler.
        /// </summary>
        /// <param name="component">Calling component (unused)</param>
        /// <param name="isChecked">New checked state</param>
        private void NARCheckChanged(UIComponent component, bool isChecked)
        {
            ModSettings.ReplaceNAR = isChecked;
            narModeDropDown.isEnabled = isChecked;
        }


        /// <summary>
        /// Replace NAR dropdown changed event handler.
        /// </summary>
        /// <param name="component">Calling component (unused)</param>
        /// <param name="index">New selected index</param>
        private void NARModeChanged(UIComponent component, int index)
        {
            ModSettings.ReplaceNARmode = (AutoReplaceXML.Replacements)index;
        }
    }
}