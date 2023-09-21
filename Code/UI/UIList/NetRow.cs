// <copyright file="NetRow.cs" company="algernon (K. Algernon A. Sheppard)">
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
    public class NetRow : UIListRow
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
        public const float CustomRowHeight = 23f;

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
        private UILabel _networkName;
        private UILabel _creatorName;
        private UILabel _stationLabel;
        private UILabel _categoryLabel;
        private UISprite _typeIconSprite;

        // ObjectData.
        private NetRowItem _thisItem;

        /// <summary>
        /// Gets the height for this row.
        /// </summary>
        public override float RowHeight => CustomRowHeight;

        /// <summary>
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public override void Display(object data, int rowIndex)
        {
            // Perform initial setup for new rows.
            if (_networkName == null)
            {
                // Add object name label.
                _networkName = AddUIComponent<UILabel>();
                _networkName.textScale = TextScale;
                _networkName.autoSize = true;

                // Add creator name label.
                _creatorName = AddUIComponent<UILabel>();
                _creatorName.textScale = TextScale;
                _creatorName.autoSize = true;

                // Add type icon.
                _typeIconSprite = AddUIComponent<UISprite>();
                _typeIconSprite.autoSize = false;
                _typeIconSprite.width = SpriteSize;
                _typeIconSprite.height = SpriteSize;

                // Add station label.
                _stationLabel = AddUIComponent<UILabel>();
                _stationLabel.autoSize = false;
                _stationLabel.autoHeight = false;
                _stationLabel.textScale = TextScale;
                _stationLabel.width = StationWidth;
                _stationLabel.height = LabelHeight;

                // Add category label.
                _categoryLabel = AddUIComponent<UILabel>();
                _categoryLabel.autoSize = false;
                _categoryLabel.autoHeight = false;
                _categoryLabel.textScale = TextScale;
                _categoryLabel.width = CategoryWidth;
                _categoryLabel.height = LabelHeight;
            }

            // Set display text.
            _thisItem = data as NetRowItem;
            if (_thisItem?.Prefab != null)
            {
                // Network name label.
                _networkName.text = _thisItem.DisplayName;
                _networkName.textScale = TextScale;
                UILabels.ResizeLabel(_networkName, NameWidth, MinTextScale);

                // Creator name label.
                _creatorName.text = _thisItem.Creator;
                _creatorName.textScale = TextScale;
                UILabels.ResizeLabel(_creatorName, width - CreatorX, MinTextScale);

                // Set category label text and tooltip.
                if (_thisItem.IsVanilla)
                {
                    _categoryLabel.text = "[v]";
                    _categoryLabel.tooltip = Translations.Translate("RON_TIP_VAN");
                }
                else if (_thisItem.IsNExt2)
                {
                    _categoryLabel.text = "[n]";
                    _categoryLabel.tooltip = Translations.Translate("RON_TIP_NEX");
                }
                else if (_thisItem.IsMod)
                {
                    _categoryLabel.text = "[m]";
                    _categoryLabel.tooltip = Translations.Translate("RON_TIP_MOD");
                }
                else
                {
                    // Default - no label or tooltip.
                    _categoryLabel.text = " ";
                    _categoryLabel.tooltip = null;
                }

                // Set station label.
                if (_thisItem.IsStation)
                {
                    _stationLabel.text = "[s]";
                    _stationLabel.tooltip = Translations.Translate("RON_TIP_STA");
                }
                else
                {
                    // Default - no label or tooltip.
                    _stationLabel.text = " ";
                    _stationLabel.tooltip = null;
                }

                // Set icon sprite.
                if (_thisItem.TypeIcon != null)
                {
                    // Set icon.
                    _typeIconSprite.atlas = UITextures.LoadSingleSpriteAtlas(_thisItem.TypeIcon);
                    _typeIconSprite.spriteName = "normal";

                    // Set tooltip.
                    switch (_thisItem.TypeIcon)
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
                _networkName.text = string.Empty;
                _creatorName.text = string.Empty;

                // Clear labels and tooltips.
                _categoryLabel.text = " ";
                _categoryLabel.tooltip = null;
                _stationLabel.text = " ";
                _stationLabel.tooltip = null;

                // Clear icon.
                _typeIconSprite.Hide();
            }

            // Set initial background as deselected state.
            Deselect(rowIndex);
        }

        /// <summary>
        /// Called when dimensions are changed, including as part of initial setup (required to set correct relative position of label).
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (_networkName != null)
            {
                // Set label positions.
                _networkName.relativePosition = new Vector2(NameX, (RowHeight - _networkName.height) / 2f);
                _creatorName.relativePosition = new Vector2(CreatorX, (RowHeight - _creatorName.height) / 2f);
                _stationLabel.relativePosition = new Vector2(StationX, (RowHeight - LabelHeight) / 2f);
                _categoryLabel.relativePosition = new Vector2(CategoryX, (RowHeight - LabelHeight) / 2f);

                // Set icon sprite position.
                _typeIconSprite.relativePosition = new Vector2(TypeX, (RowHeight - SpriteSize) / 2f);
            }
        }
    }
}