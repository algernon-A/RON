using System;
using System.Linq;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Math;
using UnityEngine;


namespace RON
{
	/// <summary>
	/// Static class to manage the BOB info panel.
	/// </summary>
	internal class ReplacerPanel : UIPanel
	{
		// Layout constants.
		private const float LeftWidth = 450f;
		private const float RightWidth = 450f;
		private const float PanelHeight = 490f;
		private const float TitleHeight = 45f;
		private const float Margin = 5f;
		private const float MiddleContentWidth = 190f;
		private const float MenuY = 50f;
		private const float ReplaceX = LeftWidth + (Margin * 3);
		private const float ReplaceY = 50f;
		private const float ReplaceWidth = 150f;
		private const float ReplaceHeight = ToolbarHeight + TitleHeight - ReplaceY - Margin;
		private const float GoToY = MenuY + 35f;
		private const float ProgressY = ReplaceY + 35f;
		private const float ToolbarHeight = 75f;
		private const float FilterX = (LeftWidth + RightWidth + Margin * 3) - 200f;
		private const float HideVanillaY = ReplaceY + 30f;
		private const float SameWidthY = HideVanillaY + 20f;
		private const float ButtonWidth = 220f;
		private const float PrevX = Margin;
		private const float NextX = LeftWidth + Margin - ButtonWidth;


		// Instance references.
		private static GameObject uiGameObject;
		private static ReplacerPanel panel;
		internal static ReplacerPanel Panel => panel;

		// Current selections.
		private NetInfo selectedTarget, selectedReplacement;

		// Panel components.
		private readonly UIFastList targetList, loadedList;
		private readonly UIButton replaceButton, prevButton, nextButton;
		private readonly UITextField nameFilter;
		private readonly UIDropDown typeDropDown;
		private readonly UILabel replacingLabel, progressLabel;
		private readonly UICheckBox sameWidthCheck, hideVanilla;

		// Status.
		private bool replacing, replacingDone;
		private float timer;
		private int timerStep;
		private ushort lastViewedSegment;

		// Nework type list.
		private const int NumTypes = 10;
		private readonly string[] netDescriptions = new string[NumTypes]
		{
			Translations.Translate("RON_PNL_ROA"),
			Translations.Translate("RON_PNL_ROB"),
			Translations.Translate("RON_PNL_ROT"),
			Translations.Translate("RON_PNL_RAI"),
			Translations.Translate("RON_PNL_RAB"),
			Translations.Translate("RON_PNL_RAT"),
			Translations.Translate("RON_PNL_PED"),
			Translations.Translate("RON_PNL_PEB"),
			Translations.Translate("RON_PNL_PET"),
			Translations.Translate("RON_PNL_DEC")
		};
		private readonly Type[] netTypes = new Type[NumTypes]
		{
			typeof(RoadAI),
			typeof(RoadBridgeAI),
			typeof(RoadTunnelAI),
			typeof(TrainTrackAI),
			typeof(TrainTrackBridgeAI),
			typeof(TrainTrackTunnelAI),
			typeof(PedestrianPathAI),
			typeof(PedestrianBridgeAI),
			typeof(PedestrianTunnelAI),
			typeof(DecorationWallAI)
		};


		/// <summary>
		/// Called by Unity every tick.  Used here to track state of any in-progress replacments.
		/// </summary>
		public override void Update()
		{
			base.Update();


			// Is a replacement underway?
			if (replacing)
			{
				// Yes - is it done?
				if (replacingDone)
				{
					// Done! Clear flags.
					replacing = false;
					replacingDone = false;

					// Done - hide 'replacing' labels and show button.
					replacingLabel.Hide();
					progressLabel.Hide();
					replaceButton.Show();

					// Rebuild target list.
					TargetList();
				}
				else
				{
					// No - still in progress - update timer.
					timer += Time.deltaTime;

					// Add a period to the progress label every 100ms.  After 30, clear and start again.
					if (timer > .1f)
					{
						if (++timerStep > 30)
						{
							progressLabel.text = ".";
							timerStep = 0;
						}
						else
						{
							progressLabel.text += ".";
						}

						// Either way, reset timer to zero.
						timer = 0f;
					}
				}
			}
		}


		/// <summary>
		/// Setter for selected target.  Called by target network list items.
		/// </summary>
		internal NetInfo SelectedTarget
		{
			set
			{
				// Don't do anything if the target hasn't changed.
				if (selectedTarget != value)
				{
					selectedTarget = value;

					// Reset last viewed segment counter.
					lastViewedSegment = 0;

					// Update loaded list if we're only showing networks of the same width.
					if (sameWidthCheck.isChecked)
					{
						LoadedList();
					}

					UpdateButtonStates();
				}
			}
		}


		/// <summary>
		/// Setter for selected replacement.  Called by target network list items.
		/// </summary>
		internal NetInfo SelectedReplacement
		{
			set
			{
				selectedReplacement = value;
				UpdateButtonStates();
			}
		}


		/// <summary>
		/// Creates the panel object in-game and displays it.
		/// </summary>
		internal static void Create()
		{
			try
			{
				// If no GameObject instance already set, create one.
				if (uiGameObject == null)
				{
					// Give it a unique name for easy finding with ModTools.
					uiGameObject = new GameObject("RONPanel");
					uiGameObject.transform.parent = UIView.GetAView().transform;

					// Create new panel instance and add it to GameObject.
					panel = uiGameObject.AddComponent<ReplacerPanel>();
					panel.transform.parent = uiGameObject.transform.parent;
				}
			}
			catch (Exception e)
			{
				Logging.LogException(e, "exception creating InfoPanel");
			}
		}


		/// <summary>
		/// Closes the panel by destroying the object (removing any ongoing UI overhead).
		/// </summary>
		internal static void Close()
		{
			// Don't do anything if no panel, or if we're in the middle of replacing.
			if (panel == null || panel.replacing)
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
		internal ReplacerPanel()
		{
			// Basic behaviour.
			autoLayout = false;
			canFocus = true;
			isInteractive = true;

			// Appearance.
			backgroundSprite = "MenuPanel2";
			opacity = 1f;

			// Size.
			size = new Vector2(LeftWidth + RightWidth + (Margin * 4), PanelHeight + TitleHeight + ToolbarHeight + (Margin * 3));

			// Default position - centre in screen.
			relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

			// Drag bar.
			UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
			dragHandle.width = this.width - 50f;
			dragHandle.height = this.height;
			dragHandle.relativePosition = Vector3.zero;
			dragHandle.target = this;

			// Title label.
			UILabel titleLabel = AddUIComponent<UILabel>();
			titleLabel.relativePosition = new Vector2(50f, 13f);
			titleLabel.text = Translations.Translate("RON_NAM");

			// Close button.
			UIButton closeButton = AddUIComponent<UIButton>();
			closeButton.relativePosition = new Vector2(width - 35, 2);
			closeButton.normalBgSprite = "buttonclose";
			closeButton.hoveredBgSprite = "buttonclosehover";
			closeButton.pressedBgSprite = "buttonclosepressed";
			closeButton.eventClick += (component, clickEvent) => Close();

			// Network type dropdown.
			typeDropDown = UIControls.AddLabelledDropDown(this, Margin, MenuY, Translations.Translate("RON_PNL_TYP"), 170f);
			typeDropDown.items = netDescriptions;
			typeDropDown.selectedIndex = 0;
			typeDropDown.eventSelectedIndexChanged += TypeChanged;

			// Target network list.
			UIPanel leftPanel = AddUIComponent<UIPanel>();
			leftPanel.width = LeftWidth;
			leftPanel.height = PanelHeight;
			leftPanel.relativePosition = new Vector2(Margin, TitleHeight + ToolbarHeight);
			targetList = UIFastList.Create<UITargetNetRow>(leftPanel);
			ListSetup(targetList);

			// Loaded network list.
			UIPanel rightPanel = AddUIComponent<UIPanel>();
			rightPanel.width = RightWidth;
			rightPanel.height = PanelHeight;
			rightPanel.relativePosition = new Vector2(LeftWidth + (Margin * 3), TitleHeight + ToolbarHeight);
			loadedList = UIFastList.Create<UIReplacementNetRow>(rightPanel);
			ListSetup(loadedList);

			// Replace button.
			replaceButton = UIControls.AddButton(this, ReplaceX, ReplaceY, Translations.Translate("RON_PNL_REP"), ReplaceWidth, ReplaceHeight, scale: 1.0f);
			replaceButton.eventClicked += Replace;

			// View previous segment button.
			prevButton = UIControls.AddSmallerButton(this, PrevX, GoToY, Translations.Translate("RON_PNL_VPS"), ButtonWidth);
			prevButton.eventClicked += PreviousSegment;

			// View next segment button.
			nextButton = UIControls.AddSmallerButton(this, NextX, GoToY, Translations.Translate("RON_PNL_VNS"), ButtonWidth);
			nextButton.eventClicked += NextSegment;

			// Name filter.
			nameFilter = UIControls.LabelledTextField(this, FilterX, ReplaceY, Translations.Translate("RON_FIL_NAME"));
			nameFilter.eventTextChanged += (control, text) => LoadedList();
			nameFilter.eventTextSubmitted += (control, text) => LoadedList();

			// Vanilla filter.
			hideVanilla = UIControls.AddCheckBox((UIComponent)(object)this, FilterX, HideVanillaY, Translations.Translate("RON_PNL_HDV"));
			hideVanilla.isChecked = true;
			hideVanilla.eventCheckChanged += (control, isChecked) => LoadedList();

			// Same width only check.
			sameWidthCheck = UIControls.AddCheckBox(this, FilterX, SameWidthY, Translations.Translate("RON_PNL_WID"));
			sameWidthCheck.isChecked = true;
			sameWidthCheck.eventCheckChanged += (control, isChecked) => LoadedList();

			// Replacing label (starts hidden).
			replacingLabel = UIControls.AddLabel(this, ReplaceX, ReplaceY, Translations.Translate("RON_PNL_RIP"), MiddleContentWidth);
			replacingLabel.Hide();

			// Progress label (starts hidden).
			progressLabel = UIControls.AddLabel(this, ReplaceX, ProgressY, ".", MiddleContentWidth);
			progressLabel.Hide();

			// Populate lists.
			TargetList();
			LoadedList();
		}


		/// <summary>
		/// Network type dropdown change handler.
		/// </summary>
		/// <param name="control">Calling component (unused)</param>
		/// <param name="index">New selected index (unused)</param>
		private void TypeChanged(UIComponent control, int index)
		{
			// Rebuild target and replacement lists.
			TargetList();
			LoadedList();
		}


		/// <summary>
		/// Updates button states (enabled/disabled) according to current control states.
		/// </summary>
		private void UpdateButtonStates()
		{
			// Enable go to segment buttons if we have a valid target, disable it otherwise.
			if (selectedTarget != null)
			{
				prevButton.Enable();
				nextButton.Enable();
			}
			else
			{
				prevButton.Disable();
				nextButton.Disable();
			}

			// Enable replace button if we have both a valid target and replacement, disable it otherwise.
			if (selectedTarget != null && selectedReplacement != null)
			{
				replaceButton.Enable();
			}
			else
			{
				replaceButton.Disable();
			}
		}


		/// <summary>
		/// Replace button event handler.
		/// <param name="control">Calling component (unused)</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		/// </summary>
		private void Replace(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			// Set flags and reset timer.
			replacing = true;
			replacingDone = false;
			timer = 0;

			// Add ReplaceNets method to simulation manager action (don't want to muck around with simulation stuff from the main thread....)
			Singleton<SimulationManager>.instance.AddAction(delegate { ReplaceNets(selectedTarget, selectedReplacement); });

			// Set UI to 'replacing' state.
			replaceButton.Disable();
			replaceButton.Hide();
			replacingLabel.Show();
			progressLabel.text = ".";
			progressLabel.Show();
		}


		/// <summary>
		/// Next segment button change handler.
		/// </summary>
		/// <param name="control">Calling component (unused)</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		private void NextSegment(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			ushort targetSegment = 0;

			// Find first segment matching current selection.
			// Need to do this for each segment instance, so iterate through all segments.
			NetSegment[] segments = NetManager.instance.m_segments.m_buffer;
			for (ushort segmentID = 0; segmentID < segments.Length; ++segmentID)
			{
				// Check for match with selected target.
				if (segments[segmentID].Info == selectedTarget)
				{
					// Got a match - set target if it isn't already set (first instance found).
					if (targetSegment == 0)
					{
						targetSegment = segmentID;
					}

					// Is the previously-shown segment counter ahead of this?
					if (segmentID > lastViewedSegment)
                    {
						// 'Fresh' segment - update target to this and finish looping, since we've found our taget.
						targetSegment = segmentID;
						break;
					}
                }
			}

			// Did we find a valid target?
			if (targetSegment != 0)
			{
				ViewSegement(targetSegment);
			}
		}


		/// <summary>
		/// Previous segment button change handler.
		/// </summary>
		/// <param name="control">Calling component (unused)</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		private void PreviousSegment(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			ushort targetSegment = 0;

			// Find first segment matching current selection.
			// Need to do this for each segment instance, so iterate through all segments.
			NetSegment[] segments = NetManager.instance.m_segments.m_buffer;
			for (ushort segmentID = (ushort)(segments.Length - 1); segmentID > 0; --segmentID)
			{
				// Check for match with selected target.
				if (segments[segmentID].Info == selectedTarget)
				{
					// Got a match - set target if it isn't already set (first instance found).
					if (targetSegment == 0)
					{
						targetSegment = segmentID;
					}

					// Is the previously-shown segment counter ahead of this?
					if (segmentID < lastViewedSegment)
					{
						// 'Fresh' segment - update target to this and finish looping, since we've found our taget.
						targetSegment = segmentID;
						break;
					}
				}
			}

			// Did we find a valid target?
			if (targetSegment != 0)
			{
				ViewSegement(targetSegment);
			}
		}


		/// <summary>
		/// Moves the camera to view the given segment.
		/// </summary>
		/// <param name="segmentID">Segment ID of target segment</param>
		private void ViewSegement(ushort segmentID)
        {
			// Yes - set camera position.
			Vector3 cameraPosition = NetManager.instance.m_segments.m_buffer[segmentID].m_middlePosition;
			cameraPosition.y = Camera.main.transform.position.y;
			ToolsModifierControl.cameraController.SetTarget(new InstanceID { NetSegment = segmentID }, cameraPosition, true);

			// Update last viewed segment to this one.
			lastViewedSegment = segmentID;
		}


		/// <summary>
		/// Perform actual network replacement.
		/// </summary>
		private void ReplaceNets(NetInfo target, NetInfo replacement)
		{
			if (target != null && replacement != null)
			{
				// Local references.
				NetManager netManager = Singleton<NetManager>.instance;
				string targetName = target.name;
				Randomizer randomizer = new Randomizer();

				// Need to do this for each segment instance, so iterate through all segments.
				NetSegment[] segments = NetManager.instance.m_segments.m_buffer;
				for (ushort segmentID = 0; segmentID < segments.Length; ++segmentID)
				{
					// Local reference.
					NetSegment segment = segments[segmentID];

					// Check that this is a valid network.
					if (segment.m_flags != NetSegment.Flags.None)
					{
						NetInfo segmentInfo = segment.Info;

						if (segmentInfo != null && segmentInfo.name.Equals(targetName))
						{
							// Found a match.  Perform a safety check for outside connections.
							if (((netManager.m_nodes.m_buffer[segment.m_startNode].m_flags & NetNode.Flags.Outside) == 0) && ((netManager.m_nodes.m_buffer[segment.m_endNode].m_flags & NetNode.Flags.Outside) == 0))
							{
								// No outside connection - okay to proceed.  Deactivate old segment.
								segment.Info.m_netAI.ManualDeactivation(segmentID, ref segment);

								// Create new segment, duplication location, direction, etc. as current segment.
								netManager.CreateSegment(out ushort newSegmentID, ref randomizer, replacement, segment.m_startNode, segment.m_endNode, segment.m_startDirection, segment.m_endDirection, segment.m_buildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, (segment.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None);

								// Remove old segment.
								netManager.ReleaseSegment(segmentID, false);

								// Ensure old segment info reference updated to this.
								segments[segmentID].Info = replacement;
							}
							else
							{
								Logging.Message("skipping outside connection segment ", segmentID.ToString(), " - ", segmentInfo.name);
							}
						}
					}
				}
			}

			// All done - set status flag.
			replacingDone = true;
		}


		/// <summary>
		/// Populates a fastlist with a list of networks currently on the map.
		/// </summary>
		/// <returns>Populated fastlist of networks on map</returns>
		private void TargetList()
		{
			// List of prefabs.
			List<NetInfo> netList = new List<NetInfo>();

			// Iterate through all segments in map.
			NetSegment[] segments = NetManager.instance.m_segments.m_buffer;
			for (ushort i = 0; i < segments.Length; ++i)
			{
				// Local reference.
				NetInfo segmentInfo = segments[i].Info;

				// See if this net info is already in our list.
				if (!netList.Contains(segmentInfo))
				{
					// No - apply network type filter.
					if (MatchType(segmentInfo))
					{
						netList.Add(segmentInfo);
					}
				}
			}

			// Create return fastlist from our filtered list, ordering by name.
			targetList.rowsData = new FastList<object>
			{
				m_buffer = netList.OrderBy(item => PrefabUtils.GetDisplayName(item)).ToArray(),
				m_size = netList.Count
			};

			// Clear current selection.
			targetList.selectedIndex = -1;
			SelectedTarget = null;
		}


		/// <summary>
		/// Populates a fastlist with a list of loaded networks.
		/// </summary>
		/// <returns>Populated fastlist of networks on map</returns>
		private void LoadedList()
		{
			// List of prefabs.
			List<NetInfo> netList = new List<NetInfo>();

			// Iterate through all loaded networks.
			for (uint i = 0u; i < PrefabCollection<NetInfo>.LoadedCount(); ++i)
			{
				// Get network and add to our list, if it isn't null.
				NetInfo network = PrefabCollection<NetInfo>.GetLoaded(i);
				if (network?.name != null)
				{
					// Apply vanilla filter.
					// Find any leading period (Steam package number).
					if (hideVanilla.isChecked && network.name.IndexOf('.') < 0)
					{
						// No leading period - it's vanilla.  Skip.
						continue;
					}

					// Apply name filter.
					if (StringExtensions.IsNullOrWhiteSpace(nameFilter.text.Trim()) || PrefabUtils.GetDisplayName(network.name).ToLower().Contains(nameFilter.text.Trim().ToLower()))
					{
						// Apply network type filter.
						if (MatchType(network))
						{
							// Apply width filter.
							if (sameWidthCheck.isChecked && selectedTarget != null)
							{
								// Check if this network has the same half-width.
								if (network.m_halfWidth != selectedTarget.m_halfWidth)
								{
									// No match; skip this one.
									continue;
								}
							}

							// Passed filtering; add this one to the list.
							netList.Add(network);
						}
					}
				}
			}

			// Create return fastlist from our filtered list, ordering by name.
			loadedList.rowsData = new FastList<object>
			{
				m_buffer = netList.OrderBy(item => PrefabUtils.GetDisplayName(item)).ToArray(),
				m_size = netList.Count
			};

			// Clear current selection.
			loadedList.selectedIndex = -1;
			SelectedReplacement = null;
		}


		/// <summary>
		/// Determines if the given net prefab matches the current net type filter.
		/// </summary>
		/// <param name="network">Net prefab to test</param>
		/// <returns>True if it matches the filter, false otherwise</returns>
		private bool MatchType(NetInfo network)
		{
			// Check for match.
			if (network.GetAI().GetType().IsAssignableFrom(netTypes[typeDropDown.selectedIndex]))
			{
				// Match - return true.
				return true;
			}

			// If we got here, we didn't get a match.
			return false;
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
			fastList.rowHeight = 30f;

			// Behaviour.
			fastList.canSelect = true;
			fastList.autoHideScrollbar = true;

			// Data.
			fastList.rowsData = new FastList<object>();
			fastList.selectedIndex = -1;
		}
	}
}
