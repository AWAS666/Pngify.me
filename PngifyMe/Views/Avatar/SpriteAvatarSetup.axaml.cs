using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DynamicData;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using PngifyMe.Views.Helper;
using System;
using System.ComponentModel;
using System.Linq;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace PngifyMe.Views.Avatar;

public partial class SpriteAvatarSetup : UserControl
{
    private Point _dragStartPosition;
    private Grid _draggedItem;
    private PropertyChangedEventHandler? _selectedChangedHandler;

    public SpriteAvatarSetup()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_selectedChangedHandler != null && DataContext is SpriteSetupViewModel oldVm)
        {
            oldVm.Settings.PropertyChanged -= _selectedChangedHandler;
            _selectedChangedHandler = null;
        }
        if (DataContext is SpriteSetupViewModel vm)
        {
            _selectedChangedHandler = (_, args) =>
            {
                if (args.PropertyName != nameof(vm.Settings.Selected)) return;
                SyncOverlayToSelection(vm);
            };
            vm.Settings.PropertyChanged += _selectedChangedHandler;
        }
    }

    private void SyncOverlayToSelection(SpriteSetupViewModel vm)
    {
        if (CanvasOverlayService.CurrentOverlayViewModel is not SpritePointOverlayViewModel spriteOverlay)
            return;
        var newSelected = vm.Settings.Selected;
        if (newSelected == null)
        {
            CanvasOverlayService.ClearOverlay();
            return;
        }
        if (newSelected == spriteOverlay.Sprite)
            return;
        var sourceControl = this.FindControl<Control>("test");
        CanvasOverlayService.SetOverlay(new SpritePointOverlayViewModel(newSelected, spriteOverlay.IsAnchorMode), sourceControl ?? this);
    }

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }

    private void ActivateState(object sender, RoutedEventArgs e)
    {
        var vm = (SpriteStates)((Button)sender).DataContext;
        LayerManager.CharacterStateHandler.ToggleState(vm.Name);
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Grid item)
        {
            _dragStartPosition = e.GetPosition(item);
            _draggedItem = item;
        }
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (_draggedItem == null || !e.GetCurrentPoint(_draggedItem).Properties.IsLeftButtonPressed)
            return;

        var currentPosition = e.GetPosition(_draggedItem);
        var delta = currentPosition - _dragStartPosition;

        if (Math.Abs(delta.X) > 3 || Math.Abs(delta.Y) > 3)
        {
            var data = new DataObject();
            data.Set("Node", _draggedItem.DataContext);
            DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
            _draggedItem = null;
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        if (!e.Data.Contains("Node") || sender is not Grid targetItem)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        var draggedNode = e.Data.Get("Node") as SpriteImage;
        var targetNode = targetItem.DataContext as SpriteImage;

        e.DragEffects = IsValidDrop(draggedNode, targetNode)
            ? DragDropEffects.Move
            : DragDropEffects.None;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.Contains("Node") || sender is not Grid targetItem)
            return;

        var draggedNode = e.Data.Get("Node") as SpriteImage;
        var targetNode = targetItem.DataContext as SpriteImage;

        if (!IsValidDrop(draggedNode, targetNode)) return;

        draggedNode.Parent.Children.Remove(draggedNode);
        targetNode.Children.Add(draggedNode);
        draggedNode.Parent = targetNode;
        ((SpriteCharacterSetup)LayerManager.CharacterStateHandler.CharacterSetup).ReloadLayerList();
        ((SpriteCharacterSettings)SettingsManager.Current.Profile.Active.AvatarSettings).Selected = draggedNode;
    }

    private bool IsValidDrop(SpriteImage draggedNode, SpriteImage targetNode)
    {
        // Prevent dropping onto itself or a descendant
        return draggedNode != targetNode && !IsDescendant(draggedNode, targetNode);
    }

    private bool IsDescendant(SpriteImage parent, SpriteImage target)
    {
        var node = target;
        while (node != null)
        {
            if (node == parent)
                return true;
            node = node.Parent;
        }
        return false;
    }

}