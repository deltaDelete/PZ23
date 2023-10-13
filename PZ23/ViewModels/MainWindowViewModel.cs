using System.Collections;
using System.Collections.Generic;
using FluentAvalonia.UI.Controls;

namespace PZ23.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public string Greeting => "Welcome to Avalonia!";
    public IEnumerable<NavigationViewItem> MenuItems { get; }

    public MainWindowViewModel() {
        MenuItems = new List<NavigationViewItem>() { };
    }
}