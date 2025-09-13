using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;

namespace CLI.UI;

/// <summary>
/// Entry point for the console UI.
/// Shows the main menu and routes to sub-menus(users/posts)
/// </summary>
public class CliApp
{
    // Repositories are injected (DI). Program.cs is the ONLY place that creates them.
    private readonly IUserRepository _users;
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;

    public CliApp(IUserRepository users, IPostRepository posts, ICommentRepository comments)
    {
        _users = users;
        _posts = posts;
        _comments = comments;
    }
    // Main loop. Blocks untile user chooses Exit.
    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("Welcome to the CLI server!");
            Console.WriteLine("1) Manage Users");
            Console.WriteLine("2) Manage Posts");
            Console.WriteLine("0) Exit ");
            Console.Write("Choose an option:");

            switch (Console.ReadLine())
            {
                case "1":
                    await new ManageUsersView(_users).RunAsync();
                    break;
                case "2":
                    await new ManagePostsView(_posts, _comments, _users).RunAsync();
                    break;
                case "0":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    break;
                
            }
        }
    }
}