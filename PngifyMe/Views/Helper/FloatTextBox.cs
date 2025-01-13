using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PngifyMe.Views.Helper;
public class FloatTextBox : TextBox
{
    public static readonly StyledProperty<float> MaxProperty =
        AvaloniaProperty.Register<FloatTextBox, float>(nameof(Max), defaultValue: float.MaxValue);

    public float Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public static readonly StyledProperty<float> MinProperty =
        AvaloniaProperty.Register<FloatTextBox, float>(nameof(Min), defaultValue: float.MinValue);

    public float Min
    {
        get => GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public FloatTextBox()
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
                Text = 0.0f.ToString();
                e.Handled = true;
                return;
            }
        }
        base.OnKeyDown(e);

        if (string.IsNullOrEmpty(Text))
            Text = 0.0f.ToString();

        float newValue = float.Parse(Text, CultureInfo.CurrentCulture);
        float? fixedVal = CheckBounds(newValue);
        if (fixedVal != null)
            SetValue((float)fixedVal);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        //Regex to allow only numeric input
        if (!Regex.IsMatch(e.Text, @"^[0-9]*$")
            || !float.TryParse(Text + e.Text, out var num)
            )
        {
            e.Handled = true;
        }
        base.OnTextInput(e);

        float newValue = float.Parse(Text);
        float? fixedVal = CheckBounds(newValue);
        if (fixedVal != null)
            SetValue((float)fixedVal);
    }

    private void SetValue(float fixedVal)
    {
        string commaSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        string result = fixedVal.ToString();
        if (!result.Contains(commaSeparator))
        {
            result += commaSeparator + "0";
        }
        Text = result;
    }

    private float? CheckBounds(float newValue)
    {
        if (newValue > Max)
            return Max;
        if (newValue < Min)
            return Min;
        return null;
    }

    private void OnTextChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
            return;

        string commaSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        if (!newValue.Contains(commaSeparator))
        {
            Text += commaSeparator + "0";
        }
    }
}

