using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework.UI;


namespace RON
{
	/// <summary>
	/// RON station track replacer panel for placing stations.
	/// </summary>
	internal class StationPanel : UIPanel
	{
		private enum TypeIndex : int
		{
			RailOnly = 0,
			MetroOnly,
			RailMetro
		}


		// Layout constants.
		private const float Margin = 5f;
		private const float TitleHeight = 50f;
		private const float ControlHeight = 30f;
		private const float ControlY = TitleHeight;
		private const float ListHeight = 6 * UINetRow.RowHeight;
		private const float ListY = ControlY + ControlHeight;
		private const float LeftX = Margin;
		private const float ListWidth = 450f;
		private const float RightPanelX = LeftX + ListWidth + Margin;
		private const float Check1X = RightPanelX;
		private const float Check2X = RightPanelX + (ListWidth / 2f);
		private const float PanelHeight = ListY + ListHeight + Margin;

		protected const float PanelWidth = RightPanelX + ListWidth + Margin;


		// Instance references.
		protected static GameObject uiGameObject;
		private static StationPanel panel;

		// Selections.
		internal static BuildingInfo currentBuilding;
		protected static List<int> eligibleNets = new List<int>();
		protected int selectedIndex;


		// Panel components.
		private readonly UICheckBox sameWidthCheck;
		protected readonly UIDropDown typeDropDown;
		private readonly RONFastList targetList, loadedList;
		private readonly UILabel titleLabel;


		/// <summary>
		/// Getter for panel instance.
		/// </summary>
		internal static StationPanel Panel => panel;


		/// <summary>
		/// Setter for selected target index.  Called by target network list items.
		/// </summary>
		internal int SelectedIndex
		{
			set
			{
				// Confirm target index validity before setting.
				if (eligibleNets.Contains(value))
				{
					selectedIndex = value;
				}
				else
				{
					// Invalid selection; set to -1.
					selectedIndex = -1;
				}

				// Regenerate loaded list on selection change.
				LoadedList();
			}
		}


		/// <summary>
		/// Setter for selected replacement.  Called by target network list items.
		/// </summary>
		internal virtual NetInfo SelectedReplacement
		{
			set
			{
				// Assign replacement network, if we've got a valid selection.
				if (selectedIndex >= 0)
				{
					currentBuilding.m_paths[selectedIndex].m_finalNetInfo = value;
				}
			}
		}


		/// <summary>
		/// Current target network as NetInfo.
		/// </summary>
		private NetInfo TargetNet => GetNetInfo(selectedIndex);


		/// <summary>
		/// Set the target building (first checking validity).
		/// </summary>
		/// <param name="selectedBuilding">Selected station building</param>
		internal static void SetTarget(BuildingInfo selectedBuilding)
		{
			// Don't do anything if selection hasn't changed (this includes after the panel has been closed while the station building is still selected).
			if (selectedBuilding == currentBuilding)
			{
				return;
			}

			// Update current reference.
			currentBuilding = selectedBuilding;

			// Reset eligible network list
			eligibleNets.Clear();

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
							eligibleNets.Add(i);
						}
					}
				}
			}

			// If no eligible nets were found, exit.
			if (eligibleNets.Count == 0)
			{
				// Close panel first if already open.
				Close();
				return;
			}

			// Create panel if not already open.
			if (panel == null)
			{
				Create<StationPanel>();
			}

			// Update panel.
			panel.SetTitle();
			panel.TargetList();
			panel.SetTypeMenu(selectedBuilding);
		}


		/// <summary>
		/// Creates the panel object in-game and displays it.
		/// </summary>
		protected static void Create<T>() where T : StationPanel
		{
			try
			{
				// If no GameObject instance already set, create one.
				if (uiGameObject == null)
				{
					// Give it a unique name for easy finding with ModTools.
					uiGameObject = new GameObject("RONStationPanel");
					uiGameObject.transform.parent = UIView.GetAView().transform;

					// Create new panel instance and add it to GameObject.
					uiGameObject.AddComponent<T>();
				}
			}
			catch (Exception e)
			{
				Logging.LogException(e, "exception creating station panel");
			}
		}


		/// <summary>
		/// Closes the panel by destroying the object (removing any ongoing UI overhead).
		/// </summary>
		internal static void Close()
		{
			// Don't do anything if no panel.
			if (panel == null)
			{
				return;
			}

			// Destroy game objects.
			GameObject.Destroy(panel);
			GameObject.Destroy(uiGameObject);

			// Let the garbage collector do its work (and also let us know that we've closed the object).
			panel = null;
			uiGameObject = null;
		}


		/// <summary>
		/// Constructor.
		/// </summary>
		internal StationPanel()
		{
			// Set instance references.
			panel = this;

			// Basic behaviour.
			autoLayout = false;
			canFocus = true;
			isInteractive = true;

			// Appearance.
			backgroundSprite = "MenuPanel2";
			opacity = 1f;

			// Size.
			size = new Vector2(PanelWidth, PanelHeight);

			// Default position - towards top-left of screen, with 50f margins.
			relativePosition = new Vector2(Mathf.Floor(GetUIView().fixedWidth - width - 50f), 50f);

			// Drag bar.
			UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
			dragHandle.width = this.width - 50f;
			dragHandle.height = this.height;
			dragHandle.relativePosition = Vector3.zero;
			dragHandle.target = this;

			// Title label.
			titleLabel = AddUIComponent<UILabel>();
			titleLabel.relativePosition = new Vector2(50f, 13f);
			SetTitle();

			// Close button.
			UIButton closeButton = AddUIComponent<UIButton>();
			closeButton.relativePosition = new Vector2(width - 35, 2);
			closeButton.normalBgSprite = "buttonclose";
			closeButton.hoveredBgSprite = "buttonclosehover";
			closeButton.pressedBgSprite = "buttonclosepressed";
			closeButton.eventClick += (component, clickEvent) => Close();

			// Decorative icon (top-left).
			UISprite iconSprite = AddUIComponent<UISprite>();
			iconSprite.relativePosition = new Vector2(5, 5);
			iconSprite.height = 32f;
			iconSprite.width = 32f;
			iconSprite.atlas = Textures.RonButtonSprites;
			iconSprite.spriteName = "normal";

			// Same width only check.
			sameWidthCheck = UIControls.AddCheckBox(this, Check1X, ControlY + 5f, Translations.Translate("RON_PNL_WID"));
			sameWidthCheck.isChecked = true;
			sameWidthCheck.eventCheckChanged += (control, isChecked) => LoadedList();

			// Type dropdown.
			typeDropDown = UIControls.AddDropDown(this, Check2X, ControlY, ListWidth / 2f);
			typeDropDown.items = new string[] { Translations.Translate("RON_STA_RAO"), Translations.Translate("RON_STA_MTO"), Translations.Translate("RON_STA_RAM") };
			typeDropDown.eventSelectedIndexChanged += (control, index) => LoadedList();

			// Target network list.
			UIPanel leftPanel = AddUIComponent<UIPanel>();
			leftPanel.width = ListWidth;
			leftPanel.height = ListHeight;
			leftPanel.relativePosition = new Vector2(Margin, ListY);
			targetList = RONFastList.Create<UIStationTargetNetRow>(leftPanel);
			ListSetup(targetList);

			// Loaded network list.
			UIPanel rightPanel = AddUIComponent<UIPanel>();
			rightPanel.width = ListWidth;
			rightPanel.height = ListHeight;
			rightPanel.relativePosition = new Vector2(RightPanelX, ListY);
			loadedList = RONFastList.Create<UIStationReplacementNetRow>(rightPanel);
			ListSetup(loadedList);
		}


		/// <summary>
		/// Returns the NetInfo of the given target network index.
		/// </summary>
		internal virtual NetInfo GetNetInfo(int index)
		{
			// Check if the given index is valid.
			if (eligibleNets != null && eligibleNets.Contains(index))
			{
				// Valid index; return NetInfo.
				return currentBuilding.m_paths[index].m_netInfo;
			}

			// If we got here, we didn't get a match; return null.
			return null;
		}


		/// <summary>
		/// Populates a fastlist with a list of eligible networks in the current building.
		/// </summary>
		/// <returns>Populated fastlist of eligible networks in the current building</returns>
		protected void TargetList()
		{
			// Create return fastlist from our list of eligible networks.
			targetList.rowsData = new FastList<object>
			{
				m_buffer = eligibleNets.Select(x => (object)x).ToArray(),
				m_size = eligibleNets.Count
			};

			// Clear current selection.
			targetList.selectedIndex = -1;
			selectedIndex = -1;

			// Force list update.
			targetList.Refresh();
		}


		/// <summary>
		/// Populates a fastlist with a list of relevant loaded networks.
		/// </summary>
		/// <returns>Populated fastlist of networks on map</returns>
		protected void LoadedList()
		{
			// Clear list if there's no current selection.
			if (selectedIndex < 0)
			{
				loadedList.Clear();
				return;
			}

			// Station status of currently selected track.
			NetInfo currentNetInfo = GetNetInfo(selectedIndex);
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
					if (sameWidthCheck.isChecked && network.m_halfWidth != TargetNet.m_halfWidth)
					{
						// No match; skip this one.
						continue;
					}

					// Apply station filter.
					if (isStation != newItem.isStation)
					{
						continue;
					}

					// Check network type filters.
					Type candidateType = network.m_netAI.GetType();
					if (candidateType.IsSubclassOf(typeof(TrainTrackBaseAI)))
					{
						// Train tracks are included unless 'metro only' is selected.
						if (typeDropDown.selectedIndex == (int)TypeIndex.MetroOnly)
						{
							continue;
						}
					}
					else if (candidateType.IsSubclassOf(typeof(MetroTrackBaseAI)))
					{
						// Metro tracks are included unless 'rail only' is selected.
						if (typeDropDown.selectedIndex == (int)TypeIndex.RailOnly)
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
			loadedList.rowsData = new FastList<object>
			{
				m_buffer = netList.OrderBy(item => item.displayName).ToArray(),
				m_size = netList.Count
			};

			// Clear current selection.
			loadedList.selectedIndex = -1;
		}


		/// <summary>
		/// Sets the type menu index.
		/// </summary>
		/// <param name="buildingInfo">Currently selected prefab</param>
		protected void SetTypeMenu(BuildingInfo buildingInfo)
		{
			int oldIndex = typeDropDown.selectedIndex;
			typeDropDown.selectedIndex = buildingInfo.GetSubService() == ItemClass.SubService.PublicTransportMetro ? (int)TypeIndex.MetroOnly : (int)TypeIndex.RailOnly;

			// Force LoadedList update if no change in index (so event handler wasn't triggered).
			if (oldIndex == typeDropDown.selectedIndex)
			{
				LoadedList();
			}
		}

		/// <summary>
		/// Sets the panel title, including the building name.
		/// </summary>
		protected void SetTitle() => titleLabel.text = (currentBuilding?.name != null ? PrefabUtils.GetDisplayName(currentBuilding) : Translations.Translate("RON_NAM")) + ": " + Translations.Translate("RON_STA_CUS");


		/// <summary>
		/// Matches train tracks to equivalent metro tracks (and vice-versa).
		/// </summary>
		/// <param name="currentType">Selected track AI type</param>
		/// <param name="candidateType">Candidate track AI type</param>
		/// <returns>True if the two provided types are matched, false otherwise</returns>
		private bool MatchTrainMetro(Type currentType, Type candidateType)
		{
			return 
				candidateType == typeof(TrainTrackAI) && currentType == typeof(MetroTrackAI) ||
				candidateType == typeof(TrainTrackBridgeAI) && currentType == typeof(MetroTrackBridgeAI) ||
				candidateType == typeof(MetroTrackAI) && currentType == typeof(TrainTrackAI) ||
				candidateType == typeof(MetroTrackBridgeAI) && currentType == typeof(TrainTrackBridgeAI)
				;
		}


		/// <summary>
		/// Performs initial fastlist setup.
		/// </summary>
		/// <param name="fastList">Fastlist to set up</param>
		private void ListSetup(UIFastList fastList)
		{
			// Apperance, size and position.
			fastList.backgroundSprite = "UnlockingPanel";
			fastList.width = fastList.parent.width;
			fastList.height = fastList.parent.height;
			fastList.relativePosition = Vector2.zero;
			fastList.rowHeight = UINetRow.RowHeight;

			// Behaviour.
			fastList.canSelect = true;
			fastList.autoHideScrollbar = true;

			// Data.
			fastList.rowsData = new FastList<object>();
			fastList.selectedIndex = -1;
		}
	}
}