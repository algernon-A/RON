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

		// If this is a station network.
		public bool isStation;


		/// <summary>
		/// Constructor - automatically sets values based on provided network prefab.
		/// </summary>
		/// <param name="network">Network prefab</param>
		public NetRowItem(NetInfo network) : this(network, PrefabUtils.GetDisplayName(network, out _), PrefabUtils.GetCreator(network), PrefabUtils.IsStation(network))
        {
        }


		/// <summary>
		/// Constructor - automatically sets values based on provided network prefab and a pre-provided data.
		/// </summary>
		/// <param name="network">Network prefab</param>
		/// <param name="displayName">Provided display name</param>
		/// <param name="creatorName">Provided creator name</param>
		/// <param name="isStation">Provided station track status</param>
		public NetRowItem(NetInfo network, string displayName, string creatorName, bool isStation)
		{
			prefab = network;
			creator = creatorName;
			this.isStation = isStation;

			// Set display name according to station status.
			if (isStation)
			{
				// Is a station track; insert "[S]" at appropriate point.
				if (displayName[0] == '[')
				{
					// Display name starts with "[" : assume it's a RON identifier, and insert the station track identifier afterwards.
					this.displayName = displayName.Insert(4, "[S] ");
				}
				else
				{
					// No RON identifier; insert station track identifier at start of string.
					this.displayName = displayName.Insert(0, "[S] ");
				}
			}
			else
			{
				// Not a station; use unchanged display name without station track identifier.
				this.displayName = displayName;
			}
		}
	}
}
