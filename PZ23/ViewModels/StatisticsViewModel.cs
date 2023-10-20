using System.ComponentModel;
using ReactiveUI.Fody.Helpers;

namespace PZ23.ViewModels;

public class StatisticsViewModel : ViewModelBase {
    [Reactive] public BindingList<TypeAmount> TypeDistribution { get; set; } = new BindingList<TypeAmount>();

    public StatisticsViewModel() {
        InitTypeDistribution();
    }

    private async void InitTypeDistribution() {
        await using var db = new MyDatabase();
        await using var reader = await db.ExecuteReaderAsync(
            """
                select ft.failure_type_name as type, count(ft.failure_type_name) as amount
                from requests
                join failure_types ft on requests.failure_type_id = ft.failure_type_id
                group by requests.failure_type_id;
            """
        );
        while (await reader.ReadAsync()) {
            TypeDistribution.Add(new(
                reader.GetString("type"),
                reader.GetInt32("amount")
                ));
        }
    }
}

public class TypeAmount {
    public string Name { get; set; }
    public int Amount { get; set; }

    public TypeAmount(string name, int amount) {
        Name = name;
        Amount = amount;
    }
}