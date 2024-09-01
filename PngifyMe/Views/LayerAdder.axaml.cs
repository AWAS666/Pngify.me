using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using PngifyMe.Layers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PngifyMe.Views;

public partial class LayerAdder : UserControl
{
    public LayerAdder()
    {
        InitializeComponent();
        var derivedTypes = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseLayer)));

        foreach (var type in derivedTypes)
        {
            var button = new Button { Content = type.Name, Margin = new Thickness(5, 5, 5, 5) };

            // Attach click event handler
            button.Click += (sender, e) => OnButtonClick(type);

            // Add button to StackPanel
            stack.Children.Add(button);
        }
        LayerManager.NewLayer += RefreshLayers;
    }

    private void RefreshLayers(object? sender, BaseLayer e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            activeStack.Children.Clear();
            foreach (var item in LayerManager.Layers)
            {
                var text = new TextBlock { Text = item.GetType().Name, Margin = new Thickness(5, 5, 5, 5) };
                activeStack.Children.Add(text);
            }
        });
    }

    private void OnButtonClick(Type type)
    {
        if (Activator.CreateInstance(type) is BaseLayer newLayer)
        {
            LayerManager.AddLayer(newLayer);
            Debug.WriteLine($"Added new instance of {type.Name} to the list.");
        }
    }
}