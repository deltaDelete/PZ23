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
    }

    public static User CurrentUser { get; private set; }

    private async Task ShowLoginWindow() {
        LoginView login = new LoginView();
        login.ShowDialog(this);
        if (!login.Result) {
            Close();
        }

        login.ViewModel.WhenAnyValue(it => it.User, selector: user => {
            CurrentUser = user!;
            return user!;
        }).Subscribe(
            u => CurrentUser = u
        );
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        ShowLoginWindow().GetAwaiter().GetResult();
    }

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