using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("groups")]
public class Group {
    [Key]
    [Column("group_id")]
    [DbType(MySqlDbType.Int32)]
    public int Id { get; set; }

    [Column("group_name")]
    [DbType(MySqlDbType.VarChar)]
    public string Name { get; set; } = string.Empty;

    [Column("permissions")]
    [DbType(MySqlDbType.VarChar)]
    public Permissions Permissions { get; set; }
}