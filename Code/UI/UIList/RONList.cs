// <copyright file="RONList.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using AlgernonCommons.UI;

    /// <summary>
    /// RON UI list.
    /// </summary>
    public class RONList : UIList
    {
        /// <summary>
        /// Sets the selection to the item matching the given prefab.
        /// If no item is found, clears the selection and resets the list.
        /// </summary>
        /// <param name="prefab">The prefab to find</param>
        public void FindItem(PrefabInfo prefab)
        {
            // Iterate through the rows list.
            for (int i = 0; i < Data.m_size; ++i)
            {
                if (Data.m_buffer[i] is NetRowItem netItem)
                {
                    // Look for an index match; individual or grouped (contained within propListItem.indexes list).
                    if (netItem.Prefab != null && netItem.Prefab == prefab)
                    {
                        // Found a match; set the selected index to this one.
                        SelectedIndex = i;

                        // If the selected index is outside the current visibility range, move the to show it.
                        if (SelectedIndex < CurrentPosition || SelectedIndex > CurrentPosition + RowCount)
                        {
                            CurrentPosition = SelectedIndex;
                        }

                        // Set the selected target item.
                        ReplacerPanel.Panel.SelectedItem = netItem;

                        // Done here; return.
                        return;
                    }
                }
            }

            // If we got here, we didn't find a match; clear the selection.
            SelectedIndex = -1;
        }
    }
}