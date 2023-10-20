using System;
using System.Collections.Generic;
using System.Reactive;
using PZ23.Views.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PZ23.ViewModels.Dialogs;

public class EditDialogViewModel : ViewModelBase {
    [Reactive]
    public List<FieldData> Fields { get; set; } = new List<FieldData>();
    [Reactive]
    public object? Item { get; set; }
    
    // public ReactiveCommand<object?, Unit> PositiveClickCommand { get; private set; }
}
public class EditDialogViewModel<T> : EditDialogViewModel {
    [Reactive]
    public new T? Item { get; set; }
    public string Title { get; set; }
    public ReactiveCommand<T?, Unit> PositiveClickCommand { get; private set; }
    public ReactiveCommand<T?, Unit> NegativeClickCommand { get; private set; }

    public EditDialogViewModel(T item, Action<T?> positiveClick, Action<T?> negativeClick, string title) {
        Item = item;
        Title = title;
        PositiveClickCommand = ReactiveCommand.Create(positiveClick);
        NegativeClickCommand = ReactiveCommand.Create(negativeClick);
    }

}