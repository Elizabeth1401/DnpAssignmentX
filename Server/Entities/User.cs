namespace Entities;

public class User
{
    public int Id { get; set; } //PK
    public string Username { get; set; } //required
    public string Password { get; set; } //required
}