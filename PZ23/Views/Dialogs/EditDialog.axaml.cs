using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using DbTools;
using DynamicData;
using PZ23.ViewModels.Dialogs;

namespace PZ23.Views.Dialogs;

public partial class EditDialog : ReactiveWindow<EditDialogViewModel> {
    public EditDialog() {
        InitializeComponent();
    }

    public static EditDialog NewInstance<T>(Action<T?> positiveClick, T? item = null, string title = "")
        where T : class, new() {
        var dialog = new EditDialog() {
            [!Window.TitleProperty] = new Binding("Title")
        };
        dialog.ViewModel = new EditDialogViewModel<T>(
            item ?? new T(),
            obj => {
                positiveClick(obj);
                dialog.Close();
            },
            obj => { dialog.Close(); },
            title
        );
        dialog.SetupField<T>();
        return dialog;
    }

    public void SetupField<T>() where T : class {
        var fields = GetFields<T>();
        var dock = new DockPanel() {
            LastChildFill = true,
            Margin = new Thickness(16)
        };
        var labels = new StackPanel() {
            Spacing = 8,
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        labels.SetValue(DockPanel.DockProperty, Dock.Left);
        var inputs = new StackPanel() {
            Spacing = 8,
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var buttons = new StackPanel() {
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children = {
                new Button() {
                    Content = "Закрыть",
                    IsCancel = true,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    [!Button.CommandProperty] = new Binding("NegativeClickCommand"),
                    [!Button.CommandParameterProperty] = new Binding("Item")
                },
                new Button() {
                    Content = "Сохранить",
                    IsDefault = true,
                    Classes = { "accent" },
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    [!Button.CommandProperty] = new Binding("PositiveClickCommand"),
                    [!Button.CommandParameterProperty] = new Binding("Item")
                },
            }
        };
        buttons.SetValue(DockPanel.DockProperty, Dock.Bottom);

        foreach (var (propertyInfo, displayName, type) in fields) {
            TemplatedControl box;
            if (type == typeof(int)) {
                box = new NumericUpDown() {
                    ShowButtonSpinner = false,
                    [!NumericUpDown.ValueProperty] = new Binding($"Item.{propertyInfo.Name}"),
                };
            }
            else if (
                type == typeof(DateTime)
                || type == typeof(DateTime?)
                || type == typeof(DateTimeOffset)
                || type == typeof(DateTimeOffset?)
            ) {
                box = new DatePicker() {
                    [!DatePicker.SelectedDateProperty] = new Binding($"Item.{propertyInfo.Name}")
                };
            }
            else {
                box = new TextBox() {
                    [!TextBox.TextProperty] = new Binding($"Item.{propertyInfo.Name}")
                };
            }

            var label = new Label() {
                Content = displayName,
                HorizontalAlignment = HorizontalAlignment.Right,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                MinHeight = 32
            };

            labels.Children.Add(label);
            inputs.Children.Add(box);
        }

        dock.Children.Add(labels);
        dock.Children.Add(buttons);
        dock.Children.Add(inputs);

        this.Content = dock;
    }

    private static IEnumerable<FieldData> GetFields<T>() {
        return typeof(T)
               .GetProperties()
               .Where(
                   it => it.GetCustomAttribute<DisplayNameAttribute>() is not null
               )
               .Select(
                   it => new FieldData(
                       it,
                       it.GetCustomAttribute<DisplayNameAttribute>()!.DisplayName,
                       it.PropertyType
                   )
               );
    }
}

public record FieldData(PropertyInfo PropertyInfo, string DisplayName, Type Type);