using DbTools;
using MySqlConnector;

namespace PZ23; 

public class MyDatabase : Database {

    public static MySqlConnectionStringBuilder ConnectionStringBuilder = new MySqlConnectionStringBuilder() {
        Server = "10.10.1.24",
        Database = "pro1_23",
        UserID = "user_01",
        Password = "user01pro"
        
        // Server = "localhost",
        // Database = "pro1_23",
        // UserID = "dev",
        // Password = "devPassword"
    };
    
    public MyDatabase() : base(ConnectionStringBuilder.ConnectionString) { }
}