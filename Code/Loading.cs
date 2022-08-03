// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using AlgernonCommons;
    using ICities;

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
            Logging.KeyMessage("version ", AssemblyUtils.CurrentVersion, " loading");

            base.OnLevelLoaded(mode);

            // Add RON tool to tool controller.
            ToolsModifierControl.toolController.gameObject.AddComponent<RONTool>();

            // Add Railway Replacer button to world info panels.
            BuiltStationPanel.AddInfoPanelButton();
        }
    }
}
