using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("clients")]
public class Client {
    [Key]
    [Column("client_id")]
    [DbType(MySqlDbType.Int32)]
    public int ClientId { get; set; }

    [DbType(MySqlDbType.VarChar)]
    [Column("last_name")]
    [DisplayName("Фамилия")]
    public string LastName { get; set; } = string.Empty;
    [DbType(MySqlDbType.VarChar)]
    [Column("first_name")]
    [DisplayName("Имя")]
    public string FirstName { get; set; } = string.Empty;
    [DbType(MySqlDbType.VarChar)]
    [Column("middle_name")]
    [DisplayName("Отчество")]
    public string MiddleName { get; set; } = string.Empty;
}