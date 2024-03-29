﻿// <copyright file="PrefabUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.Collections.Generic;
    using ColossalFramework;
    using ColossalFramework.Packaging;
    using ColossalFramework.PlatformServices;

    /// <summary>
    /// Static class for utilities dealing with prefabs.
    /// </summary>
    internal static class PrefabUtils
    {
        /// <summary>
        /// Dictionary of asset creator names.
        /// </summary>
        internal static readonly Dictionary<ulong, string> Creators = new Dictionary<ulong, string>();

        /// <summary>
        /// Dictionary of asset creator Steam ID mapping.
        /// </summary>
        internal static readonly Dictionary<ulong, ulong> CreatorMaps = new Dictionary<ulong, ulong>();

        /// <summary>
        /// Manual thumbnail mapping for vanilla elevated/bridge networks using generic thumbnails.
        /// Dictionary format is (network name (atlas name, thumbnail name)).
        /// </summary>
        internal static readonly Dictionary<string, KeyValuePair<string, string>> ThumbnailMaps = new Dictionary<string, KeyValuePair<string, string>>
        {
            { "Oneway Road Elevated", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Oneway Road Bridge", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Large Oneway Elevated", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Large Oneway Bridge", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Highway Elevated", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "Highway Bridge", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "HighwayRampElevated", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighwayramp") },
            { "Asymmetrical Three Lane Road Elevated", new KeyValuePair<string, string>("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Asymmetrical Three Lane Road Bridge", new KeyValuePair<string, string>("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Industry Road Small 01 Elevated", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Bridge", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Oneway Elevated", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Small 01 Oneway Bridge", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Medium 01 Elevated", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },
            { "Industry Road Medium 01 Bridge", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },

            // Bike roads.
            { "Basic Road Elevated Bike", new KeyValuePair<string, string>("Thumbnails", "RoadBasicBikelane") },
            { "Basic Road Bridge Bike", new KeyValuePair<string, string>("Thumbnails", "RoadBasicBikelane") },
            { "Medium Road Elevated Bike", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadMediumBike") },
            { "Medium Road Bridge Bike", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadMediumBike") },
            { "Large Road Elevated Bike", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadLArgeBike") },
            { "Large Road Bridge Bike", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadLArgeBike") },

            // Bus roads.
            { "Medium Road Elevated Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadMediumBus") },
            { "Medium Road Bridge Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadMediumBus") },
            { "Large Road Elevated Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadLargeBus") },
            { "Large Road Bridge Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadLargeBus") },

            // Tram roads.
            { "Basic Road Elevated Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Basic Road Bridge Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Oneway Road Elevated Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Oneway Road Bridge Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Medium Road Elevated Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Medium Road Bridge Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Tram Track Elevated", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Tram Track Bridge", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Oneway Tram Track Elevated", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },
            { "Oneway Tram Track Bridge", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },

            // Monorail roads.
            { "Medium Road Monorail Elevated", new KeyValuePair<string, string>("ThumbnailsExpansion4", "ThumbMonotrackRoad") },

            // Trolleybus roads.
            { "Basic Road Elevated Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Basic Road Bridge Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Oneway Road Bridge Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Oneway Road Elevated Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Medium Road Elevated Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },
            { "Medium Road Bridge Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },

            // Oneway roads.
            { "Oneway Road Slope", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Oneway Road Tunnel", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeOneway") },
            { "Asymmetrical Three Lane Road Slope", new KeyValuePair<string, string>("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Asymmetrical Three Lane Road Tunnel", new KeyValuePair<string, string>("ThumbnailsExpansion4", "ThumbnailRoadTypeAsymmetrical3lane") },
            { "Large Oneway Slope", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Large Oneway Tunnel", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeLargeOneway") },
            { "Highway Slope", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "Highway Tunnel", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighway") },
            { "HighwayRamp Slope", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighwayramp") },
            { "HighwayRamp Tunnel", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeHighwayramp") },

            // Industry roads.
            { "Industry Road Small 01 Slope", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Tunnel", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmall") },
            { "Industry Road Small 01 Oneway Slope", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Small 01 Oneway Tunnel", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustrySmallOneway") },
            { "Industry Road Medium 01 Slope", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },
            { "Industry Road Medium 01 Tunnel", new KeyValuePair<string, string>("ThumbnailsExpansion7", "ThumbRoadtypeIndustryMedium") },

            // Larger bus roads.
            { "Medium Road Slope Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadMediumBus") },
            { "Medium Road Tunnel Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadMediumBus") },
            { "Large Road Slope Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadLargeBus") },
            { "Large Road Tunnel Bus", new KeyValuePair<string, string>("Thumbnails", "ThumbRoadLargeBus") },

            // Tram roads.
            { "Basic Road Slope Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Basic Road Tunnel Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTwolaneTram") },
            { "Oneway Road Slope Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Oneway Road Tunnel Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTyperTwolaneOnewayTram") },
            { "Medium Road Slope Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Medium Road Tunnel Tram", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeFourlaneTram") },
            { "Tram Track Slope", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Tram Track Tunnel", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadTypeTramTracks") },
            { "Oneway Tram Track Slope", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },
            { "Oneway Tram Track Tunnel", new KeyValuePair<string, string>("Thumbnails", "ThumbnailRoadtypeTramtrackOneway") },

            // Trolleybus roads.
            { "Basic Road Slope Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Basic Road Tunnel Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneTrolleybus") },
            { "Oneway Road Slope Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Oneway Road Tunnel Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeTwolaneOnewayTrolleybus") },
            { "Medium Road Slope Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },
            { "Medium Road Tunnel Trolleybus", new KeyValuePair<string, string>("ThumbnailsExpansion9", "ThumbnailRoadTypeFourlaneTrolleybus") },

            // NExt2 roads.
            { "BasicRoadMdn Bridge", new KeyValuePair<string, string>("BASICROADMDN", "BASICROADMDN") },
            { "BasicRoadMdn Elevated", new KeyValuePair<string, string>("BASICROADMDN", "BASICROADMDN") },
            { "BasicRoadMdn Slope", new KeyValuePair<string, string>("BASICROADMDN", "BASICROADMDN") },
            { "BasicRoadMdn Tunnel", new KeyValuePair<string, string>("BASICROADMDN", "BASICROADMDN") },
            { "BasicRoadPntMdn Bridge", new KeyValuePair<string, string>("BASICROADPNTMDN", "BASICROADPNTMDN") },
            { "BasicRoadPntMdn Elevated", new KeyValuePair<string, string>("BASICROADPNTMDN", "BASICROADPNTMDN") },
            { "BasicRoadPntMdn Slope", new KeyValuePair<string, string>("BASICROADMDN", "BASICROADMDN") },
            { "BasicRoadPntMdn Tunnel", new KeyValuePair<string, string>("BASICROADMDN", "BASICROADMDN") },
            { "BasicRoadTL Bridge", new KeyValuePair<string, string>("BASICROADTL", "BASICROADTL") },
            { "BasicRoadTL Elevated", new KeyValuePair<string, string>("BASICROADTL", "BASICROADTL") },
            { "BasicRoadTL Slope", new KeyValuePair<string, string>("BASICROADTL", "BASICROADTL") },
            { "BasicRoadTL Tunnel", new KeyValuePair<string, string>("BASICROADTL", "BASICROADTL") },

            // NExt2 oneway roads.
            { "Oneway3L Bridge", new KeyValuePair<string, string>("ONEWAY3L", "ONEWAY3L") },
            { "Oneway3L Elevated", new KeyValuePair<string, string>("ONEWAY3L", "ONEWAY3L") },
            { "Oneway3L Slope", new KeyValuePair<string, string>("ONEWAY3L", "ONEWAY3L") },
            { "Oneway3L Tunnel", new KeyValuePair<string, string>("ONEWAY3L", "ONEWAY3L") },
            { "Oneway4L Bridge", new KeyValuePair<string, string>("ONEWAY4L", "ONEWAY4L") },
            { "Oneway4L Elevated", new KeyValuePair<string, string>("ONEWAY4L", "ONEWAY4L") },
            { "Oneway4L Slope", new KeyValuePair<string, string>("ONEWAY4L", "ONEWAY4L") },
            { "Oneway4L Tunnel", new KeyValuePair<string, string>("ONEWAY4L", "ONEWAY4L") },

            // NExt2 avenues.
            { "Small Avenue Bridge", new KeyValuePair<string, string>("SMALL_AVENUE", "SMALL_AVENUE") },
            { "Small Avenue Elevated", new KeyValuePair<string, string>("SMALL_AVENUE", "SMALL_AVENUE") },
            { "Small Avenue Slope", new KeyValuePair<string, string>("SMALL_AVENUE", "SMALL_AVENUE") },
            { "Small Avenue Tunnel", new KeyValuePair<string, string>("SMALL_AVENUE", "SMALL_AVENUE") },
            { "Medium Avenue Bridge", new KeyValuePair<string, string>("MEDIUM_AVENUE", "MEDIUM_AVENUE") },
            { "Medium Avenue Elevated", new KeyValuePair<string, string>("MEDIUM_AVENUE", "MEDIUM_AVENUE") },
            { "Medium Avenue Slope", new KeyValuePair<string, string>("MEDIUM_AVENUE", "MEDIUM_AVENUE") },
            { "Medium Avenue Tunnel", new KeyValuePair<string, string>("MEDIUM_AVENUE", "MEDIUM_AVENUE") },
            { "Medium Avenue TL Bridge", new KeyValuePair<string, string>("MEDIUM_AVENUE_TL", "MEDIUM_AVENUE_TL") },
            { "Medium Avenue TL Elevated", new KeyValuePair<string, string>("MEDIUM_AVENUE_TL", "MEDIUM_AVENUE_TL") },
            { "Medium Avenue TL Slope", new KeyValuePair<string, string>("MEDIUM_AVENUE_TL", "MEDIUM_AVENUE_TL") },
            { "Medium Avenue TL Tunnel", new KeyValuePair<string, string>("MEDIUM_AVENUE_TL", "MEDIUM_AVENUE_TL") },
            { "Six-Lane Avenue Median Bridge", new KeyValuePair<string, string>("SIX_LANE_AVENUE_MEDIAN", "SIX_LANE_AVENUE_MEDIAN") },
            { "Six-Lane Avenue Median Elevated", new KeyValuePair<string, string>("SIX_LANE_AVENUE_MEDIAN", "SIX_LANE_AVENUE_MEDIAN") },
            { "Six-Lane Avenue Median Slope", new KeyValuePair<string, string>("SIX_LANE_AVENUE_MEDIAN", "SIX_LANE_AVENUE_MEDIAN") },
            { "Six-Lane Avenue Median Tunnel", new KeyValuePair<string, string>("SIX_LANE_AVENUE_MEDIAN", "SIX_LANE_AVENUE_MEDIAN") },
            { "Eight-Lane Avenue Bridge", new KeyValuePair<string, string>("EIGHT_LANE_AVENUE", "EIGHT_LANE_AVENUE") },
            { "Eight-Lane Avenue Elevated", new KeyValuePair<string, string>("EIGHT_LANE_AVENUE", "EIGHT_LANE_AVENUE") },
            { "Eight-Lane Avenue Slope", new KeyValuePair<string, string>("EIGHT_LANE_AVENUE", "EIGHT_LANE_AVENUE") },
            { "Eight-Lane Avenue Tunnel", new KeyValuePair<string, string>("EIGHT_LANE_AVENUE", "EIGHT_LANE_AVENUE") },

            // NExt2 highways.
            { "Small Rural Highway Bridge", new KeyValuePair<string, string>("SMALL_RURAL_HIGHWAY", "SMALL_RURAL_HIGHWAY") },
            { "Small Rural Highway Elevated", new KeyValuePair<string, string>("SMALL_RURAL_HIGHWAY", "SMALL_RURAL_HIGHWAY") },
            { "Small Rural Highway Slope", new KeyValuePair<string, string>("RURAL_HIGHWAY", "RURAL_HIGHWAY") },
            { "Small Rural Highway Tunnel", new KeyValuePair<string, string>("RURAL_HIGHWAY", "RURAL_HIGHWAY") },
            { "Rural Highway Bridge", new KeyValuePair<string, string>("RURAL_HIGHWAY", "RURAL_HIGHWAY") },
            { "Rural Highway Elevated", new KeyValuePair<string, string>("RURAL_HIGHWAY", "RURAL_HIGHWAY") },
            { "Rural Highway Slope", new KeyValuePair<string, string>("RURAL_HIGHWAY", "RURAL_HIGHWAY") },
            { "Rural Highway Tunnel", new KeyValuePair<string, string>("RURAL_HIGHWAY", "RURAL_HIGHWAY") },
            { "Highway2L2W Bridge", new KeyValuePair<string, string>("HIGHWAY2L2W", "HIGHWAY2L2W") },
            { "Highway2L2W Elevated", new KeyValuePair<string, string>("HIGHWAY2L2W", "HIGHWAY2L2W") },
            { "Highway2L2W Slope", new KeyValuePair<string, string>("HIGHWAY2L2W", "HIGHWAY2L2W") },
            { "Highway2L2W Tunnel", new KeyValuePair<string, string>("HIGHWAY2L2W", "HIGHWAY2L2W") },
            { "Four-Lane Highway Bridge", new KeyValuePair<string, string>("FOUR_LANE_HIGHWAY", "FOUR_LANE_HIGHWAY") },
            { "Four-Lane Highway Elevated", new KeyValuePair<string, string>("FOUR_LANE_HIGHWAY", "FOUR_LANE_HIGHWAY") },
            { "Four-Lane Highway Slope", new KeyValuePair<string, string>("FIVE_LANE_HIGHWAY", "FIVE_LANE_HIGHWAY") },
            { "Four-Lane Highway Tunnel", new KeyValuePair<string, string>("FIVE_LANE_HIGHWAY", "FIVE_LANE_HIGHWAY") },
            { "Five-Lane Highway Bridge", new KeyValuePair<string, string>("FIVE_LANE_HIGHWAY", "FIVE_LANE_HIGHWAY") },
            { "Five-Lane Highway Elevated", new KeyValuePair<string, string>("FIVE_LANE_HIGHWAY", "FIVE_LANE_HIGHWAY") },
            { "Five-Lane Highway Slope", new KeyValuePair<string, string>("FIVE_LANE_HIGHWAY", "FIVE_LANE_HIGHWAY") },
            { "Five-Lane Highway Tunnel", new KeyValuePair<string, string>("FIVE_LANE_HIGHWAY", "FIVE_LANE_HIGHWAY") },
            { "Large Highway Bridge", new KeyValuePair<string, string>("LARGE_HIGHWAY", "LARGE_HIGHWAY") },
            { "Large Highway Elevated", new KeyValuePair<string, string>("LARGE_HIGHWAY", "LARGE_HIGHWAY") },
            { "Large Highway Slope", new KeyValuePair<string, string>("LARGE_HIGHWAY", "LARGE_HIGHWAY") },
            { "Large Highway Tunnel", new KeyValuePair<string, string>("LARGE_HIGHWAY", "LARGE_HIGHWAY") },

            // NExt2 asymmetrical roads.
            { "AsymRoadL1R2 Bridge", new KeyValuePair<string, string>("ASYMROADL1R2", "ASYMROADL1R2") },
            { "AsymRoadL1R2 Elevated", new KeyValuePair<string, string>("ASYMROADL1R2", "ASYMROADL1R2") },
            { "AsymRoadL1R2 Slope", new KeyValuePair<string, string>("ASYMROADL1R2", "ASYMROADL1R2") },
            { "AsymRoadL1R2 Tunnel", new KeyValuePair<string, string>("ASYMROADL1R2", "ASYMROADL1R2") },
            { "AsymRoadL1R3 Bridge", new KeyValuePair<string, string>("ASYMROADL1R3", "ASYMROADL1R3") },
            { "AsymRoadL1R3 Elevated", new KeyValuePair<string, string>("ASYMROADL1R3", "ASYMROADL1R3") },
            { "AsymRoadL1R3 Slope", new KeyValuePair<string, string>("ASYMROADL1R3", "ASYMROADL1R3") },
            { "AsymRoadL1R3 Tunnel", new KeyValuePair<string, string>("ASYMROADL1R3", "ASYMROADL1R3") },
            { "AsymAvenueL2R3 Bridge", new KeyValuePair<string, string>("ASYMAVENUEL2R3", "ASYMAVENUEL2R3") },
            { "AsymAvenueL2R3 Elevated", new KeyValuePair<string, string>("ASYMAVENUEL2R3", "ASYMAVENUEL2R3") },
            { "AsymAvenueL2R3 Slope", new KeyValuePair<string, string>("ASYMAVENUEL2R3", "ASYMAVENUEL2R3") },
            { "AsymAvenueL2R3 Tunnel", new KeyValuePair<string, string>("ASYMAVENUEL2R3", "ASYMAVENUEL2R3") },
            { "AsymAvenueL2R4 Bridge", new KeyValuePair<string, string>("ASYMAVENUEL2R4", "ASYMAVENUEL2R4") },
            { "AsymAvenueL2R4 Elevated", new KeyValuePair<string, string>("ASYMAVENUEL2R4", "ASYMAVENUEL2R4") },
            { "AsymAvenueL2R4 Slope", new KeyValuePair<string, string>("ASYMAVENUEL2R4", "ASYMAVENUEL2R4") },
            { "AsymAvenueL2R4 Tunnel", new KeyValuePair<string, string>("ASYMAVENUEL2R4", "ASYMAVENUEL2R4") },
            { "AsymHighwayL1R2 Bridge", new KeyValuePair<string, string>("ASYMHIGHWAYL1R2", "ASYMHIGHWAYL1R2") },
            { "AsymHighwayL1R2 Elevated", new KeyValuePair<string, string>("ASYMHIGHWAYL1R2", "ASYMHIGHWAYL1R2") },
            { "AsymHighwayL1R2 Slope", new KeyValuePair<string, string>("ASYMHIGHWAYL1R2", "ASYMHIGHWAYL1R2") },
            { "AsymHighwayL1R2 Tunnel", new KeyValuePair<string, string>("ASYMHIGHWAYL1R2", "ASYMHIGHWAYL1R2") },

            // NExt2 busways.
            { "Small Busway Bridge", new KeyValuePair<string, string>("SMALL_BUSWAY", "SMALL_BUSWAY") },
            { "Small Busway Elevated", new KeyValuePair<string, string>("SMALL_BUSWAY", "SMALL_BUSWAY") },
            { "Small Busway Slope", new KeyValuePair<string, string>("SMALL_BUSWAY", "SMALL_BUSWAY") },
            { "Small Busway Tunnel", new KeyValuePair<string, string>("SMALL_BUSWAY", "SMALL_BUSWAY") },
            { "Small Busway OneWay Bridge", new KeyValuePair<string, string>("SMALL_BUSWAY_ONEWAY", "SMALL_BUSWAY_ONEWAY") },
            { "Small Busway OneWay Elevated", new KeyValuePair<string, string>("SMALL_BUSWAY_ONEWAY", "SMALL_BUSWAY_ONEWAY") },
            { "Small Busway OneWay Slope", new KeyValuePair<string, string>("SMALL_BUSWAY_ONEWAY", "SMALL_BUSWAY_ONEWAY") },
            { "Small Busway OneWay Tunnel", new KeyValuePair<string, string>("SMALL_BUSWAY_ONEWAY", "SMALL_BUSWAY_ONEWAY") },
            { "Large Road Bridge With Bus Lanes", new KeyValuePair<string, string>("LARGE_ROAD_WITH_BUS_LANES", "LARGE_ROAD_WITH_BUS_LANES") },
            { "Large Road Elevated With Bus Lanes", new KeyValuePair<string, string>("LARGE_ROAD_WITH_BUS_LANES", "LARGE_ROAD_WITH_BUS_LANES") },
            { "Large Road Slope With Bus Lanes", new KeyValuePair<string, string>("LARGE_ROAD_WITH_BUS_LANES", "LARGE_ROAD_WITH_BUS_LANES") },
            { "Large Road Tunnel With Bus Lanes", new KeyValuePair<string, string>("LARGE_ROAD_WITH_BUS_LANES", "LARGE_ROAD_WITH_BUS_LANES") },

            // NExt2 pedestrian roads.
            { "Zonable Pedestrian Elevated", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_PAVEMENT", "ZONABLE_PEDESTRIAN_PAVEMENT") },
            { "Zonable Pedestrian Gravel Elevated", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_GRAVEL", "ZONABLE_PEDESTRIAN_GRAVEL") },
            { "Zonable Pedestrian Gravel Tiny Elevated", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_GRAVEL_TINY", "ZONABLE_PEDESTRIAN_GRAVEL_TINY") },
            { "Zonable Pedestrian Pavement Tiny Elevated", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_PAVEMENT_TINY", "ZONABLE_PEDESTRIAN_PAVEMENT_TINY") },
            { "Zonable Pedestrian Boardwalk Tiny Elevated", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_BOARDWALK_TINY", "ZONABLE_PEDESTRIAN_BOARDWALK_TINY") },
            { "Zonable Pedestrian Stone Tiny Road Bridge", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_STONE_TINY_ROAD", "ZONABLE_PEDESTRIAN_STONE_TINY_ROAD") },
            { "Zonable Pedestrian Stone Tiny Road Elevated", new KeyValuePair<string, string>("ZONABLE_PEDESTRIAN_STONE_TINY_ROAD", "ZONABLE_PEDESTRIAN_STONE_TINY_ROAD") },
            { "Zonable Promenade Elevated", new KeyValuePair<string, string>("ZONABLE_PROMENADE", "ZONABLE_PROMENADE") },
        };

        /// <summary>
        /// Populates the creators dictionary.
        /// </summary>
        internal static void GetCreators()
        {
            // Iterate through all loaded packages.
            foreach (Package.Asset asset in PackageManager.FilterAssets(new Package.AssetType[] { UserAssetType.CustomAssetMetaData }))
            {
                if (asset?.package != null)
                {
                    // Try to get steam ID of this package.
                    if (ulong.TryParse(asset.package.packageName, out ulong steamID) && !asset.package.packageAuthor.IsNullOrWhiteSpace())
                    {
                        // Check to see if we already have a record for the steam ID.
                        if (!Creators.ContainsKey(steamID))
                        {
                            // No existing record - get package author name and add to dictionary.
                            if (ulong.TryParse(asset.package.packageAuthor.Substring("steamid:".Length), out ulong authorID))
                            {
                                Creators.Add(steamID, new Friend(new UserID(authorID)).personaName);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the creator of the given network.
        /// </summary>
        /// <param name="network">Network to check.</param>
        /// <returns>Creator name.</returns>
        internal static string GetCreator(NetInfo network)
        {
            // See if we can parse network workshop number from network name (number before period).
            int period = network.name.IndexOf(".");
            if (period > 0)
            {
                // Attempt to parse substring before period.
                if (ulong.TryParse(network.name.Substring(0, period), out ulong steamID))
                {
                    // Check to see if we have an entry.
                    if (Creators.ContainsKey(steamID))
                    {
                        return Creators[steamID];
                    }
                }
            }

            // If we got here, we didn't find a valid creator.
            return null;
        }

        /// <summary>
        /// Populates the given parent dictionaries with parent/child network info.
        /// </summary>
        /// <param name="slopeParents">Slope parent dictionary.</param>
        /// <param name="elevatedParents">Elevated parent dictionary.</param>
        /// <param name="bridgeParents">Bridge parent dictionary.</param>
        /// <param name="tunnelParents">Tunnel parent dictionary.</param>
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
        /// Checks to see if the given network prefab is a station network.
        /// </summary>
        /// <param name="network">Network prefab to check.</param>
        /// <returns>True if this is a station network, false otherwise.</returns>
        internal static bool IsStation(NetInfo network)
        {
            // Check to see if this an eligible station network type (TranTrackBase [includes Monorail] or TranTrackBase).
            PrefabAI netAI = network.GetAI();
            if (netAI is TrainTrackBaseAI || netAI is MetroTrackBaseAI)
            {
                // Check lanes for stations.
                foreach (NetInfo.Lane lane in network.m_lanes)
                {
                    // Station networks are have a stop type.
                    if (lane.m_stopType != VehicleInfo.VehicleType.None)
                    {
                        // Found a station; return true.
                        return true;
                    }
                }
            }

            // If we got here, we didn't find a station; return false.
            return false;
        }

        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// Called by the settings panel fastlist.
        /// </summary>
        /// <param name="prefab">Original (raw) prefab.</param>
        /// <returns>Cleaned display name.</returns>
        internal static string GetDisplayName(PrefabInfo prefab)
        {
            // Null check.
            if (prefab?.name == null)
            {
                return "null";
            }

            // If not custom content, return full name.
            if (!prefab.m_isCustomContent)
            {
                return prefab.name;
            }

            // Otherwise, omit the package number, and trim off any trailing _Data.
            int index = prefab.name.IndexOf('.');
            return prefab.name.Substring(index + 1).Replace("_Data", string.Empty);
        }

        /// <summary>
        /// Adds a parent-child network relationship entry to the specified dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary (child, parent) to add to.</param>
        /// <param name="child">Child prefab.</param>
        /// <param name="parent">Parent prefab.</param>
        private static void RecordParent(Dictionary<NetInfo, NetInfo> dictionary, NetInfo child, NetInfo parent)
        {
            // Null checks - child is routine, but the others shouldn't be.
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