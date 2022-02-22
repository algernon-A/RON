using HarmonyLib;
using RON.MessageBox;


namespace RON
{
    /// <summary>
    /// Harmony Postfix patch for OnLevelLoaded.  This enables us to perform setup tasks after all loading has been completed.
    /// </summary>
    [HarmonyPatch(typeof(LoadingWrapper))]
    [HarmonyPatch("OnLevelLoaded")]
    public static class OnLevelLoadedPatch
    {
        /// <summary>
        /// Harmony postfix to perform actions require after the level has loaded.
        /// </summary>
        public static void Postfix()
        {
            // Display any missing NExt2 network notifications.
            if (ResolveLegacyPrefabPatch.missingNetworks != null)
            {
                ListMessageBox missingNetBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                missingNetBox.AddParas(Translations.Translate("ERR_NXT"));

                // List of dot points.
                missingNetBox.AddList(ResolveLegacyPrefabPatch.missingNetworks);

                // Closing para.
                missingNetBox.AddParas(Translations.Translate("MES_PAGE"));
            }


            // Record list of loaded networks.
            //AutoReplaceXML.SaveFile();

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager.OptionsEventHook();

            // Add RON tool to tool controller.
            ToolsModifierControl.toolController.gameObject.AddComponent<RONTool>();

            // Enable tool.
            RONTool.gameLoaded = true;

            Logging.KeyMessage("loading complete");
        }
    }
}