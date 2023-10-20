using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PZ23.ViewModels;

namespace PZ23.Views; 

public partial class StatisticsView : ReactiveUserControl<StatisticsViewModel> {
    public StatisticsView() {
        InitializeComponent();
        ViewModel = new StatisticsViewModel();
    }
}