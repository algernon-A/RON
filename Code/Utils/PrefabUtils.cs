namespace RON
{
    /// <summary>
    /// Static class for utilities dealing with prefabs.
    /// </summary>
    internal static class PrefabUtils
    {
        /// <summary>
        /// Returns a cleaned-up display name for the given prefab.
        /// </summary>
        /// <param name="prefab">Prefab</param>
        /// <returns>Cleaned display name</returns>
        internal static string GetDisplayName(NetInfo prefab)
        {
            string fullName = prefab.name;

            // Find any leading period (Steam package number).
            int num = fullName.IndexOf('.');

            // If no period, assume it's either vanilla or NExt
            if (num < 0)
            {
                // Check for prefab class beginning with NExt.
                if (prefab.m_class.name.StartsWith("NExt"))
                {
                    // It's a NExt asset; return full name preceeded by NExt flag.
                    return "[n] " + fullName;
                }

                // If we got here, it's vanilla; return full name preceeded by vanilla flag.
                return "[v] " + fullName;
            }

            // Otherwise, omit the package number, and trim off any trailing _Data.
            return fullName.Substring(num + 1).Replace("_Data", "");
        }


        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// Called by the settings panel fastlist.
        /// </summary>
        /// <param name="fullName">Original (raw) prefab name</param>
        /// <returns>Cleaned display name</returns>
        internal static string GetDisplayName(string fullName)
        {
            // Find any leading period (Steam package number).
            int num = fullName.IndexOf('.');

            // If no period, assume vanilla asset; return full name preceeded by vanilla flag.
            if (num < 0)
            {
                return "[v] " + fullName;
            }

            // Otherwise, omit the package number, and trim off any trailing _Data.
            return fullName.Substring(num + 1).Replace("_Data", "");
        }
    }
}