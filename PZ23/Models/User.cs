using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models;

[Table("users")]
public class User {
    [Key]
    [Column("user_id")]
    [DbType(MySqlDbType.Int32)]
    public int Id { get; set; }

    [Column("username")]
    [DbType(MySqlDbType.VarChar)]
    public string Username { get; set; } = string.Empty;

    [Column("password")]
    [DbType(MySqlDbType.VarChar)]
    public string Password { get; set; } = string.Empty;
}