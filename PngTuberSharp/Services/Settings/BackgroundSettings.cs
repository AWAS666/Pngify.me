using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text.Json.Serialization;

namespace PngTuberSharp.Settings
{
    public partial class BackgroundSettings : ObservableObject
    {
        private Avalonia.Media.IBrush colour = Avalonia.Media.Brushes.Lime;

        [JsonIgnore]
        public IBrush Colour
        {
            get => colour;
            set
            {
                SetProperty(ref colour, value);
                _colourHex = BrushToHex(value);
            }
        }
        public string Image { get; set; } = string.Empty;
        public string ColourHex
        {
            get => _colourHex;
            set
            {
                _colourHex = value;
                Colour = HexToBrush(value);
            }
        }

        private string _colourHex;

        public BackgroundSettings()
        {
            _colourHex = BrushToHex(colour);
        }

        public static string BrushToHex(IBrush brush)
        {
            if (brush is ImmutableSolidColorBrush solidColorBrush)
            {
                var color = solidColorBrush.Color;
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return string.Empty;

        }

        public static IBrush HexToBrush(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            if (hex.Length != 6 && hex.Length != 8)
            {
                throw new ArgumentException("Invalid hex format.");
            }

            var color = Color.Parse($"#{hex.ToLower()}");
            return new ImmutableSolidColorBrush(color);
        }

    }
}
