using System;
using System.Linq;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;


namespace RON
{
	// Display order state.
	internal enum OrderBy
    {
		NameAscending = 0,
		NameDescending,
		CreatorAscending,
		CreatorDescending
    }

	// String search type state.
	internal enum SearchTypes
    {
		SearchNetwork = 0,
		SearchCreator,
		NumTypes
    }


	/// <summary>
	/// Static class to manage the BOB info panel.
	/// </summary>
	internal class ReplacerPanel : UIPanel
	{
		// Layout constants - general.
		private const float Margin = 5f;

		// Layout constants - Y.
		private const float TitleHeight = 45f;
		private const float ToolbarHeight = 75f;
		private const float ListHeight = 15 * UINetRow.RowHeight;
		private const float PreviewHeight = 100f;
		private const float ToolRow1Y = TitleHeight + Margin;
		private const float ToolRow2Y = ToolRow1Y + 35f;
		private const float SpacerBarY = TitleHeight + ToolbarHeight + Margin;
		private const float ListTitleY = SpacerBarY + 15f;
		private const float ListHeaderY = ListTitleY + 30f;
		private const float ListY = ListHeaderY + 20f;
		private const float PanelHeight = ListY + ListHeight + Margin;
		private const float HideVanillaY = ToolRow1Y + 30f;
		private const float SameWidthY = HideVanillaY + 20f;
		private const float ReplacementSpriteY = ListY + (PreviewHeight * 2f);

		// Layout constants - X.
		private const float LeftWidth = 450f;
		private const float OrderArrowWidth = 32f;
		private const float PreviewWidth = 109f;
		private const float PreviewArrowWidth = 32f;
		private const float MiddleWidth = PreviewWidth + PreviewArrowWidth;
		private const float RightWidth = 450f;
		private const float MiddlePanelX = Margin + LeftWidth + Margin;
		private const float RightPanelX = MiddlePanelX + MiddleWidth + Margin;
		private const float PanelWidth = RightPanelX + RightWidth + Margin;
		private const float ReplaceWidth = 150f;
		private const float FilterX = (PanelWidth - Margin) - 360f;
		private const float FilterWidth = 220;
		private const float FilterMenuxX = FilterX + FilterWidth + Margin;
		private const float FilterMenuWidth = PanelWidth - FilterMenuxX - Margin;
		private const float ButtonWidth = 220f;
		private const float PrevX = Margin;
		private const float NextX = LeftWidth + Margin - ButtonWidth;


		// Instance references.
		private static GameObject uiGameObject;
		private static ReplacerPanel panel;
		internal static ReplacerPanel Panel => panel;

		// Current selections.
		private NetInfo selectedTarget, selectedReplacement;

		// Segment info record.
		internal readonly Dictionary<NetInfo, List<ushort>> segmentDict = new Dictionary<NetInfo, List<ushort>>();

		// Parent-child network relation dictionaries.
		internal readonly Dictionary<NetInfo, NetInfo> slopeParents = new Dictionary<NetInfo, NetInfo>();
		internal readonly Dictionary<NetInfo, NetInfo> elevatedParents = new Dictionary<NetInfo, NetInfo>();
		internal readonly Dictionary<NetInfo, NetInfo> bridgeParents = new Dictionary<NetInfo, NetInfo>();
		internal readonly Dictionary<NetInfo, NetInfo> tunnelParents = new Dictionary<NetInfo, NetInfo>();

		// Panel components.
		private readonly UIFastList targetList, loadedList;
		private readonly UIButton replaceButton, undoButton, prevButton, nextButton;
		private readonly UIButton targetNameButton, targetCreatorButton, loadedNameButton, loadedCreatorButton;
		private readonly UITextField nameFilter;
		private readonly UIDropDown typeDropDown, searchTypeMenu;
		private readonly UILabel replacingLabel, progressLabel;
		private readonly UICheckBox sameWidthCheck, hideVanilla, globalCheck, districtCheck, segmentCheck;
		private readonly UISprite targetPreviewSprite, replacementPreviewSprite;

		// Status.
		internal bool replacingDone;
		private bool replacing;
		private float timer;
		private int timerStep;
		private ushort lastViewedSegment;

		// Search settings.
		private int targetSearchStatus, loadedSearchStatus;



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
					undoButton.Show();

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

					// Update display (preview and button states).
					DisplayNetwork(selectedTarget, targetPreviewSprite);
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
				// Assign new replacement.
				selectedReplacement = value;

				// Update display (preview and button states).
				DisplayNetwork(selectedReplacement, replacementPreviewSprite);
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
			size = new Vector2(PanelWidth, PanelHeight);

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

			// Decorative icon (top-left).
			UISprite iconSprite = AddUIComponent<UISprite>();
			iconSprite.relativePosition = new Vector2(5, 5);
			iconSprite.height = 32f;
			iconSprite.width = 32f;
			iconSprite.atlas = Textures.RonButtonSprites;
			iconSprite.spriteName = "normal";

			// Network type dropdown.
			typeDropDown = UIControls.AddLabelledDropDown(this, Margin, ToolRow1Y, Translations.Translate("RON_PNL_TYP"), 250f);
			typeDropDown.items = netDescriptions;
			typeDropDown.selectedIndex = 0;
			typeDropDown.eventSelectedIndexChanged += TypeChanged;

			// Spacer panel.
			UIPanel spacerPanel = AddUIComponent<UIPanel>();
			spacerPanel.width = PanelWidth - (Margin * 2);
			spacerPanel.height = 5f;
			spacerPanel.relativePosition = new Vector2(Margin, SpacerBarY);
			spacerPanel.backgroundSprite = "WhiteRect";

			// Target network list.
			UIPanel leftPanel = AddUIComponent<UIPanel>();
			leftPanel.width = LeftWidth;
			leftPanel.height = ListHeight;
			leftPanel.relativePosition = new Vector2(Margin, ListY);
			targetList = UIFastList.Create<UITargetNetRow>(leftPanel);
			ListSetup(targetList);

			// Loaded network list.
			UIPanel rightPanel = AddUIComponent<UIPanel>();
			rightPanel.width = RightWidth;
			rightPanel.height = ListHeight;
			rightPanel.relativePosition = new Vector2(RightPanelX, ListY);
			loadedList = UIFastList.Create<UIReplacementNetRow>(rightPanel);
			ListSetup(loadedList);

			// List titles.
			UIControls.AddLabel(this, Margin, ListTitleY, Translations.Translate("RON_PNL_MAP"), LeftWidth);
			UIControls.AddLabel(this, RightPanelX, ListTitleY, Translations.Translate("RON_PNL_AVA"), RightWidth);

			// List headers.
			UIControls.AddLabel(this, Margin + UINetRow.NameX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_NET"), UINetRow.CreatorX - UINetRow.NameX);
			UIControls.AddLabel(this, Margin + UINetRow.CreatorX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_CRE"), LeftWidth - UINetRow.CreatorX);
			UIControls.AddLabel(this, RightPanelX + UINetRow.NameX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_NET"), UINetRow.CreatorX - UINetRow.NameX);
			UIControls.AddLabel(this, RightPanelX + UINetRow.CreatorX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_CRE"), RightWidth - UINetRow.CreatorX);

			// Order buttons.
			targetNameButton = ArrowButton(this, Margin + UINetRow.NameX, ListHeaderY, OrderArrowWidth, ListY - ListHeaderY);
			targetCreatorButton = ArrowButton(this, Margin + UINetRow.CreatorX, ListHeaderY, OrderArrowWidth, ListY - ListHeaderY);
			loadedNameButton = ArrowButton(this, RightPanelX + UINetRow.NameX, ListHeaderY, OrderArrowWidth, ListY - ListHeaderY);
			loadedCreatorButton = ArrowButton(this, RightPanelX + UINetRow.CreatorX, ListHeaderY, OrderArrowWidth, ListY - ListHeaderY);

			targetNameButton.eventClicked += SortTargets;
			targetCreatorButton.eventClicked += SortTargets;
			loadedNameButton.eventClicked += SortLoaded;
			loadedCreatorButton.eventClicked += SortLoaded;
			
			// Default is name ascending.
			SetFgSprites(targetNameButton, "IconUpArrow2Focused");
			SetFgSprites(loadedNameButton, "IconUpArrow2Focused");

			// Replace button.
			replaceButton = UIControls.AddButton(this, MiddlePanelX, ToolRow1Y, Translations.Translate("RON_PNL_REP"), ReplaceWidth, scale: 1.0f);
			replaceButton.eventClicked += Replace;

			// Undo button.
			undoButton = UIControls.AddButton(this, MiddlePanelX, ToolRow2Y, Translations.Translate("RON_PNL_UND"), ReplaceWidth);
			undoButton.eventClicked += Undo;

			// View previous segment button.
			prevButton = UIControls.AddSmallerButton(this, PrevX, ToolRow2Y, Translations.Translate("RON_PNL_VPS"), ButtonWidth);
			prevButton.eventClicked += PreviousSegment;

			// View next segment button.
			nextButton = UIControls.AddSmallerButton(this, NextX, ToolRow2Y, Translations.Translate("RON_PNL_VNS"), ButtonWidth);
			nextButton.eventClicked += NextSegment;

			// Name filter.
			nameFilter = UIControls.LabelledTextField(this, FilterX, ToolRow1Y, Translations.Translate("RON_FIL_NAME"), FilterWidth, 25f, vertPad: 5);
			nameFilter.eventTextChanged += (control, text) => LoadedList();
			nameFilter.eventTextSubmitted += (control, text) => LoadedList();

			// Search by name/author dropdown.
			searchTypeMenu = UIControls.AddDropDown(this, FilterMenuxX, ToolRow1Y, FilterMenuWidth);
			searchTypeMenu.items = new string[(int)SearchTypes.NumTypes] { Translations.Translate("RON_PNL_NET"), Translations.Translate("RON_PNL_CRE") };
			searchTypeMenu.selectedIndex = (int)SearchTypes.SearchNetwork;
			searchTypeMenu.eventSelectedIndexChanged += (control, isChecked) => LoadedList();

			// Vanilla filter.
			hideVanilla = UIControls.AddCheckBox((UIComponent)(object)this, FilterX, HideVanillaY, Translations.Translate("RON_PNL_HDV"));
			hideVanilla.isChecked = true;
			hideVanilla.eventCheckChanged += (control, isChecked) => LoadedList();

			// Same width only check.
			sameWidthCheck = UIControls.AddCheckBox(this, FilterX, SameWidthY, Translations.Translate("RON_PNL_WID"));
			sameWidthCheck.isChecked = true;
			sameWidthCheck.eventCheckChanged += (control, isChecked) => LoadedList();

			// Replacing label (starts hidden).
			replacingLabel = UIControls.AddLabel(this, MiddlePanelX, ToolRow1Y, Translations.Translate("RON_PNL_RIP"), ReplaceWidth);
			replacingLabel.Hide();

			// Progress label (starts hidden).
			progressLabel = UIControls.AddLabel(this, MiddlePanelX, ToolRow2Y, ".", ReplaceWidth);
			progressLabel.Hide();

			// Preview sprites.
			targetPreviewSprite = AddPreviewSprite(MiddlePanelX + PreviewArrowWidth, ListY);
			replacementPreviewSprite = AddPreviewSprite(MiddlePanelX, ReplacementSpriteY);

			// Arrow sprites.
			AddArrowSprite(targetPreviewSprite, -PreviewArrowWidth, "ArrowLeft");
			AddArrowSprite(replacementPreviewSprite, PreviewWidth, "ArrowRight");

			// Tree/Prop checkboxes.
			globalCheck = IconToggleCheck(this, typeDropDown.relativePosition.x + typeDropDown.width + Margin, ToolRow1Y, "ToolbarIconZoomOutGlobeDisabled", "ToolbarIconZoomOutGlobe", "Ingame", "RON_PNL_GLB");
			districtCheck = IconToggleCheck(this, globalCheck.relativePosition.x + globalCheck.width, ToolRow1Y, "DistrictPaintDisabled", "DistrictPaint", "Thumbnails", "RON_PNL_DIS");
			segmentCheck = IconToggleCheck(this, districtCheck.relativePosition.x + districtCheck.width, ToolRow1Y, "ThumbnailRoadTypeSmallRoadDisabled", "ThumbnailRoadTypeSmallRoad", "Thumbnails", "RON_PNL_SEG");
			globalCheck.isChecked = true;
			globalCheck.eventCheckChanged += CheckChanged;
			districtCheck.eventCheckChanged += CheckChanged;
			segmentCheck.eventCheckChanged += CheckChanged;

			// Populate lists.
			PrefabUtils.GetCreators();
			TargetList();
			LoadedList();

			// Force loaded list refresh.
			loadedList.Refresh();

			// Populate parent dictionaries.
			PrefabUtils.GetParents(slopeParents, elevatedParents, bridgeParents, tunnelParents);
		}


		/// <summary>
		/// Network type dropdown change handler.
		/// </summary>
		/// <param name="control">Calling component</param>
		/// <param name="index">New selected index</param>
		private void CheckChanged(UIComponent control, bool isChecked)
        {
			if (isChecked)
            {
				if (control == globalCheck)
                {
					districtCheck.isChecked = false;
					segmentCheck.isChecked = false;
				}
				else if (control == districtCheck)
				{
					globalCheck.isChecked = false;
					segmentCheck.isChecked = false;
				}
				else if (control == segmentCheck)
				{
					globalCheck.isChecked = false;
					districtCheck.isChecked = false;
				}
			}
			else if (!globalCheck.isChecked && !districtCheck.isChecked && !segmentCheck.isChecked)
            {
				(control as UICheckBox).isChecked = true;
            }

			// Ensure correct checkbox label.
			if (segmentCheck.isChecked)
            {
				replaceButton.text = Translations.Translate("RON_PNL_RES");
            }
			else
            {
				replaceButton.text = Translations.Translate("RON_PNL_REP");
			}
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
		/// Replace button event handler.
		/// <param name="control">Calling component (unused)</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		/// </summary>
		private void Replace(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			// Only do stuff if we've got valid selections.
			if (selectedTarget != null & selectedReplacement != null)
			{
				// Set panel to replacing state.
				SetReplacing();

				// Create list of replacement segments.
				List<ushort> segments;
				if (segmentCheck.isChecked)
                {
					// Single replacement only.
					segments = new List<ushort> { lastViewedSegment };
                }
				else
                {
					// Multiple replacements - get list from segment dictionary.
					segments = segmentDict[selectedTarget];
                }

				// Add ReplaceNets method to simulation manager action (don't want to muck around with simulation stuff from the main thread....)
				Singleton<SimulationManager>.instance.AddAction(delegate { Replacer.ReplaceNets(selectedTarget, selectedReplacement, segments,  globalCheck.isChecked ? (ushort)0 : lastViewedSegment, districtCheck.isChecked ); });
			}
		}


		/// <summary>
		/// Undo button event handler.
		/// <param name="control">Calling component (unused)</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		/// </summary>
		private void Undo(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			// Only do stuff if we've got a valid undo state.
			if (Replacer.HasUndo)
			{
				// Set panel to replacing state.
				SetReplacing();

				// Add ReplaceNets method to simulation manager action (don't want to muck around with simulation stuff from the main thread....)
				Singleton<SimulationManager>.instance.AddAction(delegate { Replacer.Undo(); });
			}
		}


		/// <summary>
		/// Next segment button event handler.
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
		/// Previous segment button event handler.
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
		/// Loaded list sort button event handler.
		/// <param name="control">Calling component</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		/// </summary>
		private void SortLoaded(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			// Check if we are using the name or creator button.
			if (control == loadedNameButton)
			{
				// Name button.
				// Toggle status (set to descending if we're currently ascending, otherwise set to ascending).
				if (loadedSearchStatus == (int)OrderBy.NameAscending)
				{
					// Order by name descending.
					loadedSearchStatus = (int)OrderBy.NameDescending;
				}
				else
				{
					// Order by name ascending.
					loadedSearchStatus = (int)OrderBy.NameAscending;
				}

				// Reset name order buttons.
				SetSortButton(loadedNameButton, loadedCreatorButton, loadedSearchStatus);
			}
			else if (control == loadedCreatorButton)
			{
				// Creator button.
				// Toggle status (set to descending if we're currently ascending, otherwise set to ascending).
				if (loadedSearchStatus == (int)OrderBy.CreatorAscending)
				{
					// Order by creator descending.
					loadedSearchStatus = (int)OrderBy.CreatorDescending;
				}
				else
				{
					// Order by name ascending.
					loadedSearchStatus = (int)OrderBy.CreatorAscending;
				}

				// Reset name order buttons.
				SetSortButton(loadedCreatorButton, loadedNameButton, loadedSearchStatus);
			}

			// Regenerate loaded list.
			LoadedList();
		}


		/// <summary>
		/// Target list sort button event handler.
		/// <param name="control">Calling component</param>
		/// <param name="mouseEvent">Mouse event (unused)</param>
		/// </summary>
		private void SortTargets(UIComponent control, UIMouseEventParameter mouseEvent)
		{
			// Check if we are using the name or creator button.
			if (control == targetNameButton)
			{
				// Name button.
				// Toggle status (set to descending if we're currently ascending, otherwise set to ascending).
				if (targetSearchStatus == (int)OrderBy.NameAscending)
				{
					// Order by name descending.
					targetSearchStatus = (int)OrderBy.NameDescending;
				}
				else
				{
					// Order by name ascending.
					targetSearchStatus = (int)OrderBy.NameAscending;
				}

				// Reset name order buttons.
				SetSortButton(targetNameButton, targetCreatorButton, targetSearchStatus);
			}
			else if (control == targetCreatorButton)
			{
				// Creator button.
				// Toggle status (set to descending if we're currently ascending, otherwise set to ascending).
				if (targetSearchStatus == (int)OrderBy.CreatorAscending)
				{
					// Order by creator descending.
					targetSearchStatus = (int)OrderBy.CreatorDescending;
				}
				else
				{
					// Order by name ascending.
					targetSearchStatus = (int)OrderBy.CreatorAscending;
				}

				// Reset name order buttons.
				SetSortButton(targetCreatorButton, targetNameButton, targetSearchStatus);
			}

			// Regenerate loaded list.
			TargetList();
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
			replaceButton.isEnabled = selectedTarget != null && selectedReplacement != null;

			// Enable undo button if we have a valid undo buffer.
			undoButton.isEnabled = Replacer.HasUndo;
		}


		/// <summary>
		/// Initializes the 'replacing' state - sets flags, timers, UI state.
		/// </summary>
		private void SetReplacing()
		{
			// Set flags and reset timer.
			replacing = true;
			replacingDone = false;
			timer = 0;

			// Set UI to 'replacing' state.
			replaceButton.Disable();
			undoButton.Disable();
			replaceButton.Hide();
			undoButton.Hide();
			replacingLabel.Show();
			progressLabel.text = ".";
			progressLabel.Show();
		}


		/// <summary>
		/// Populates a fastlist with a list of networks currently on the map.
		/// </summary>
		/// <returns>Populated fastlist of networks on map</returns>
		private void TargetList()
		{
			// Clear segment dictionary.
			segmentDict.Clear();

			// List of prefabs to display.
			Dictionary<NetInfo, NetRowItem> netList = new Dictionary<NetInfo, NetRowItem>();

			// Iterate through all segments in map.
			NetManager netManager = Singleton<NetManager>.instance;
			NetSegment[] segments = netManager.m_segments.m_buffer;
			for (ushort i = 0; i < segments.Length; ++i)
			{
				// Local references.
				NetInfo segmentInfo = segments[i].Info;

				// Ignore segments null infos or with outside connections.
				if (segmentInfo != null && ((netManager.m_nodes.m_buffer[segments[i].m_startNode].m_flags & NetNode.Flags.Outside) == 0) && ((netManager.m_nodes.m_buffer[segments[i].m_endNode].m_flags & NetNode.Flags.Outside) == 0))
				{
					// See if this net info is already in our list.
					if (!netList.ContainsKey(segmentInfo))
					{
						// No - apply network type filter.
						if (MatchType(segmentInfo))
						{
							netList.Add(segmentInfo, new NetRowItem
							{
								prefab = segmentInfo,
								displayName = PrefabUtils.GetDisplayName(segmentInfo, out bool _),
								creator = PrefabUtils.GetCreator(segmentInfo)
							});
						}
					}

					// Add segment to segment dictionary.
					if (segmentDict.ContainsKey(segmentInfo))
					{
						// Net already exists - add segment to existing entry.
						segmentDict[segmentInfo].Add(i);
					}
					else
					{
						// Net entry doesn't exist yet; create it with this segment as first segment entry.
						segmentDict.Add(segmentInfo, new List<ushort> { i });
					}
				}
			}

			// Create new object list for fastlist, ordering as approprite.
			object[] objectArray;
			switch (targetSearchStatus)
			{
				case (int)OrderBy.NameDescending:
					objectArray = netList.Values.OrderByDescending(item => item.displayName).ToArray();
					break;
				case (int)OrderBy.CreatorAscending:
					objectArray = netList.Values.OrderBy(item => item.creator).ToArray();
					break;
				case (int)OrderBy.CreatorDescending:
					objectArray = netList.Values.OrderByDescending(item => item.creator).ToArray();
					break;
				default:
					objectArray = netList.Values.OrderBy(item => item.displayName).ToArray();
					break;
			}

			// Create return fastlist from our filtered list, ordering by name.
			targetList.rowsData = new FastList<object>
			{
				m_buffer = objectArray,
				m_size = objectArray.Length
			};

			// Clear current selection.
			targetList.selectedIndex = -1;
			SelectedTarget = null;

			// Force list update.
			targetList.Refresh();
		}


		/// <summary>
		/// Populates a fastlist with a list of loaded networks.
		/// </summary>
		/// <returns>Populated fastlist of networks on map</returns>
		private void LoadedList()
		{
			// List of prefabs to display.
			List<NetRowItem> netList = new List<NetRowItem>();

			// Iterate through all loaded networks.
			for (uint i = 0u; i < PrefabCollection<NetInfo>.LoadedCount(); ++i)
			{
				// Get network and add to our list, if it isn't null.
				NetInfo network = PrefabCollection<NetInfo>.GetLoaded(i);
				if (network?.name != null)
				{
					// Get display name and creator name.
					string displayName = PrefabUtils.GetDisplayName(network, out bool vaNext);

					// Apply 'hide vanilla and Next' filter.
					if (hideVanilla.isChecked && vaNext)
					{
						// It's vanilla/NExt.  Skip.
						continue;
					}

					string creator = PrefabUtils.GetCreator(network);

					// Apply text filter.
					string trimmedText = nameFilter.text.Trim();
					if (
						StringExtensions.IsNullOrWhiteSpace(trimmedText) ||
						(searchTypeMenu.selectedIndex == (int)SearchTypes.SearchNetwork && displayName.ToLower().Contains(trimmedText.ToLower())) ||
						(searchTypeMenu.selectedIndex == (int)SearchTypes.SearchCreator && creator.ToLower().Contains(trimmedText.ToLower()))
						)
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
							netList.Add(new NetRowItem
							{
								prefab = network,
								displayName = displayName,
								creator = creator
							});
						}
					}
				}
			}


			// Create new object list for fastlist, ordering as approprite.
			object[] objectArray;
			switch (loadedSearchStatus)
			{
				case (int)OrderBy.NameDescending:
					objectArray = netList.OrderByDescending(item => item.displayName).ToArray();
					break;
				case (int)OrderBy.CreatorAscending:
					objectArray = netList.OrderBy(item => item.creator).ToArray();
					break;
				case (int)OrderBy.CreatorDescending:
					objectArray = netList.OrderByDescending(item => item.creator).ToArray();
					break;
				default:
					objectArray = netList.OrderBy(item => item.displayName).ToArray();
					break;
			}

			// Create return fastlist from our filtered list, ordering by name.
			loadedList.rowsData = new FastList<object>
			{
				m_buffer = objectArray,
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
			// Make sure we have a valid net and AI.
			if (network?.GetAI() is  NetAI ai)
			{
				// Check for match.
				if (ai.GetType().IsAssignableFrom(netTypes[typeDropDown.selectedIndex]))
				{
					// Match - return true.
					return true;
				}
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
			fastList.rowHeight = UINetRow.RowHeight;

			// Behaviour.
			fastList.canSelect = true;
			fastList.autoHideScrollbar = true;

			// Data.
			fastList.rowsData = new FastList<object>();
			fastList.selectedIndex = -1;
		}


		/// <summary>
		/// Updates the display to the specified network (displaying thumbnail and setting button states).
		/// </summary>
		/// <param name="network">Network to display</param>
		/// <param name="previewSprite">Preview sprite to display network thumbnail</param>
		private void DisplayNetwork(NetInfo network, UISprite previewSprite)
        {
			// Try to get preview sprite components.
			FindThumbnail(network, out UITextureAtlas previewAtlas, out string previewThumb);

			// Update preview image; check if this prefab has a valid thumbnail.
			if (network != null && previewAtlas != null && previewThumb != null)
			{
				// Valid thumbnail - preview it.
				previewSprite.atlas = previewAtlas;
				previewSprite.spriteName = previewThumb;
				previewSprite.Show();
			}
			else
			{
				// No valid thumbnail - hide preview.
				previewSprite.atlas = Textures.RonButtonSprites;
				previewSprite.spriteName = "normal";
				previewSprite.Hide();
			}

			UpdateButtonStates();
		}


		/// <summary>
		/// Sets the states of the two given sort buttons to match the given search status.
		/// </summary>
		/// <param name="activeButton">Currently active sort button</param>
		/// <param name="inactiveButton">Inactive button (other sort button for same list)</param>
		/// <param name="searchStatus">Search status to apply</param>
		private void SetSortButton(UIButton activeButton, UIButton inactiveButton, int searchStatus)
		{
			bool ascending = searchStatus == (int)OrderBy.CreatorAscending || searchStatus == (int)OrderBy.NameAscending;

			// Toggle status (set to descending if we're currently ascending, otherwise set to ascending).
			if (ascending)
			{
				// Order ascending.
				SetFgSprites(activeButton, "IconUpArrow2Focused");
			}
			else
			{
				// Order descending.
				SetFgSprites(activeButton, "IconDownArrow2Focused");
			}

			// Reset name order button.
			SetFgSprites(inactiveButton, "IconUpArrow2");
		}


		/// <summary>
		/// Finds a thumbnail image for the selected prefab.
		/// </summary>
		/// <param name="network">Network prefab to find thumbnail for</param>
		/// <param name="atlas">Thumbnail atlas</param>
		/// <param name="thumbnail">Thumbnail sprite name</param>
		private void FindThumbnail(NetInfo network, out UITextureAtlas atlas, out string thumbnail)
		{
			// Null check - quite possible.
			if (network == null)
            {
				atlas = null;
				thumbnail = null;
				return;
            }

			// Try original info first.
			UITextureAtlas thumbAtlas = network.m_Atlas;
			string thumbName = network.m_Thumbnail;

			// Check for any overrides.
			if (PrefabUtils.thumbnailMaps.ContainsKey(network.name))
            {
				KeyValuePair<string, string> entry = PrefabUtils.thumbnailMaps[network.name];
				if (!entry.Key.Equals(thumbAtlas.name))
                {
					thumbAtlas = TextureUtils.GetTextureAtlas(entry.Key); 
                }

				thumbName = entry.Value;
            }
			else 
			// If we didn't get a valid thumbnail directly, then try to find a parent and use its thumbnail.
			if (thumbAtlas == null || thumbName.IsNullOrWhiteSpace())
			{
				// Try slope parent.
				if (slopeParents.ContainsKey(network))
				{
					thumbAtlas = slopeParents[network].m_Atlas;
					thumbName = slopeParents[network].m_Thumbnail;
				}
				// Try elevated parent.
				else if (elevatedParents.ContainsKey(network))
				{
					thumbAtlas = elevatedParents[network].m_Atlas;
					thumbName = elevatedParents[network].m_Thumbnail;
				}
				// Try bridge parent.
				else if (bridgeParents.ContainsKey(network))
				{
					thumbAtlas = bridgeParents[network].m_Atlas;
					thumbName = bridgeParents[network].m_Thumbnail;
				}
				// Try tunnel parent.
				else if (tunnelParents.ContainsKey(network))
				{
					thumbAtlas = tunnelParents[network].m_Atlas;
					thumbName = tunnelParents[network].m_Thumbnail;
				}
			}

			// Whatever we got, that's the best we can do.
			atlas = thumbAtlas;
			thumbnail = thumbName;
		}


		/// <summary>
		/// Adds a preview image sprite at the specified coordinates (starts hidden).
		/// </summary>
		/// <param name="xPos">Relative X position</param>
		/// <param name="yPos">Relative Y position</param>
		/// <returns>New UI sprite</returns>
		private UISprite AddPreviewSprite(float xPos, float yPos)
		{
			UISprite previewSprite = AddUIComponent<UISprite>();
			previewSprite.relativePosition = new Vector2(xPos, yPos);
			previewSprite.height = PreviewHeight;
			previewSprite.width = PreviewWidth;
			previewSprite.Hide();

			return previewSprite;
		}


		/// <summary>
		/// Adds a preview arrow sprite at the specified coordinates with the specified thumbnail from "ingame" atlas.
		/// </summary>
		/// <param name="parent">Parent component</param>
		/// <param name="xPos">Relative X position</param>
		/// <param name="spriteName">Sprite name</param>
		/// <returns>Nw UI sprite</returns>
		private UISprite AddArrowSprite(UIComponent parent, float xPos, string spriteName)
		{
			UISprite arrowSprite = parent.AddUIComponent<UISprite>();
			arrowSprite.relativePosition = new Vector2(xPos, 0f);
			arrowSprite.height = PreviewHeight;
			arrowSprite.width = PreviewArrowWidth;
			arrowSprite.atlas = TextureUtils.InGameAtlas;
			arrowSprite.spriteName = spriteName;

			return arrowSprite;
		}


		/// <summary>
		/// Adds an arrow button.
		/// </summary>
		/// <param name="parent">Parent component</param>
		/// <param name="posX">Relative X postion</param>
		/// <param name="posY">Relative Y position</param>
		/// <param name="width">Button width (default 32)</param>
		/// <param name="height">Button height (default 20)</param>
		/// <returns>New arrow button</returns>
		private UIButton ArrowButton(UIComponent parent, float posX, float posY, float width = 32f, float height = 20f)
		{
			UIButton button = parent.AddUIComponent<UIButton>();

			// Size and position.
			button.size = new Vector2(width, height);
			button.relativePosition = new Vector2(posX, posY);

			// Appearance.
			SetFgSprites(button, "IconUpArrow2");
			button.canFocus = false;

			return button;
		}


		/// <summary>
		/// Sets the foreground sprites for the given button to the specified sprite.
		/// </summary>
		/// <param name="button">Targeted button</param>
		/// <param name="spriteName">Sprite name</param>
		private void SetFgSprites(UIButton button, string spriteName)
		{
			button.normalFgSprite = button.hoveredFgSprite = button.pressedFgSprite = button.focusedFgSprite = spriteName;
		}


		/// <summary>
		/// Adds an icon toggle checkbox.
		/// </summary>
		/// <param name="parent">Parent component</param>
		/// <param name="xPos">Relative X position</param>
		/// <param name="yPos">Relative Y position</param>
		/// <param name="defaultSprite">Default (unchecked) sprite name</param>
		/// <param name="checkedSprite">Checked sprite name</param>
		/// <param name="atlasName">Atlas name</param>
		/// <param name="tooltipKey">Tooltip translation key</param>
		/// <returns>New checkbox</returns>
		private UICheckBox IconToggleCheck(UIComponent parent, float xPos, float yPos, string defaultSprite, string checkedSprite, string atlasName, string tooltipKey)
		{
			const float ToggleSpriteSize = 24f;

			// Size and position.
			UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
			checkBox.width = ToggleSpriteSize;
			checkBox.height = ToggleSpriteSize;
			checkBox.clipChildren = true;
			checkBox.relativePosition = new Vector2(xPos, yPos);

			// Checkbox sprites.
			UISprite sprite = checkBox.AddUIComponent<UISprite>();
			sprite.atlas = TextureUtils.GetTextureAtlas(atlasName);
			sprite.spriteName = defaultSprite;
			sprite.size = new Vector2(ToggleSpriteSize, ToggleSpriteSize);
			sprite.relativePosition = Vector3.zero;

			checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
			((UISprite)checkBox.checkedBoxObject).atlas = TextureUtils.GetTextureAtlas(atlasName);
			((UISprite)checkBox.checkedBoxObject).spriteName = checkedSprite;
			checkBox.checkedBoxObject.size = new Vector2(ToggleSpriteSize, ToggleSpriteSize);
			checkBox.checkedBoxObject.relativePosition = Vector3.zero;

			checkBox.tooltip = Translations.Translate(tooltipKey);

			return checkBox;
		}
	}
}
