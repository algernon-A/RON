﻿using System;
using UnityEngine;
using ICities;
using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// Class to handle the mod's options panel.
    /// </summary>
    internal static class OptionsPanelManager
    {
        // Parent UI panel reference.
        internal static UIScrollablePanel optionsPanel;
        private static UIPanel gameOptionsPanel;

        // Instance references.
        private static GameObject optionsGameObject;
        private static RONOptionsPanel panel;

        // Accessors.
        internal static RONOptionsPanel Panel => panel;
        internal static bool IsOpen => optionsGameObject != null;


        /// <summary>
        /// Options panel setup.
        /// </summary>
        /// <param name="helper">UIHelperBase parent</param>
        internal static void Setup(UIHelperBase helper)
        {
            // Set up tab strip and containers.
            optionsPanel = ((UIHelper)helper).self as UIScrollablePanel;
            optionsPanel.autoLayout = false;
        }


        /// <summary>
        /// Attaches an event hook to options panel visibility, to enable/disable mod hokey when the panel is open.
        /// </summary>
        internal static void OptionsEventHook()
        {
            // Get options panel instance.
            gameOptionsPanel = UIView.library.Get<UIPanel>("OptionsPanel");

            if (gameOptionsPanel == null)
            {
                Logging.Error("couldn't find OptionsPanel");
            }
            else
            {
                // Simple event hook to create/destroy GameObject based on appropriate visibility.
                gameOptionsPanel.eventVisibilityChanged += (control, isVisible) =>
                {
                    // Create/destroy based on whether or not we're now visible.
                    if (isVisible)
                    {
                        Create();
                    }
                    else
                    {
                        Close();
                    }
                };
            }
        }


        /// <summary>
        /// Refreshes the options panel (destroys and rebuilds) on a locale change when the options panel is open.
        /// </summary>
        internal static void LocaleChanged()
        {
            if (gameOptionsPanel != null && gameOptionsPanel.isVisible)
            {
                Logging.KeyMessage("changing locale");

                Close();
                Create();
            }
        }


        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        private static void Create()
        {
            try
            {
                // If no instance already set, create one.
                if (optionsGameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    optionsGameObject = new GameObject("RONOptionsPanel");
                    optionsGameObject.transform.parent = optionsPanel.transform;

                    panel = optionsGameObject.AddComponent<RONOptionsPanel>();

                    // Set up and show panel.
                    Panel.Setup(optionsPanel.width, optionsPanel.height);

                    // Disable hotkey while options panel is open.
                    UIThreading.Operating = false;
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating options panel");
            }
        }


        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        private static void Close()
        {
            // Save settings first.
            ModSettings.Save();

            // We're no longer visible - destroy our game object.
            if (optionsGameObject != null)
            {
                GameObject.Destroy(optionsGameObject);
                optionsGameObject = null;

                // Re-enable hotkey.
                UIThreading.Operating = true;
            }
        }
    }
}