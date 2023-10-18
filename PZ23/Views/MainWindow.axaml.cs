using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ReactiveUI.Fody.Helpers;

namespace PZ23.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
    public MainWindow() {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
    }

    [Reactive]
    public User? CurrentUser { get; set; }

    [Reactive]
    public ObservableCollection<Group> CurrentUserGroups { get; set; } = new();

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