﻿using UnityEngine;
using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// An individual prop row.
    /// </summary>
    public class UINetRow : UIPanel, IUIFastListRow
    {
        // Layout constants.
        public const float RowHeight = 23f;
        public const float PaddingY = 5f;
        public const float NameX = 5f;
        public const float NameWidth = 300f;
        public const float CreatorX = NameX + NameWidth + 10f;
        public const float TextScale = 0.8f;

        // Panel components.
        private UIPanel panelBackground;
        protected UILabel objectName, creatorName;

        // ObjectData.
        protected NetRowItem thisItem;
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
                objectName.relativePosition = new Vector2(NameX, PaddingY);
                creatorName.relativePosition = new Vector2(CreatorX, PaddingY);
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
        /// Generates and displays a network row.
        /// </summary>
        /// <param name="data">Object to list</param>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public virtual void Display(object data, bool isRowOdd)
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
                objectName.textScale = TextScale;
                objectName.autoSize = false;
                objectName.autoHeight = true;
                objectName.width = NameWidth;
                objectName.relativePosition = new Vector2(NameX, PaddingY);
            }

            if (creatorName == null)
            {
                // Add object name label.
                creatorName = AddUIComponent<UILabel>();
                creatorName.textScale = TextScale;
                creatorName.autoSize = false;
                creatorName.autoHeight = true;
                creatorName.width = this.width - CreatorX;
                creatorName.relativePosition = new Vector2(CreatorX, PaddingY);
            }

            // Set display text.
            thisItem = data as NetRowItem;
            if (thisItem?.prefab != null)
            {
                objectName.text = thisItem.displayName;
                creatorName.text = thisItem.creator;
            }
            else
            {
                // Null reference; clear text.
                objectName.text = null;
                creatorName.text = null;
            }

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
    }
}

