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

        // UUI hotkey.
        [XmlIgnore]
        private static readonly UnsavedInputKey uuiKey = new UnsavedInputKey(name: "Transfer Controller hotkey", keyCode: KeyCode.N, control: false, shift: false, alt: true);


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

        // Auto-replace NAR tracks on load.
        [XmlIgnore]
        private static bool replaceNAR = true;


        // Language.
        [XmlElement("Language")]
        public string Language
        {
            get => Translations.CurrentLanguage;

            set => Translations.CurrentLanguage = value;
        }

        
        // RON tool hotkey.
        [XmlElement("PanelKey")]
        public KeyBinding XMLPanelKey
        {
            get => uuiKey.KeyBinding;

            set => uuiKey.KeyBinding = value;
        }


        // Show Railway Replacer panel.
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


        // Auto-replace NAR tracks on load.
        [XmlElement("ReplaceNAR")]
        public bool XMLReplaceNAR
        {
            get => replaceNAR;
            set => replaceNAR = value;
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
        /// Replace NAR tracks on load.
        /// </summary>
		[XmlIgnore]
        internal static bool ReplaceNAR { get => replaceNAR; set => replaceNAR = value; }


        /// <summary>
        /// Current hotkey as UUI UnsavedInputKey.
        /// </summary>
        [XmlIgnore]
        internal static UnsavedInputKey UUIKey => uuiKey;


        /// <summary>
        /// The current hotkey settings as ColossalFramework InputKey.
        /// </summary>
        [XmlIgnore]
        internal static InputKey ToolKey
        {
            get => uuiKey.value;

            set => uuiKey.value = value;
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


        /// <summary>
        /// Encode keybinding as saved input key for UUI.
        /// </summary>
        /// <returns></returns>
        internal InputKey Encode() => SavedInputKey.Encode((KeyCode)keyCode, control, shift, alt);
    }


    /// <summary>
    /// UUI unsaved input key.
    /// </summary>
    public class UnsavedInputKey : UnifiedUI.Helpers.UnsavedInputKey
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Reference name</param>
        /// <param name="keyCode">Keycode</param>
        /// <param name="control">Control modifier key status</param>
        /// <param name="shift">Shift modifier key status</param>
        /// <param name="alt">Alt modifier key status</param>
        public UnsavedInputKey(string name, KeyCode keyCode, bool control, bool shift, bool alt) :
            base(keyName: name, modName: "Repaint", Encode(keyCode, control: control, shift: shift, alt: alt))
        {
        }


        /// <summary>
        /// Called by UUI when a key conflict is resolved.
        /// Used here to save the new key setting.
        /// </summary>
        public override void OnConflictResolved() => ModSettings.Save();


        /// <summary>
        /// 
        /// </summary>
        public KeyBinding KeyBinding
        {
            get => new KeyBinding { keyCode = (int)Key, control = Control, shift = Shift, alt = Alt };
            set => this.value = value.Encode();
        }
    }
}