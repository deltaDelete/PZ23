using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("request_statuses")]
public class RequestStatus {
    [Key]
    [Column("request_status_id")]
    [DbType(MySqlDbType.Int32)]
    public int RequestStatusId { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("request_status_name")]
    public string RequestStatusName { get; set; } = string.Empty;
}