﻿using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using UnifiedUI.Helpers;


namespace RON
{
	/// <summary>
	/// The RON selection tool.
	/// </summary>
	public class RONTool : DefaultTool
	{
		// Cursor textures.
		private CursorInfo lightCursor;
		private CursorInfo darkCursor;


		/// <summary>
		/// Selection mode.
		/// </summary>
		public enum Mode
		{
			Select,
			NodeOrSegment,
			Building,
			PropOrTree
		}


		/// <summary>
		/// Instance reference.
		/// </summary>
		public static RONTool Instance => ToolsModifierControl.toolController?.gameObject?.GetComponent<RONTool>();


		/// <summary>
		/// Returns true if the zoning tool is currently active, false otherwise.
		/// </summary>
		public static bool IsActiveTool => Instance != null && ToolsModifierControl.toolController.CurrentTool == Instance;


		/// <summary>
		/// Initialise the tool.
		/// Called by unity when the tool is created.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			// Load cursors.
			lightCursor = TextureUtils.LoadCursor("ron_cursor_light.png");
			darkCursor = TextureUtils.LoadCursor("ron_cursor_dark.png");
			m_cursor = darkCursor;

			// Create new UUI button.
			UIComponent uuiButton = UUIHelpers.RegisterToolButton(
				name: nameof(RONTool),
				groupName: null, // default group
				tooltip: Translations.Translate("RON_NAM"),
				tool: this,
				icon: UUIHelpers.LoadTexture(UUIHelpers.GetFullPath<RONMod>("Resources", "ron_uui.png")),
				hotkeys: new UUIHotKeys { ActivationKey = ModSettings.UUIKey }
				);
		}


		// Ignore nodes, citizens, disasters, districts, transport lines, and vehicles.
		public override NetNode.Flags GetNodeIgnoreFlags() => NetNode.Flags.All;
		public override CitizenInstance.Flags GetCitizenIgnoreFlags() => CitizenInstance.Flags.All;
		public override DisasterData.Flags GetDisasterIgnoreFlags() => DisasterData.Flags.All;
		public override District.Flags GetDistrictIgnoreFlags() => District.Flags.All;
		public override TransportLine.Flags GetTransportIgnoreFlags() => TransportLine.Flags.None;
		public override VehicleParked.Flags GetParkedVehicleIgnoreFlags() => VehicleParked.Flags.All;
		public override TreeInstance.Flags GetTreeIgnoreFlags() => TreeInstance.Flags.All;
		public override PropInstance.Flags GetPropIgnoreFlags() => PropInstance.Flags.All;
		public override Vehicle.Flags GetVehicleIgnoreFlags() => Vehicle.Flags.LeftHandDrive | Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding;


		// Select all buildings.
		public override Building.Flags GetBuildingIgnoreFlags() => Building.Flags.All;

		/// <summary>
		/// Called by the game.  Sets which network segments are ignored by the tool (always returns none, i.e. all segments are selectable by the tool).
		/// </summary>
		/// <param name="nameOnly">Always set to false</param>
		/// <returns>NetSegment.Flags.None</returns>
		public override NetSegment.Flags GetSegmentIgnoreFlags(out bool nameOnly)
		{
			nameOnly = false;
			return NetSegment.Flags.None;
		}


		/// <summary>
		/// Called by the game every simulation step.
		/// Performs raycasting to select hovered instance.
		/// </summary>
		public override void SimulationStep()
		{
			// Get base mouse ray.
			Ray mouseRay = m_mouseRay;

			// Get raycast input.
			RaycastInput input = new RaycastInput(mouseRay, m_mouseRayLength)
			{
				m_rayRight = m_rayRight,
				m_netService = GetService(),
				m_buildingService = GetService(),
				m_propService = GetService(),
				m_treeService = GetService(),
				m_districtNameOnly = Singleton<InfoManager>.instance.CurrentMode != InfoManager.InfoMode.Districts,
				m_ignoreTerrain = GetTerrainIgnore(),
				m_ignoreNodeFlags = GetNodeIgnoreFlags(),
				m_ignoreSegmentFlags = GetSegmentIgnoreFlags(out input.m_segmentNameOnly),
				m_ignoreBuildingFlags = GetBuildingIgnoreFlags(),
				m_ignoreTreeFlags = GetTreeIgnoreFlags(),
				m_ignorePropFlags = GetPropIgnoreFlags(),
				m_ignoreVehicleFlags = GetVehicleIgnoreFlags(),
				m_ignoreParkedVehicleFlags = GetParkedVehicleIgnoreFlags(),
				m_ignoreCitizenFlags = GetCitizenIgnoreFlags(),
				m_ignoreTransportFlags = GetTransportIgnoreFlags(),
				m_ignoreDistrictFlags = GetDistrictIgnoreFlags(),
				m_ignoreParkFlags = GetParkIgnoreFlags(),
				m_ignoreDisasterFlags = GetDisasterIgnoreFlags(),
				m_transportTypes = GetTransportTypes()
			};

			// Enable ferry line selection.
			input.m_netService.m_itemLayers |= ItemClass.Layer.FerryPaths;

			ToolErrors errors = ToolErrors.None;
			RaycastOutput output;

			// Cursor is dark by default.
			m_cursor = darkCursor;

			// Is the base mouse ray valid?
			if (m_mouseRayValid)
			{
				// Yes - raycast.
				if (RayCast(input, out output))
				{
					// Set base tool accurate position.
					m_accuratePosition = output.m_hitPos;

					// Check for network hits.
					if (output.m_netSegment != 0)
					{
						// Networks.
						if (CheckSegment(output.m_netSegment, ref errors))
						{
							// CheckSegment passed - record hit position and set cursor to light cursor.
							output.m_hitPos = Singleton<NetManager>.instance.m_segments.m_buffer[output.m_netSegment].GetClosestPosition(output.m_hitPos);
							m_cursor = lightCursor;
						}
						else
						{
							// CheckSegment failed - deselect segment.
							output.m_netSegment = 0;
						}
					}

					// Create new hover instance and set hovered type (if applicable).
					InstanceID hoverInstance = InstanceID.Empty;
					if (output.m_netSegment != 0)
					{
						hoverInstance.NetSegment = output.m_netSegment;
					}

					// Update tool hover instance.
					m_hoverInstance = hoverInstance;
				}
				else
				{
					// Raycast failed.
					errors = ToolErrors.RaycastFailed;
				}
			}
			else
			{
				// No valid mouse ray.
				output = default;
				errors = ToolErrors.RaycastFailed;
			}

			// Set mouse position and record errors.
			m_mousePosition = output.m_hitPos;
			m_selectErrors = errors;
		}


		/// <summary>
		/// Called by game when overlay is to be rendered.
		/// </summary>
		/// <param name="cameraInfo">Current camera instance</param>
		public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
		{
			// Colors for rendering.
			Color segmentColor = GetToolColor(false, false);

			// Local references.
			ToolManager toolManager = Singleton<ToolManager>.instance;
			NetSegment[] segmentBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;

			base.RenderOverlay(cameraInfo);

			if (ReplacerPanel.Panel?.selectedSegments != null)
			{
				foreach (ushort segmentID in ReplacerPanel.Panel.selectedSegments)
				{
					toolManager.m_drawCallData.m_overlayCalls++;
					NetTool.RenderOverlay(cameraInfo, ref segmentBuffer[segmentID], segmentColor, segmentColor);
				}
			}
		}


		/// <summary>
		/// Toggles the current tool to/from the RON tool.
		/// </summary>
		internal static void ToggleTool()
		{
			// Activate RON tool if it isn't already; if already active, deactivate it by selecting the default tool instead.
			if (!IsActiveTool)
			{
				// Activate RON tool.
				ToolsModifierControl.toolController.CurrentTool = Instance;
			}
			else
			{
				// Activate default tool.
				ToolsModifierControl.SetTool<DefaultTool>();
			}
		}


		/// <summary>
		/// Called by game when tool is enabled.
		/// Used to open the replacer panel.
		/// </summary>
		protected override void OnEnable()
		{
			// Make sure that game is loaded before activating tool.
			if (!OnLevelLoadedPatch.loaded)
			{
				// Loading not complete - deactivate tool by seting default tool.
				ToolsModifierControl.SetTool<DefaultTool>();
			}
			else
            {
				Logging.KeyMessage("premature activation attempt");
            }

			base.OnEnable();
			ReplacerPanel.Create();
		}


		/// <summary>
		/// Called by game when tool is disabled.
		/// Used to close the replacer panel.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			ReplacerPanel.Close();
		}


		/// <summary>
		/// Tool GUI event processing.
		/// Called by game every GUI update.
		/// </summary>
		/// <param name="e">Event</param>
		protected override void OnToolGUI(Event e)
		{
			// Don't do anything if mouse is inside UI or if there are any errors other than failed raycast.
			if (m_toolController.IsInsideUI || (m_selectErrors != ToolErrors.None && m_selectErrors != ToolErrors.RaycastFailed))
			{
				return;
			}

			// Try to get a hovered network instance.
			ushort segment = m_hoverInstance.NetSegment;
			if (segment != 0)
			{
				// Check for mousedown events with button zero.
				if (e.type == EventType.MouseDown && e.button == 0)
				{
					// Got one; use the event.
					UIInput.MouseUsed();

					// Send the result to the panel.
					ReplacerPanel.Panel?.SetTarget(segment);
				}
			}
		}
    }
}
