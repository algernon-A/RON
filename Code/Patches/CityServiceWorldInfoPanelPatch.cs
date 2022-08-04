// <copyright file="CityServiceWorldInfoPanelPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using HarmonyLib;

    /// <summary>
    /// Harmony Postfix patch to toggle visibility of RON station replacer button.
    /// </summary>
    [HarmonyPatch(typeof(CityServiceWorldInfoPanel), "OnSetTarget")]
    internal static class CityServiceWorldInfoPanelPatch
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