using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbTools;
using MySqlConnector;

namespace PZ23.Models; 
[Table("execution")]
public class Execution {
        [Key]
        [Column("execution_id")]
        [DbType(MySqlDbType.Int32)]
        public int ExecutionId { get; set; }
        [DbType(MySqlDbType.Int32)]
        [Column("executor_id")]
        public int ExecutorId { get; set; }

        public Executor? Executor { get; set; }
        [DbType(MySqlDbType.Int32)]
        [Column("request_id")]
        public int RequestId { get; set; }

        public Request? Request { get; set; }
}