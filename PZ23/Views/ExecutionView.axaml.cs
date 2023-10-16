using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZ23.ViewModels;

namespace PZ23.Views;

public partial class ExecutionView : UserControl
{
    public ExecutionView()
    {
        InitializeComponent();
        DataContext = new ExecutionViewModel();
    }
}