using ICities;
using RON.MessageBox;


namespace RON
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Set up options panel event handler and prime input key.
            OptionsPanel.OptionsEventHook();
            UIThreading.ignore = false;

            // Initialise select tool.
            ToolsModifierControl.toolController.gameObject.AddComponent<RONTool>();

            // Record list of loaded networks.
            //AutoReplaceXML.SaveFile();

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
        }
    }
}
