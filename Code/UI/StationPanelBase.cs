// <copyright file="StationPanelBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

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
        protected internal static List<int> s_eligibleNets = new List<int>();

        /// <summary>
        /// Selected network index number.
        /// </summary>
        protected internal int m_selectedIndex;

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
        private readonly UIDropDown _typeDropDown;
        private readonly UICheckBox _sameWidthCheck;
        private readonly UIList _targetList;
        private readonly UIList _loadedList;

        /// <summary>
        /// Initializes a new instance of the <see cref="StationPanelBase{TRow}"/> class.
        /// </summary>
        internal StationPanelBase()
        {
            // Title label.
            SetTitle();

            // Decorative icon (top-left).
            SetIcon(UITextures.LoadQuadSpriteAtlas("RonButton"), "normal");

            // Same width only check.
            _sameWidthCheck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, ControlY + 5f, Translations.Translate("RON_PNL_WID"));
            _sameWidthCheck.isChecked = true;
            _sameWidthCheck.eventCheckChanged += (c, isChecked) => LoadedList();

            // Type dropdown.
            _typeDropDown = UIDropDowns.AddDropDown(this, Check2X, ControlY, ListWidth / 2f);
            _typeDropDown.items = new string[] { Translations.Translate("RON_STA_RAO"), Translations.Translate("RON_STA_MTO"), Translations.Translate("RON_STA_RAM") };
            _typeDropDown.eventSelectedIndexChanged += (control, index) => LoadedList();

            // Target network list.
            _targetList = UIList.AddUIList<TRow>(this, Margin, ListY, ListWidth, ListHeight, NetRow.CustomRowHeight);
            _targetList.EventSelectionChanged += (c, selectedItem) => SelectedIndex = selectedItem is int intItem ? intItem : -1;

            // Loaded network list.
            _loadedList = UIList.AddUIList<NetRow>(this, RightPanelX, ListY, ListWidth, ListHeight, NetRow.CustomRowHeight);
            _loadedList.EventSelectionChanged += (c, selectedItem) => SelectedReplacement = (selectedItem as NetRowItem)?.Prefab;
        }

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
        /// Sets the selected target index.  Called by target network list items.
        /// </summary>
        internal int SelectedIndex
        {
            set
            {
                // Confirm target index validity before setting.
                if (s_eligibleNets.Contains(value))
                {
                    m_selectedIndex = value;
                }
                else
                {
                    // Invalid selection; set to -1.
                    m_selectedIndex = -1;
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
                if (m_selectedIndex >= 0)
                {
                    s_currentBuilding.m_paths[m_selectedIndex].m_finalNetInfo = value;
                }
            }
        }

        /// <summary>
        /// Gets the panel's title.
        /// </summary>
        protected override string PanelTitle => Translations.Translate("RON_NAM");

        /// <summary>
        /// Gets the target network as NetInfo.
        /// </summary>
        private NetInfo TargetNet => GetNetInfo(m_selectedIndex);

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
                            s_eligibleNets.Add(i);
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
                stationPanel.SetTypeMenu(selectedBuilding);
            }
        }

        /// <summary>
        /// Returns the NetInfo of the given target network index.
        /// </summary>
        /// <param name="index">Target network index.</param>
        /// <returns>NetInfo of the target network index.</returns>
        internal virtual NetInfo GetNetInfo(int index)
        {
            // Check if the given index is valid.
            if (s_eligibleNets != null && s_eligibleNets.Contains(index))
            {
                // Valid index; return NetInfo.
                return s_currentBuilding.m_paths[index].m_netInfo;
            }

            // If we got here, we didn't get a match; return null.
            return null;
        }

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
            m_selectedIndex = -1;
        }

        /// <summary>
        /// Populates a fastlist with a list of relevant loaded networks.
        /// </summary>
        protected void LoadedList()
        {
            // Clear list if there's no current selection.
            if (m_selectedIndex < 0)
            {
                _loadedList.Clear();
                return;
            }

            // Ensure valid target selection before proceeding.
            NetInfo currentNetInfo = GetNetInfo(m_selectedIndex);
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
                        // Train tracks are included unless 'metro only' is selected.
                        if (_typeDropDown.selectedIndex == (int)TypeIndex.MetroOnly)
                        {
                            continue;
                        }
                    }
                    else if (candidateType.IsSubclassOf(typeof(MetroTrackBaseAI)))
                    {
                        // Metro tracks are included unless 'rail only' is selected.
                        if (_typeDropDown.selectedIndex == (int)TypeIndex.RailOnly)
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
                            if (!MatchTrainMetro(currentAIType, candidateType))
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
        protected void SetTypeMenu(BuildingInfo buildingInfo)
        {
            int oldIndex = _typeDropDown.selectedIndex;
            _typeDropDown.selectedIndex = buildingInfo.GetSubService() == ItemClass.SubService.PublicTransportMetro ? (int)TypeIndex.MetroOnly : (int)TypeIndex.RailOnly;

            // Force LoadedList update if no change in index (so event handler wasn't triggered).
            if (oldIndex == _typeDropDown.selectedIndex)
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
        /// <returns>True if the two provided types are matched, false otherwise.</returns>
        private bool MatchTrainMetro(Type currentType, Type candidateType)
        {
            return
                (candidateType == typeof(TrainTrackAI) && currentType == typeof(MetroTrackAI)) ||
                (candidateType == typeof(TrainTrackBridgeAI) && currentType == typeof(MetroTrackBridgeAI)) ||
                (candidateType == typeof(MetroTrackAI) && currentType == typeof(TrainTrackAI)) ||
                (candidateType == typeof(MetroTrackBridgeAI) && currentType == typeof(TrainTrackBridgeAI))
                ;
        }
    }
}