using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZ23.ViewModels;

namespace PZ23.Views; 

public partial class GearView : UserControl {
    public GearView() {
        InitializeComponent();
        DataContext = new GearViewModel();
    }
}