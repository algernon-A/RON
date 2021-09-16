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


		/// <summary>
		/// Harmony Prefix patch for BuildConfig.ResolveLegacyPrefab to substitute named networks on loading.
		/// </summary>
		/// <param name="__result">Original method result</param>
		/// <param name="name">Original network name</param>
		/// <returns>False (don't execute original method) if a replacement was found, true (continue on to original method) otherwise</returns>
		public static bool Prefix(ref string __result, string name)
		{
			// Don't do anything without being enabled.
			if (ModSettings.replaceNExt2)
			{
				// Ensure that we're only looking for networks.
				if (!new StackTrace().GetFrame(2).GetMethod().DeclaringType.ToString().Contains("[NetInfo]"))
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
							// Found a target name match; replace.
							__result = entry.replacementName;

							Logging.Message("replacing prefab ", name, " with ", __result);

							// Don't execute original method.
							return false;
						}
					}
				}
			}

			// If we got here, no substitution was performed; continue on to original method.
			return true;
		}
	}
}