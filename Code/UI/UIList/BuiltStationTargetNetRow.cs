// <copyright file="BuiltStationTargetNetRow.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using ColossalFramework;

    /// <summary>
    /// Prop row fastlist item for target networks for the station panel.
    /// </summary>
    public class BuiltStationTargetNetRow : NetRow
    {
        /// <summary>
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public override void Display(object data, int rowIndex)
        {
            // Get index number.
            if (data is int segment)
            {
                    base.Display(new NetRowItem(Singleton<NetManager>.instance.m_segments.m_buffer[segment].Info), rowIndex);
            }
        }
    }
}
