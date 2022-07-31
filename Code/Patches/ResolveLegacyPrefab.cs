using System.Collections.Generic;
using HarmonyLib;


namespace RON
{
	/// <summary>
	/// Harmony patch to substitute networks at loading.
	/// </summary>
	[HarmonyPatch(typeof(BuildConfig), nameof(BuildConfig.ResolveLegacyPrefab))]
	public static class ResolveLegacyPrefabPatch
	{
		private static AutoReplaceXML[] autoReplaceFiles;
		private static bool attemptedRead = false;
		private static HashSet<ReplaceEntry> attemptedReplacements;


		/// <summary>
		/// Harmony Prefix patch for BuildConfig.ResolveLegacyPrefab to substitute named networks on loading.
		/// </summary>
		/// <param name="__result">Original method result</param>
		/// <param name="name">Original network name</param>
		/// <returns>False (don't execute original method) if a replacement was found, true (continue on to original method) otherwise</returns>
		public static bool Prefix(ref string __result, string name)
		{
			// Don't do anything without being enabled.
			if (ModSettings.ReplaceNExt2 | ModSettings.ReplaceNAR)
			{

				// If we haven't attempted to read the auto replace file already, do so.
				if (!attemptedRead)
				{
					// Load configuration file and set flag to indicate attempt.
					autoReplaceFiles = AutoReplaceXML.LoadReplacements();
					attemptedRead = true;

					// Initialize missing networks list.
					attemptedReplacements = new HashSet<ReplaceEntry>();
				}

				// Did we sucessfully read the auto replace file?
				if (autoReplaceFiles != null)
				{
					// Yes - iterate through file list.
					for (int i = 0; i < autoReplaceFiles.Length; ++i)
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
						foreach (ReplaceEntry entry in autoReplaceFiles[i].AutoReplacements)
						{
							// Check for match.
							if (entry != null && entry.targetName.Equals(name))
							{
								// Replacement found; return replacement name and don't execute original method.
								__result = entry.replacementName;
								Logging.Message("attempting to replace network ", name, " with ", __result);

								// Add target to our list of attempted replacements.
								attemptedReplacements.Add(entry);

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
		/// <returns>List of missing replacement network names</returns>
		internal static List<string> CheckMissingNets()
		{
			// Return hashset.
			List<string> missingNets = new List<string>();

			// Don't do anything if we haven't attempted any replacements.
			if (attemptedReplacements != null)
			{
				// Iterate through each attempted replacemnt.
				foreach (ReplaceEntry entry in attemptedReplacements)
				{
					// Check if this prefab was loaded.
					if (PrefabCollection<NetInfo>.FindLoaded(entry.replacementName) == null)
					{
						// Prefab wasn't loaded - add to our return list.
						missingNets.Add(entry.replacementName);
					}
				}

				// Free memory.
				attemptedReplacements.Clear();
				attemptedReplacements = null;
			}

			// Free memory.
			autoReplaceFiles = null;

			return missingNets;
		}
	}
}