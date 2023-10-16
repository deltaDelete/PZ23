using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZ23.ViewModels;

namespace PZ23.Views;

public partial class RequestView : UserControl
{
    public RequestView()
    {
        InitializeComponent();
        DataContext = new RequestViewModel();
    }
}