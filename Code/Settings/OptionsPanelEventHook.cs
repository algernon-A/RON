using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// Class to handle the mod's options panel.
    /// </summary>
    internal static class OptionsPanel
    {
        private static UIPanel gameOptionsPanel;


        /// <summary>
        /// Attaches an event hook to options panel visibility, to enable/disable mod hokey when the panel is open.
        /// </summary>
        public static void OptionsEventHook()
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
                        UIThreading.ignore = true;
                    }
                    else
                    {
                        UIThreading.ignore = false;
                    }
                };
            }
        }
    }
}