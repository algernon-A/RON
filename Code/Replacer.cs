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
		internal static void ReplaceNets(NetInfo target, NetInfo replacement)
		{
			try
			{
				// Input checks.
				if (target?.name == null || replacement?.name == null)
                {
					Logging.Error("null parameter passed to ReplaceNets");
                }

				// Ensure segment dictionary exists.
				Dictionary<NetInfo, List<ushort>> segmentDict = ReplacerPanel.Panel?.segmentDict;
				if (segmentDict == null)
                {
					Logging.Error("null reference for segment dictionary when replacing nets");
                }

				// Ensure segment dictionary has this key.
				if (!segmentDict.ContainsKey(target))
				{
					Logging.Error("no segment dictionary entry for target network ", target.name);
				}

				// Copy segment IDs from segment dictionary to avoid concurrency issues while repacing.
				ushort[] segmentIDs = new ushort[segmentDict[target].Count];
				segmentDict[target].CopyTo(segmentIDs);

				// Local references.
				NetManager netManager = Singleton<NetManager>.instance;
				Randomizer randomizer = new Randomizer();
				NetSegment[] segments = netManager.m_segments.m_buffer;

				// Initialize undo buffer.
				undoBuffer = new List<ushort>();
				undoPrefab = target;

				// Iterate through each segment ID in our prepared list. 
				for (int i = 0; i < segmentIDs.Length; ++i)
				{
					// Local references.
					ushort segmentID = segmentIDs[i];
					NetSegment segment = segments[segmentID];

					// Null check, just in case.
					NetInfo segmentInfo = segment.Info;
					if (segmentInfo != null)
					{
						// Check that this is an active network before we do actual replacement.
						if (segment.m_flags != NetSegment.Flags.None)
						{
							// Get segment name and prority.
							bool priority = netManager.IsPriorityRoad(segmentID, out bool _);
							ushort nameSeed = segment.m_nameSeed;
							bool customNameFlag = (segment.m_flags & NetSegment.Flags.CustomName) != 0;
							string segmentName = netManager.GetSegmentName(segmentID);

							NetSegment.Flags flags = segment.m_flags & NetSegment.Flags.Original;

							// Get segment 'original' status.
							NetSegment.Flags originalFlag = segment.m_flags & NetSegment.Flags.Original;

							// Active network segment - replace segment.
							ushort newSegmentID = ReplaceNet(segmentID, segments, replacement, ref randomizer);

							// Set nameseed and priority of new segment to match original.
							segments[newSegmentID].m_nameSeed = nameSeed;
							netManager.SetPriorityRoad(newSegmentID, priority);

							// Set 'original' status of new segment.
							segments[newSegmentID].m_flags |= originalFlag;

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
							segments[segmentID].Info = replacement;
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
