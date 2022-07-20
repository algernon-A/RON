using HarmonyLib;
using RON.MessageBox;
using System.Collections.Generic;


namespace RON
{
    /// <summary>
    /// Harmony Postfix patch for OnLevelLoaded.  This enables us to perform setup tasks after all loading has been completed.
    /// </summary>
    [HarmonyPatch(typeof(LoadingWrapper))]
    [HarmonyPatch("OnLevelLoaded")]
    public static class OnLevelLoadedPatch
    {
        // Loading flag.
        internal static bool loaded = false;


        /// <summary>
        /// Harmony postfix to perform actions require after the level has loaded.
        /// </summary>
        public static void Postfix()
        {
            // Display any missing network notifications.
            List<string> missingNets = ResolveLegacyPrefabPatch.CheckMissingNets();
            if (missingNets.Count > 0)
            {
                ListMessageBox missingNetBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                missingNetBox.AddParas(Translations.Translate("ERR_NXT"));

                // List of dot points.
                missingNetBox.AddList(missingNets);

                // Closing para.
                missingNetBox.AddParas(Translations.Translate("MES_PAGE"));
            }

            // Record list of loaded networks.
            //AutoReplaceXML.SaveFile();

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager.OptionsEventHook();

            Logging.KeyMessage("loading complete");
            loaded = true;
        }
    }
}