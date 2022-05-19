using ICities;


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
            Logging.KeyMessage("version ", RONMod.Version, " loading");

            base.OnLevelLoaded(mode);

            // Add RON tool to tool controller.
            ToolsModifierControl.toolController.gameObject.AddComponent<RONTool>();
        }
    }
}
