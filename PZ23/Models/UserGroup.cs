using System.ComponentModel.DataAnnotations.Schema;
using DbTools;

namespace PZ23.Models; 

[Table("user_groups")]
public class UserGroup {
    [DbTools.ForeignKey("user_groups.user_id", "users.user_id", "users")]
    public User? User { get; set; }
    [DbTools.ForeignKey("user_groups.group_id", "groups.group_id", "groups")]
    public Group? Group { get; set; }
}