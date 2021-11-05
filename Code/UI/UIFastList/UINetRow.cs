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
        public const float Margin = 5f;
        public const float RowHeight = 23f;
        public const float PaddingY = 5f;
        public const float IndicatorWidth = 35f;
        public const float NameX = Margin + IndicatorWidth;
        public const float NameWidth = 270f;
        public const float CreatorX = NameX + NameWidth + Margin;
        public const float TextScale = 0.8f;
        public const float MinTextScale = 0.6f;

        // Panel components.
        private UIPanel panelBackground;
        protected UILabel networkName, creatorName, indicatorLabel;

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

            if (networkName != null)
            {
                // Set position and size.
                Background.width = width;
                networkName.relativePosition = new Vector2(NameX, PaddingY);
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
            if (networkName == null)
            {
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = parent.width;
                height = RowHeight;

                // Add object name label.
                networkName = AddUIComponent<UILabel>();
                networkName.autoSize = true;
                networkName.autoHeight = true;
                networkName.relativePosition = new Vector2(NameX, PaddingY);

                // Add creator name label.
                creatorName = AddUIComponent<UILabel>();
                creatorName.autoSize = true;
                creatorName.autoHeight = true;
                creatorName.relativePosition = new Vector2(CreatorX, PaddingY);

                // Add indicator label
                indicatorLabel = AddUIComponent<UILabel>();
                indicatorLabel.textScale = TextScale;
                indicatorLabel.autoSize = false;
                indicatorLabel.autoHeight = true;
                indicatorLabel.width = IndicatorWidth;
            }

            // Set display text.
            thisItem = data as NetRowItem;
            if (thisItem?.prefab != null)
            {
                // Network name label.
                networkName.text = thisItem.displayName;
                networkName.textScale = TextScale;
                ResizeLabel(networkName, NameWidth, MinTextScale);
                
                // Creator name label.
                creatorName.text = thisItem.creator;
                creatorName.textScale = TextScale;
                ResizeLabel(creatorName, width - CreatorX, MinTextScale);

                // Set indicator label text.
                if (thisItem.isVanilla)
                {
                    indicatorLabel.text = "[v]";
                }
                else if (thisItem.isNExt2)
                {
                    indicatorLabel.text = "[n]";
                }
                else if (thisItem.isMod)
                {
                    indicatorLabel.text = "[m]";
                }
                else
                {
                    indicatorLabel.text = string.Empty;
                }

                // Set indicator label position.
                indicatorLabel.relativePosition = new Vector2(Margin, PaddingY);

                // Set indicator for stations.
                if (thisItem.isStation)
                {
                    // Move to left to accomodate greater width if a flag already exists.
                    if (!indicatorLabel.text.Equals(string.Empty))
                    {
                        indicatorLabel.relativePosition = new Vector2(Margin / 2f, PaddingY);
                    }

                    indicatorLabel.text += "[s]";
                }
            }
            else
            {
                // Null reference; clear text.
                networkName.text = string.Empty;
                creatorName.text = string.Empty;
                indicatorLabel.text = string.Empty;
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


        /// <summary>
        /// Dynamically resizes a text label by shrinking the text scale until it fits within the desired maximum width.
        /// </summary>
        /// <param name="label">Label to resize</param>
        /// <param name="maxWidth">Maximum acceptable label width</param>
        /// <param name="minScale">Minimum acceptible label scale (to nearest increment of 0.05f</param>
        private void ResizeLabel(UILabel label, float maxWidth, float minScale)
        {
            // Don't do anything with negative widths or scales.
            if (maxWidth < 10f || minScale < 0.5f)
            {
                return;
            }

            // Make sure label is autosizeable and up-to-date.
            label.autoSize = true;
            label.PerformLayout();

            // Iterate through text scales until minimum is reached.
            while (label.width > maxWidth && label.textScale > minScale)
            {
                label.textScale -= 0.05f;
                label.PerformLayout();
            }
        }
    }
}

