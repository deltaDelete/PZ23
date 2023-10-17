using DbTools;
using MySqlConnector;

namespace PZ23; 

public class MyDatabase : Database {

    public static MySqlConnectionStringBuilder ConnectionStringBuilder = new MySqlConnectionStringBuilder() {
        Server = "10.10.1.24",
        Database = "pro1_23",
        UserID = "user01",
        Password = "user01pro"
    };
    
    public MyDatabase() : base(ConnectionStringBuilder.ConnectionString) { }
}