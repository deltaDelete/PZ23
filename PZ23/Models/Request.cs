using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 

[Table("requests")]
public class Request {
    [Key]
    [Column("request_id")]
    [DbType(MySqlDbType.Int32)]
    public int RequestId { get; set; }
    [DbType(MySqlDbType.Date)]
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("gear_id")]
    public int GearId { get; set; }

    public Gear? Gear { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("failure_type_id")]
    public int FailureTypeId { get; set; }

    public FailureType? Type { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("failure_description")]
    public string FailureDescription { get; set; } = String.Empty;
    [DbType(MySqlDbType.Int32)]
    [Column("client_id")]
    public int ClientId { get; set; }

    public Client? Client { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("priority_id")]
    public int PriorityId { get; set; }

    public Priority? Priority { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("request_status_id")]
    public int RequestStatusId { get; set; }

    public RequestStatus? RequestStatus { get; set; }
}