﻿// <copyright file="StationPanelBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;

    /// <summary>
    /// RON station track replacer panel for placing stations.
    /// </summary>
    /// <typeparam name="TRow">Target UIList row type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Protected internal fields")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Protected internal fields")]
    internal class StationPanelBase<TRow> : StandalonePanel
        where TRow : NetRow
    {
        /// <summary>
        /// Panel width.
        /// </summary>
        protected internal const float CalculatedPanelWidth = RightPanelX + ListWidth + Margin;

        /// <summary>
        /// Currently selected building.
        /// </summary>
        protected internal static BuildingInfo s_currentBuilding;

        /// <summary>
        /// List of eligible networks in the current building.
        /// </summary>
        protected internal static List<PathIndex> s_eligibleNets = new List<PathIndex>();

        /// <summary>
        /// Selected network index number.
        /// </summary>
        protected internal PathIndex m_selectedIndex;

        // Layout constants - private.
        private const float TitleHeight = 50f;
        private const float ControlHeight = 30f;
        private const float ControlY = TitleHeight;
        private const float ListHeight = 6 * NetRow.CustomRowHeight;
        private const float ListY = ControlY + ControlHeight;
        private const float LeftX = Margin;
        private const float ListWidth = 450f;
        private const float RightPanelX = LeftX + ListWidth + Margin;
        private const float Check1X = RightPanelX;
        private const float Check2X = RightPanelX + (ListWidth / 2f);
        private const float CalculatedPanelHeight = ListY + ListHeight + Margin;

        // Panel components.
        private UICheckBox _trainCheck;
        private UICheckBox _metroCheck;
        private UICheckBox _sameWidthCheck;
        private UIList _targetList;
        private UIList _loadedList;

        /// <summary>
        /// Network type selection enum.
        /// </summary>
        private enum TypeIndex : int
        {
            RailOnly = 0,
            MetroOnly,
            RailMetro,
        }

        /// <summary>
        /// Gets the panel width.
        /// </summary>
        public override float PanelWidth => CalculatedPanelWidth;

        /// <summary>
        /// Gets the panel height.
        /// </summary>
        public override float PanelHeight => CalculatedPanelHeight;

        /// <summary>
        /// Gets or sets the selected target index.  Called by target network list items.
        /// </summary>
        internal PathIndex SelectedIndex
        {
            get => m_selectedIndex;

            set
            {
                // Confirm target index validity before setting.
                if (s_eligibleNets.Contains(value))
                {
                    m_selectedIndex = value;
                }
                else
                {
                    // Invalid selection; invalidate this entry.
                    m_selectedIndex.Invalidate();
                }

                // Regenerate loaded list on selection change.
                LoadedList();
            }
        }

        /// <summary>
        /// Sets selected replacement.  Called by target network list items.
        /// </summary>
        internal virtual NetInfo SelectedReplacement
        {
            set
            {
                // Assign replacement network, if we've got a valid selection.
                if (m_selectedIndex.IsValid)
                {
                    if (m_selectedIndex.m_subBuilding >= 0)
                    {
                        // Sub-building.
                        s_currentBuilding.m_subBuildings[m_selectedIndex.m_subBuilding].m_buildingInfo.m_paths[m_selectedIndex.m_pathIndex].m_finalNetInfo = value;
                    }
                    else
                    {
                        // Main building.
                        s_currentBuilding.m_paths[m_selectedIndex.m_pathIndex].m_finalNetInfo = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the panel's title.
        /// </summary>
        protected override string PanelTitle => Translations.Translate("RON_NAM");

        /// <summary>
        /// Gets the selected target network as NetInfo.
        /// </summary>
        protected virtual NetInfo TargetNet => GetPathInfo(m_selectedIndex)?.m_netInfo;

        /// <summary>
        /// Called by Unity when the object is created.
        /// Used to perform setup.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Title label.
            SetTitle();

            // Decorative icon (top-left).
            SetIcon(UITextures.LoadQuadSpriteAtlas("RonButton"), "normal");

            // Same width only check.
            _sameWidthCheck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, ControlY + 5f, Translations.Translate("RON_PNL_WID"));
            _sameWidthCheck.isChecked = true;
            _sameWidthCheck.eventCheckChanged += (c, isChecked) => LoadedList();

            _trainCheck = UICheckBoxes.AddLabelledCheckBox(this, Check2X, ControlY - 20f, Translations.Translate("RON_STA_SRT"));
            _trainCheck.eventCheckChanged += (c, isChecked) => LoadedList();

            _metroCheck = UICheckBoxes.AddLabelledCheckBox(this, Check2X, ControlY, Translations.Translate("RON_STA_SMT"));
            _metroCheck.eventCheckChanged += (c, isChecked) => LoadedList();

            // Target network list.
            _targetList = UIList.AddUIList<TRow>(this, Margin, ListY, ListWidth, ListHeight, NetRow.CustomRowHeight);
            _targetList.EventSelectionChanged += (c, selectedItem) =>
            {
                if (selectedItem is PathIndex pathIndex)
                {
                    SelectedIndex = pathIndex;
                }
                else
                {
                    SelectedIndex.Invalidate();
                }
            };

            // Loaded network list.
            _loadedList = UIList.AddUIList<NetRow>(this, RightPanelX, ListY, ListWidth, ListHeight, NetRow.CustomRowHeight);
            _loadedList.EventSelectionChanged += (c, selectedItem) => SelectedReplacement = (selectedItem as NetRowItem)?.Prefab;
        }

        /// <summary>
        /// Set the target building (first checking validity).
        /// </summary>
        /// <param name="selectedBuilding">Selected station building.</param>
        internal static void SetTarget(BuildingInfo selectedBuilding)
        {
            // Don't do anything if selection hasn't changed (this includes after the panel has been closed while the station building is still selected).
            if (selectedBuilding == s_currentBuilding)
            {
                return;
            }

            // Update current reference.
            s_currentBuilding = selectedBuilding;

            // Reset eligible network list
            s_eligibleNets.Clear();

            // Iterate through each path in building.
            if (selectedBuilding?.m_paths != null)
            {
                for (int i = 0; i < selectedBuilding.m_paths.Length; ++i)
                {
                    if (selectedBuilding.m_paths[i] != null)
                    {
                        // Check for matching track.
                        NetAI netAI = selectedBuilding.m_paths[i].m_netInfo.m_netAI;
                        if (netAI is TrainTrackBaseAI || netAI is MetroTrackBaseAI)
                        {
                            // Found a railway track - add index to list.
                            s_eligibleNets.Add(new PathIndex(-1, i));
                        }
                    }
                }
            }

            // Check sub-building.
            if (selectedBuilding?.m_subBuildings != null)
            {
                for (int i = 0; i < selectedBuilding.m_subBuildings.Length; ++i)
                {
                    BuildingInfo.SubInfo subBuilding = selectedBuilding.m_subBuildings[i];
                    if (subBuilding != null && subBuilding.m_buildingInfo.m_paths != null)
                    {
                        for (int pathIndex = 0; pathIndex < subBuilding.m_buildingInfo.m_paths.Length; ++pathIndex)
                        {
                            if (subBuilding.m_buildingInfo.m_paths[pathIndex] != null)
                            {
                                // Check for matching track.
                                NetAI netAI = subBuilding.m_buildingInfo.m_paths[pathIndex].m_netInfo.m_netAI;
                                if (netAI is TrainTrackBaseAI || netAI is MetroTrackBaseAI)
                                {
                                    // Found a railway track - add index to list.
                                    s_eligibleNets.Add(new PathIndex(i, pathIndex));
                                }
                            }
                        }
                    }
                }
            }

            // If no eligible nets were found, exit.
            if (s_eligibleNets.Count == 0)
            {
                // Close panel if already open.
                StandalonePanelManager<StationPanel>.Panel?.Close();
                return;
            }

            // Create panel and set initial values.
            StandalonePanelManager<StationPanel>.Create();
            if (StandalonePanelManager<StationPanel>.Panel is StationPanel stationPanel)
            {
                stationPanel.SetTitle();
                stationPanel.TargetList();
                stationPanel.SetDefaultChecks(selectedBuilding);
            }
        }

        /// <summary>
        /// Gets the <see cref="NetInfo"/> prefab indicated by the given <see cref="PathIndex"/> against the currently selected building.
        /// </summary>
        /// <param name="pathIndex"><see cref="PathIndex"/> record.</param>
        /// <returns><see cref="NetInfo"/> prefab, or <c>null</c> if none.</returns>
        internal NetInfo IndexedNet(PathIndex pathIndex) => GetPathInfo(pathIndex)?.m_netInfo;

        /// <summary>
        /// Populates a fastlist with a list of eligible networks in the current building.
        /// </summary>
        protected void TargetList()
        {
            // Create return fastlist from our list of eligible networks.
            _targetList.Data = new FastList<object>
            {
                m_buffer = s_eligibleNets.Select(x => (object)x).ToArray(),
                m_size = s_eligibleNets.Count,
            };

            // Clear selection.
            m_selectedIndex.Invalidate();
        }

        /// <summary>
        /// Populates a fastlist with a list of relevant loaded networks.
        /// </summary>
        protected void LoadedList()
        {
            // Clear list if there's no current selection.
            if (!m_selectedIndex.IsValid)
            {
                _loadedList.Clear();
                return;
            }

            // Ensure valid target selection before proceeding.
            NetInfo currentNetInfo = TargetNet;
            if (currentNetInfo == null)
            {
                _loadedList.Clear();
                return;
            }

            // Station status of currently selected track.
            bool isStation = PrefabUtils.IsStation(currentNetInfo);
            NetAI currentNetAI = currentNetInfo.m_netAI;
            Type currentAIType = currentNetAI.GetType();

            // Elevated station tracks from Extra Train Station Tracks need special handling, as they don't use the TrainTrackBridgeAI.
            if (currentNetAI is TrainTrackAI trackAI && currentNetInfo.name.StartsWith("Station Track Eleva"))
            {
                currentAIType = typeof(TrainTrackBridgeAI);
            }

            // List of prefabs to display.
            List<NetRowItem> netList = new List<NetRowItem>();

            // Iterate through all loaded networks.
            for (uint i = 0u; i < PrefabCollection<NetInfo>.LoadedCount(); ++i)
            {
                // Get network and add to our list, if it isn't null.
                NetInfo network = PrefabCollection<NetInfo>.GetLoaded(i);
                if (network?.name != null)
                {
                    // Create new NetRowItem from this network.
                    NetRowItem newItem = new NetRowItem(network);

                    // Check if this network has the same half-width, if the checkbox is selected.
                    if (_sameWidthCheck.isChecked && network.m_halfWidth != TargetNet.m_halfWidth)
                    {
                        // No match; skip this one.
                        continue;
                    }

                    // Apply station filter.
                    if (isStation != newItem.IsStation)
                    {
                        continue;
                    }

                    // Check network type filters.
                    Type candidateType = network.m_netAI.GetType();
                    if (candidateType.IsSubclassOf(typeof(TrainTrackBaseAI)))
                    {
                        // Don't include train tracks unless train checkbox is selected.
                        if (!_trainCheck.isChecked)
                        {
                            continue;
                        }
                    }
                    else if (candidateType.IsSubclassOf(typeof(MetroTrackBaseAI)))
                    {
                        // Don't include metro tracks unless metro checkbox is selected.
                        if (!_metroCheck.isChecked)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // Unsupported network type.
                        continue;
                    }

                    // Additional checks if the two AI types don't match perfectly.
                    if (currentAIType != candidateType)
                    {
                        // Elevated station tracks from Extra Train Station Tracks need special handling, as they don't use the TrainTrackBridgeAI.
                        if (!(network.name.StartsWith("Station Track Eleva") && currentAIType == typeof(TrainTrackBridgeAI)))
                        {
                            // Still no match - check for metro-train match for station track types.
                            if (!MatchTrainMetro(currentAIType, candidateType, network))
                            {
                                continue;
                            }
                        }
                    }

                    // Passed filtering; add this one to the list.
                    netList.Add(newItem);
                }
            }

            // Create return fastlist from our filtered list, ordering by name.
            _loadedList.Data = new FastList<object>
            {
                m_buffer = netList.OrderBy(item => item.DisplayName).ToArray(),
                m_size = netList.Count,
            };
        }

        /// <summary>
        /// Sets the type menu index.
        /// </summary>
        /// <param name="buildingInfo">Currently selected prefab.</param>
        protected void SetDefaultChecks(BuildingInfo buildingInfo)
        {
            // Record previous states.
            bool oldTrainCheckState = _trainCheck.isChecked;
            bool oldMetroCheckState = _metroCheck.isChecked;

            // Set new states.
            _trainCheck.isChecked = buildingInfo.GetSubService() != ItemClass.SubService.PublicTransportMetro;
            _metroCheck.isChecked = !_trainCheck.isChecked;

            // Force LoadedList update if no change in states (so event handler wasn't triggered).
            if (oldTrainCheckState == _trainCheck.isChecked && oldMetroCheckState == _metroCheck.isChecked)
            {
                LoadedList();
            }
        }

        /// <summary>
        /// Sets the panel title, including the building name.
        /// </summary>
        protected void SetTitle() =>
            TitleText = (s_currentBuilding?.name != null ? PrefabUtils.GetDisplayName(s_currentBuilding)
            : Translations.Translate("RON_NAM")) + ": " + Translations.Translate("RON_STA_CUS");

        /// <summary>
        /// Matches train tracks to equivalent metro tracks (and vice-versa).
        /// </summary>
        /// <param name="currentType">Selected track AI type.</param>
        /// <param name="candidateType">Candidate track AI type.</param>
        /// <param name="candidateInfo">Candidate track NetInfo.</param>
        /// <returns>True if the two provided types are matched, false otherwise.</returns>
        private bool MatchTrainMetro(Type currentType, Type candidateType, NetInfo candidateInfo)
        {
            return
                (candidateType == typeof(TrainTrackAI) && currentType == typeof(MetroTrackAI)) ||
                (candidateType == typeof(TrainTrackBridgeAI) && currentType == typeof(MetroTrackBridgeAI)) ||
                (candidateType == typeof(MetroTrackAI) && currentType == typeof(TrainTrackAI)) ||
                (candidateType == typeof(MetroTrackBridgeAI) && currentType == typeof(TrainTrackBridgeAI)) ||
                (candidateType == typeof(TrainTrackAI) && currentType == typeof(MetroTrackTunnelAI) && candidateInfo.name.ToLower().IndexOf("sunken") >= 0)
                ;
        }

        /// <summary>
        /// Gets the <see cref="BuildingInfo.PathInfo"/> indexed by the given <see cref="PathIndex"/>.
        /// </summary>
        /// <param name="pathIndex"><see cref="PathIndex"/>.</param>
        /// <returns><see cref="BuildingInfo.PathInfo"/> indexed by this record (null if none).</returns>
        private BuildingInfo.PathInfo GetPathInfo(PathIndex pathIndex)
        {
            // Validity check.
            if (!pathIndex.IsValid || !s_currentBuilding)
            {
                return null;
            }

            BuildingInfo.PathInfo[] paths;
            if (pathIndex.m_subBuilding >= 0)
            {
                // Sub-building.
                if (s_currentBuilding.m_subBuildings == null || pathIndex.m_subBuilding >= s_currentBuilding.m_subBuildings.Length)
                {
                    Logging.Error("invalid subBuilding index of ", pathIndex.m_subBuilding, " with sub-building array size of ", s_currentBuilding.m_subBuildings.Length);
                    return null;
                }

                paths = s_currentBuilding.m_subBuildings[pathIndex.m_subBuilding].m_buildingInfo.m_paths;
            }
            else
            {
                // Main building.
                paths = s_currentBuilding.m_paths;
            }

            if (pathIndex.m_pathIndex >= paths.Length)
            {
                Logging.Error("invalid pathIndex of ", pathIndex.m_pathIndex, " with path array size of ", paths.Length);
                return null;
            }

            return paths[pathIndex.m_pathIndex];
        }
    }
}