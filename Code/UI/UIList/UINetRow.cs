// <copyright file="UINetRow.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// An individual prop row.
    /// </summary>
    public class UINetRow : UIListRow
    {
        /// <summary>
        /// Network name label relative X position.
        /// </summary>
        public const float NameX = TypeX + SpriteSize + IndicatorMargin;

        /// <summary>
        /// Creator name label relative X position.
        /// </summary>
        public const float CreatorX = NameX + NameWidth + Margin;

        /// <summary>
        /// Default row height.
        /// </summary>
        public new const float DefaultRowHeight = 23f;

        // Layout constants - private.
        private const float IndicatorMargin = 2f;
        private const float StationX = IndicatorMargin;
        private const float StationWidth = 17f;
        private const float CategoryX = StationX + StationWidth + IndicatorMargin;
        private const float SpriteSize = 20f;
        private const float TypeX = CategoryX + CategoryWidth + IndicatorMargin;
        private const float CategoryWidth = 23f;
        private const float NameWidth = 270f;
        private const float LabelHeight = 14f;
        private const float TextScale = 0.8f;
        private const float MinTextScale = 0.65f;

        // Panel components.
        protected UILabel m_networkName;
        protected UILabel m_creatorName;
        protected UILabel m_stationLabel;
        protected UILabel m_categoryLabel;
        private UISprite _typeIconSprite;

        // ObjectData.
        protected NetRowItem m_thisItem;
        protected int m_index;

        /// <summary>
        /// Gets the height for this row.
        /// </summary>
        public override float RowHeight => DefaultRowHeight;

        /// <summary>
        /// Called when dimensions are changed, including as part of initial setup (required to set correct relative position of label).
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (m_networkName != null)
            {
                // Set label positions.
                m_networkName.relativePosition = new Vector2(NameX, (RowHeight - m_networkName.height) / 2f);
                m_creatorName.relativePosition = new Vector2(CreatorX, (RowHeight - m_creatorName.height) / 2f);
                m_stationLabel.relativePosition = new Vector2(StationX, (RowHeight - LabelHeight) / 2f);
                m_categoryLabel.relativePosition = new Vector2(CategoryX, (RowHeight - LabelHeight) / 2f);

                // Set icon sprice position.
                _typeIconSprite.relativePosition = new Vector2(TypeX, (RowHeight - SpriteSize) / 2f);
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
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public override void Display(object data, int rowIndex)
        {
            // Perform initial setup for new rows.
            if (m_networkName == null)
            {
                // Add object name label.
                m_networkName = AddUIComponent<UILabel>();
                m_networkName.textScale = TextScale;
                m_networkName.autoSize = true;

                // Add creator name label.
                m_creatorName = AddUIComponent<UILabel>();
                m_creatorName.textScale = TextScale;
                m_creatorName.autoSize = true;

                // Add type icon.
                _typeIconSprite = AddUIComponent<UISprite>();
                _typeIconSprite.autoSize = false;
                _typeIconSprite.width = SpriteSize;
                _typeIconSprite.height = SpriteSize;

                // Add station label.
                m_stationLabel = AddUIComponent<UILabel>();
                m_stationLabel.autoSize = false;
                m_stationLabel.autoHeight = false;
                m_stationLabel.textScale = TextScale;
                m_stationLabel.width = StationWidth;
                m_stationLabel.height = LabelHeight;

                // Add category label.
                m_categoryLabel = AddUIComponent<UILabel>();
                m_categoryLabel.autoSize = false;
                m_categoryLabel.autoHeight = false;
                m_categoryLabel.textScale = TextScale;
                m_categoryLabel.width = CategoryWidth;
                m_categoryLabel.height = LabelHeight;
            }

            // Set display text.
            m_thisItem = data as NetRowItem;
            if (m_thisItem?.prefab != null)
            {
                // Network name label.
                m_networkName.text = m_thisItem.displayName;
                m_networkName.textScale = TextScale;
                UILabels.ResizeLabel(m_networkName, NameWidth, MinTextScale);

                // Creator name label.
                m_creatorName.text = m_thisItem.creator;
                m_creatorName.textScale = TextScale;
                UILabels.ResizeLabel(m_creatorName, width - CreatorX, MinTextScale);

                // Set category label text and tooltip.
                if (m_thisItem.isVanilla)
                {
                    m_categoryLabel.text = "[v]";
                    m_categoryLabel.tooltip = Translations.Translate("RON_TIP_VAN");
                }
                else if (m_thisItem.isNExt2)
                {
                    m_categoryLabel.text = "[n]";
                    m_categoryLabel.tooltip = Translations.Translate("RON_TIP_NEX");
                }
                else if (m_thisItem.isMod)
                {
                    m_categoryLabel.text = "[m]";
                    m_categoryLabel.tooltip = Translations.Translate("RON_TIP_MOD");
                }
                else
                {
                    // Default - no label or tooltip.
                    m_categoryLabel.text = " ";
                    m_categoryLabel.tooltip = null;
                }

                // Set station label.
                if (m_thisItem.isStation)
                {
                    m_stationLabel.text = "[s]";
                    m_stationLabel.tooltip = Translations.Translate("RON_TIP_STA");
                }
                else
                {
                    // Default - no label or tooltip.
                    m_stationLabel.text = " ";
                    m_stationLabel.tooltip = null;
                }

                // Set icon sprite.
                if (m_thisItem.typeIcon != null)
                {
                    // Set icon.
                    _typeIconSprite.atlas = UITextures.LoadSingleSpriteAtlas(m_thisItem.typeIcon);
                    _typeIconSprite.spriteName = "normal";

                    // Set tooltip.
                    switch (m_thisItem.typeIcon)
                    {
                        case "ron_bridge":
                            _typeIconSprite.tooltip = Translations.Translate("RON_TIP_BRI");
                            break;
                        case "ron_elevated":
                            _typeIconSprite.tooltip = Translations.Translate("RON_TIP_ELE");
                            break;
                        case "ron_tunnel":
                            _typeIconSprite.tooltip = Translations.Translate("RON_TIP_TUN");
                            break;
                        default:
                            _typeIconSprite.tooltip = null;
                            break;
                    }

                    // Ensure visibility.
                    _typeIconSprite.Show();
                }
                else
                {
                    // Hide icon.
                    _typeIconSprite.Hide();
                    _typeIconSprite.tooltip = null;
                }

                // Set label positions and heights (accounting for any change in text scaling).
                OnSizeChanged();
            }
            else
            {
                // Null reference; clear text.
                m_networkName.text = string.Empty;
                m_creatorName.text = string.Empty;

                // Clear labels and tooltips.
                m_categoryLabel.text = " ";
                m_categoryLabel.tooltip = null;
                m_stationLabel.text = " ";
                m_stationLabel.tooltip = null;

                // Clear icon.
                _typeIconSprite.Hide();
            }

            // Set initial background as deselected state.
            Deselect(rowIndex);
        }
    }
}

