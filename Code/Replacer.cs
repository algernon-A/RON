using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Math;


namespace RON
{
    /// <summary>
    /// Static class for network replacement tools.
    /// </summary>
    internal static class Replacer
    {
		// Undo buffer.
		private static List<ushort> undoBuffer;
		private static NetInfo undoPrefab;


		/// <summary>
		/// Checks if we have a valid undo buffer (at least one undo record).
		/// </summary>
		/// <returns>True if there's a valid undo action to perform, false otherwise</returns>
		internal static bool HasUndo => undoPrefab != null && undoBuffer != null && undoBuffer.Count > 0;


		/// <summary>
		/// Perform actual network replacement.
		/// </summary>
		/// <param name="target">Target netInfo</param>
		/// <param name="replacement">Replacement netInfo</param>
		/// <param name="segmentList">Array of segment IDs</param>
		/// <param name="selectedSegment">Selected segment for individual or district replacement (set to 0 for global replacement)</param>
		/// <param name="districtOnly">Set to true if replacement should be only within the same district, false otherwise</param>
		internal static void ReplaceNets(NetInfo target, NetInfo replacement, List<ushort> segmentList, ushort selectedSegment, bool districtOnly)
		{
			try
			{
				// Input checks.
				if (target?.name == null || replacement?.name == null)
                {
					Logging.Error("null parameter passed to ReplaceNets");
					return;
                }

				// Ensure segment list is valid..
				if (segmentList == null || segmentList.Count == 0)
                {
					Logging.Error("null reference for segment dictionary when replacing nets");
					return;
                }

				// Local references.
				NetManager netManager = Singleton<NetManager>.instance;
				Randomizer randomizer = new Randomizer();
				NetSegment[] segmentBuffer = netManager.m_segments.m_buffer;
				DistrictManager districtManager = Singleton<DistrictManager>.instance;

				// Ensure we have a selected segment when we're doing district replacement.
				ushort districtID = 0;
				if (districtOnly)
                {
					if (selectedSegment == 0)
                    {
						Logging.Error("no selected segment passed to ReplaceNets for district replacement");
						return;
                    }

					// Get district of selected segment.
					districtID = districtManager.GetDistrict(segmentBuffer[selectedSegment].m_middlePosition);

					Logging.Message("set to district replacement with district ", districtManager.GetDistrictName(districtID));
                }

				// Determine target segment(s).
				ushort[] segmentIDs;

				// If single segment replacement only (selectedSegment isn't zero and district replacement isn't set), just create an array with only the single segment in the list.
				if (!districtOnly && selectedSegment != 0)
				{
					segmentIDs = new ushort[] { selectedSegment };
				}
				else
				{
					// Not a single-segment replacement: copy segment IDs from segment list to avoid concurrency issues while replacing.
					segmentIDs = new ushort[segmentList.Count];
					segmentList.CopyTo(segmentIDs, 0);
				}

				// Initialize undo buffer.
				undoBuffer = new List<ushort>();
				undoPrefab = target;

				// Iterate through each segment ID in our prepared list. 
				for (int i = 0; i < segmentIDs.Length; ++i)
				{
					// Local references.
					ushort segmentID = segmentIDs[i];
					NetSegment segment = segmentBuffer[segmentID];

					// Null check, just in case.
					NetInfo segmentInfo = segment.Info;
					if (segmentInfo != null)
					{
						// Check that this is an active network before we do actual replacement.
						if (segment.m_flags != NetSegment.Flags.None)
						{
							// Check for district match, if applicable.
							if (districtOnly)
                            {
								if (districtManager.GetDistrict(netManager.m_nodes.m_buffer[segment.m_startNode].m_position) != districtID || districtManager.GetDistrict(netManager.m_nodes.m_buffer[segment.m_endNode].m_position) != districtID)
                                {
									// No district match - skip this segment.
									continue;
                                }
                            }

							// Get segment name and prority.
							bool priority = netManager.IsPriorityRoad(segmentID, out bool _);
							ushort nameSeed = segment.m_nameSeed;
							bool customNameFlag = (segment.m_flags & NetSegment.Flags.CustomName) != 0;
							string segmentName = netManager.GetSegmentName(segmentID);

							NetSegment.Flags flags = segment.m_flags & NetSegment.Flags.Original;

							// Get segment 'original' status.
							NetSegment.Flags originalFlag = segment.m_flags & NetSegment.Flags.Original;

							// Active network segment - replace segment.
							ushort newSegmentID = ReplaceNet(segmentID, segmentBuffer, replacement, ref randomizer);

							// Set nameseed and priority of new segment to match original.
							segmentBuffer[newSegmentID].m_nameSeed = nameSeed;
							netManager.SetPriorityRoad(newSegmentID, priority);

							// Set 'original' status of new segment.
							segmentBuffer[newSegmentID].m_flags |= originalFlag;

							// Restore any custom name.
							if (customNameFlag && segmentName != null)
							{
								netManager.SetSegmentNameImpl(newSegmentID, segmentName);
							}

							// Add new segment ID to undo buffer.
							undoBuffer.Add(newSegmentID);
						}
						else
						{
							// Inactive network segment - just replace info directly..
							segmentBuffer[segmentID].Info = replacement;
						}
					}
				}
			}
			catch (Exception e)
			{
				// Don't care too much - just want to make sure that we set the status flag correctly and not hang in the 'processing' state indefinitely.
				Logging.LogException(e, "network replacement exception");
			}

			// All done - let replacer panel know we're finished (if its still open).
			if (ReplacerPanel.Panel != null)
			{
				ReplacerPanel.Panel.replacingDone = true;
			}
		}


		/// <summary>
		/// Reverts all segments in the undo list to the set undo prefab.
		/// </summary>
		internal static void Undo()
        {
			try
			{
				// Local references.
				NetSegment[] segments = Singleton<NetManager>.instance.m_segments.m_buffer;
				Randomizer randomizer = new Randomizer();

				// Replace each segment in undo buffer.
				foreach(ushort segmentID in undoBuffer)
                {
					ReplaceNet(segmentID, segments, undoPrefab, ref randomizer);
                }
			}
			catch (Exception e)
			{
				// Don't care too much - just want to make sure that we set the status flag correctly and not hang in the 'processing' state indefinitely.
				Logging.LogException(e, "network replacement exception");
			}

			// All done - clear undo buffer.
			undoPrefab = null;
			undoBuffer.Clear();
			
			// Let replacer panel know we're finished (if its still open).
			if (ReplacerPanel.Panel != null)
			{
				ReplacerPanel.Panel.replacingDone = true;
			}
		}


		/// <summary>
		/// Replace a given segment with the specified prefab.
		/// </summary>
		/// <param name="segmentID">Segment to replace</param>
		/// <param name="segments">Segment buffer reference</param>
		/// <param name="replacement">Replacement prefab to apply</param>
		/// <param name="randomizer">Randomizer</param>
		/// <returns>New segment ID</returns>
		private static ushort ReplaceNet(ushort segmentID, NetSegment[] segments, NetInfo replacement, ref Randomizer randomizer)
        {
			// Local references.
			NetManager netManager = Singleton<NetManager>.instance;
			NetSegment segment = segments[segmentID];

			// Deactivate old segment.
			segment.Info.m_netAI.ManualDeactivation(segmentID, ref segments[segmentID]);

			// Create new segment, duplication location, direction, etc. as current segment.
			netManager.CreateSegment(out ushort newSegmentID, ref randomizer, replacement, segment.m_startNode, segment.m_endNode, segment.m_startDirection, segment.m_endDirection, segment.m_buildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, (segment.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None);

			// Remove old segment.
			netManager.ReleaseSegment(segmentID, false);

			// Ensure old segment info reference updated to this.
			segments[segmentID].Info = replacement;

			return newSegmentID;
		}
	}
}
