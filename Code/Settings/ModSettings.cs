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
        // Language.
        [XmlElement("Language")]
        public string Language
        {
            get => Translations.Language;

            set => Translations.Language = value;
        }

        // Hotkey element.
        [XmlElement("PanelKey")]
        public KeyBinding PanelKey
        {
            get
            {
                return new KeyBinding
                {
                    keyCode = (int)UIThreading.hotKey,
                    control = UIThreading.hotCtrl,
                    shift = UIThreading.hotShift,
                    alt = UIThreading.hotAlt
                };
            }
            set
            {
                UIThreading.hotKey = (KeyCode)value.keyCode;
                UIThreading.hotCtrl = value.control;
                UIThreading.hotShift = value.shift;
                UIThreading.hotAlt = value.alt;

                // Update the UUI SavedInputKey instance to reflect the changes.
                UpdateSavedInputKey();
            }
        }


        /// <summary>
        /// Convers RON hotkey into SavedInputKey format for communicating with UUI.
        /// </summary>
        [XmlIgnore]
        public static SavedInputKey PanelSavedKey
        {
            get
            {
                // Create SavedInputKey instance for UUI communication if it doesn't already exist.
                if (_uuiSavedKey == null)
                {
                    _uuiSavedKey = new SavedInputKey("RON hotkey", "RON hotkey", key: UIThreading.hotKey, control: UIThreading.hotCtrl, shift: UIThreading.hotShift, alt: UIThreading.hotAlt, true);
                }

                // Return reference.
                return _uuiSavedKey;
            }

            set
            {
                // Convert provided SavedInputKey format to RON native format.
                UIThreading.hotKey = value.Key;
                UIThreading.hotShift = value.Shift;
                UIThreading.hotCtrl = value.Control;
                UIThreading.hotAlt = value.Alt;

                // Update the UUI SavedInputKey instance to reflect the changes.
                UpdateSavedInputKey();

                // Save updated settings.
                SettingsUtils.SaveSettings();
            }
        }


        /// <summary>
        /// Updates the UUI SavedInputKey reference with current RON hotkey values.
        /// </summary>
        internal static void UpdateSavedInputKey()
        {
            // Create SavedInputKey instance for UUI communication if it doesn't already exist.
            if (_uuiSavedKey == null)
            {
                // Use getter null check to create instance.
                _uuiSavedKey = PanelSavedKey;
            }
            else
            {
                // If it already exists, update instance with current RON values.
                _uuiSavedKey.Key = UIThreading.hotKey;
                _uuiSavedKey.Control = UIThreading.hotCtrl;
                _uuiSavedKey.Shift = UIThreading.hotShift;
                _uuiSavedKey.Alt = UIThreading.hotAlt;
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

        // Enable advanced mode.
        [XmlIgnore]
        private static bool enableAdvanced = false;


        // Auto-replace Network Extensions 2 roads on load.
        [XmlIgnore]
        private static bool replaceNExt2 = true;


        // SavedInputKey reference for communicating with UUI.
        [XmlIgnore]
        private static SavedInputKey _uuiSavedKey;
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