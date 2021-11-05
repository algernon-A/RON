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
			ReplacerPanel.Panel.SelectedItem = thisItem;
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

		// If this is a vanilla/NExt2/mod asset.
		public bool isVanilla = false, isNExt2 = false, isMod = false;


		/// <summary>
		/// Constructor - automatically sets values based on provided network prefab.
		/// </summary>
		/// <param name="network">Network prefab</param>
		public NetRowItem(NetInfo network)
        {
			prefab = network;
			GetDisplayName();
			CheckIsMod();
			creator = PrefabUtils.GetCreator(network);
			isStation = PrefabUtils.IsStation(network);
		}


		/// <summary>
		/// Sets displayName to a cleaned-up display name for the given prefab.
		/// </summary>
		/// <param name="isVanilla">Set to true if this is a vanilla network, false otherwise</param>
		/// <param name="isNExt2">Set to true if this is a NExt network, false otherwise</param>
		private void GetDisplayName()
		{
			string fullName = prefab.name;

			// Find any leading period (Steam package number).
			int period = fullName.IndexOf('.');

			// If no period, assume it's either vanilla or NExt
			if (period < 0)
			{
				// Check for NEext prefabs.  NExt prefabs aren't as consistent as would be ideal....
				isNExt2 = (
					prefab.m_class.name.StartsWith("NExt") ||
					prefab.m_class.name.StartsWith("NEXT") ||
					prefab.name.StartsWith("Small Busway") ||
					prefab.name.EndsWith("With Bus Lanes") ||
					prefab.name.Equals("PlainStreet2L") ||
					prefab.name.StartsWith("Highway2L2W") ||
					prefab.name.StartsWith("AsymHighwayL1R2")
				);
				isVanilla = !isNExt2;
				displayName = fullName;
			}

			// Otherwise, omit the package number, and trim off any trailing _Data.
			displayName = fullName.Substring(period + 1).Replace("_Data", "");
		}


		/// <summary>
		/// Sets the isMod field according to whether the network is from a mod, e.g. Extra Train Station Tracks.
		/// </summary>
		/// <param name="prefab">Network prefab to check</param>
		/// <returns></returns>
		private void CheckIsMod()
		{
			// Check for Extra Train Station Tracks prefabs.
			isMod = prefab.name.StartsWith("Station") ||
					prefab.name.StartsWith("Train Station Track (");
		}
	}
}
