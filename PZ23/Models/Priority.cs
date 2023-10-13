using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("priorities")]
public class Priority {
    [Key]
    [Column("priority_id")]
    [DbType(MySqlDbType.Int32)]
    public int PriorityId { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("priority_name")]
    public string PriorityName { get; set; } = string.Empty;
}