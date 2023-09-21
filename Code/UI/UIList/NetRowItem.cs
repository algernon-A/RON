// <copyright file="NetRowItem.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    /// <summary>
    /// Data structure class for individual net row display lines.
    /// </summary>
    public class NetRowItem
    {
        // Private fields.
        private readonly NetInfo _prefab;
        private readonly string _creator;
        private readonly bool _isStation = false;
        private string _displayName;

        // Network indicator flags - if this is a vanilla/NExt2/mod asset.
        private bool _isVanilla = false;
        private bool _isNExt2 = false;
        private bool _isMod = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetRowItem"/> class.
        /// </summary>
        /// <param name="network">Network prefab.</param>
        public NetRowItem(NetInfo network)
        {
            _prefab = network;
            GetDisplayName();
            _creator = PrefabUtils.GetCreator(network);
            _isStation = PrefabUtils.IsStation(network);
        }

        /// <summary>
        /// Gets the network prefab.
        /// </summary>
        public NetInfo Prefab => _prefab;

        /// <summary>
        /// Gets the network display name.
        /// </summary>
        public string DisplayName => _displayName;

        /// <summary>
        /// Gets the creator name.
        /// </summary>
        public string Creator => _creator;

        /// <summary>
        /// Gets a value indicating whether this is a station track network.
        /// </summary>
        public bool IsStation => _isStation;

        /// <summary>
        /// Gets a value indicating whether this is a vanilla network.
        /// </summary>
        public bool IsVanilla => _isVanilla;

        /// <summary>
        /// Gets a value indicating whether this is a NExt2 network.
        /// </summary>
        public bool IsNExt2 => _isNExt2;

        /// <summary>
        /// Gets a value indicating whether network is from a mod (other than NExt2).
        /// </summary>
        public bool IsMod => _isMod;

        /// <summary>
        /// Gets or sets the network type icon.
        /// </summary>
        public string TypeIcon { get; set; }

        /// <summary>
        /// Sets displayName to a cleaned-up display name for the given prefab and also sets network indicator flags.
        /// </summary>
        private void GetDisplayName()
        {
            // Make sure we've got a valid network before doing anything else.
            string fullName = _prefab?.name;
            if (fullName == null || _prefab.m_netAI == null)
            {
                _displayName = "Null";
                return;
            }

            // Find any leading period (Steam package number).
            int period = fullName.IndexOf('.');

            // If no period, assume it's either vanilla or Mod.
            if (period < 0)
            {
                // Check for NExt prefabs.  NExt prefabs aren't as consistent as would be ideal....
                _isNExt2 =
                    _prefab.m_class.name.StartsWith("NExt") ||
                    _prefab.m_class.name.StartsWith("NEXT") ||
                    _prefab.name.StartsWith("Small Busway") ||
                    _prefab.name.EndsWith("With Bus Lanes") ||
                    _prefab.name.Equals("PlainStreet2L") ||
                    _prefab.name.StartsWith("Highway2L2W") ||
                    _prefab.name.StartsWith("AsymHighwayL1R2");

                // Check for Extra Train Station Tracks, OneWayTrainTrack, and MOM prefabs; this overrides the NExt2 check due to some OneWayTrainTrack prefabs having 'NExtSingleStationTrack' ItemClass (and hence being picked up above as NExt2 items).
                _isMod = _prefab.name.StartsWith("Station") ||
                _prefab.name.StartsWith("Train Station Track (") ||
                _prefab.name.StartsWith("Rail1L") ||
                _prefab.m_netAI.GetType().Namespace?.Equals("MetroOverhaul") == true;
                _isNExt2 = _isNExt2 && !_isMod;

                // Set vanilla flag and display name.
                _isVanilla = !(_isNExt2 || _isMod);
                _displayName = fullName;
            }
            else
            {
                // Otherwise, omit the package number, and trim off any trailing _Data.
                _displayName = fullName.Substring(period + 1).Replace("_Data", string.Empty);
            }
        }
    }
}
