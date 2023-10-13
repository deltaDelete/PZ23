using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("gears")]
public class Gear {
    [Key]
    [Column("gear_id")]
    [DbType(MySqlDbType.Int32)]
    public int GearId { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("gear_name")]
    public string GearName { get; set; } = string.Empty;
}