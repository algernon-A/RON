﻿// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.Collections.Generic;
    using AlgernonCommons.Patching;
    using ICities;

    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : PatcherLoadingBase<OptionsPanel, PatcherBase>
    {
        /// <summary>
        /// Gets a list of permitted loading modes.
        /// </summary>
        protected override List<AppMode> PermittedModes => new List<AppMode> { AppMode.Game, AppMode.MapEditor, AppMode.AssetEditor, AppMode.ScenarioEditor };

        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);

            // Add RON tool to tool controller.
            ToolsModifierControl.toolController.gameObject.AddComponent<RONTool>();

            // Add Railway Replacer button to world info panels.
            BuiltStationPanel.AddInfoPanelButton();
        }
    }
}
