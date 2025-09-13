using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;
/// <summary>
/// Create a new user (username+password)
/// </summary>
public class CreateUserView
{
    private readonly IUserRepository _users;
    public CreateUserView(IUserRepository users)
    { 
        _users = users;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Creating user");
        var username = ReadRequired("Username: ");
        var password = ReadRequired("Password: ");
        
        var user = new User {Username = username, Password = password};
        await _users.AddAsync(user); // interface
        
        Console.WriteLine($"User created with ID = {user.Id}");
    }

    private static string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write(label);
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s))
                return s.Trim();
            Console.WriteLine("Value is required");
        }
    }
}