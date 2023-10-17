using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using PZ23.Models;

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
        set => SetField(ref _isSortByDescending, value);
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if(value == _searchQuery) return;
            _searchQuery = value;
            RaisePropertyChanging();
        }
    }

    public BindingList<Client> Items
    {
        get => _items;
        set => SetField(ref _items, value);
    }

    public int Take
    {
        get => _take;
        set => SetField(ref _take, value);
    }

    public int Skip
    {
        get => _skip;
        set
        {
            if (value >= _itemsFull.Count)
            {
                return;
            }

            if (!SetField(ref _skip, value))
            {
                return;
            }

            CurrentPage = (int)Math.Ceiling(value / (double)Take) + 1;
        }
    }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (!SetField(ref _currentPage, value))
            {
                return;
            }
            
            ReevaluateCommands();
        }
    }

    public int TotalPages => (int)Math.Ceiling(Filtered.Count / (double)Take);

    public List<Client> Filtered
    {
        get => _filtered;
        set
        {
            if (SetField(ref _filtered, value))
            {
                TakeFirst();
                RaisePropertyChanged(nameof(TotalPages));
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public Client SelectedRow
    {
        get => _selectedRow;
        set
        {
            if (!SetField(ref _selectedRow, value))
            {
                return;
            }
            RemoveItemCommand.RaiseCanExecuteChanged(null, EventArgs.Empty);
            EditItemCommand.RaiseCanExecuteChanged(null, EventArgs.Empty);
        }
    }
    
    #endregion
    
    public Command<Client> EditItemCommand { get; }
    public Command<Client> RemoveItemCommand { get; }
    public ICommand NewItemCommand { get; }
    public Command TakeNextCommand { get; }
    public Command TakePrevCommand { get; }
    public Command TakeFirstCommand { get; }
    public Command TakeLastCommand { get; }

    public ClientViewModel(Window view)
    {
        _clientView = view;
        EditItemCommand = new Command<Client>(EditItem, client => client is not null);
        RemoveItemCommand = new Command<Client>(RemoveItem, client => client is not null);
        NewItemCommand = new AsyncCommand(NewItem);
        GetDataFromDb();
        TakeNextCommand = new Command(TakeNext, () => CurrentPage < TotalPages);
        TakePrevCommand = new Command(TakePrev, () => CurrentPage > 1);
        TakeFirstCommand = new Command(TakeFirst, () => CurrentPage > 1);
        TakeLastCommand = new Command(TakeLast, () => CurrentPage < TotalPages);
        PropertyChanged += OnSearchChanged;
    }

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
            var users = db.GetAsync<Client>();
            var list = await users.ToListAsync();
            list = list.Select(it =>
            {
                it
            })
        })
    }
}