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
		// Undo buffer
		private static Dictionary<ushort, NetInfo> undoBuffer;


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
				undoBuffer = new Dictionary<ushort, NetInfo>();

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
								// No outside connection - okay to proceed.  Start by adding segment to undo buffer.
								undoBuffer.Add(segmentID, segmentInfo);

								// Deactivate old segment.
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
	}
}
