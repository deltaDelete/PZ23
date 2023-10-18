using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PZ23.Models;
using ReactiveUI;

namespace PZ23.ViewModels; 

public class ClientViewModel : ViewModelBase
{
    private string _searchQuery = string.Empty;
    private BindingList<Client> _items = new();
    private List<Client> _itemsFull = null!;
    private int _selectedSearchColumn;
    private bool _isSortByDescending = false;
    private int _take = 10;
    private int _skip = 0;
    private int _currentPage = 1;
    private List<Client> _filtered = new List<Client>();
    private bool _isLoading = true;
    private Client? _selectedRow;

    #region Notifying Properties

    public int SelectedSearchColumn
    {
        get => _selectedSearchColumn;
        set
        {
            this.RaisePropertyChanging();
            if(value == _selectedSearchColumn) return;
            _selectedSearchColumn = value;
            this.RaisePropertyChanging();
        }
    }

    public bool IsSortByDescending
    {
        get => _isSortByDescending;
        set => this.RaiseAndSetIfChanged(ref _isSortByDescending, value);
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if(value == _searchQuery) return;
            _searchQuery = value;
            this.RaisePropertyChanging();
        }
    }

    public BindingList<Client> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public int Take
    {
        get => _take;
        set => this.RaiseAndSetIfChanged(ref _take, value);
    }

    public int Skip
    {
        get => _skip;
        set => this.RaiseAndSetIfChanged(ref _skip, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public int TotalPages => (int)Math.Ceiling(Filtered.Count / (double)Take);

    public List<Client> Filtered
    {
        get => _filtered;
        set => this.RaiseAndSetIfChanged(ref _filtered, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public Client? SelectedRow
    {
        get => _selectedRow;
        set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
    }
    
    #endregion
    
    public ReactiveCommand<Client, Unit> EditItemCommand { get; }
    public ReactiveCommand<Client, Unit> RemoveItemCommand { get; }
    public ReactiveCommand<Unit, Unit> NewItemCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeNextCommand { get; }
    public ReactiveCommand <Unit, Unit> TakePrevCommand { get; }
    public ReactiveCommand <Unit, Unit> TakeFirstCommand { get; }
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
            : tuple.column switch
            {
                1 => _itemsFull
                    .Where(it => it.ClientId.ToString().Contains(tuple.query)),
                2 => _itemsFull
                    .Where(it => it.LastName.ToLower().Contains(tuple.query.ToLower())),
                3 => _itemsFull
                    .Where(it => it.FirstName.ToLower().Contains(tuple.query.ToLower())),
                4 => _itemsFull
                    .Where(it => it.MiddleName.ToLower().Contains(tuple.query.ToLower()))
            };

        Filtered = tuple.column switch
        {
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

    private async void GetDataFromDb()
    {
        await Task.Run(async () =>
        {
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

    private async void RemoveItem(Client? arg)
    {
        if(arg is null) return;
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

    private void RemoveLocal(Client arg)
    {
        Items.Remove(arg);
        _itemsFull.Remove(arg);
        Filtered.Remove(arg);
    }

    private async void EditItem(Client? arg)
    {
        if(arg is null) return;
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

    private void ReplaceItem(Client prevItem, Client newItem)
    {
        if (Filtered.Contains(prevItem))
        {
            var index = Filtered.IndexOf(prevItem);
            Filtered[index] = newItem;
        }

        if (_itemsFull.Contains(prevItem))
        {
            var index = _itemsFull.IndexOf(prevItem);
            _itemsFull[index] = newItem;
        }

        if (Items.Contains(prevItem))
        {
            var index = _itemsFull.IndexOf(prevItem);
            Items[index] = newItem;
        }
    }

    private async Task NewItem()
    {
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

    private void TakeNext()
    {
        Skip += Take;
        Items = new(
            Filtered.Skip(Skip).Take(Take).ToList()
            );
    }

    private void TakePrev()
    {
        Skip -= Take;
        Items = new(
            Filtered.Skip(Skip).Take(Take).ToList()
            );
    }

    private void TakeFirst()
    {
        Skip = 0;
        Items = new(
            Filtered.Take(Take).ToList()
            );
    }

    private void TakeLast()
    {
        Skip = Filtered.Count - Take;
        Items = new(
            Filtered.TakeLast(Take).ToList()
        );
    }
}