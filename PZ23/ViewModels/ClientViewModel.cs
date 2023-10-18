using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PZ23.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PZ23.ViewModels;

public class ClientViewModel : ViewModelBase {
    private List<Client> _itemsFull = null!;

    #region Notifying Properties

    [Reactive] public int SelectedSearchColumn { get; set; }

    [Reactive] public bool IsSortByDescending { get; set; } = false;

    [Reactive] public string SearchQuery { get; set; } = string.Empty;

    [Reactive] public BindingList<Client> Items { get; set; } = new();

    [Reactive] public int Take { get; set; } = 10;

    [Reactive] public int Skip { get; set; } = 0;

    [Reactive] public int CurrentPage { get; set; } = 1;

    public int TotalPages => (int)Math.Ceiling(Filtered.Count / (double)Take);

    [Reactive] public List<Client> Filtered { get; set; } = new List<Client>();

    [Reactive] public bool IsLoading { get; set; } = true;

    [Reactive] public Client? SelectedRow { get; set; } = null!;

    #endregion

    public ReactiveCommand<Client, Unit> EditItemCommand { get; }
    public ReactiveCommand<Client, Unit> RemoveItemCommand { get; }
    public ReactiveCommand<Unit, Unit> NewItemCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeNextCommand { get; }
    public ReactiveCommand<Unit, Unit> TakePrevCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeFirstCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeLastCommand { get; }

    public ClientViewModel() {
        var canExecute1 = this.WhenAnyValue(x => x.CurrentPage, selector: it => it < TotalPages);
        var canExecute2 = this.WhenAnyValue(x => x.CurrentPage, selector: it => it > 1);
        var canExecute3 = this.WhenAnyValue(x => x.CurrentPage, x => x.TotalPages, selector: (i1, i2) => i1 < i2);

        var canExecute4 = this.WhenAnyValue(x => x.SelectedRow, selector: client => client is not null);

        TakeNextCommand = ReactiveCommand.Create(TakeNext, canExecute1);
        TakePrevCommand = ReactiveCommand.Create(TakePrev, canExecute2);
        TakeFirstCommand = ReactiveCommand.Create(TakeFirst, canExecute2);
        TakeLastCommand = ReactiveCommand.Create(TakeLast, canExecute3);
        EditItemCommand = ReactiveCommand.Create<Client>(EditItem, canExecute4);
        RemoveItemCommand = ReactiveCommand.Create<Client>(RemoveItem, canExecute4);
        NewItemCommand = ReactiveCommand.CreateFromTask(NewItem);

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
                    .Where(it => it.ClientId.ToString().Contains(tuple.query)),
                2 => _itemsFull
                    .Where(it => it.LastName.ToLower().Contains(tuple.query.ToLower())),
                3 => _itemsFull
                    .Where(it => it.FirstName.ToLower().Contains(tuple.query.ToLower())),
                4 => _itemsFull
                    .Where(it => it.MiddleName.ToLower().Contains(tuple.query.ToLower()))
            };

        Filtered = tuple.column switch {
            2 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.LastName).ToList()
                : filtered.OrderBy(it => it.LastName).ToList(),
            3 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.FirstName).ToList()
                : filtered.OrderBy(it => it.FirstName).ToList(),
            4 => tuple.isDescending
                ? filtered.OrderByDescending(it => it.MiddleName).ToList()
                : filtered.OrderBy(it => it.MiddleName).ToList(),
            _ => tuple.isDescending
                ? filtered.OrderByDescending(it => it.ClientId).ToList()
                : filtered.OrderBy(it => it.ClientId).ToList()
        };
    }

    private async void GetDataFromDb() {
        await Task.Run(async () => {
            IsLoading = true;
            await using var db = new MyDatabase();
            var list = await db.GetAsync<Client>().ToListAsync();
            _itemsFull = list;
            Filtered = _itemsFull;
            IsLoading = false;
            SearchQuery = string.Empty;
            return Task.CompletedTask;
        });
    }

    private async void RemoveItem(Client? arg) {
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

    private void RemoveLocal(Client arg) {
        Items.Remove(arg);
        _itemsFull.Remove(arg);
        Filtered.Remove(arg);
    }

    private async void EditItem(Client? arg) {
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

    private void ReplaceItem(Client prevItem, Client newItem) {
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