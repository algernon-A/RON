using ColossalFramework.UI;


namespace RON
{
    public class RONFastList : UIFastList
    {
        /// <summary>
        /// Use this to create the UIFastList.
        /// Do NOT use AddUIComponent.
        /// I had to do that way because MonoBehaviors classes cannot be generic
        /// </summary>
        /// <typeparam name="T">The type of the row UI component</typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static new RONFastList Create<T>(UIComponent parent)
            where T : UIPanel, IUIFastListRow
        {
            RONFastList list = parent.AddUIComponent<RONFastList>();
            list.m_rowType = typeof(T);
            return list;
        }


        /// <summary>
        /// Sets the selection to the item matching the given prefab.
        /// If no item is found, clears the selection and resets the list.
        /// </summary>
        /// <param name="item">The item to find</param>
        public void FindItem(PrefabInfo prefab)
        {
            // Iterate through the rows list.
            for (int i = 0; i < m_rowsData.m_size; ++i)
            {
                if (m_rowsData.m_buffer[i] is NetRowItem netItem)
                {
                    // Look for an index match; individual or grouped (contained within propListItem.indexes list).
                    if (netItem.prefab != null && netItem.prefab == prefab)
                    {
                        // Found a match; set the selected index to this one.
                        selectedIndex = i;

                        // If the selected index is outside the current visibility range, move the to show it.
                        if (selectedIndex < listPosition || selectedIndex > listPosition + m_rows.m_size)
                        {
                            listPosition = selectedIndex;
                        }

                        // Set the selected target item.
                        ReplacerPanel.Panel.SelectedItem = netItem;

                        // Done here; return.
                        return;
                    }
                }
            }

            // If we got here, we didn't find a match; clear the selection and reset the list position.
            selectedIndex = -1;
            listPosition = 0f;
        }
    }
}