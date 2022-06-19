using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;


namespace RON
{
	/// <summary>
	/// RON station track replacer panel for already-built stations.
	/// </summary>
    internal class BuiltStationPanel : StationPanel
    {
		// WorldInfoPanel button to activate panel.
		private static UIButton panelButton;


		/// <summary>
		/// Constructor.
		/// </summary>
		internal BuiltStationPanel()
		{
			// Check for eligible networks in this building.
			CheckEligibleNets();

			// If no eligible nets were found, exit.
			if (eligibleNets.Count == 0)
			{
				// Close panel first if already open.
				Close();
				return;
			}

			// Refresh the panel.
			(Panel as BuiltStationPanel)?.RefreshPanel();
		}


		/// <summary>
		/// Sets the WorldInfoPanel button visibility.
		/// </summary>
		internal static void SetPanelButtonState()
		{
			// Null check for safety.
			if (panelButton == null)
			{
				return;
			}

			// Determine eligible networks in currently selected building.
			CheckEligibleNets();

			// Hide button if no eligible networks were found, otherwise show it.
			panelButton.isVisible = eligibleNets.Count > 0;

			// Close the panel (if it's already open) on change of target.
			Close();
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
			panelButton = infoPanel.component.AddUIComponent<UIButton>();

			// Basic button setup.
			panelButton.atlas = Textures.RonButtonSprites;
			panelButton.size = new Vector2(PanelButtonSize, PanelButtonSize);
			panelButton.normalFgSprite = "normal";
			panelButton.focusedFgSprite = "hovered";
			panelButton.hoveredFgSprite = "hovered";
			panelButton.pressedFgSprite = "pressed";
			panelButton.disabledFgSprite = "disabled";
			panelButton.name = "RONReplacerButton";
			panelButton.tooltip = Translations.Translate("RON_STA_CUS");

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
			panelButton.AlignTo(infoPanel.component, UIAlignAnchor.TopLeft);
			panelButton.relativePosition += new Vector3(infoPanel.component.width - 70f - PanelButtonSize, relativeY, 0f);

			// Event handler.
			panelButton.eventClick += (control, clickEvent) =>
			{
				// Toggle panel visibility.
				if (uiGameObject == null)
				{
					Create<BuiltStationPanel>();
					Panel.absolutePosition = panelButton.absolutePosition + new Vector3(-PanelWidth / 2f, PanelButtonSize + 10f);
				}
				else
				{
					Close();
				}

				// Manually unfocus control, otherwise it can stay focused until next UI event (looks untidy).
				control.Unfocus();
			};

			infoPanel.component.eventVisibilityChanged += (control, isVisible) => Close();
		}


		/// <summary>
		/// Returns the NetInfo of the given target network index.
		/// </summary>
		internal override NetInfo GetNetInfo(int index)
		{
			// Check if the given index is valid.
			if (eligibleNets != null && eligibleNets.Contains(index))
			{
				// Valid index; return NetInfo.
				return Singleton<NetManager>.instance.m_segments.m_buffer[index].Info;
			}

			// If we got here, we didn't get a match; return null.
			return null;
		}


		/// <summary>
		/// Setter for selected replacement.  Called by target network list items.
		/// </summary>
		internal override NetInfo SelectedReplacement
		{
			set
			{
				// Assign replacement network, if we've got a valid selection.
				if (selectedIndex > 0)
				{
					Singleton<SimulationManager>.instance.AddAction(() => Replacer.ReplaceNets(GetNetInfo(selectedIndex), value, new List<ushort> { (ushort)selectedIndex }, false));
				}
			}
		}


		/// <summary>
		/// Checks for eligible networks in this building.
		/// </summary>
		private static void CheckEligibleNets()
		{
			// Local references.
			Building[] buildingBuffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
			NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
			NetSegment[] segmentBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;

			// Reset eligible network list
			eligibleNets.Clear();

			// Get current building.
			ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
			if (buildingID == 0)
			{
				return;
			}

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
						nodes.Contains(segmentBuffer[segmentID].m_endNode) &&
						!eligibleNets.Contains(segmentID))
					{
						// Check to ensure that we only use train and metro track networks (e.g. no invisible pedestrian paths!)
						NetAI netAI = segmentBuffer[segmentID].Info.m_netAI;
						if (netAI is TrainTrackBaseAI || netAI is MetroTrackBaseAI)
						{
							// Eligible segment - add to our list.
							eligibleNets.Add(segmentID);
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
			TargetList();
			LoadedList();
			SetTitle();
		}
	}
}