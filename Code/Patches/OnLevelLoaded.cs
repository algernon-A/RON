// <copyright file="OnLevelLoaded.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.Collections.Generic;
    using AlgernonCommons;
    using AlgernonCommons.Notifications;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using HarmonyLib;

    /// <summary>
    /// Harmony Postfix patch for OnLevelLoaded.  This enables us to perform setup tasks after all loading has been completed.
    /// </summary>
    [HarmonyPatch(typeof(LoadingWrapper))]
    [HarmonyPatch("OnLevelLoaded")]
    public static class OnLevelLoadedPatch
    {
        /// <summary>
        /// Gets or sets the current loaded state.
        /// </summary>
        internal static bool Loaded { get; set; } = false;

        /// <summary>
        /// Harmony postfix to perform actions require after the level has loaded.
        /// </summary>
        public static void Postfix()
        {
            // Display any missing network notifications.
            List<string> missingNets = ResolveLegacyPrefabPatch.CheckMissingNets();
            if (missingNets.Count > 0)
            {
                ListNotification missingNetNotification = NotificationBase.ShowNotification<ListNotification>();

                // Key text items.
                missingNetNotification.AddParas(Translations.Translate("ERR_NXT"));

                // List of dot points.
                missingNetNotification.AddList(missingNets);

                // Closing para.
                missingNetNotification.AddParas(Translations.Translate("MES_PAGE"));
            }

            // Record list of loaded networks.
            AutoReplaceXML.SaveFile();

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager<OptionsPanel>.OptionsEventHook();

            Logging.KeyMessage("loading complete");
            Loaded = true;
        }
    }
}