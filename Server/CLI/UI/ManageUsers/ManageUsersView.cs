using RepositoryContracts;

namespace CLI.UI.ManageUsers;
/// <summary>
/// Users submenu (create/list).
/// </summary>
public class ManageUsersView
{
    private readonly IUserRepository _users;
    public ManageUsersView(IUserRepository users)
    {
        _users = users;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\n===USERS===");
            Console.WriteLine("1) Create new user");
            Console.WriteLine("2) List users");
            Console.WriteLine("0) Back to Main menu");
            Console.WriteLine("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await new CreateUserView(_users).RunAsync();
                    break;
                case "2":
                    await new ListUsersView(_users).RunAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }
}