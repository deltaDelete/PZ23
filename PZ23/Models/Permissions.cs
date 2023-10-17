using System;

namespace PZ23.Models; 

[Flags]
public enum Permissions {
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    Insert = 8
}