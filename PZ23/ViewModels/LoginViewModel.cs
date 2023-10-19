using System.Collections.Generic;
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

public class LoginViewModel : ViewModelBase {
    private readonly LoginView _view;

    [Reactive] public string Login { get; set; } = string.Empty;
    [Reactive] public string Password { get; set; } = string.Empty;

    [Reactive]
    public bool IsPromptIncorrect { get; set; } = false;

    [Reactive] public User? User { get; set; } = null;

    [Reactive] public List<Group> Groups { get; set; } = new List<Group>();

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
        CloseCommand = ReactiveCommand.Create<Window>(Close);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }
    public ReactiveCommand<Window, Unit> CloseCommand { get; set; }

    private async Task LoginAsync() {
        await using var db = new MyDatabase();
        User = await db.GetAsync<User>().FirstOrDefaultAsync(x => x.Username == Login && x.Password == Password);
        if (User is null) {
            _view.Result = false;
            IsPromptIncorrect = true;
            return;
        }

        Groups = await db.GetAsync<UserGroup>().Where(ug => ug.User.Id == User.Id)
            .Select(x => x.Group)
            .ToListAsync();

        _view.Close();
    }

    private static void Close(Window window) {
        window.Close();
    }
}