using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PZ23.Models;
using PZ23.Views;
using PZ23.Views.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PZ23.ViewModels;

public class RequestViewModel: ViewModelBase
{
    private List<Request> _itemsFull = null!;

    private static MainWindow MainWindow => (App.Current as App).MainWindow;

    #region Notifying Properties

    [Reactive] public int SelectedSearchColumn { get; set; }

    [Reactive] public bool IsSortByDescending { get; set; } = false;

    [Reactive] public string SearchQuery { get; set; } = string.Empty;

    [Reactive] public BindingList<Request> Items { get; set; } = new();

    [Reactive] public int Take { get; set; } = 10;

    [Reactive] public int Skip { get; set; } = 0;

    [Reactive] public int CurrentPage { get; set; } = 1;

    public int TotalPages => (int)Math.Ceiling(Filtered.Count / (double)Take);

    [Reactive] public List<Request> Filtered { get; set; } = new List<Request>();

    [Reactive] public bool IsLoading { get; set; } = true;

    [Reactive] public Request? SelectedRow { get; set; } = null!;

    #endregion

    public ReactiveCommand<Request, Unit> EditItemCommand { get; }
    public ReactiveCommand<Request, Unit> RemoveItemCommand { get; }
    public ReactiveCommand<Unit, Unit> NewItemCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeNextCommand { get; }
    public ReactiveCommand<Unit, Unit> TakePrevCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeFirstCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeLastCommand { get; }

    public RequestViewModel() {
        var canTakeNext = this.WhenAnyValue(
            x => x.CurrentPage,
            selector: it => it < TotalPages);
        var canTakeBack = this.WhenAnyValue(
            x => x.CurrentPage,
            selector: it => it > 1);
        var canTakeLast = this.WhenAnyValue(
            x => x.CurrentPage, 
            x => x.TotalPages,
            selector: (i1, i2) => i1 < i2);

        var canEdit = this.WhenAnyValue(
            x => x.SelectedRow, 
            selector: request => request is not null 
                                && MainWindow.CurrentUserGroups
                                .Any(it => it.Permissions.HasFlag(Permissions.Write)));

        var canInsert = MainWindow.WhenAnyValue(
            it => it.CurrentUserGroups,
            selector: it => it.Any(group => group.Permissions.HasFlag(Permissions.Insert))
        );

        TakeNextCommand = ReactiveCommand.Create(TakeNext, canTakeNext);
        TakePrevCommand = ReactiveCommand.Create(TakePrev, canTakeBack);
        TakeFirstCommand = ReactiveCommand.Create(TakeFirst, canTakeBack);
        TakeLastCommand = ReactiveCommand.Create(TakeLast, canTakeLast);
        EditItemCommand = ReactiveCommand.Create<Request>(EditItem, canEdit);
        RemoveItemCommand = ReactiveCommand.Create<Request>(RemoveItem, canEdit);
        NewItemCommand = ReactiveCommand.CreateFromTask(NewItem, canInsert);

        GetDataFromDb();

        this.WhenAnyValue(
                x => x.SearchQuery,
                x => x.SelectedSearchColumn,
                x => x.IsSortByDescending
            )
            .DistinctUntilChanged()
            .Subscribe(OnSearchChanged);
        this.WhenAnyValue(
            x => x.Filtered
        ).Subscribe(_ => TakeFirst());
    }

    private void OnSearchChanged((string query, int column, bool isDescending) tuple) {
        if (_itemsFull is null) {
            return;
        }

        var filtered = tuple.query == ""
            ? _itemsFull
            : tuple.column switch {
                1 => _itemsFull
                    .Where(it => it.RequestId.ToString().Contains(tuple.query)),
                2 => _itemsFull
                    .Where(it => it.StartDate.ToString(CultureInfo.InvariantCulture).Contains(tuple.query.ToLower())),
                3 => _itemsFull
                    .Where(it => it.Gear!.GearName.ToString().Contains(tuple.query.ToLower())),
                4 => _itemsFull
                    .Where(it => it.Type!.FailureTypeName.ToLower().Contains(tuple.query.ToLower())),
                5 => _itemsFull
                    .Where(it => it.FailureDescription.ToLower().Contains(tuple.query.ToLower())),
                6 => _itemsFull
                    .Where(it => $"{it.Client!.LastName} {it.Client!.MiddleName} {it.Client!.FirstName}".ToLower().Contains(tuple.query.ToLower())),
                7 => _itemsFull
                    .Where(it => it.Priority!.PriorityName.ToLower().Contains(tuple.query.ToLower())),
                8 => _itemsFull
                    .Where(it => it.RequestStatus!.RequestStatusName.ToLower().Contains(tuple.query.ToLower())),
                _ => _itemsFull
                    .Where(
                        it => it.StartDate.ToString(CultureInfo.InvariantCulture).Contains(tuple.query.ToLower()) || 
                              it.Gear!.GearName.ToString().Contains(tuple.query.ToLower()) ||
                              it.Type!.FailureTypeName.ToLower().Contains(tuple.query.ToLower()) ||
                                   it.RequestId.ToString().Contains(tuple.query.ToLower()) ||
                              it.FailureDescription.ToLower().Contains(tuple.query.ToLower()) ||
                              $"{it.Client!.LastName} {it.Client!.MiddleName} {it.Client!.FirstName}".ToLower().Contains(tuple.query.ToLower()) ||
                              it.Priority!.PriorityName.ToLower().Contains(tuple.query.ToLower()) ||
                              it.RequestStatus!.RequestStatusName.ToLower().Contains(tuple.query.ToLower())
                        )
            };

        Filtered = tuple.column switch {
            2 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.StartDate).ToList()
                : filtered.OrderBy(it => it.StartDate).ToList(),
            3 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.Gear!.GearName).ToList()
                : filtered.OrderBy(it => it.Gear!.GearName).ToList(),
            4 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.Type!.FailureTypeName).ToList()
                : filtered.OrderBy(it => it.Type!.FailureTypeName).ToList(),
            5 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.FailureDescription).ToList()
                : filtered.OrderBy(it => it.FailureDescription).ToList(),
            6 => tuple.isDescending
                ? filtered.OrderByDescending(it => $"{it.Client!.LastName} {it.Client!.MiddleName} {it.Client!.FirstName}").ToList()
                : filtered.OrderBy(it => $"{it.Client!.LastName} {it.Client!.MiddleName} {it.Client!.FirstName}").ToList(),
            7 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.Priority!.PriorityName).ToList()
                : filtered.OrderBy(it => it.Priority!.PriorityName).ToList(),
            8 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.RequestStatus!.RequestStatusName).ToList()
                : filtered.OrderBy(it => it.RequestStatus!.RequestStatusName).ToList(),
            _ => tuple.isDescending
                ? filtered.OrderByDescending(it => it.RequestId).ToList()
                : filtered.OrderBy(it => it.RequestId).ToList()
        };
    }

    private async void GetDataFromDb() {
        await Task.Run(async () => {
            IsLoading = true;
            await using var db = new MyDatabase();
            var list = await db.GetAsync<Request>().ToListAsync();
            _itemsFull = list;
            Filtered = _itemsFull;
            IsLoading = false;
            SearchQuery = string.Empty;
            return Task.CompletedTask;
        });
    }

    private async void RemoveItem(Request? arg) {
        if (arg is null) return;
        // await new ConfirmationDialog(
        //     "Вы собираетесь удалить строку",
        //     $"Пользователь: {arg.LastName} {arg.FirstName} {arg.MiddleName}",
        //     async dialog =>
        //     {
        //         await using var db = new MyDatabase();
        //         await db.RemoveAsync(arg);
        //         RemoveLocal(arg);
        //     },
        //     dialog => { }
        // ).ShowDialog(_clientView);
        throw new NotImplementedException();
    }

    private void RemoveLocal(Request arg) {
        Items.Remove(arg);
        _itemsFull.Remove(arg);
        Filtered.Remove(arg);
    }

    private async void EditItem(Request? arg) {
        await EditDialog.NewInstance(
            async item => {
                await using var db = new MyDatabase();
                await db.UpdateAsync(item.RequestId, item);
                ReplaceItem(arg, item);
            },
            arg,
            title: "Изменить заявку"
        ).ShowDialog(MainWindow);
    }

    private void ReplaceItem(Request prevItem, Request newItem) {
        if (Filtered.Contains(prevItem)) {
            var index = Filtered.IndexOf(prevItem);
            Filtered[index] = newItem;
        }

        if (_itemsFull.Contains(prevItem)) {
            var index = _itemsFull.IndexOf(prevItem);
            _itemsFull[index] = newItem;
        }

        if (Items.Contains(prevItem)) {
            var index = _itemsFull.IndexOf(prevItem);
            Items[index] = newItem;
        }
    }

    private async Task NewItem() {
        await EditDialog.NewInstance<Request>(
            async i => {
                await using var db = new MyDatabase();
                int newItemId = Convert.ToInt32(await db.InsertAsync(i));
                i.ClientId = newItemId;
                _itemsFull.Add(i);
                if (Items.Count < 10) {
                    Items.Add(i);
                }
            },
            title: "Новая заявка"
        ).ShowDialog(MainWindow);
    }

    private void TakeNext() {
        Skip += Take;
        Items = new(
            Filtered.Skip(Skip).Take(Take).ToList()
        );
    }

    private void TakePrev() {
        Skip -= Take;
        Items = new(
            Filtered.Skip(Skip).Take(Take).ToList()
        );
    }

    private void TakeFirst() {
        Skip = 0;
        Items = new(
            Filtered.Take(Take).ToList()
        );
    }

    private void TakeLast() {
        Skip = Filtered.Count - Take;
        Items = new(
            Filtered.TakeLast(Take).ToList()
        );
    }
}