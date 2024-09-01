using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace PngTuberSharp.Views.Helper
{
    public class UintTextBox : TextBox
    {
        public static readonly StyledProperty<uint> MaxProperty =
            AvaloniaProperty.Register<UintTextBox, uint>(nameof(Max), defaultValue: uint.MaxValue);

        public uint Max
        {
            get => GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static readonly StyledProperty<uint> MinProperty =
            AvaloniaProperty.Register<UintTextBox, uint>(nameof(Min), defaultValue: uint.MinValue);
        private bool inputHandler;

        public uint Min
        {
            get => GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public UintTextBox()
        {
            this.GetObservable(TextProperty).Subscribe(OnTextChanged);
        }
        protected override Type StyleKeyOverride { get { return typeof(TextBox); } }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.PhysicalKey == PhysicalKey.Backspace)
            {
                if (Text.Length <= 1)
                {
                    Text = "0";
                    e.Handled = true;
                    return;
                }
            }
            base.OnKeyDown(e);

            if (string.IsNullOrEmpty(Text))
                Text = "0";

            uint newValue = uint.Parse(Text);
            uint? fixedVal = CheckBounds(newValue);
            if (fixedVal != null)
                Text = fixedVal.ToString();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            inputHandler = true;
            // Regex to allow only numeric input
            if (!Regex.IsMatch(e.Text, "^[0-9]*$")
                || !uint.TryParse(e.Text, out var num)
                )
            {
                e.Handled = true;
            }

            base.OnTextInput(e);

            uint newValue = uint.Parse(Text);
            uint? fixedVal = CheckBounds(newValue);
            if (fixedVal != null)
                Text = fixedVal.ToString();
            inputHandler = false;
        }

        private uint? CheckBounds(uint newValue)
        {
            if (newValue > Max)
                return Max;
            if (newValue < Min)
                return Min;
            return null;
        }

        private void OnTextChanged(string newValue)
        {
            return;
            if (inputHandler)
                return;
            string valueToSet = newValue;
            if (string.IsNullOrEmpty(valueToSet))
            {
                valueToSet = "0";
            }
            else if (!Regex.IsMatch(valueToSet, "^[0-9]*$"))
            {
                // Remove any non-numeric characters
                valueToSet = Regex.Replace(valueToSet, @"[^0-9]", string.Empty);

                // Move the caret to the end
                CaretIndex = valueToSet.Length;
            }
            if (!string.IsNullOrEmpty(valueToSet))
            {
                var fixedVal = CheckBounds(uint.Parse(valueToSet));
                if (fixedVal != null)
                    valueToSet = fixedVal.ToString();
            }

            SetValue(TextProperty, valueToSet);
        }

    }
}
