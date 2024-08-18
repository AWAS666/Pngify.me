using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using PngTuberSharp.Services;
using PngTuberSharp.Settings;
using System.Linq;
using System.Reflection;

namespace PngTuberSharp.Views;

public partial class LayoutMenu : UserControl
{
    public LayoutMenu()
    {
        InitializeComponent();
        background.ItemsSource = typeof(Brushes)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
             .Where(p => typeof(IBrush).IsAssignableFrom(p.PropertyType))
            .Select(p => (IBrush)p.GetValue(null));

        background.SelectedValue = SettingsManager.Current.Background.Colour;
        background.SelectionChanged += Background_SelectionChanged;
        UpdateText();
        backgroundHex.KeyUp += BackgroundHex_KeyUp;
    }

    private void BackgroundHex_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (backgroundHex.Text.Length > 7)
        {
            e.Handled = true;
            return;
        }
        try
        {
            var color = BackgroundSettings.HexToBrush(backgroundHex.Text);
            background.SelectedValue = null;
            SettingsManager.Current.Background.Colour = color;
        }
        catch (System.Exception)
        {

        }
    }

    private void UpdateText()
    {
        backgroundHex.Text = BackgroundSettings.BrushToHex(SettingsManager.Current.Background.Colour);
    }

    private void Background_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1)
            return;
        SettingsManager.Current.Background.Colour = (IBrush)e.AddedItems[0];
        UpdateText();
    }

    private void SetToTransparant(object sender, RoutedEventArgs e)
    {
        background.SelectedValue = Brushes.Transparent;
        SettingsManager.Current.Background.Colour = Brushes.Transparent;
        UpdateText();
    }
}