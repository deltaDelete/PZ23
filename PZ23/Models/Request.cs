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

    [DbTools.ForeignKey("requests.gear_id", "gears.gear_id", "gears")]
    public Gear? Gear { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("failure_type_id")]
    public int FailureTypeId { get; set; }

    [DbTools.ForeignKey("requests.failure_type_id", "failure_types.failure_type_id", "failure_types")]
    public FailureType? Type { get; set; }
    [DbType(MySqlDbType.VarChar)]
    [Column("failure_description")]
    public string FailureDescription { get; set; } = String.Empty;
    [DbType(MySqlDbType.Int32)]
    [Column("client_id")]
    public int ClientId { get; set; }

    [DbTools.ForeignKey("requests.client_id", "clients.client_id", "clients")]
    public Client? Client { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("priority_id")]
    public int PriorityId { get; set; }

    [DbTools.ForeignKey("requests.priority_id", "priorities.priority_id", "priorities")]
    public Priority? Priority { get; set; }
    [DbType(MySqlDbType.Int32)]
    [Column("request_status_id")]
    public int RequestStatusId { get; set; }

    [DbTools.ForeignKey("requests.request_status_id", "request_statuses.request_status_id", "request_statuses")]
    public RequestStatus? RequestStatus { get; set; }
}