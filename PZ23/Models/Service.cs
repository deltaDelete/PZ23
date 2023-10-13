using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("services")]
public class Service {
    [Key]
    [Column("service_id")]
    [DbType(MySqlDbType.Int32)]
    public int ServiceId { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("service_name")]
    public string ServiceName { get; set; } = string.Empty;
    [DbType(MySqlDbType.Decimal)]
    [Column("service_price")]
    public decimal ServicePrice { get; set; }
}