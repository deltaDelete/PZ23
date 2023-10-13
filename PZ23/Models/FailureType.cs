using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("failure_types")]
public class FailureType {
    [Key]
    [Column("failure_type_id")]
    [DbType(MySqlDbType.Int32)]
    public int FailureTypeId { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("failure_type_name")]
    public string FailureTypeName { get; set; } = string.Empty;
}