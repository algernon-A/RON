using UnityEngine;
using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// An individual prop row.
    /// </summary>
    public class UINetRow : UIPanel, IUIFastListRow
    {
        // Layout constants.
        private const float RowHeight = 30f;

        // Layout variables.
        private float labelX;

        // Panel components.
        private UIPanel panelBackground;
        private UILabel objectName;

        // ObjectData.
        protected NetInfo thisPrefab;
        protected int index;


        // Background for each list item.
        public UIPanel Background
        {
            get
            {
                if (panelBackground == null)
                {
                    panelBackground = AddUIComponent<UIPanel>();
                    panelBackground.width = width;
                    panelBackground.height = RowHeight;
                    panelBackground.relativePosition = Vector2.zero;

                    panelBackground.zOrder = 0;
                }

                return panelBackground;
            }
        }


        /// <summary>
        /// Called when dimensions are changed, including as part of initial setup (required to set correct relative position of label).
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (objectName != null)
            {
                Background.width = width;
                objectName.relativePosition = new Vector2(labelX, 5f);
            }
        }


        /// <summary>
        /// Mouse click event handler - updates the selected building to what was clicked.
        /// </summary>
        /// <param name="p">Mouse event parameter</param>
        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);
            UpdateSelection();
        }


        /// <summary>
        /// Updates current replacement selection when this item is selected.
        /// </summary>
        public virtual void UpdateSelection()
        {
        }


        /// <summary>
        /// Generates and displays a building row.
        /// </summary>
        /// <param name="data">Object to list</param>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Display(object data, bool isRowOdd)
        {
            // Perform initial setup for new rows.
            if (objectName == null)
            {
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = parent.width;
                height = RowHeight;

                // Add object name label.
                objectName = AddUIComponent<UILabel>();
                objectName.width = this.width - 10f;
            }


            // See if our attached data is a raw PropInfo (e.g an available prop item as opposed to a PropListItem replacment record).
            thisPrefab = data as NetInfo;
            if (thisPrefab != null)
            {
                // Display its (cleaned-up) name.
                objectName.text = GetDisplayName(thisPrefab.name);
                labelX = 10f;
            }

            // Set label position
            objectName.relativePosition = new Vector2(labelX, 5f);

            // Set initial background as deselected state.
            Deselect(isRowOdd);
        }


        /// <summary>
        /// Highlights the selected row.
        /// </summary>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Select(bool isRowOdd)
        {
            Background.backgroundSprite = "ListItemHighlight";
            Background.color = new Color32(255, 255, 255, 255);
        }


        /// <summary>
        /// Unhighlights the (un)selected row.
        /// </summary>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Deselect(bool isRowOdd)
        {
            if (isRowOdd)
            {
                // Lighter background for odd rows.
                Background.backgroundSprite = "UnlockingItemBackground";
                Background.color = new Color32(0, 0, 0, 128);
            }
            else
            {
                // Darker background for even rows.
                Background.backgroundSprite = null;
            }
        }


        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// Called by the settings panel fastlist.
        /// </summary>
        /// <param name="fullName">Original (raw) prefab name</param>
        /// <returns>Cleaned display name</returns>
        internal static string GetDisplayName(string fullName)
        {
            // Find any leading period (Steam package number).
            int num = fullName.IndexOf('.');

            // If no period, assume vanilla asset; return full name preceeded by vanilla flag.
            if (num < 0)
            {
                return "[v] " + fullName;
            }

            // Otherwise, omit the package number, and trim off any trailing _Data.
            return fullName.Substring(num + 1).Replace("_Data", "");
        }
    }
}

