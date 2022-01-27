using System;
using UnityEngine;
using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// An individual prop row.
    /// </summary>
    public class UINetRow : UIPanel, IUIFastListRow
    {
        // Layout constants - public.
        public const float RowHeight = 23f;
        public const float NameX = CategoryX + CategoryWidth + IndicatorMargin;
        public const float CreatorX = NameX + NameWidth + Margin;

        // Layout constants - private.
        private const float Margin = 5f;
        private const float IndicatorMargin = 2f;
        private const float StationX = IndicatorMargin;
        private const float StationWidth = 17f;
        private const float CategoryX = StationX + StationWidth + IndicatorMargin;
        private const float CategoryWidth = 23f;
        private const float NameWidth = 270f;
        private const float LabelHeight = 14f;
        private const float TextScale = 0.8f;
        private const float MinTextScale = 0.65f;

        // Panel components.
        private UIPanel panelBackground;
        protected UILabel networkName, creatorName, stationLabel, categoryLabel;

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
                // Set background with.
                Background.width = width;

                // Set label positions.
                networkName.relativePosition = new Vector2(NameX, (RowHeight - networkName.height) / 2f);
                creatorName.relativePosition = new Vector2(CreatorX, (RowHeight - creatorName.height) / 2f);
                stationLabel.relativePosition = new Vector2(StationX, (RowHeight - LabelHeight) / 2f);
                categoryLabel.relativePosition = new Vector2(CategoryX, (RowHeight - LabelHeight) / 2f);
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
                networkName.textScale = TextScale;
                networkName.autoSize = true;

                // Add creator name label.
                creatorName = AddUIComponent<UILabel>();
                creatorName.textScale = TextScale;
                creatorName.autoSize = true;

                // Add station label.
                stationLabel = AddUIComponent<UILabel>();
                stationLabel.autoSize = false;
                stationLabel.autoHeight = false;
                stationLabel.textScale = TextScale;
                stationLabel.width = StationWidth;
                stationLabel.height = LabelHeight;

                // Add category label.
                categoryLabel = AddUIComponent<UILabel>();
                categoryLabel.autoSize = false;
                categoryLabel.autoHeight = false;
                categoryLabel.textScale = TextScale;
                categoryLabel.width = CategoryWidth;
                categoryLabel.height = LabelHeight;
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

                // Set category label text and tooltip.
                if (thisItem.isVanilla)
                {
                    categoryLabel.text = "[v]";
                    categoryLabel.tooltip = Translations.Translate("RON_TIP_VAN");
                }
                else if (thisItem.isNExt2)
                {
                    categoryLabel.text = "[n]";
                    categoryLabel.tooltip = Translations.Translate("RON_TIP_NEX");
                }
                else if (thisItem.isMod)
                {
                    categoryLabel.text = "[m]";
                    categoryLabel.tooltip = Translations.Translate("RON_TIP_MOD");
                }
                else
                {
                    // Default - no label or tooltip.
                    categoryLabel.text = " ";
                    categoryLabel.tooltip = null;
                }

                // Set station label.
                if (thisItem.isStation)
                {
                    stationLabel.text = "[s]";
                    stationLabel.tooltip = Translations.Translate("RON_TIP_STA");
                }
                else
                {
                    // Default - no label or tooltip.
                    stationLabel.text = " ";
                    stationLabel.tooltip = null;
                }

                // Set label positions and heights (accounting for any change in text scaling).
                OnSizeChanged();
            }
            else
            {
                // Null reference; clear text.
                networkName.text = string.Empty;
                creatorName.text = string.Empty;

                // Clear labels and tooltips.
                categoryLabel.text = " ";
                categoryLabel.tooltip = null;
                stationLabel.text = " ";
                stationLabel.tooltip = null;
            }

            // Set initial background as deselected state.
            Deselect(isRowOdd);

            // Disable panel interactivity (avoids conflict with overlapping dropdown menus).
            Background.isInteractive = false;
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
                label.textScale -= 0.01f;
                label.PerformLayout();
            }

            // Finally, clamp label size.
            label.autoSize = false;
            if (label.width > maxWidth)
            {
                label.width = maxWidth;
            }
        }
    }
}

