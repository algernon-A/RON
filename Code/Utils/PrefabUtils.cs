using System.Collections.Generic;


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
                // Check for NEext prefabs.  NExt prefabs aren't as consistent as would be ideal....
                if (
                    prefab.m_class.name.StartsWith("NExt") ||
                    prefab.m_class.name.StartsWith("NEXT") ||
                    prefab.name.StartsWith("Small Busway") ||
                    prefab.name.EndsWith("With Bus Lanes") ||
                    prefab.name.Equals("PlainStreet2L") ||
                    prefab.name.Equals("Highway2L2W") ||
                    prefab.name.Equals("AsymHighwayL1R2")
                )
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


        /// <summary>
        /// Populates the given parent dictionaries with parent/child network info.
        /// </summary>
        /// <param name="parentDict"></param>
        internal static void GetParents(Dictionary<NetInfo, NetInfo> slopeParents, Dictionary<NetInfo, NetInfo> elevatedParents, Dictionary<NetInfo, NetInfo> bridgeParents, Dictionary<NetInfo, NetInfo> tunnelParents)
        {
            // Iterate through all loaded net prefabs.
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); ++i)
            {
                NetInfo prefab = PrefabCollection<NetInfo>.GetLoaded(i);

                if (prefab.GetAI() is RoadAI roadAI)
                {
                    // Road networks.
                    RecordParent(slopeParents, roadAI.m_slopeInfo, prefab);
                    RecordParent(elevatedParents, roadAI.m_elevatedInfo, prefab);
                    RecordParent(bridgeParents, roadAI.m_bridgeInfo, prefab);
                    RecordParent(tunnelParents, roadAI.m_tunnelInfo, prefab);
                }
                else if (prefab.GetAI() is TrainTrackAI railAI)
                {
                    // Rail networks.
                    RecordParent(slopeParents, railAI.m_slopeInfo, prefab);
                    RecordParent(elevatedParents, railAI.m_elevatedInfo, prefab);
                    RecordParent(bridgeParents, railAI.m_bridgeInfo, prefab);
                    RecordParent(tunnelParents, railAI.m_tunnelInfo, prefab);
                }
                else if (prefab.GetAI() is PedestrianPathAI pathAI)
                {
                    // Rail networks.
                    RecordParent(slopeParents, pathAI.m_slopeInfo, prefab);
                    RecordParent(elevatedParents, pathAI.m_elevatedInfo, prefab);
                    RecordParent(bridgeParents, pathAI.m_bridgeInfo, prefab);
                    RecordParent(tunnelParents, pathAI.m_tunnelInfo, prefab);
                }
            }
        }


        /// <summary>
        /// Adds a parent-child network relationship entry to the specified dictionary.
        /// </summary>
        /// <param name="dict">Dictionary <child, parent></child> to add to</param>
        /// <param name="child">Child prefab</param>
        /// <param name="parent">Parent prefab</param>
        private static void RecordParent(Dictionary<NetInfo, NetInfo> dictionary, NetInfo child, NetInfo parent)
        {
            // Null checks - child is routine, but the othes shouldn't be.
            if (child == null || parent == null || dictionary == null)
            {
                return;
            }    

            // Check for existing entry.
            if (!dictionary.ContainsKey(child))
            {
                // No existing entry - add.
                dictionary.Add(child, parent);
            }
            else
            {
                Logging.Message("parent network ", parent.name, " has duplicate child ", child.name);
            }
        }
    }
}