using System.Xml.Serialization;
using UnityEngine;


namespace RON
{
    /// <summary>
    /// Static class to hold global mod settings.
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
            }
        }


        /// <summary>
        /// Advanced mode (any network type replacement)
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


        [XmlElement("AdvancedMode")]
        public bool XMLAdvancedMode
        {
            get => enableAdvanced;
            set => enableAdvanced = value;
        }

        [XmlElement("ReplaceNExt2")]
        public bool XMLReplaceNext
        {
            get => replaceNExt2;
            set => replaceNExt2 = value;
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