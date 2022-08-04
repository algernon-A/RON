// <copyright file="ResolveLegacyPrefab.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.Collections.Generic;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to substitute networks at loading.
    /// </summary>
    [HarmonyPatch(typeof(BuildConfig), nameof(BuildConfig.ResolveLegacyPrefab))]
    public static class ResolveLegacyPrefab
    {
        private static AutoReplaceXML[] s_autoReplaceFiles;
        private static bool s_attemptedRead = false;
        private static HashSet<AutoReplaceXML.ReplaceEntry> s_attemptedReplacements;

        /// <summary>
        /// Harmony Prefix patch for BuildConfig.ResolveLegacyPrefab to substitute named networks on loading.
        /// </summary>
        /// <param name="__result">Original method result.</param>
        /// <param name="name">Original network name.</param>
        /// <returns>False (don't execute original method) if a replacement was found, true (continue on to original method) otherwise.</returns>
        public static bool Prefix(ref string __result, string name)
        {
            // Don't do anything without being enabled.
            if (ModSettings.ReplaceNExt2 | ModSettings.ReplaceNAR)
            {
                // If we haven't attempted to read the auto replace file already, do so.
                if (!s_attemptedRead)
                {
                    // Load configuration file and set flag to indicate attempt.
                    s_autoReplaceFiles = AutoReplaceXML.LoadReplacements();
                    s_attemptedRead = true;

                    // Initialize missing networks list.
                    s_attemptedReplacements = new HashSet<AutoReplaceXML.ReplaceEntry>();
                }

                // Did we sucessfully read the auto replace file?
                if (s_autoReplaceFiles != null)
                {
                    // Yes - iterate through file list.
                    for (int i = 0; i < s_autoReplaceFiles.Length; ++i)
                    {
                        // Don't replace NExt2 roads if setting isn't enabled.
                        if (i == (int)AutoReplaceXML.Replacements.NExt2 & !ModSettings.ReplaceNExt2)
                        {
                            continue;
                        }

                        // Don't replace MOM tracks if setting isn't enabled.
                        if (i == (int)AutoReplaceXML.Replacements.MOM & !ModSettings.ReplaceMOM)
                        {
                            continue;
                        }

                        // Don't replace NAR tracks with R2 if setting isn't enabled.
                        if (i == (int)AutoReplaceXML.Replacements.NAR_R2 & !(ModSettings.ReplaceNAR & ModSettings.ReplaceNARmode == AutoReplaceXML.Replacements.NAR_R2))
                        {
                            continue;
                        }

                        // Don't replace NAR tracks with BP if setting isn't enabled.
                        if (i == (int)AutoReplaceXML.Replacements.NAR_BP & !(ModSettings.ReplaceNAR & ModSettings.ReplaceNARmode == AutoReplaceXML.Replacements.NAR_BP))
                        {
                            continue;
                        }

                        // Iterate through each entry in this file.
                        foreach (AutoReplaceXML.ReplaceEntry entry in s_autoReplaceFiles[i].AutoReplacements)
                        {
                            // Check for match.
                            if (entry.TargetName != null && entry.TargetName.Equals(name))
                            {
                                // Replacement found; return replacement name and don't execute original method.
                                __result = entry.ReplacementName;
                                Logging.Message("attempting to replace network ", name, " with ", __result);

                                // Add target to our list of attempted replacements.
                                s_attemptedReplacements.Add(entry);

                                // Don't execute original method.
                                return false;
                            }
                        }
                    }
                }
            }

            // If we got here, no substitution was performed; continue on to original method.
            return true;
        }

        /// <summary>
        /// Checks for any missing networks that didn't have available substitutes.
        /// </summary>
        /// <returns>List of missing replacement network names.</returns>
        internal static List<string> CheckMissingNets()
        {
            // Return hashset.
            List<string> missingNets = new List<string>();

            // Don't do anything if we haven't attempted any replacements.
            if (s_attemptedReplacements != null)
            {
                // Iterate through each attempted replacemnt.
                foreach (AutoReplaceXML.ReplaceEntry entry in s_attemptedReplacements)
                {
                    // Check if this prefab was loaded.
                    if (PrefabCollection<NetInfo>.FindLoaded(entry.ReplacementName) == null)
                    {
                        // Prefab wasn't loaded - add to our return list.
                        missingNets.Add(entry.ReplacementName);
                    }
                }

                // Free memory.
                s_attemptedReplacements.Clear();
                s_attemptedReplacements = null;
            }

            // Free memory.
            s_autoReplaceFiles = null;

            return missingNets;
        }
    }
}