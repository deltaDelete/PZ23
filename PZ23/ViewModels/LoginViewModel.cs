using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using DynamicData.Binding;
using PZ23.Models;
using PZ23.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace PZ23.ViewModels;

public class LoginViewModel : ViewModelBase, IEnableLogger {
    private readonly LoginView _view;

    [Reactive] public string Login { get; set; } = string.Empty;
    [Reactive] public string Password { get; set; } = string.Empty;
    [Reactive] public bool IsPromptIncorrect { get; set; }

    [Reactive] public User? User { get; set; } = null;

    public LoginViewModel(LoginView view) {
        _view = view;

        var canExecute = this.WhenAnyValue(
                x => x.Login,
                x => x.Password,
                (s, s1) => !string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(s1)
            )
            .DistinctUntilChanged();
        LoginCommand = ReactiveCommand.CreateFromTask(
            LoginAsync,
            canExecute
        );
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }

    public async Task LoginAsync() {
        await using var db = new MyDatabase();
        User = await db.GetAsync<User>().FirstOrDefaultAsync(x => x.Username == Login && x.Password == Password);
        if (User is null) {
            this.Log().Warn("user is null");
            _view.Result = false;
            IsPromptIncorrect = true;
            return;
        }

        this.Log().Warn("Logging in");
        _view.Result = true;
        _view.Close();
    }
}