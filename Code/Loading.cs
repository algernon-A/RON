using ICities;
using RON.MessageBox;


namespace RON
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Status flag.
        internal static bool Loaded { get; private set; } = false;


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
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

            // Initialise select tool.
            ToolsModifierControl.toolController.gameObject.AddComponent<RONTool>();

            // Record list of loaded networks.
            //AutoReplaceXML.SaveFile();


            // Record loaded status and prime input key.
            Loaded = true;
            UIThreading.ignore = false;

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager.OptionsEventHook();
        }
    }
}
