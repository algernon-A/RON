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
