using HarmonyLib;


namespace RON
{
    /// <summary>
    /// Harmony Postfix patch to toggle visibility of RON station replacer button.
    /// </summary>
    [HarmonyPatch(typeof(CityServiceWorldInfoPanel), "OnSetTarget")]
    internal static class DistrictPanelPatch
    {
        /// <summary>
        /// Harmony Postfix patch to update RON station replacer button visibility status when selection changes.
        /// </summary>
        public static void Postfix()
        {
            BuiltStationPanel.SetPanelButtonState();
        }
    }
}