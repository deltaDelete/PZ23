using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("executors")]
public class Executor {
    [Key]
    [Column("executor_id")]
    [DbType(MySqlDbType.Int32)]
    public int ExecutorId { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;
    [DbType(MySqlDbType.VarChar)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;
    [DbType(MySqlDbType.VarChar)]
    [Column("middle_name")]
    public string MiddleName { get; set; } = string.Empty;
}