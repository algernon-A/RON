using System.Collections.Generic;
using System.Diagnostics;
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
		internal static List<string> missingNetworks;


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
				// Ensure that we're only looking for networks.
				string callingMethod = new StackTrace().GetFrame(2).GetMethod().DeclaringType.ToString();
				if (!callingMethod.Equals("PrefabCollection`1[NetInfo]") && !callingMethod.Equals("LoadingScreenMod.CustomDeserializer"))
				{
					// Not a network - continue on to original method.
					return true;
				}

				// If we haven't attempted to read the auto replace file already, do so.
				if (!attemptedRead)
				{
					// Load configuration file and set flag to indicate attempt.
					autoReplaceFiles = AutoReplaceXML.LoadReplacements();
					attemptedRead = true;
				}

				// Did we sucessfully read the auto replace file?
				if (autoReplaceFiles != null)
				{
					// Yes - iterate through file list.
					for (int i = 0; i < autoReplaceFiles.Length; ++i)
                    {
						// Don't replace NExt2 roads if setting isn't enabled.
						if (i == (int)AutoReplaceXML.Replacements.NExt2 && !ModSettings.ReplaceNExt2)
                        {
							continue;
						}

						// Don't replace NAR tracks if setting isn't enabled.
						if (i == (int)AutoReplaceXML.Replacements.NAR && !ModSettings.ReplaceNAR)
						{
							continue;
						}

						// Iterate through each entry in this file.
						foreach (ReplaceEntry entry in autoReplaceFiles[i].AutoReplacements)
						{
							if (entry != null && entry.targetName.Equals(name))
							{
								// Found a target name match; check to see if we have a replacement.
								string replacementName = entry.replacementName;
								if (PrefabCollection<NetInfo>.FindLoaded(replacementName) == null)
								{
									// No replacement found.
									Logging.Error("couldn't find replacement ", replacementName, " for NExt2 network ", name);

									// Add missing name to list, creating it if we haven't already.
									if (missingNetworks == null)
									{
										missingNetworks = new List<string>();
									}
									missingNetworks.Add(name);

									// Execute original method.
									return true;
								}
								else
								{
									// Replacement found; return replacement name and don't execute original method.
									__result = entry.replacementName;
									Logging.Message("attempting to replace network ", name, " with ", __result);

									// Don't execute original method.
									return false;
								}
							}
						}
					}
				}
			}

			// If we got here, no substitution was performed; continue on to original method.
			return true;
		}
	}
}