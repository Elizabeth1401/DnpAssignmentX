using RepositoryContracts;

namespace CLI.UI.ManageUsers;
/// <summary>
/// List all users using GetMany()
/// </summary>
public class ListUsersView
{
    private readonly IUserRepository _users;
    public ListUsersView(IUserRepository users)
        {
        _users = users;
        }

    public Task RunAsync()
    {
        Console.WriteLine("Listing users");
        var item = _users.GetMany().OrderBy(u => u.Id).ToList(); //IQueryable -> enumerate
        foreach (var user in item)
        {
            Console.WriteLine($"[{user.Id}] {user.Username}");
        }
        if (!item.Any())
        {
            Console.WriteLine("No users found");
        }
        return Task.CompletedTask;
    }

}