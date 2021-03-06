using System.Text;


namespace RON
{
	/// <summary>
	/// Prop row fastlist item for loaded props/trees.
	/// </summary>
	public class UIReplacementNetRow : UINetRow
	{
		/// <summary>
		/// Called when this item is selected.
		/// </summary>
		public override void UpdateSelection()
		{
			// Update currently selected replacement prefab.
			ReplacerPanel.Panel.SelectedReplacement = thisItem.prefab;
		}
	}


	/// <summary>
	/// Prop row fastlist item for building props/trees.
	/// </summary>
	public class UITargetNetRow : UINetRow
	{
		/// <summary>
		/// Called when this item is selected.
		/// </summary>
		public override void UpdateSelection()
		{
			// Update currently selected target prefab.
			ReplacerPanel.Panel.SelectedTarget = thisItem.prefab;
		}


		/// <summary>
		/// Generates and displays a network row.
		/// </summary>
		/// <param name="data">Object to list</param>
		/// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
		/*public override void Display(object data, bool isRowOdd)
        {
			// Same setup as usual first.
			base.Display(data, isRowOdd);


			// Tooltip with list of segments - first check that semgment dictionary exists and has an entry for this prefab.
			if (ReplacerPanel.Panel?.segmentDict != null && ReplacerPanel.Panel.segmentDict.ContainsKey(thisPrefab))
			{
				// Stringbuilder to build tooltip string.
				StringBuilder segmentList = new StringBuilder();
				segmentList.AppendLine("Segments:");

				// All good - iterate through each segment in dictionary entry and append it to the tooltip.
				foreach (ushort segment in ReplacerPanel.Panel.segmentDict[thisPrefab])
				{
					segmentList.AppendLine(segment.ToString());
				}

				// Set tooltip.
				objectName.tooltip = segmentList.ToString();
			}
		}*/
	}


	/// <summary>
	/// Data structure class for individual net row display lines.
	/// </summary>
	public class NetRowItem
    {
		// Network prefab.
		public NetInfo prefab;

		// Display name.
		public string displayName;

		// Creator name.
		public string creator;
    }
}
