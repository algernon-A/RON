// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.Linq;
    using AlgernonCommons;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// RON options panel.
    /// </summary>
    public class OptionsPanel : UIPanel
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float SubTitleX = 49f;
        private const float CheckRowHeight = 22f;
        private const float GroupMargin = 40f;

        // Panel components.
        private UIDropDown _narModeDropDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPanel"/> class.
        /// </summary>
        internal OptionsPanel()
        {
            // Size and placement.
            this.autoLayout = false;

            // Add controls.
            // Y position indicator.
            float currentY = Margin;

            // Sub-label font.
            UIFont subLabelFont = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");

            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            currentY += languageDropDown.parent.height + GroupMargin;

            // Hotkey control.
            OptionsKeymapping keyMapping = languageDropDown.parent.parent.gameObject.AddComponent<UUIKeymapping>();
            keyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += keyMapping.Panel.height + GroupMargin;

            // Show Railway Replacer checkbox.
            UICheckBox railwayReplacerCheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("RON_OPT_RRP"));
            railwayReplacerCheck.isChecked = ModSettings.ShowRailwayReplacer;
            railwayReplacerCheck.eventCheckChanged += (c, isChecked) => ModSettings.ShowRailwayReplacer = isChecked;
            currentY += CheckRowHeight + GroupMargin;

            // Advanced mode checkbox.
            UICheckBox advancedCheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("RON_OPT_ADV"));
            advancedCheck.isChecked = ModSettings.EnableAdvanced;
            advancedCheck.eventCheckChanged += (c, isChecked) => ModSettings.EnableAdvanced = isChecked;
            currentY += CheckRowHeight + Margin;

            // Advanced mode sub-label.
            UILabel advancedCheckSubLabel = UILabels.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_ADV2"), textScale: 1.125f);
            advancedCheckSubLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");
            currentY += CheckRowHeight + GroupMargin;

            // Replace NExt2 roads on load checkbox.
            UICheckBox replaceNextCheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("RON_OPT_NEX"));
            replaceNextCheck.isChecked = ModSettings.ReplaceNExt2;
            replaceNextCheck.eventCheckChanged += (c, isChecked) => ModSettings.ReplaceNExt2 = isChecked;
            currentY += CheckRowHeight + Margin;

            // Replace NExt2 roads on load sub-label.
            UILabel replaceNext2CheckSubLabel = UILabels.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NEX2"), textScale: 1.125f);
            replaceNext2CheckSubLabel.font = subLabelFont;
            currentY += CheckRowHeight + GroupMargin;

            // Replace MOM tracks on load checkbox.
            UICheckBox replaceMOMCheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("RON_OPT_MOM"));
            replaceMOMCheck.isChecked = ModSettings.ReplaceMOM;
            replaceMOMCheck.eventCheckChanged += (c, isChecked) => ModSettings.ReplaceMOM = isChecked;
            currentY += CheckRowHeight + Margin;

            // Replace MOM tracks on load sub-label.
            UILabel replaceMOMCheckSubLabel = UILabels.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NEX2"), textScale: 1.125f);
            replaceMOMCheckSubLabel.font = subLabelFont;
            currentY += CheckRowHeight + GroupMargin;

            // Replace NAR tracks on load checkbox.
            UICheckBox replaceNARcheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("RON_OPT_NAR"));
            replaceNARcheck.isChecked = ModSettings.ReplaceNAR;
            replaceNARcheck.eventCheckChanged += NARCheckChanged;
            currentY += CheckRowHeight;

            // NAR replacement mode dropdown - custom sized.
            _narModeDropDown = UIDropDowns.AddPlainDropDown(this, LeftMargin + LeftMargin, currentY, string.Empty, new string[] { Translations.Translate("RON_OPT_NAR_R2"), Translations.Translate("RON_OPT_NAR_BP") }, (int)ModSettings.ReplaceNARmode);
            _narModeDropDown.textScale = 1f;
            _narModeDropDown.height = 29f;
            _narModeDropDown.width = 500f;
            _narModeDropDown.autoListWidth = false;
            _narModeDropDown.listWidth = 500;
            (_narModeDropDown.parent as UIPanel).autoLayout = false;
            _narModeDropDown.relativePosition = Vector2.zero;
            _narModeDropDown.isEnabled = ModSettings.ReplaceNAR;
            _narModeDropDown.eventSelectedIndexChanged += NARModeChanged;
            currentY += _narModeDropDown.height + 2f;

            // Replace NAR tracks on load sub-label.
            UILabel replaceNARCheckSubLabel = UILabels.AddLabel(this, SubTitleX, currentY, Translations.Translate("RON_OPT_NAR2"), textScale: 1.125f);
            replaceNARCheckSubLabel.font = subLabelFont;
            currentY += replaceNARCheckSubLabel.height + GroupMargin;

            UICheckBox loggingCheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (c, isChecked) => { Logging.DetailLogging = isChecked; };
        }

        /// <summary>
        /// Replace NAR check changed event handler.
        /// </summary>
        /// <param name="component">Calling component (unused).</param>
        /// <param name="isChecked">New checked state.</param>
        private void NARCheckChanged(UIComponent component, bool isChecked)
        {
            ModSettings.ReplaceNAR = isChecked;
            _narModeDropDown.isEnabled = isChecked;
        }

        /// <summary>
        /// Replace NAR dropdown changed event handler.
        /// </summary>
        /// <param name="component">Calling component (unused).</param>
        /// <param name="index">New selected index.</param>
        private void NARModeChanged(UIComponent component, int index)
        {
            ModSettings.ReplaceNARmode = (AutoReplaceXML.Replacements)index;
        }
    }
}