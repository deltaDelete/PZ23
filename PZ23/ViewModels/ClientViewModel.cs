using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using FluentAvalonia.Core;
using PZ23.Models;
using ReactiveUI;

namespace PZ23.ViewModels; 

public class ClientViewModel : ViewModelBase
{
    private readonly Window _clientView;
    private string _searchQuery = string.Empty;
    private BindingList<Client> _items = new();
    private List<Client> _itemsFull;
    private int _selectedSearchColumn;
    private bool _isSortByDescending = false;
    private int _take = 10;
    private int _skip = 0;
    private int _currentPage = 1;
    private List<Client> _filtered = new List<Client>();
    private bool _isLoading = true;
    private Client _selectedRow;

    public ClientViewModel()
    {
        var canExecute1 = this.WhenAnyValue(x => x.CurrentPage, selector: it => it < TotalPages);
        var canExecute2 = this.WhenAnyValue(x => x.CurrentPage, selector: it => it > 1);
        var canExecute3 = this.WhenAnyValue(x => x.CurrentPage, x => x.TotalPages, selector: (i1, i2) => i1 < i2);

        TakeNextCommand = ReactiveCommand.Create(TakeNext, canExecute1);
        TakePrevCommand = ReactiveCommand.Create(TakePrev, canExecute2);
        TakeFirstCommand = ReactiveCommand.Create(TakeFirst, canExecute2);
        TakeLastCommand = ReactiveCommand.Create(TakeLast, canExecute3);
        EditItemCommand = ReactiveCommand.Create(EditItem, );
    }

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

    public Client SelectedRow
    {
        get => _selectedRow;
        set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
    }
    
    #endregion
    
    public ReactiveCommand<Unit, Unit> EditItemCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveItemCommand { get; }
    public ICommand NewItemCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeNextCommand { get; }
    public ReactiveCommand <Unit, Unit> TakePrevCommand { get; }
    public ReactiveCommand <Unit, Unit> TakeFirstCommand { get; }
    public ReactiveCommand<Unit, Unit> TakeLastCommand { get; }

    private void OnSearchChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName !=nameof(SearchQuery) && e.PropertyName!=nameof(SelectedSearchColumn) && e.PropertyName!=nameof(IsSortByDescending))
        {
            return;
        }

        var filtered = SearchQuery == ""
            ? _itemsFull
            : SelectedSearchColumn switch
            {
                1 => _itemsFull
                    .Where(it => it.ClientId.ToString().Contains(SearchQuery)),
                2 => _itemsFull
                    .Where(it => it.LastName.ToLower().Contains(SearchQuery.ToLower())),
                3 => _itemsFull
                    .Where(it => it.FirstName.ToLower().Contains(SearchQuery.ToLower())),
                4 => _itemsFull
                    .Where(it => it.MiddleName.ToLower().Contains(SearchQuery.ToLower()))
            };

        Filtered = SelectedSearchColumn switch
        {
            2 => IsSortByDescending
                ? filtered.OrderByDescending(it => it.LastName).ToList()
                : filtered.OrderBy(it => it.LastName).ToList(),
            3 => IsSortByDescending
                ? filtered.OrderByDescending(it => it.FirstName).ToList()
                : filtered.OrderBy(it => it.FirstName).ToList(),
            4 => IsSortByDescending
                ? filtered.OrderByDescending(it => it.MiddleName).ToList()
                : filtered.OrderBy(it => it.MiddleName).ToList(),
            _ => IsSortByDescending
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
            var users = db.GetAsync<Client>().ToListAsync();
            IsLoading = false;
            Dispatcher.UIThread.Invoke(ReevaluateCommands);
            SearchQuery = string.Empty;
            return Task.CompletedTask;
        });
    }

    private async void RemoveItem(Client? arg)
    {
        if(arg is null) return;
        await new ConfirmationDialog(
            "Вы собираетесь удалить строку",
            $"Пользователь: {arg.LastName} {arg.FirstName} {arg.MiddleName}",
            async dialog =>
            {
                await using var db = new MyDatabase();
                await db.RemoveAsync(arg);
                RemoveLocal(arg);
            },
            dialog => { }
        ).ShowDialog(_clientView);
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
        await new EditClientDialog(
            arg,
            async client =>
            {
                await using var db = new MyDatabase();
                await db.UpdateAsync(client.ClientId, client);
                ReplaceItem(arg, client);
            },
            "Изменить клиента"
        ).ShowDialog(_clientView);
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
        await new EditClientDialog(
            new Client(),
            async client =>
            {
                await using var db = new MyDatabase();
                int newItemId = await db.InsertAsync(client);
                client.ClientId = newItemId;
                _itemsFull.Add(client);
            },
            "Добавить клиента"
        ).ShowDialog(_clientView);
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

    private void ReevaluateCommands()
    {
        TakeFirstCommand.RaiseCanExecuteChanged(null, EventArgs.Empty);
        TakePrevCommand.RaiseCanExecuteChanged(null, EventArgs.Empty);
        TakeNextCommand.RaiseCanExecuteChanged(null, EventArgs.Empty);
        TakeLastCommand.RaiseCanExecuteChanged(null, EventArgs.Empty);
    }
}