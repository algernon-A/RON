﻿// <copyright file="ReplacerPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using AlgernonCommons;
	using AlgernonCommons.Notifications;
	using AlgernonCommons.Translation;
	using AlgernonCommons.UI;
	using ColossalFramework;
	using ColossalFramework.UI;
	using UnityEngine;

	/// <summary>
	/// RON network replacer panel.
	/// </summary>
	internal class ReplacerPanel : UIPanel
	{
		// Layout constants - general.
		private const float Margin = 5f;
		private const float ToggleSpriteSize = 24f;
		private const float ReplaceButtonHeight = 30f;

		// Layout constants - Y.
		private const float TitleHeight = 45f;
		private const float ToolRowHeight = 35f;
		private const float ToolbarHeight = ToolRowHeight * 3f;
		private const float ListHeight = 15 * UINetRow.DefaultRowHeight;
		private const float PreviewHeight = 100f;
		private const float ToolRow1Y = TitleHeight + Margin;
		private const float ToolRow2Y = ToolRow1Y + ToolRowHeight;
		private const float ToolRow3Y = ToolRow2Y + ToolRowHeight;
		private const float SpacerBarY = TitleHeight + ToolbarHeight + Margin;
		private const float ListTitleY = SpacerBarY + 15f;
		private const float ListHeaderY = ListTitleY + 30f;
		private const float ListY = ListHeaderY + 20f;
		private const float PanelHeight = ListY + ListHeight + Margin;
		private const float HideVanillaY = ToolRow1Y + 30f;
		private const float SameWidthY = HideVanillaY + 20f;
		private const float AdvancedY = SameWidthY + 20f;
		private const float ReplacementSpriteY = ListY + (PreviewHeight * 2f);
		private const float CheckY = ToolRow1Y + ((ReplaceButtonHeight - ToggleSpriteSize) / 2f);

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

		// Network type list.
		private const int NumTypes = 22;
		private readonly string[] netDescriptions = new string[NumTypes]
		{
			Translations.Translate("RON_PNL_ROA"),
			Translations.Translate("RON_PNL_ROB"),
			Translations.Translate("RON_PNL_ROT"),
			Translations.Translate("RON_PNL_RAI"),
			Translations.Translate("RON_PNL_RAB"),
			Translations.Translate("RON_PNL_RAT"),
			Translations.Translate("RON_PNL_MTI"),
			Translations.Translate("RON_PNL_MTB"),
			Translations.Translate("RON_PNL_MTT"),
			Translations.Translate("RON_PNL_MOI"),
			Translations.Translate("RON_PNL_PED"),
			Translations.Translate("RON_PNL_PEB"),
			Translations.Translate("RON_PNL_PET"),
			Translations.Translate("RON_PNL_DEC"),
			Translations.Translate("RON_PNL_ELI"),
			Translations.Translate("RON_PNL_PEA"),
			Translations.Translate("RON_PNL_QUA"),
			Translations.Translate("RON_PNL_CAN"),
			Translations.Translate("RON_PNL_PIP"),
			Translations.Translate("RON_PNL_RUN"),
			Translations.Translate("RON_PNL_TAX"),
			Translations.Translate("RON_PNL_CON")
		};

		// AI type for each network type.
		private readonly Type[] netTypes = new Type[NumTypes]
		{
			typeof(RoadAI),
			typeof(RoadBridgeAI),
			typeof(RoadTunnelAI),
			typeof(TrainTrackAI),
			typeof(TrainTrackBridgeAI),
			typeof(TrainTrackTunnelAI),
			typeof(MetroTrackAI),
			typeof(MetroTrackBridgeAI),
			typeof(MetroTrackTunnelAI),
			typeof(MonorailTrackAI),
			typeof(PedestrianPathAI),
			typeof(PedestrianBridgeAI),
			typeof(PedestrianTunnelAI),
			typeof(DecorationWallAI),
			typeof(PowerLineAI),
			typeof(PedestrianWayAI),
			typeof(QuayAI),
			typeof(CanalAI),
			typeof(WaterPipeAI),
			typeof(AirportAreaRunwayAI),
			typeof(AirportAreaTaxiwayAI),
			typeof(ConcourseAI)
		};

		// AI type for each network type - secondary type for category.
		// Where there's no secondary type, we'll reuse the primary type, to save on conditional checks later.
		private readonly Type[] secondaryNetTypes = new Type[NumTypes]
		{
			typeof(RoadAI),
			typeof(RoadBridgeAI),
			typeof(RoadTunnelAI),
			typeof(TrainTrackAI),
			typeof(TrainTrackBridgeAI),
			typeof(TrainTrackTunnelAI),
			Type.GetType("MetroOverhaul.MOMMetroTrackAI,MetroOverhaul", false),		// MOM, secondary type to Metro.
			Type.GetType("MetroOverhaul.MOMMetroTrackBridgeAI,MetroOverhaul", false),	// MOM, secondary type to Metro.
			Type.GetType("MetroOverhaul.MOMMetroTrackTunnelAI,MetroOverhaul", false),	// MOM, secondary type to Metro.
			typeof(MonorailTrackAI),
			typeof(PedestrianPathAI),
			typeof(PedestrianBridgeAI),
			typeof(PedestrianTunnelAI),
			typeof(DecorationWallAI),
			typeof(PowerLineAI),
			typeof(PedestrianWayAI),
			typeof(QuayAI),
			typeof(CanalAI),
			typeof(WaterPipeAI),
			typeof(AirportAreaRunwayAI),
			typeof(AirportAreaTaxiwayAI),
			typeof(ConcourseAI)
		};

		// InfoManager view modes for each network type.
		private readonly InfoManager.InfoMode[] netInfoModes = new InfoManager.InfoMode[NumTypes]
		{
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.Underground, // RoadTunnelAI
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.Underground, // TrainTrackTunnelAI
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.Underground, // MetroTrackTunnelAI
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.Underground, // PedestrianTunnelAI
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.Water, // WaterPipeAI
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None,
			InfoManager.InfoMode.None
		};

		// InfoManager view submodes for each network type.
		private readonly InfoManager.SubInfoMode[] netSubInfoModes = new InfoManager.SubInfoMode[NumTypes]
		{
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.MaintenanceDepots, // Per game implementation.
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.MaintenanceDepots, // Per game implementation.
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.MaintenanceDepots, // Per game implementation.
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.MaintenanceDepots, // Per game implementation.
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.Oil, // Yes, that's right - follows game implementation ('Water' also shows surface water)
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None,
			InfoManager.SubInfoMode.None
		};

		// Instance references.
		private static GameObject s_uiGameObject;
		private static ReplacerPanel s_panel;
		internal static ReplacerPanel Panel => s_panel;

		// Current selections.
		internal List<ushort> selectedSegments;
		private ushort _currentSegment;
		private NetInfo _selectedReplacement;

		// Segment info record.
		internal readonly Dictionary<NetInfo, List<ushort>> segmentDict = new Dictionary<NetInfo, List<ushort>>();

		// Parent-child network relation dictionaries.
		internal readonly Dictionary<NetInfo, NetInfo> slopeParents = new Dictionary<NetInfo, NetInfo>();
		internal readonly Dictionary<NetInfo, NetInfo> elevatedParents = new Dictionary<NetInfo, NetInfo>();
		internal readonly Dictionary<NetInfo, NetInfo> bridgeParents = new Dictionary<NetInfo, NetInfo>();
		internal readonly Dictionary<NetInfo, NetInfo> tunnelParents = new Dictionary<NetInfo, NetInfo>();

		// Panel components.
		private readonly RONList _targetList;
		private readonly RONList _loadedList;
		private readonly UIButton replaceButton, undoButton, deleteButton, prevButton, nextButton;
		private readonly UIButton targetNameButton, targetCreatorButton, loadedNameButton, loadedCreatorButton;
		private readonly UITextField nameFilter;
		private readonly UIDropDown typeDropDown, searchTypeMenu;
		private readonly UILabel replacingLabel, progressLabel;
		private readonly UICheckBox sameWidthCheck, hideVanilla, advancedCheck, globalCheck, districtCheck, segmentCheck;
		private readonly UISprite targetPreviewSprite, replacementPreviewSprite;

		// Status.
		private bool _replacingDone;
		private bool _replacing;
		private float _timer;
		private int _timerStep;

		// Search settings.
		private int targetSearchStatus, loadedSearchStatus;

		/// <summary>
		/// Called by Unity every tick.  Used here to track state of any in-progress replacments.
		/// </summary>
		public override void Update()
		{
			base.Update();

			// Is a replacement underway?
			if (_replacing)
			{
				// Yes - is it done?
				if (_replacingDone)
				{
					// Done! Clear flags.
					_replacing = false;
					_replacingDone = false;

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
					_timer += Time.deltaTime;

					// Add a period to the progress label every 100ms.  After 30, clear and start again.
					if (_timer > .1f)
					{
						if (++_timerStep > 30)
						{
							progressLabel.text = ".";
							_timerStep = 0;
						}
						else
						{
							progressLabel.text += ".";
						}

						// Either way, reset timer to zero.
						_timer = 0f;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the currently selected NetRowItem.  Called by target network list items.
		/// </summary>
		internal NetRowItem SelectedItem
		{
			private get => _selectedItem;

			set
			{
				// Don't do anything if the target hasn't changed.
				if (_selectedItem != value)
				{
					// Update target reference.
					_selectedItem = value;

					// Reset selected segment.
					_currentSegment = 0;

					// Update selected segments.
					SetSelectedSegments();

					// Update loaded list.
					LoadedList();

					// Update display (preview and button states).
					DisplayNetwork(SelectedPrefab, targetPreviewSprite);
				}
			}
		}
		private NetRowItem _selectedItem;

		/// <summary>
		/// Gets or sets the NetInfo for selected replacement.  Called by target network list items.
		/// </summary>
		internal NetInfo SelectedReplacement
		{
			set
			{
				// Assign new replacement.
				_selectedReplacement = value;

				// Update display (preview and button states).
				DisplayNetwork(_selectedReplacement, replacementPreviewSprite);
			}
		}

		/// <summary>
		/// Sets a value indicating whether replacement work has finished.
		/// </summary>
		internal bool ReplacingDone { set => _replacingDone = value; }

		/// <summary>
		/// Gets or sets the currently selected network prefab.
		/// </summary>
		private NetInfo SelectedPrefab => _selectedItem?.prefab;

		/// <summary>
		/// Creates the panel object in-game and displays it.
		/// </summary>
		internal static void Create()
		{
			try
			{
				// If no GameObject instance already set, create one.
				if (s_uiGameObject == null)
				{
					// Give it a unique name for easy finding with ModTools.
					s_uiGameObject = new GameObject("RONPanel");
					s_uiGameObject.transform.parent = UIView.GetAView().transform;

					// Create new panel instance and add it to GameObject.
					s_panel = s_uiGameObject.AddComponent<ReplacerPanel>();
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
			if (s_panel == null || s_panel._replacing)
			{
				return;
			}

			// Destroy game objects.
			GameObject.Destroy(s_panel);
			GameObject.Destroy(s_uiGameObject);

			// Let the garbage collector do its work (and also let us know that we've closed the object).
			s_panel = null;
			s_uiGameObject = null;
		}

		/// <summary>
		/// Sets the target segment and updates the display and selection accordingly.
		/// </summary>
		/// <param name="segmentID">Target segment ID.</param>
		internal void SetTarget(ushort segmentID)
        {
			// Get selected network info and AI.
			NetInfo selectedNet = Singleton<NetManager>.instance.m_segments.m_buffer[segmentID].Info;
			Type aiType = selectedNet?.GetAI()?.GetType();

			// Check for null AIs.
			if (aiType == null)
            {
				return;
            }

			// Elevated station tracks from Extra Train Station Tracks need special handling, as they don't use the TrainTrackBridgeAI.
			if (aiType == typeof(TrainTrackAI) && selectedNet.name.StartsWith("Station Track Eleva"))
			{
				aiType = typeof(TrainTrackBridgeAI);
			}

			// Try to match 
			for (int i = 0; i < NumTypes; ++i)
            {
				if (aiType.Equals(netTypes[i]) || aiType.Equals(secondaryNetTypes[i]))
                {
					// Match found - set dropdown menu.
					typeDropDown.selectedIndex = i;

					// Set target list position.
					_targetList.FindItem(selectedNet);

					// Set selected network segement.
					_currentSegment = segmentID;

					// Set selected segments.
					SetSelectedSegments();

					// All done here.
					return;
				}
			}
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
			width = PanelWidth;
			height = PanelHeight;

			// Default position - centre in screen.
			relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

			// Drag bar.
			UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
			dragHandle.width = PanelWidth - 50f;
			dragHandle.height = PanelHeight;
			dragHandle.relativePosition = Vector2.zero;
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
			closeButton.eventClick += (component, clickEvent) => RONTool.ToggleTool();

			// Decorative icon (top-left).
			UISprite iconSprite = AddUIComponent<UISprite>();
			iconSprite.relativePosition = new Vector2(5, 5);
			iconSprite.height = 32f;
			iconSprite.width = 32f;
			iconSprite.atlas = UITextures.LoadQuadSpriteAtlas("RonButton");
			iconSprite.spriteName = "normal";

			// Network type dropdown.
			typeDropDown = UIDropDowns.AddLabelledDropDown(this, Margin, ToolRow1Y, Translations.Translate("RON_PNL_TYP"), 250f);
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
			_targetList = UIList.AddUIList<RONList, UINetRow>(leftPanel);
			ListSetup(_targetList);
			_targetList.EventSelectionChanged += (control, selectedItem) => SelectedItem = selectedItem as NetRowItem;

			// Loaded network list.
			UIPanel rightPanel = AddUIComponent<UIPanel>();
			rightPanel.width = RightWidth;
			rightPanel.height = ListHeight;
			rightPanel.relativePosition = new Vector2(RightPanelX, ListY);
			_loadedList = UIList.AddUIList<RONList, UINetRow>(rightPanel);
			ListSetup(_loadedList);
			_loadedList.EventSelectionChanged += (control, selectedItem) => SelectedReplacement = (selectedItem as NetRowItem)?.prefab;

			// List titles.
			UILabels.AddLabel(this, Margin, ListTitleY, Translations.Translate("RON_PNL_MAP"), LeftWidth);
			UILabels.AddLabel(this, RightPanelX, ListTitleY, Translations.Translate("RON_PNL_AVA"), RightWidth);

			// List headers.
			UILabels.AddLabel(this, Margin + UINetRow.NameX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_NET"), UINetRow.CreatorX - UINetRow.NameX);
			UILabels.AddLabel(this, Margin + UINetRow.CreatorX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_CRE"), LeftWidth - UINetRow.CreatorX);
			UILabels.AddLabel(this, RightPanelX + UINetRow.NameX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_NET"), UINetRow.CreatorX - UINetRow.NameX);
			UILabels.AddLabel(this, RightPanelX + UINetRow.CreatorX + OrderArrowWidth, ListHeaderY, Translations.Translate("RON_PNL_CRE"), RightWidth - UINetRow.CreatorX);

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
			replaceButton = UIButtons.AddButton(this, MiddlePanelX, ToolRow1Y, Translations.Translate("RON_PNL_REP"), ReplaceWidth, ReplaceButtonHeight, scale: 1.0f);
			replaceButton.eventClicked += Replace;

			// Undo button.
			undoButton = UIButtons.AddButton(this, MiddlePanelX, ToolRow2Y, Translations.Translate("RON_PNL_UND"), ReplaceWidth, ReplaceButtonHeight);
			undoButton.eventClicked += Undo;

			// Delete button.
			deleteButton = UIButtons.AddButton(this, MiddlePanelX, ToolRow3Y, Translations.Translate("RON_PNL_DEL"), ReplaceWidth, ReplaceButtonHeight);
			deleteButton.eventClicked += Delete;

			// View previous segment button.
			prevButton = UIButtons.AddSmallerButton(this, PrevX, ToolRow2Y, Translations.Translate("RON_PNL_VPS"), ButtonWidth);
			prevButton.eventClicked += PreviousSegment;

			// View next segment button.
			nextButton = UIButtons.AddSmallerButton(this, NextX, ToolRow2Y, Translations.Translate("RON_PNL_VNS"), ButtonWidth);
			nextButton.eventClicked += NextSegment;

			// Name filter.
			nameFilter = UITextFields.AddLabelledTextField(this, FilterX, ToolRow1Y, Translations.Translate("RON_FIL_NAME"), FilterWidth, 25f, vertPad: 5);
			nameFilter.eventTextChanged += (control, text) => LoadedList();
			nameFilter.eventTextSubmitted += (control, text) => LoadedList();

			// Search by name/author dropdown.
			searchTypeMenu = UIDropDowns.AddDropDown(this, FilterMenuxX, ToolRow1Y, FilterMenuWidth);
			searchTypeMenu.items = new string[(int)SearchTypes.NumTypes] { Translations.Translate("RON_PNL_NET"), Translations.Translate("RON_PNL_CRE") };
			searchTypeMenu.selectedIndex = (int)SearchTypes.SearchNetwork;
			searchTypeMenu.eventSelectedIndexChanged += (control, isChecked) => LoadedList();

			// Vanilla filter.
			hideVanilla = UICheckBoxes.AddLabelledCheckBox((UIComponent)(object)this, FilterX, HideVanillaY, Translations.Translate("RON_PNL_HDV"));
			hideVanilla.isChecked = true;
			hideVanilla.eventCheckChanged += (control, isChecked) => LoadedList();

			// Same width only check.
			sameWidthCheck = UICheckBoxes.AddLabelledCheckBox(this, FilterX, SameWidthY, Translations.Translate("RON_PNL_WID"));
			sameWidthCheck.isChecked = true;
			sameWidthCheck.eventCheckChanged += (control, isChecked) => LoadedList();

			// Advanced mode check.
			if (ModSettings.EnableAdvanced)
			{
				advancedCheck = UICheckBoxes.AddLabelledCheckBox(this, FilterX, AdvancedY, Translations.Translate("RON_PNL_ADV"));
				advancedCheck.eventCheckChanged += (control, isChecked) => LoadedList();
			}

			// Replacing label (starts hidden).
			replacingLabel = UILabels.AddLabel(this, MiddlePanelX, ToolRow1Y, Translations.Translate("RON_PNL_RIP"), ReplaceWidth);
			replacingLabel.Hide();

			// Progress label (starts hidden).
			progressLabel = UILabels.AddLabel(this, MiddlePanelX, ToolRow2Y, ".", ReplaceWidth);
			progressLabel.Hide();

			// Preview sprites.
			targetPreviewSprite = AddPreviewSprite(MiddlePanelX + PreviewArrowWidth, ListY);
			replacementPreviewSprite = AddPreviewSprite(MiddlePanelX, ReplacementSpriteY);

			// Arrow sprites.
			AddArrowSprite(targetPreviewSprite, -PreviewArrowWidth, "ArrowLeft");
			AddArrowSprite(replacementPreviewSprite, PreviewWidth, "ArrowRight");

			// Global/district/segment checkboxes
			segmentCheck = IconToggleCheck(this, LeftWidth + Margin - ToggleSpriteSize, CheckY, "ron_segment_small", "RON_PNL_SEG");
			districtCheck = IconToggleCheck(this, LeftWidth + Margin - (ToggleSpriteSize * 2f), CheckY, "ron_district_small", "RON_PNL_DIS");
			globalCheck = IconToggleCheck(this, LeftWidth + Margin - (ToggleSpriteSize * 3f), CheckY, "ron_global_small", "RON_PNL_GLB");
			globalCheck.isChecked = true;
			globalCheck.eventCheckChanged += CheckChanged;
			districtCheck.eventCheckChanged += CheckChanged;
			segmentCheck.eventCheckChanged += CheckChanged;

			// Populate lists.
			PrefabUtils.GetCreators();
			TargetList();
			LoadedList();

			// Populate parent dictionaries.
			PrefabUtils.GetParents(slopeParents, elevatedParents, bridgeParents, tunnelParents);
		}

		/// <summary>
		/// Performs the delete action.
		/// </summary>
		internal void DeleteNets()
		{
			// Only do stuff if we've got a valid selection.
			if (SelectedPrefab != null)
			{
				// Set panel to replacing state.
				SetReplacing();

				// Add ReplaceNets method to simulation manager action (don't want to muck around with simulation stuff from the main thread....)
				bool isGlobal = globalCheck.isChecked;

				// Copy segment IDs from segment list to avoid concurrency issues while replacing.
				ushort[] segments = new ushort[selectedSegments.Count];
				selectedSegments.CopyTo(segments);
				Singleton<SimulationManager>.instance.AddAction(() => Replacer.DeleteNets(segments));
			}
		}

		/// <summary>
		/// Sets the list of currently selected segments.
		/// </summary>
		private void SetSelectedSegments()
		{
			// Create list of replacement segments.
			if (segmentCheck.isChecked)
			{
				// Single replacement only.
				selectedSegments = new List<ushort> { _currentSegment };
			}
			else if (districtCheck.isChecked && _currentSegment != 0 && SelectedPrefab != null)
			{
				// District replacement (requires active selection).
				selectedSegments = new List<ushort>();

				// Local references.
				NetManager netManager = Singleton<NetManager>.instance;
				NetSegment[] segmentBuffer = netManager.m_segments.m_buffer;
				NetNode[] nodeBuffer = netManager.m_nodes.m_buffer;
				DistrictManager districtManager = Singleton<DistrictManager>.instance;

				// Get district of selected segment.
				ushort districtID = districtManager.GetDistrict(segmentBuffer[_currentSegment].m_middlePosition);

				// Iterate through each segment of this type in our dictionary, adding to our list of selected segments if both start and end nodes are in the same district as the initial segement.
				foreach (ushort districtSegment in segmentDict[SelectedPrefab])
				{
					if (districtManager.GetDistrict(nodeBuffer[segmentBuffer[districtSegment].m_startNode].m_position) == districtID && districtManager.GetDistrict(nodeBuffer[segmentBuffer[districtSegment].m_endNode].m_position) == districtID)
					{
						selectedSegments.Add(districtSegment);
					}
				}
			}
			else if (SelectedPrefab != null && segmentDict != null)
			{
				// Global replacements - just use list from segment dictionary.
				selectedSegments = segmentDict[SelectedPrefab];
			}
		}

		/// <summary>
		/// Network type dropdown change handler.
		/// </summary>
		/// <param name="c">Calling component.</param>
		/// <param name="index">New selected index.</param>
		private void CheckChanged(UIComponent c, bool isChecked)
        {
			// If this checkbox is checked, unselect others.
			if (isChecked)
            {
				if (c == globalCheck)
                {
					districtCheck.isChecked = false;
					segmentCheck.isChecked = false;
				}
				else if (c == districtCheck)
				{
					globalCheck.isChecked = false;
					segmentCheck.isChecked = false;
				}
				else if (c == segmentCheck)
				{
					globalCheck.isChecked = false;
					districtCheck.isChecked = false;
				}
			}
			else if (!globalCheck.isChecked && !districtCheck.isChecked && !segmentCheck.isChecked)
            {
				// No checkbox is selected - re-select this one and don't do anything else.
				(c as UICheckBox).isChecked = true;
				return;
            }

			// Ensure correct checkbox label.
			if (segmentCheck.isChecked)
            {
				replaceButton.text = Translations.Translate("RON_PNL_RES");
				deleteButton.text = Translations.Translate("RON_PNL_DES");
			}
			else
            {
				replaceButton.text = Translations.Translate("RON_PNL_REP");
				deleteButton.text = Translations.Translate("RON_PNL_DEL");
			}

			// Update selected segements.
			SetSelectedSegments();
        }

		/// <summary>
		/// Network type dropdown change handler.
		/// </summary>
		/// <param name="control">Calling component (unused).</param>
		/// <param name="index">New selected index (unused).</param>
		private void TypeChanged(UIComponent control, int index)
		{
			// Rebuild target and replacement lists.
			TargetList();
			LoadedList();

			// Set info view mode.
			Singleton<InfoManager>.instance.SetCurrentMode(netInfoModes[index], netSubInfoModes[index]);
		}

		/// <summary>
		/// Replace button event handler.
		/// <param name="c">Calling component (unused).</param>
		/// <param name="p">Mouse event (unused).</param>
		/// </summary>
		private void Replace(UIComponent c, UIMouseEventParameter p)
		{
			// Only do stuff if we've got valid selections.
			if (SelectedPrefab != null & _selectedReplacement != null)
			{
				// Set panel to replacing state.
				SetReplacing();

				// Add ReplaceNets method to simulation manager action (don't want to muck around with simulation stuff from the main thread....)
				bool isGlobal = globalCheck.isChecked;
				Singleton<SimulationManager>.instance.AddAction(delegate { Replacer.ReplaceNets(SelectedPrefab, _selectedReplacement, selectedSegments, isGlobal); } );
			}
		}

		/// <summary>
		/// Delete button event handler.
		/// <param name="c">Calling component (unused).</param>
		/// <param name="p">Mouse event (unused).</param>
		/// </summary>
		internal void Delete(UIComponent c, UIMouseEventParameter p)
		{
			// Only do stuff if we've got a valid selection.
			if (SelectedPrefab != null && selectedSegments.Count > 0)
			{
				// Display delete confirmation box.
				YesNoNotification warningNotification = NotificationBase.ShowNotification<YesNoNotification>();
				warningNotification.YesButton.eventClicked += (button, clickEvent) => DeleteNets();

				// Singlular or plural?
				warningNotification.AddParas(Translations.Translate(selectedSegments.Count > 1 ? "RON_WAR_DEL" : "RON_WAR_DES"),
					Translations.Translate("RON_WAR_UND")) ;
			}
		}

		/// <summary>
		/// Undo button event handler.
		/// <param name="c">Calling component (unused).</param>
		/// <param name="p">Mouse event (unused).</param>
		/// </summary>
		private void Undo(UIComponent c, UIMouseEventParameter p)
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
		/// <param name="c">Calling component (unused).</param>
		/// <param name="p">Mouse event (unused).</param>
		private void NextSegment(UIComponent c, UIMouseEventParameter p)
		{
			ushort targetSegment = 0;

			// Find first segment matching current selection.
			// Need to do this for each segment instance, so iterate through all segments.
			NetSegment[] segments = NetManager.instance.m_segments.m_buffer;
			for (ushort segmentID = 0; segmentID < segments.Length; ++segmentID)
			{
				// Check for match with selected target.
				if (segments[segmentID].Info == SelectedPrefab)
				{
					// Got a match - set target if it isn't already set (first instance found).
					if (targetSegment == 0)
					{
						targetSegment = segmentID;
					}

					// Is the selected segment ahead of this?
					if (segmentID > _currentSegment)
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
		/// <param name="c">Calling component (unused).</param>
		/// <param name="p">Mouse event (unused).</param>
		private void PreviousSegment(UIComponent c, UIMouseEventParameter p)
		{
			ushort targetSegment = 0;

			// Find first segment matching current selection.
			// Need to do this for each segment instance, so iterate through all segments.
			NetSegment[] segments = NetManager.instance.m_segments.m_buffer;
			for (ushort segmentID = (ushort)(segments.Length - 1); segmentID > 0; --segmentID)
			{
				// Check for match with selected target.
				if (segments[segmentID].Info == SelectedPrefab)
				{
					// Got a match - set target if it isn't already set (first instance found).
					if (targetSegment == 0)
					{
						targetSegment = segmentID;
					}

					// Is the previously-shown segment counter ahead of this?
					if (segmentID < _currentSegment)
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
		/// <param name="c">Calling component</param>
		/// <param name="p">Mouse event (unused)</param>
		/// </summary>
		private void SortLoaded(UIComponent c, UIMouseEventParameter p)
		{
			// Check if we are using the name or creator button.
			if (c == loadedNameButton)
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
			else if (c == loadedCreatorButton)
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
		/// <param name="c">Calling component.</param>
		/// <param name="p">Mouse event (unused).</param>
		/// </summary>
		private void SortTargets(UIComponent c, UIMouseEventParameter p)
		{
			// Check if we are using the name or creator button.
			if (c == targetNameButton)
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
			else if (c == targetCreatorButton)
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

			// Regenerate target list.
			TargetList();
		}

		/// <summary>
		/// Moves the camera to view the given segment.
		/// </summary>
		/// <param name="segmentID">Segment ID of target segment.</param>
		private void ViewSegement(ushort segmentID)
        {
			// Yes - set camera position.
			Vector3 cameraPosition = NetManager.instance.m_segments.m_buffer[segmentID].m_middlePosition;
			cameraPosition.y = Camera.main.transform.position.y;
			ToolsModifierControl.cameraController.SetTarget(new InstanceID { NetSegment = segmentID }, cameraPosition, true);

			// Update last viewed segment to this one.
			_currentSegment = segmentID;

			// Updated selected segments.
			SetSelectedSegments();
		}

		/// <summary>
		/// Updates button states (enabled/disabled) according to current control states.
		/// </summary>
		private void UpdateButtonStates()
		{
			// Enable go to segment buttons if we have a valid target, disable it otherwise.
			if (SelectedPrefab != null)
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
			replaceButton.isEnabled = SelectedPrefab != null && _selectedReplacement != null;

			// Enable delete button if a valid target is selected.
			deleteButton.isEnabled = SelectedPrefab != null;

			// Enable undo button if we have a valid undo buffer.
			undoButton.isEnabled = Replacer.HasUndo;
		}

		/// <summary>
		/// Initializes the 'replacing' state - sets flags, timers, UI state.
		/// </summary>
		private void SetReplacing()
		{
			// Set flags and reset timer.
			_replacing = true;
			_replacingDone = false;
			_timer = 0;

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
		/// <returns>Populated fastlist of networks on map.</returns>
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
				// Skip any inleigible flags.
				if ((segments[i].m_flags & NetSegment.Flags.Created) == 0)
                {
					continue;
                }

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
							// Filters passed - get type icon and add to list.
							NetRowItem newItem = new NetRowItem(segmentInfo);
							newItem.typeIcon = GetTypeIcon(segmentInfo);
							netList.Add(segmentInfo, newItem);
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
			_targetList.Data = new FastList<object>
			{
				m_buffer = objectArray,
				m_size = objectArray.Length
			};

			// Clear current selection.
			_targetList.SelectedIndex = -1;
			SelectedItem = null;

			// Ensure that network type selection dropdown is on top.
			typeDropDown.BringToFront();
		}

		/// <summary>
		/// Populates a fastlist with a list of loaded networks.
		/// </summary>
		/// <returns>Populated fastlist of loaded networks, after filtering</returns>
		private void LoadedList()
		{
			// List of prefabs to display.
			List<NetRowItem> netList = new List<NetRowItem>();

			// Don't do anything if there's no current selection and we haven't selected 'show all networks'.
			if (SelectedPrefab != null || (advancedCheck != null && advancedCheck.isChecked))
			{
				// Iterate through all loaded networks.
				for (uint i = 0u; i < PrefabCollection<NetInfo>.LoadedCount(); ++i)
				{
					// Get network and add to our list, if it isn't null.
					NetInfo network = PrefabCollection<NetInfo>.GetLoaded(i);
					if (network?.name != null)
					{
						// Create new NetRowItem from this network.
						NetRowItem newItem = new NetRowItem(network);

						// Apply 'hide vanilla and Next' filter.
						if (hideVanilla.isChecked && (newItem.isVanilla || newItem.isNExt2))
						{
							// It's vanilla/NExt.  Skip.
							continue;
						}

						// Apply text filter.
						string trimmedText = nameFilter.text.Trim();
						if (
							StringExtensions.IsNullOrWhiteSpace(trimmedText) ||
							(searchTypeMenu.selectedIndex == (int)SearchTypes.SearchNetwork && newItem.displayName.ToLower().Contains(trimmedText.ToLower())) ||
							(searchTypeMenu.selectedIndex == (int)SearchTypes.SearchCreator && newItem.creator.ToLower().Contains(trimmedText.ToLower()))
							)
						{
							// Apply network type filter or advanced mode, as applicable.
							bool advancedMode = advancedCheck != null && advancedCheck.isChecked;
							if (advancedMode || MatchType(network))
							{
								// Apply width filter.
								if (sameWidthCheck.isChecked && SelectedPrefab != null)
								{
									// Check if this network has the same half-width.
									if (network.m_halfWidth != SelectedPrefab.m_halfWidth)
									{
										// No match; skip this one.
										continue;
									}
								}

								// Apply station filter.
								if (!advancedMode && newItem.isStation != SelectedItem.isStation)
								{
									continue;
								}

								// Passed filtering; get type icon and add this one to the list.
								newItem.typeIcon = GetTypeIcon(network);
								netList.Add(newItem);
							}
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
			_loadedList.Data = new FastList<object>
			{
				m_buffer = objectArray,
				m_size = netList.Count
			};

			// Clear current selection.
			_loadedList.SelectedIndex = -1;
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
			NetAI ai = network?.m_netAI;
			if (ai != null)
			{
				Type aiType = ai.GetType();

				// Elevated station tracks from Extra Train Station Tracks need special handling, as they don't use the TrainTrackBridgeAI.
				if (aiType == typeof(TrainTrackAI) && network.name.StartsWith("Station Track Eleva"))
                {
					aiType = typeof(TrainTrackBridgeAI);
                }

				// Check for match.
				if (aiType.IsAssignableFrom(netTypes[typeDropDown.selectedIndex]) || aiType.IsAssignableFrom(secondaryNetTypes[typeDropDown.selectedIndex]))
				{
					// Match - return true.
					return true;
				}
			}

			// If we got here, we didn't get a match.
			return false;
		}

		/// <summary>
		/// Performs initial UIList setup.
		/// </summary>
		/// <param name="uiList">UIList to set up.</param>
		private void ListSetup(UIList uiList)
		{
			// Appearance, size and position.
			uiList.BackgroundSprite = "UnlockingPanel";
			uiList.width = uiList.parent.width;
			uiList.height = uiList.parent.height;
			uiList.relativePosition = Vector2.zero;

			// Data.
			uiList.Data = new FastList<object>();
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
				previewSprite.atlas = UITextures.LoadQuadSpriteAtlas("RonButton");
				previewSprite.spriteName = "normal";
				previewSprite.Hide();
			}

			UpdateButtonStates();
		}

		/// <summary>
		/// Retuns the type icon filename for the given network.
		/// </summary>
		/// <param name="network">Network to get icon for.</param>
		/// <returns>Type icon filename, or null if no type icon is available.</returns>
		private string GetTypeIcon(NetInfo network)
		{
			// Find matching icon type.
			if (elevatedParents.ContainsKey(network))
			{
				return "ron_elevated";
			}
			else if (bridgeParents.ContainsKey(network))
			{
				return "ron_bridge";
			}
			else if (tunnelParents.ContainsKey(network) || slopeParents.ContainsKey(network))
			{
				return "ron_tunnel";
			}

			// If we got here, then no icon was specified; return null.
			return null;
		}

		/// <summary>
		/// Sets the states of the two given sort buttons to match the given search status.
		/// </summary>
		/// <param name="activeButton">Currently active sort button<./param>
		/// <param name="inactiveButton">Inactive button (other sort button for same list).</param>
		/// <param name="searchStatus">Search status to apply.</param>
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

			// Reset inactive button.
			SetFgSprites(inactiveButton, "IconUpArrow2");
		}

		/// <summary>
		/// Finds a thumbnail image for the selected prefab.
		/// </summary>
		/// <param name="network">Network prefab to find thumbnail for.</param>
		/// <param name="atlas">Thumbnail atlas.</param>
		/// <param name="thumbnail">Thumbnail sprite name.</param>
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
					thumbAtlas = UITextures.GetTextureAtlas(entry.Key); 
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
		/// <param name="xPos">Relative X position.</param>
		/// <param name="yPos">Relative Y position.</param>
		/// <returns>New UI sprite.</returns>
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
		/// <param name="parent">Parent component.</param>
		/// <param name="xPos">Relative X position.</param>
		/// <param name="spriteName">Sprite name.</param>
		/// <returns>Nw UI sprite.</returns>
		private UISprite AddArrowSprite(UIComponent parent, float xPos, string spriteName)
		{
			UISprite arrowSprite = parent.AddUIComponent<UISprite>();
			arrowSprite.relativePosition = new Vector2(xPos, 0f);
			arrowSprite.height = PreviewHeight;
			arrowSprite.width = PreviewArrowWidth;
			arrowSprite.atlas = UITextures.InGameAtlas;
			arrowSprite.spriteName = spriteName;

			return arrowSprite;
		}

		/// <summary>
		/// Adds an arrow button.
		/// </summary>
		/// <param name="parent">Parent component.</param>
		/// <param name="posX">Relative X postion.</param>
		/// <param name="posY">Relative Y position.</param>
		/// <param name="width">Button width (default 32).</param>
		/// <param name="height">Button height (default 20).</param>
		/// <returns>New arrow button.</returns>
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
		/// <param name="button">Targeted button.</param>
		/// <param name="spriteName">Sprite name.</param>
		private void SetFgSprites(UIButton button, string spriteName)
		{
			button.normalFgSprite = button.hoveredFgSprite = button.pressedFgSprite = button.focusedFgSprite = spriteName;
		}

		/// <summary>
		/// Adds an icon toggle checkbox.
		/// </summary>
		/// <param name="parent">Parent component.</param>
		/// <param name="xPos">Relative X position.</param>
		/// <param name="yPos">Relative Y position.</param>
		/// <param name="fileName">Sprite atlas file name (without .png).</param>
		/// <param name="tooltipKey">Tooltip translation key.</param>
		/// <returns>New checkbox</returns>
		private UICheckBox IconToggleCheck(UIComponent parent, float xPos, float yPos, string fileName, string tooltipKey)
		{
			// Size and position.
			UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
			checkBox.width = ToggleSpriteSize;
			checkBox.height = ToggleSpriteSize;
			checkBox.clipChildren = true;
			checkBox.relativePosition = new Vector2(xPos, yPos);

			try
			{
				// Checkbox sprites.
				UISprite sprite = checkBox.AddUIComponent<UISprite>();
				sprite.atlas = UITextures.LoadQuadSpriteAtlas(fileName);
				sprite.spriteName = "normal";
				sprite.size = new Vector2(ToggleSpriteSize, ToggleSpriteSize);
				sprite.relativePosition = Vector3.zero;

				checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
				((UISprite)checkBox.checkedBoxObject).atlas = sprite.atlas;
				((UISprite)checkBox.checkedBoxObject).spriteName = "pressed";
				checkBox.checkedBoxObject.size = new Vector2(ToggleSpriteSize, ToggleSpriteSize);
				checkBox.checkedBoxObject.relativePosition = Vector3.zero;

				checkBox.tooltip = Translations.Translate(tooltipKey);
			}
			catch (Exception e)
            {
				Logging.LogException(e, "exception creating icon toggle check");
            }

			return checkBox;
		}

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
	}
}
