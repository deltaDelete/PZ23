using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("request_services")]
public class RequestService {
    [Key]
    [Column("request_services_id")]
    [DbType(MySqlDbType.Int32)]
    public int RequestServiceId { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("request_id")]
    public int RequestId { get; set; }

    public Request? Request { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("service_id")]
    public int ServiceId { get; set; }

    public Service? Service { get; set; }
}