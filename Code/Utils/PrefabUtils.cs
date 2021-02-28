using System.Collections.Generic;


namespace RON
{
    /// <summary>
    /// Static class for utilities dealing with prefabs.
    /// </summary>
    internal static class PrefabUtils
    {
        // Manual thumbnail mapping for vanilla elevated/bridge networks using generic thumbnails.
        // Dictionary format is <network name <atlas name, thumbnail name>>
        internal readonly static Dictionary<string, KeyValuePair<string, string>> thumbnailMaps = new Dictionary<string, KeyValuePair<string, string>>
        {
            { "Oneway Road Elevated", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Oneway Road Bridge", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Large Oneway Elevated", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Large Oneway Bridge", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Highway Elevated", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "Highway Bridge", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "HighwayRampElevated", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighwayramp") },
            { "Asymmetrical Three Lane Road Elevated", new KeyValuePair<string, string> ("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Asymmetrical Three Lane Road Bridge", new KeyValuePair<string, string> ("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Industry Road Small 01 Elevated", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Bridge", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Oneway Elevated", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Small 01 Oneway Bridge", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Medium 01 Elevated", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },
            { "Industry Road Medium 01 Bridge", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },

            { "Basic Road Elevated Bike", new KeyValuePair<string, string> ("Thumbnails", "RoadBasicBikelane") },
            { "Basic Road Bridge Bike", new KeyValuePair<string, string> ("Thumbnails", "RoadBasicBikelane") },
            { "Medium Road Elevated Bike", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadMediumBike") },
            { "Medium Road Bridge Bike", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadMediumBike") },
            { "Large Road Elevated Bike", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadLArgeBike") },
            { "Large Road Bridge Bike", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadLArgeBike") },

            { "Medium Road Elevated Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadMediumBus") },
            { "Medium Road Bridge Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadMediumBus") },
            { "Large Road Elevated Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadLargeBus") },
            { "Large Road Bridge Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadLargeBus") },

            { "Basic Road Elevated Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Basic Road Bridge Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Oneway Road Elevated Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Oneway Road Bridge Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Medium Road Elevated Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Medium Road Bridge Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Tram Track Elevated", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Tram Track Bridge", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Oneway Tram Track Elevated", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },
            { "Oneway Tram Track Bridge", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },

            { "Medium Road Monorail Elevated", new KeyValuePair<string, string> ("ThumbnailsExpansion4", "ThumbMonotrackRoad") },

            { "Basic Road Elevated Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Basic Road Bridge Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Oneway Road Bridge Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Oneway Road Elevated Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Medium Road Elevated Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },
            { "Medium Road Bridge Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },

            { "Oneway Road Slope", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Oneway Road Tunnel", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Asymmetrical Three Lane Road Slope", new KeyValuePair<string, string> ("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Asymmetrical Three Lane Road Tunnel", new KeyValuePair<string, string> ("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Large Oneway Slope", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Large Oneway Tunnel", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Highway Slope", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "Highway Tunnel", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "HighwayRamp Slope", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighwayramp") },
            { "HighwayRamp Tunnel", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeHighwayramp") },

            { "Industry Road Small 01 Slope", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Tunnel", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Oneway Slope", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Small 01 Oneway Tunnel", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Medium 01 Slope", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },
            { "Industry Road Medium 01 Tunnel", new KeyValuePair<string, string> ("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },

            { "Medium Road Slope Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadMediumBus") },
            { "Medium Road Tunnel Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadMediumBus") },
            { "Large Road Slope Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadLargeBus") },
            { "Large Road Tunnel Bus", new KeyValuePair<string, string> ("Thumbnails", "ThumbRoadLargeBus") },

            { "Basic Road Slope Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Basic Road Tunnel Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Oneway Road Slope Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Oneway Road Tunnel Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Medium Road Slope Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Medium Road Tunnel Tram", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Tram Track Slope", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Tram Track Tunnel", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Oneway Tram Track Slope", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },
            { "Oneway Tram Track Tunnel", new KeyValuePair<string, string> ("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },

            { "Basic Road Slope Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Basic Road Tunnel Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Oneway Road Slope Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Oneway Road Tunnel Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Medium Road Slope Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },
            { "Medium Road Tunnel Trolleybus", new KeyValuePair<string, string> ("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },
        };


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
        }
    }
}