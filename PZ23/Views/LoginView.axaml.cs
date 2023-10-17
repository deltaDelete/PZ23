using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PZ23.Models;
using PZ23.ViewModels;

namespace PZ23.Views; 

public partial class LoginView : ReactiveWindow<LoginViewModel> {
    public LoginView() {
        InitializeComponent();
        ViewModel = new LoginViewModel(this);
    }

    /// <summary>
    /// После закрытия диалога TRUE если вход успешен
    /// </summary>
    public bool Result { get; set; }
}