using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Helpers;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace PngifyMe.ViewModels;

public partial class LogViewerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LogEntry> allLogs = new();

    [ObservableProperty]
    private ObservableCollection<LogEntry> filteredLogs = new();

    [ObservableProperty]
    private bool showVerbose = true;

    [ObservableProperty]
    private bool showDebug = true;

    [ObservableProperty]
    private bool showInformation = true;

    [ObservableProperty]
    private bool showWarning = true;

    [ObservableProperty]
    private bool showError = true;

    [ObservableProperty]
    private bool showFatal = true;

    [ObservableProperty]
    private string searchText = string.Empty;

    public LogViewerViewModel()
    {
        var logEntries = LogViewerSink.LogEntries;
        foreach (var entry in logEntries)
        {
            AllLogs.Add(entry);
        }
        logEntries.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
            {
                foreach (LogEntry entry in e.NewItems)
                {
                    AllLogs.Add(entry);
                }
            }
            if (e.OldItems != null)
            {
                foreach (LogEntry entry in e.OldItems)
                {
                    AllLogs.Remove(entry);
                }
            }
        };
        
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ShowVerbose) ||
                e.PropertyName == nameof(ShowDebug) ||
                e.PropertyName == nameof(ShowInformation) ||
                e.PropertyName == nameof(ShowWarning) ||
                e.PropertyName == nameof(ShowError) ||
                e.PropertyName == nameof(ShowFatal) ||
                e.PropertyName == nameof(SearchText) ||
                e.PropertyName == nameof(AllLogs))
            {
                ApplyFilters();
            }
        };

        AllLogs.CollectionChanged += (s, e) => ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = AllLogs.AsEnumerable();

        if (!ShowVerbose)
            filtered = filtered.Where(l => l.Level != LogEventLevel.Verbose);
        if (!ShowDebug)
            filtered = filtered.Where(l => l.Level != LogEventLevel.Debug);
        if (!ShowInformation)
            filtered = filtered.Where(l => l.Level != LogEventLevel.Information);
        if (!ShowWarning)
            filtered = filtered.Where(l => l.Level != LogEventLevel.Warning);
        if (!ShowError)
            filtered = filtered.Where(l => l.Level != LogEventLevel.Error);
        if (!ShowFatal)
            filtered = filtered.Where(l => l.Level != LogEventLevel.Fatal);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(l =>
                l.Message.ToLowerInvariant().Contains(searchLower) ||
                (l.Exception?.ToLowerInvariant().Contains(searchLower) ?? false));
        }

        var filteredList = filtered.OrderByDescending(l => l.Timestamp).Take(5000).ToList();
        FilteredLogs.Clear();
        foreach (var log in filteredList)
        {
            FilteredLogs.Add(log);
        }
    }

    [RelayCommand]
    private void ClearLogs()
    {
        LogViewerSink.LogEntries.Clear();
    }
}

