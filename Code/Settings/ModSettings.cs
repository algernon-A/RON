// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.XML;
    using UnityEngine;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    [XmlRoot("RON")]
    public class ModSettings : SettingsXMLBase
    {
        /// <summary>
        /// UUI key.
        /// </summary>
        [XmlIgnore]
        private static readonly UnsavedInputKey UUIKey = new UnsavedInputKey(name: "RONr hotkey", keyCode: KeyCode.N, control: false, shift: false, alt: true);

        /// <summary>
        /// Gets the settings file name.
        /// </summary>
        [XmlIgnore]
        private static readonly string SettingsFile = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "RON-settings.xml");

        /// <summary>
        /// Gets or sets a value indicating whether advanced mode is active (true) or inactive (false).
        /// </summary>
        [XmlIgnore]
        private static bool enableAdvanced = false;

        /// <summary>
        /// Gets or sets a value indicating whether the railway replacer panel should be automatically shown when a station building is selected (true).
        /// </summary>
        [XmlIgnore]
        private static bool showRailwayReplacer = true;

        /// <summary>
        /// Gets or sets a value indicating whether Network Extensions 2 roads should be auto-replaced on load (true).
        /// </summary>
        [XmlIgnore]
        private static bool replaceNExt2 = true;

        /// <summary>
        /// Gets or sets a value indicating whether Metro Overhaul Mod tracks should be auto-replaced on load (true).
        /// </summary>
        [XmlIgnore]
        private static bool replaceMOM = true;

        /// <summary>
        /// Gets or sets a value indicating whether North Americal Rail tracks should be auto-replaced on load (true).
        /// </summary>
        [XmlIgnore]
        private static bool replaceNAR = true;

        /// <summary>
        /// Gets or sets NAR track replacement mode.
        /// </summary>
        [XmlIgnore]
        private static AutoReplaceXML.Replacements replaceNARmode = AutoReplaceXML.Replacements.NAR_R2;

        /// <summary>
        /// Gets or sets the RON hotkey.
        /// </summary>
        [XmlElement("PanelKey")]
        public Keybinding XMLPanelKey
        {
            get => UUIKey.Keybinding;

            set => UUIKey.Keybinding = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether advanced mode is active (true) or inactive (false).
        /// </summary>
        [XmlElement("AdvancedMode")]
        public bool XMLAdvancedMode
        {
            get => enableAdvanced;
            set => enableAdvanced = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the railway replacer panel should be automatically shown when a station building is selected (true).
        /// </summary>
        [XmlElement("ShowRailwayReplacer")]
        public bool XMLShowRailwayReplacer
        {
            get => showRailwayReplacer;
            set => showRailwayReplacer = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Network Extensons 2 roads should be auto-replaced on load (true).
        /// </summary>
        [XmlElement("ReplaceNExt2")]
        public bool XMLReplaceNext
        {
            get => replaceNExt2;
            set => replaceNExt2 = value;
        }

        /*
        /// <summary>
        /// Gets or sets a value indicating whether Metro Overhaul Mod tracks should be auto-replaced on load (true).
        /// </summary>
        [XmlElement("ReplaceMOM")]
        public bool XMLReplaceMOM
        {
            get => replaceMOM;
            set => replaceMOM = value;
        }
        */

        /// <summary>
        /// Gets or sets a value indicating whether North Americal Rail tracks should be auto-replaced on load (true).
        /// </summary>
        [XmlElement("ReplaceNAR")]
        public bool XMLReplaceNAR
        {
            get => replaceNAR;
            set => replaceNAR = value;
        }

        /// <summary>
        /// Gets or sets the replacement mode selection for NAR auto-replace.
        /// </summary>
        [XmlElement("ReplaceMode_NAR")]
        public AutoReplaceXML.Replacements XMLReplaceNARmode
        {
            get => replaceNARmode;
            set => replaceNARmode = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the railway replacer panel should be automatically shown when a station building is selected (true).
        /// </summary>
        [XmlIgnore]
        internal static bool ShowRailwayReplacer { get => showRailwayReplacer; set => showRailwayReplacer = value; }

        /// <summary>
        /// Gets or sets a value indicating whether advanced mode is active (true) or inactive (false).
        /// </summary>
        [XmlIgnore]
        internal static bool EnableAdvanced { get => enableAdvanced; set => enableAdvanced = value; }

        /// <summary>
        /// Gets or sets a value indicating whether Network Extensons 2 roads should be auto-replaced on load (true).
        /// </summary>
        [XmlIgnore]
        internal static bool ReplaceNExt2 { get => replaceNExt2; set => replaceNExt2 = value; }

        /// <summary>
        /// Gets or sets a value indicating whether Metro Overhaul Mod tracks should be auto-replaced on load (true).
        /// </summary>
        [XmlIgnore]
        internal static bool ReplaceMOM { get => replaceMOM; set => replaceMOM = value; }

        /// <summary>
        /// Gets or sets a value indicating whether North Americal Rail tracks should be auto-replaced on load (true).
        /// </summary>
        [XmlIgnore]
        internal static bool ReplaceNAR { get => replaceNAR; set => replaceNAR = value; }

        /// <summary>
        /// Gets or sets the replacement mode selection for NAR auto-replace.
        /// </summary>
        [XmlIgnore]
        internal static AutoReplaceXML.Replacements ReplaceNARmode { get => replaceNARmode; set => replaceNARmode = value; }

        /// <summary>
        /// Gets the current hotkey as a UUI UnsavedInputKey.
        /// </summary>
        [XmlIgnore]
        internal static UnsavedInputKey ToolKey => UUIKey;

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        internal static void Load() => XMLFileUtils.Load<ModSettings>(SettingsFile);

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        internal static void Save() => XMLFileUtils.Save<ModSettings>(SettingsFile);
    }
}