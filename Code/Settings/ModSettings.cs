namespace RON
{
    /// <summary>
    /// Static class to hold global mod settings.
    /// </summary>
    internal static class ModSettings
    {
        // Enable advanced mode.
        private static bool enableAdvanced = false;

        // Auto-replace Network Extensions 2 roads on load.
        private static bool replaceNExt2 = true;


        /// <summary>
        /// Advanced mode (any network type replacement)
        /// </summary>
        internal static bool EnableAdvanced
        {
            get => enableAdvanced;

            set
            {
                enableAdvanced = value;
                SettingsUtils.SaveSettings();
            }
        }


        /// <summary>
        /// Replace Network Extensions 2 roads on load.
        /// </summary>
        internal static bool ReplaceNExt2
        {
            get => replaceNExt2;

            set
            {
                replaceNExt2 = value;
                SettingsUtils.SaveSettings();
            }
        }
    }
}