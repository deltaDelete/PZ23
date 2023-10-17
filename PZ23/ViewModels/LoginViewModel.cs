using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DynamicData.Binding;
using PZ23.Models;
using PZ23.Views;
using ReactiveUI;

namespace PZ23.ViewModels;

public class LoginViewModel : ViewModelBase {
    private readonly Window _view;
    private string _login;
    private string _password;
    private bool _isPromptIncorrect;

    public string Login {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    public string Password {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public bool IsPromptIncorrect {
        get => _isPromptIncorrect;
        set => this.RaiseAndSetIfChanged(ref _isPromptIncorrect, value);
    }

    public LoginViewModel(Window view) {
        _view = view;
        LoginCommand = ReactiveCommand.CreateFromTask(
            LoginAsync);
    }

    public ReactiveCommand<Unit,Unit> LoginCommand { get; set; }

    public async Task LoginAsync() {
        await using var db = new MyDatabase();
        var user = await db.GetAsync<User>().FirstOrDefaultAsync(x => x.Username == Login && x.Password == Password);
        if (user is null) {
            IsPromptIncorrect = true;
            return;
        }

        MainWindow window = new MainWindow() {
            DataContext = new MainWindowViewModel()
        };
        (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow = window;
        _view.Close();
        window.Show();
    }
}