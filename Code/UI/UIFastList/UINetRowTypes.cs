namespace RON
{
	/// <summary>
	/// Prop row fastlist item for loaded (replacement) networks.
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
	/// Prop row fastlist item for target networks.
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
	/// Prop row fastlist item for target networks for the station panel.
	/// </summary>
	public class UIStationTargetNetRow : UINetRow
	{
		// Station path index of this row item.
		private int thisIndex;


		/// <summary>
		/// Called when this item is selected.
		/// </summary>
		public override void UpdateSelection()
		{
			// Update currently selected replacement prefab.
			StationPanel.Panel.SelectedIndex = thisIndex;
		}


		/// <summary>
		/// Generates and displays a network row.
		/// </summary>
		/// <param name="data">Object to list</param>
		/// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
		public override void Display(object data, bool isRowOdd)
        {
			// Get index number.
			if (data is int index)
            {
				Logging.Message("found row index ", index.ToString());
				thisIndex = index;

				// Display using underlying netinfo of index.
				base.Display(new NetRowItem(StationPanel.Panel.GetNetInfo(index)), isRowOdd);
			}
        }
    }


	/// <summary>
	/// Prop row fastlist item for loaded (replacement) networks for the station panel.
	/// </summary>
	public class UIStationReplacementNetRow : UINetRow
	{
		/// <summary>
		/// Called when this item is selected.
		/// </summary>
		public override void UpdateSelection()
		{
			// Update currently selected replacement prefab.
			StationPanel.Panel.SelectedReplacement = thisItem.prefab;
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

		// Network indicator flags - if this is a vanilla/NExt2/mod asset.
		public bool isVanilla = false, isNExt2 = false, isMod = false;


		/// <summary>
		/// Constructor - automatically sets values based on provided network prefab.
		/// </summary>
		/// <param name="network">Network prefab</param>
		public NetRowItem(NetInfo network)
		{
			prefab = network;
			GetDisplayName();
			creator = PrefabUtils.GetCreator(network);
			isStation = PrefabUtils.IsStation(network);
		}


		/// <summary>
		/// Sets displayName to a cleaned-up display name for the given prefab and also sets network indicator flags.
		/// </summary>
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

				// Check for Extra Train Station Tracks and OneWayTrainTrck prefabs; this overrides the NExt2 check due to some OneWayTrainTrack prefabs haveing 'NExtSingleStaitonTrack' ItemClass (and hence being picked up above as NExt2 items).
				isMod = prefab.name.StartsWith("Station") ||
				prefab.name.StartsWith("Train Station Track (") ||
				prefab.name.StartsWith("Rail1L");
				isNExt2 = isNExt2 && !isMod;

				// Set vanilla flag and display name.
				isVanilla = !(isNExt2 || isMod);
				displayName = fullName;
			}

			// Otherwise, omit the package number, and trim off any trailing _Data.
			displayName = fullName.Substring(period + 1).Replace("_Data", "");
		}
	}
}
