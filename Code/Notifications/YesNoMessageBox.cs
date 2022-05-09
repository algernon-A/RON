using ColossalFramework.UI;


namespace RON.MessageBox
{
    /// <summary>
    /// Message box with separate pargaraphs and/or lists of dot points, with 'close' and 'dont show again' buttons.
    /// </summary>
    public class YesNoMessageBox : ListMessageBox
    {
        // Don't Show Again button.
        private UIButton noButton, yesButton;

        // Number of buttons for this panel (for layout).
        protected override int NumButtons => 2;

        // Accessor.
        public UIButton YesButton => yesButton;

        /// <summary>
        /// Adds buttons to the message box.
        /// </summary>
        public override void AddButtons()
        {
            // Add no button.
            noButton = AddButton(1, NumButtons, Close);
            noButton.text = Translations.Translate("MES_NO");

            // Add yes button.
            yesButton = AddButton(2, NumButtons, Close);
            yesButton.text = Translations.Translate("MES_YES");
        }
    }
}