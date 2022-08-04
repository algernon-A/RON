// <copyright file="BuildingToolPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using HarmonyLib;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
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