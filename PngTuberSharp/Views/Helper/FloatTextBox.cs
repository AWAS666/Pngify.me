using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Text.RegularExpressions;

namespace PngTuberSharp.Views.Helper
{
    public class FloatTextBox : TextBox
    {
        private bool handling;

        public FloatTextBox()
        {
            this.GetObservable(TextProperty).Subscribe(OnTextChanged);
        }
        protected override Type StyleKeyOverride { get { return typeof(TextBox); } }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            // Regex to allow only numeric input
            if (!Regex.IsMatch(e.Text, "^[0-9]*$")
                || !float.TryParse(e.Text, out var num)
                )
            {
                e.Handled = true;
            }
            base.OnTextInput(e);
        }

        private void OnTextChanged(string newValue)
        {
            if (handling)
                return;
            handling = true;
            if (string.IsNullOrEmpty(newValue))
            {
                Text = "0";
            }
            else if (!Regex.IsMatch(newValue, "^[0-9]*$"))
            {
                // Remove any non-numeric characters
                Text = Regex.Replace(newValue, @"[^0-9\.]", "");

                // Move the caret to the end
                CaretIndex = Text.Length;
            }

            handling = false;
        }

    }
}
