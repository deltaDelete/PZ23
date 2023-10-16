using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace PZ23.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private NavigationViewItem? _selectedNavigation = null;

    public NavigationViewItem? SelectedNavigation {
        get => _selectedNavigation;
        set => this.RaiseAndSetIfChanged(ref _selectedNavigation, value);
    }

    public MainWindowViewModel() {
        
    }

}