using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using PZ23.Models;
using PZ23.ViewModels;
using ReactiveUI;

namespace PZ23.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
    public MainWindow() {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
    }

    public static User? CurrentUser { get; set; }
    
    public static UserGroup? UserGroup { get; set; }

    private void NavigationView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e) {
        if (e.SelectedItem is not NavigationViewItem item) {
            return;
        }

        if (item.Tag is not Type viewType) {
            return;
        }

        Frame.Navigate(viewType, null, new SlideNavigationTransitionInfo() {
            Effect = SlideNavigationTransitionEffect.FromBottom
        });
    }
}