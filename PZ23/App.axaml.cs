using System;
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
            var login = ShowLoginWindow(
                u => {
                    if (u is null) return;
                    MainWindow.CurrentUser = u;
                    mainWindow.IsEnabled = true;
                }
            );
            login.Closed += (_, _) => {
                if (MainWindow.CurrentUser is not null) return;
                mainWindow.Close();
            };
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static LoginView ShowLoginWindow(Action<User?> action) {
        var login = new LoginView();

        login.ViewModel.WhenAnyValue(it => it.User, selector: user => user).Subscribe(action);

        login.Show();
        return login;
    }
}