// <copyright file="UINetRowTypes.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

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
			ReplacerPanel.Panel.SelectedReplacement = m_thisItem.prefab;
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
			ReplacerPanel.Panel.SelectedItem = m_thisItem;
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
		/// Generates and displays a row.
		/// </summary>
		/// <param name="data">Object data to display.</param>
		/// <param name="rowIndex">Row index number (for background banding).</param>
		public override void Display(object data, int rowIndex)
        {
			// Get index number.
			if (data is int index)
            {
				thisIndex = index;

				// Display using underlying netinfo of index.
				base.Display(new NetRowItem(StationPanel.Panel.GetNetInfo(index)), rowIndex);
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
			StationPanel.Panel.SelectedReplacement = m_thisItem.prefab;
		}
	}
}
