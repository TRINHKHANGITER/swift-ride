using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Domain;

public enum Role
{
    Rider, Driver
}

public sealed class Account
{
    [Key]
    public Guid Uuid { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Rider; 
}
