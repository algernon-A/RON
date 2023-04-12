// <copyright file="PathIndex.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
    using AlgernonCommons;

    /// <summary>
    /// Path index struct.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Internal data struct")]
    internal struct PathIndex
    {
        /// <summary>
        /// Sub-building index number (-1 for parent building).
        /// </summary>
        internal int m_subBuilding;

        /// <summary>
        /// Path index number (-1 for no entry).
        /// </summary>
        internal int m_pathIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathIndex"/> struct.
        /// </summary>
        /// <param name="subBuilding">Sub-building index for this record.</param>
        /// <param name="path">Path index for this record.</param>
        internal PathIndex(int subBuilding, int path)
        {
            m_subBuilding = subBuilding;
            m_pathIndex = path;
        }

        /// <summary>
        /// Gets a value indicating whether this PathIndex is valid.
        /// </summary>
        internal bool IsValid => m_pathIndex >= 0;

        /// <summary>
        /// Invalidates this record.
        /// </summary>
        internal void Invalidate()
        {
            m_pathIndex = -1;
            m_subBuilding = -1;
        }
    }
}
