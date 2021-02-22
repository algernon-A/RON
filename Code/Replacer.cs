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
				// Local references.
				NetManager netManager = Singleton<NetManager>.instance;
				string targetName = target.name;
				Randomizer randomizer = new Randomizer();

				// Initialize undo buffer.
				undoBuffer = new List<ushort>();
				undoPrefab = target;

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
								// Replace segment, adding new segment ID to undo buffer.
								undoBuffer.Add(ReplaceNet(segmentID, segments, replacement, ref randomizer));
							}
							else
							{
								Logging.Message("skipping outside connection segment ", segmentID.ToString(), " - ", segmentInfo.name);
							}
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
				Logging.Message("undoing");

				// Local references.
				NetSegment[] segments = Singleton<NetManager>.instance.m_segments.m_buffer;
				Randomizer randomizer = new Randomizer();

				// Replace each segment in undo buffer.
				foreach(ushort segmentID in undoBuffer)
                {
					Logging.Message("undoing segment ", segmentID.ToString());
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
			Logging.Message("replacing segment ", segmentID.ToString(), " with ", replacement.name);

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
