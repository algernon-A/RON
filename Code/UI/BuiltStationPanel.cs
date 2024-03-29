﻿// <copyright file="BuiltStationPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using System.Collections.Generic;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// RON station track replacer panel for already-built stations.
    /// </summary>
    internal class BuiltStationPanel : StationPanelBase<BuiltStationTargetNetRow>
    {
        // WorldInfoPanel button to activate panel.
        private static UIButton s_panelButton;

        // Status.
        private bool _replacingDone;
        private bool _replacing;

        /// <summary>
        /// Sets selected replacement.  Called by target network list items.
        /// </summary>
        internal override NetInfo SelectedReplacement
        {
            set
            {
                Logging.Message("selectedReplacement set to ", value?.name ?? "null");

                // Assign replacement network, if we've got a valid selection.
                if (m_selectedIndex.IsValid)
                {
                    _replacing = true;
                    _replacingDone = false;
                    Singleton<SimulationManager>.instance.AddAction(() => Replacer.ReplaceNets(GetNetInfo(m_selectedIndex.m_pathIndex), value, new List<ushort> { (ushort)m_selectedIndex.m_pathIndex }, false));
                }
            }
        }

        /// <summary>
        /// Sets a value indicating whether replacement work has finished.
        /// </summary>
        internal bool ReplacingDone { set => _replacingDone = value; }

        /// <summary>
        /// Gets the selected target network as NetInfo.
        /// </summary>
        protected override NetInfo TargetNet => m_selectedIndex.IsValid ? GetNetInfo(m_selectedIndex.m_pathIndex) : null;

        /// <summary>
        /// Called by Unity when the object is created.
        /// Used to perform setup.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Check for eligible networks in this building.
            CheckEligibleNets();

            // If no eligible nets were found, exit.
            if (s_eligibleNets.Count == 0)
            {
                // Close panel first if already open.
                Close();
                return;
            }

            // Refresh the panel.
            RefreshPanel();
        }

        /// <summary>
        /// Called by Unity every tick.  Used here to track state of any in-progress replacements.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Set selected segment for highlighting.
            int pathIndex = m_selectedIndex.m_pathIndex;
            if (pathIndex > 0)
            {
                ToolBasePatch.SelectedSegment = (ushort)pathIndex;
            }
            else
            {
                ToolBasePatch.SelectedSegment = 0;
            }

            // Is a replacement underway?
            if (_replacing)
            {
                // Yes - is it done?
                if (_replacingDone)
                {
                    // Done! Clear flags.
                    _replacing = false;
                    _replacingDone = false;

                    // Recheck eligible networks.
                    CheckEligibleNets();

                    // Rebuild target list.
                    TargetList();
                }
            }
        }

        /// <summary>
        /// Called by Unity when the panel is destroyed.
        /// </summary>
        public override void OnDestroy()
        {
            // Cancel any segment highlighting.
            ToolBasePatch.SelectedSegment = 0;

            base.OnDestroy();
        }

        /// <summary>
        /// Sets the WorldInfoPanel button visibility.
        /// </summary>
        internal static void SetPanelButtonState()
        {
            // Null check for safety.
            if (s_panelButton == null)
            {
                return;
            }

            // Determine eligible networks in currently selected building.
            CheckEligibleNets();

            // Hide button if no eligible networks were found, otherwise show it.
            s_panelButton.isVisible = s_eligibleNets.Count > 0;

            // Close the panel (if it's already open) on change of target.
            StandalonePanelManager<BuiltStationPanel>.Panel?.Close();
        }

        /// <summary>
        /// Adds a RON button to the service building info panel to open the RON station replacer panel for that building.
        /// The button will be added to the right of the panel with a small margin from the panel edge, at the relative Y position specified.
        /// The button overlaps the position used by ABLC and Transfer Controller in other panels, as they won't show for these (station) buildings.
        /// </summary>
        internal static void AddInfoPanelButton()
        {
            const float PanelButtonSize = 28f;

            CityServiceWorldInfoPanel infoPanel = UIView.library.Get<CityServiceWorldInfoPanel>(typeof(CityServiceWorldInfoPanel).Name);

            // Don't do anything if we didn't get the info panel, e.g. if we're in an editor.
            if (infoPanel == null)
            {
                return;
            }

            s_panelButton = infoPanel.component.AddUIComponent<UIButton>();

            // Basic button setup.
            s_panelButton.atlas = UITextures.LoadQuadSpriteAtlas("RonButton");
            s_panelButton.size = new Vector2(PanelButtonSize, PanelButtonSize);
            s_panelButton.normalFgSprite = "normal";
            s_panelButton.focusedFgSprite = "hovered";
            s_panelButton.hoveredFgSprite = "hovered";
            s_panelButton.pressedFgSprite = "pressed";
            s_panelButton.disabledFgSprite = "disabled";
            s_panelButton.name = "RONReplacerButton";
            s_panelButton.tooltip = Translations.Translate("RON_STA_CUS");

            // Find ProblemsPanel relative position to position button.
            // We'll use 40f as a default relative Y in case something doesn't work.
            UIComponent problemsPanel;
            float relativeY = 40f;

            // Player info panels have wrappers, zoned ones don't.
            UIComponent wrapper = infoPanel.Find("Wrapper");
            if (wrapper == null)
            {
                problemsPanel = infoPanel.Find("ProblemsPanel");
            }
            else
            {
                problemsPanel = wrapper.Find("ProblemsPanel");
            }

            try
            {
                // Position button vertically in the middle of the problems panel.  If wrapper panel exists, we need to add its offset as well.
                relativeY = (wrapper == null ? 0 : wrapper.relativePosition.y) + problemsPanel.relativePosition.y + ((problemsPanel.height - PanelButtonSize) / 2f);
            }
            catch
            {
                // Don't really care; just use default relative Y.
                Logging.Message("couldn't find ProblemsPanel relative position");
            }

            // Set position.
            s_panelButton.AlignTo(infoPanel.component, UIAlignAnchor.TopRight);
            s_panelButton.relativePosition += new Vector3(-70f, relativeY, 0f);

            // Event handler.
            s_panelButton.eventClick += (c, p) =>
            {
                // Don't do anything if no eligible nets.
                if (s_eligibleNets.Count > 0)
                {
                    StandalonePanelManager<BuiltStationPanel>.Create();
                    StandalonePanelManager<BuiltStationPanel>.Panel.absolutePosition = s_panelButton.absolutePosition + new Vector3(-CalculatedPanelWidth / 2f, PanelButtonSize + 200f);
                }

                // Manually unfocus control, otherwise it can stay focused until next UI event (looks untidy).
                c.Unfocus();
            };

            infoPanel.component.eventVisibilityChanged += (c, isVisible) => StandalonePanelManager<BuiltStationPanel>.Panel?.Close();
        }

        /// <summary>
        /// Returns the NetInfo of the given target network index.
        /// </summary>
        /// <param name="index">Target network index.</param>
        /// <returns>NetInfo.</returns>
        internal NetInfo GetNetInfo(int index) => Singleton<NetManager>.instance.m_segments.m_buffer[index].Info;

        /// <summary>
        /// Checks for eligible networks in this building.
        /// </summary>
        private static void CheckEligibleNets()
        {
            // Local references.
            Building[] buildingBuffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
            NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            NetSegment[] segmentBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;

            // Reset eligible network list.
            s_eligibleNets.Clear();

            // Get current building.
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (buildingID == 0)
            {
                return;
            }

            // Set building info.
            s_currentBuilding = buildingBuffer[buildingID].Info;

            // Iterate through each node in building and list them.
            List<ushort> nodes = new List<ushort>();
            ushort netNode = buildingBuffer[buildingID].m_netNode;
            while (netNode != 0)
            {
                if (!nodes.Contains(netNode))
                {
                    nodes.Add(netNode);
                }

                netNode = nodeBuffer[netNode].m_nextBuildingNode;
            }

            // Check sub-buildings.
            ushort subBuildingID = buildingBuffer[buildingID].m_subBuilding;
            while (subBuildingID != 0)
            {
                netNode = buildingBuffer[subBuildingID].m_netNode;
                while (netNode != 0)
                {
                    if (!nodes.Contains(netNode))
                    {
                        nodes.Add(netNode);
                    }

                    netNode = nodeBuffer[netNode].m_nextBuildingNode;
                }

                subBuildingID = buildingBuffer[subBuildingID].m_subBuilding;
            }

            // Now, check each node in the list.
            foreach (ushort nodeID in nodes)
            {
                // Check for all segments leading from this node.
                for (int i = 0; i < 8; ++i)
                {
                    // Looking for segments whose start and end nodes are both in the list of nodes attached to this building (internal building network).
                    ushort segmentID = nodeBuffer[nodeID].GetSegment(i);
                    if (segmentID != 0 &&
                        nodes.Contains(segmentBuffer[segmentID].m_startNode) &&
                        nodes.Contains(segmentBuffer[segmentID].m_endNode))
                    {
                        {
                            // Check to ensure that we only use train and metro track networks (e.g. no invisible pedestrian paths!)
                            NetAI netAI = segmentBuffer[segmentID].Info.m_netAI;
                            if (netAI is TrainTrackBaseAI || netAI is MetroTrackBaseAI)
                            {
                                // Eligible segment - add to our list, if we haven't already.
                                PathIndex newPathIndex = new PathIndex(-1, segmentID);
                                if (!s_eligibleNets.Contains(newPathIndex))
                                {
                                    s_eligibleNets.Add(newPathIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the panel based on the current building selection.
        /// </summary>
        private void RefreshPanel()
        {
            SetTitle();
            TargetList();
            SetDefaultChecks(s_currentBuilding);
        }
    }
}