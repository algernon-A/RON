using HarmonyLib;


namespace RON
{
    /// <summary>
    /// Harmony patches to implement station track selection on placement.
    /// </summary>
    [HarmonyPatch(typeof(BuildingTool))]
    public static class BuildingToolPatches
    {
        /// <summary>
        /// Harmony postfix patch to detect when a station building is selected, and if so, display the station track selection panel.
        /// </summary>
        [HarmonyPatch("OnToolUpdate")]
        [HarmonyPostfix]
        public static void OnToolUpdate(BuildingTool __instance)
        {
            // Show station panel if option is set.
            if (ModSettings.ShowRailwayReplacer)
            {
                StationPanel.SetTarget(__instance.m_prefab);
            }
        }


        /// <summary>
        /// Harmony postfix patch to close the station track selection panel when the building tool is disabled.
        /// </summary>
        [HarmonyPatch("OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable()
        {
            // Clear station reference.
            StationPanel.SetTarget(null);
        }
    }
}