// <copyright file="ToolBasePatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using ColossalFramework;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patches to implement station track highlighting.
    /// </summary>
    [HarmonyPatch(typeof(ToolBase))]
    public static class ToolBasePatch
    {
        // Segment to highlight (0 for none).
        private static ushort s_selectedSegment = 0;

        /// <summary>
        /// Sets the selected network segment for highlighting (0 for none).
        /// </summary>
        internal static ushort SelectedSegment { set => s_selectedSegment = value; }

        /// <summary>
        /// Harmony postfix patch to highlight the selected station track (if any).
        /// </summary>
        /// <param name="cameraInfo">Camera instance.</param>
        [HarmonyPatch(nameof(ToolBase.RenderOverlay))]
        [HarmonyPostfix]
        public static void RenderOverlayPostfix(RenderManager.CameraInfo cameraInfo)
        {
            if (s_selectedSegment > 0)
            {
                NetTool.RenderOverlay(cameraInfo, ref Singleton<NetManager>.instance.m_segments.m_buffer[s_selectedSegment], Color.magenta, Color.red);
            }
        }
    }
}