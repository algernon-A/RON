using System.Xml.Serialization;
using UnityEngine;
using ColossalFramework;


namespace RON
{
    /// <summary>
    /// Global mod settings.
    /// </summary>
	[XmlRoot("RON")]
    public class ModSettings
    {
        // Enable advanced mode.
        [XmlIgnore]
        private static bool enableAdvanced = false;

        // Auto-replace Network Extensions 2 roads on load.
        [XmlIgnore]
        private static bool replaceNExt2 = true;

        // SavedInputKey reference for communicating with UUI.
        [XmlIgnore]
        private static readonly SavedInputKey uuiSavedKey = new SavedInputKey("BOB hotkey", "BOB hotkey", key: KeyCode.B, control: false, shift: false, alt: true, false);


        // Language.
        [XmlElement("Language")]
        public string Language
        {
            get => Translations.CurrentLanguage;

            set => Translations.CurrentLanguage = value;
        }

        // Hotkey element.
        [XmlElement("PanelKey")]
        public KeyBinding PanelKey
        {
            get
            {
                return new KeyBinding
                {
                    keyCode = (int)PanelSavedKey.Key,
                    control = PanelSavedKey.Control,
                    shift = PanelSavedKey.Shift,
                    alt = PanelSavedKey.Alt
                };
            }
            set
            {
                uuiSavedKey.Key = (KeyCode)value.keyCode;
                uuiSavedKey.Control = value.control;
                uuiSavedKey.Shift = value.shift;
                uuiSavedKey.Alt = value.alt;
            }
        }


        // Advanced mode.
        [XmlElement("AdvancedMode")]
        public bool XMLAdvancedMode
        {
            get => enableAdvanced;
            set => enableAdvanced = value;
        }


        // Auto-replace NExt2 roads on load.
        [XmlElement("ReplaceNExt2")]
        public bool XMLReplaceNext
        {
            get => replaceNExt2;
            set => replaceNExt2 = value;
        }


        /// <summary>
        /// Advanced mode (any network type replacement).
        /// </summary>
		[XmlIgnore]
        internal static bool EnableAdvanced
        {
            get => enableAdvanced;

            set
            {
                // Don't do anything if no change.
                if (enableAdvanced != value)
                {
                    enableAdvanced = value;
                    SettingsUtils.SaveSettings();
                }
            }
        }


        /// <summary>
        /// Replace Network Extensions 2 roads on load.
        /// </summary>
		[XmlIgnore]
        internal static bool ReplaceNExt2
        {
            get => replaceNExt2;

            set
            {
                // Don't do anything if no change.
                if (replaceNExt2 != value)
                {
                    replaceNExt2 = value;
                    SettingsUtils.SaveSettings();
                }
            }
        }


        /// <summary>
        /// Panel hotkey as ColossalFramework SavedInputKey.
        /// </summary>
        [XmlIgnore]
        internal static SavedInputKey PanelSavedKey => uuiSavedKey;


        /// <summary>
        /// The current hotkey settings as ColossalFramework InputKey.
        /// </summary>
        /// </summary>
        [XmlIgnore]
        internal static InputKey CurrentHotkey
        {
            get => uuiSavedKey.value;

            set => uuiSavedKey.value = value;
        }
    }


    /// <summary>
    /// Basic keybinding class - code and modifiers.
    /// </summary>
    public class KeyBinding
    {
        [XmlAttribute("KeyCode")]
        public int keyCode;

        [XmlAttribute("Control")]
        public bool control;

        [XmlAttribute("Shift")]
        public bool shift;

        [XmlAttribute("Alt")]
        public bool alt;
    }
}