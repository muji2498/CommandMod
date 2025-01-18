using System;

namespace CommandMod;

[Flags]
public enum Roles
{
    None = 0,
    Owner = 1,
    Admin = 2,
    Moderator = 4,
}