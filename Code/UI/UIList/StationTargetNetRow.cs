// <copyright file="StationTargetNetRow.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using AlgernonCommons.UI;

    /// <summary>
    /// Prop row fastlist item for target networks for the station panel.
    /// </summary>
    public class StationTargetNetRow : NetRow
    {
        /// <summary>
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public override void Display(object data, int rowIndex)
        {
            // Get index number.
            if (data is PathIndex index)
            {
                // Display using underlying netinfo of index.
                if (StandalonePanelManager<StationPanel>.Panel?.IndexedNet(index) is NetInfo netInfo)
                {
                    base.Display(new NetRowItem(netInfo), rowIndex);
                }
            }
        }
    }
}
