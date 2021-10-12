using UnityEngine;
using ICities;


namespace RON
{
    public class UIThreading : ThreadingExtensionBase
    {
        // Key settings.
        public static KeyCode hotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "N");
        public static bool hotCtrl = false;
        public static bool hotAlt = true;
        public static bool hotShift = false;

        // Flags.
        internal static bool ignore = true;
        private bool processed = false;


        /// <summary>
        /// Look for keypress to open GUI.
        /// </summary>
        /// <param name="realTimeDelta"></param>
        /// <param name="simulationTimeDelta"></param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Has hotkey been pressed while we're not ignoring input?
            if (!ignore && hotKey != KeyCode.None && Input.GetKey(hotKey))
            {
                // Check modifier keys according to settings.
                bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
                bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                // Modifiers have to *exactly match* settings, e.g. "alt-N" should not trigger on "ctrl-alt-N".
                bool altOkay = altPressed == hotAlt;
                bool ctrlOkay = ctrlPressed == hotCtrl;
                bool shiftOkay = shiftPressed == hotShift;

                // Process keystroke.
                if (altOkay && ctrlOkay && shiftOkay)
                {
                    // Only process if we're not already doing so.
                    if (!processed)
                    {
                        // Set processed flag.
                        processed = true;

                        // Toggle tool status.
                        RONTool.ToggleTool();
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    processed = false;
                }
            }
            else
            {
                // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                processed = false;
            }
        }
    }
}