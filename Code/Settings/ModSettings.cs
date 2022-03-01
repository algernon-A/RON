using System;
using System.IO;
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
        // Settings file name.
        [XmlIgnore]
        private static readonly string SettingsFileName = "RON-settings.xml";

        // User settings directory.
        [XmlIgnore]
        private static readonly string UserSettingsDir = ColossalFramework.IO.DataLocation.localApplicationData;


        // Enable advanced mode.
        [XmlIgnore]
        private static bool enableAdvanced = false;

        // Show Railway Replacer panel.
        [XmlIgnore]
        private static bool showRailwayReplacer = true;

        // Auto-replace Network Extensions 2 roads on load.
        [XmlIgnore]
        private static bool replaceNExt2 = true;

        // SavedInputKey reference for communicating with UUI.
        [XmlIgnore]
        private static readonly SavedInputKey uuiSavedKey = new SavedInputKey("RON hotkey", "RON hotkey", key: KeyCode.N, control: false, shift: false, alt: true, false);


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

                // Reset any erroneous 'Alt-B' settings.
                if ((KeyCode)value.keyCode == KeyCode.B && !value.control && !value.shift && value.alt)
                {
                    Logging.Message("overriding hotkey to alt-N");
                    uuiSavedKey.Key = KeyCode.N;
                }
            }
        }


        // Show Railway Repalcer panel.
        [XmlElement("ShowRailwayReplacer")]
        public bool XMLShowRailwayReplacer
        {
            get => showRailwayReplacer;
            set => showRailwayReplacer = value;
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


        /// Show Railway Repalcer mod.
        /// </summary>
		[XmlIgnore]
        internal static bool ShowRailwayReplacer { get => showRailwayReplacer; set=> showRailwayReplacer = value; }


        /// <summary>
        /// Advanced mode (any network type replacement).
        /// </summary>
		[XmlIgnore]
        internal static bool EnableAdvanced { get => enableAdvanced; set => enableAdvanced = value; }


        /// <summary>
        /// Replace Network Extensions 2 roads on load.
        /// </summary>
		[XmlIgnore]
        internal static bool ReplaceNExt2 { get => replaceNExt2; set => replaceNExt2 = value; }


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


        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void Load()
        {
            try
            {
                // Attempt to read new settings file (in user settings directory).
                string fileName = Path.Combine(UserSettingsDir, SettingsFileName);
                if (!File.Exists(fileName))
                {
                    // No settings file in user directory; use application directory instead.
                    fileName = SettingsFileName;
                }

                // Check to see if configuration file exists.
                if (File.Exists(fileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                        if (!(xmlSerializer.Deserialize(reader) is ModSettings settingsFile))
                        {
                            Logging.Error("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Logging.Message("no settings file found");
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading XML settings file");
            }
        }


        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save()
        {
            try
            {
                // Save into user local settings.
                using (StreamWriter writer = new StreamWriter(Path.Combine(UserSettingsDir, SettingsFileName)))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    xmlSerializer.Serialize(writer, new ModSettings());
                }

                // Cleaning up after ourselves - delete any old config file in the application directory.
                if (File.Exists(SettingsFileName))
                {
                    File.Delete(SettingsFileName);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception saving XML settings file");
            }
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