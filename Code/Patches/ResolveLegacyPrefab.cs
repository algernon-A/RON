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
		private static AutoReplaceXML autoReplaceXML;
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
			if (ModSettings.ReplaceNExt2)
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
					autoReplaceXML = AutoReplaceXML.LoadSettings();
					attemptedRead = true;
				}

				// Did we sucessfully read the auto replace file?
				if (autoReplaceXML != null)
				{
					// Yes - iterate through replacements looking for name match.
					foreach (ReplaceEntry entry in autoReplaceXML.AutoReplacements)
					{
						if (entry.targetName.Equals(name))
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
								Logging.Message("attempting to replace NExt2 network ", name, " with ", __result);

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
	}
}