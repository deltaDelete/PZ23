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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PZ23.ViewModels; 

public class ServiceViewModel: ViewModelBase {
    private List<Service> _itemsFull = null!;

    private static MainWindow MainWindow => (App.Current as App).MainWindow;

    #region Notifying Properties

    [Reactive] public int SelectedSearchColumn { get; set; }

    [Reactive] public bool IsSortByDescending { get; set; } = false;

    [Reactive] public string SearchQuery { get; set; } = string.Empty;

    [Reactive] public BindingList<Service> Items { get; set; } = new();

    [Reactive] public int Take { get; set; } = 10;

    [Reactive] public int Skip { get; set; } = 0;

    [Reactive] public int CurrentPage { get; set; } = 1;

    public int TotalPages => (int)Math.Ceiling(Filtered.Count / (double)Take);

    [Reactive] public List<Service> Filtered { get; set; } = new List<Service>();

    [Reactive] public bool IsLoading { get; set; } = true;

    [Reactive] public Service? SelectedRow { get; set; } = null!;

    #endregion

    public ReactiveCommand<Service, Unit> EditItemCommand { get; }
    public ReactiveCommand<Service, Unit> RemoveItemCommand { get; }
    public ReactiveCommand<Unit, Unit> NewItemCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeNextCommand { get; }
    public ReactiveCommand<Unit, Unit> TakePrevCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeFirstCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeLastCommand { get; }

    public ServiceViewModel() {
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
            selector: service => service is not null 
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
        EditItemCommand = ReactiveCommand.Create<Service>(EditItem, canEdit);
        RemoveItemCommand = ReactiveCommand.Create<Service>(RemoveItem, canEdit);
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
                    .Where(it => it.ServiceId.ToString().Contains(tuple.query)),
                2 => _itemsFull
                    .Where(it => it.ServiceName.ToLower().Contains(tuple.query.ToLower())),
                3 => _itemsFull
                    .Where(it => it.ServicePrice.ToString(CultureInfo.InvariantCulture).Contains(tuple.query.ToLower())),
                _ => _itemsFull
                    .Where(
                        it => it.ServiceName.ToLower().Contains(tuple.query.ToLower()) || 
                                   it.ServicePrice.ToString(CultureInfo.InvariantCulture).Contains(tuple.query.ToLower()) ||
                                   it.ServiceId.ToString().Contains(tuple.query.ToLower())
                        )
            };

        Filtered = tuple.column switch {
            2 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.ServiceName).ToList()
                : filtered.OrderBy(it => it.ServiceName).ToList(),
            3 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.ServicePrice).ToList()
                : filtered.OrderBy(it => it.ServicePrice).ToList(),
            _ => tuple.isDescending
                ? filtered.OrderByDescending(it => it.ServiceId).ToList()
                : filtered.OrderBy(it => it.ServiceId).ToList()
        };
    }

    private async void GetDataFromDb() {
        await Task.Run(async () => {
            IsLoading = true;
            await using var db = new MyDatabase();
            var list = await db.GetAsync<Service>().ToListAsync();
            _itemsFull = list;
            Filtered = _itemsFull;
            IsLoading = false;
            SearchQuery = string.Empty;
            return Task.CompletedTask;
        });
    }

    private async void RemoveItem(Service? arg) {
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

    private void RemoveLocal(Service arg) {
        Items.Remove(arg);
        _itemsFull.Remove(arg);
        Filtered.Remove(arg);
    }

    private async void EditItem(Service? arg) {
        if (arg is null) return;
        // await new EditClientDialog(
        //     arg,
        //     async client =>
        //     {
        //         await using var db = new MyDatabase();
        //         await db.UpdateAsync(client.ClientId, client);
        //         ReplaceItem(arg, client);
        //     },
        //     "Изменить клиента"
        // ).ShowDialog(_clientView);
        throw new NotImplementedException();
    }

    private void ReplaceItem(Service prevItem, Service newItem) {
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
        // await new EditClientDialog(
        //     new Client(),
        //     async client =>
        //     {
        //         await using var db = new MyDatabase();
        //         int newItemId = Convert.ToInt32(await db.InsertAsync(client));
        //         client.ClientId = newItemId;
        //         _itemsFull.Add(client);
        //     },
        //     "Добавить клиента"
        // ).ShowDialog(_clientView);
        throw new NotImplementedException();
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