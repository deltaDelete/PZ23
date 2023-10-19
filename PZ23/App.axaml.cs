using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PZ23.Models;
using PZ23.ViewModels;
using PZ23.Views;
using ReactiveUI;

namespace PZ23;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var mainWindow = new MainWindow() {
                IsEnabled = false
            };
            var login = CreateLoginWindow(
                (u, g) => {
                    if (u is null) return;
                    mainWindow.CurrentUser = u;
                    mainWindow.CurrentUserGroups = new ObservableCollection<Group>(g);
                    mainWindow.IsEnabled = true;
                }
            );
            login.Closed += (_, _) => {
                if (mainWindow.CurrentUser is not null) return;
                mainWindow.Close();
            };
            mainWindow.Opened += (_, _) => {
                login.Show(mainWindow);
            };
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public MainWindow MainWindow => (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow! as MainWindow;

    private static LoginView CreateLoginWindow(Action<User?, List<Group>> action) {
        var login = new LoginView();
        login.ViewModel.WhenAnyValue(
            it => it.User,
            it => it.Groups)
            .Subscribe(tuple => action(tuple.Item1, tuple.Item2));
        return login;
    }
}