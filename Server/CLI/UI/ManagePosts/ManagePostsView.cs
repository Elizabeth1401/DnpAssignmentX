using RepositoryContracts;

namespace CLI.UI.ManagePosts;
/// <summary>
/// Posts submenu (create/overview/single/add-comment)
/// </summary>
public class ManagePostsView
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;
    private readonly IUserRepository _users;

    public ManagePostsView(IPostRepository posts, ICommentRepository comments, IUserRepository users)
    {
        _posts = posts;
        _comments = comments;
        _users = users;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\n===POSTS===");
            Console.WriteLine("1) Create new post");
            Console.WriteLine("2) View posts overview");
            Console.WriteLine("3) View single post");
            Console.WriteLine("4) Add new comment");
            Console.WriteLine("0) Back to the main menu");
            Console.Write("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await new CreatePostView(_posts,_users).RunAsync();
                    break;
                case "2":
                    await new ListPostsView(_posts).RunAsync();
                    break;
                case "3":
                    await new SinglePostView(_posts,_comments,_users).RunAsync();
                    break;
                case "4":
                    await new AddCommentView(_comments,_posts,_users).RunAsync();
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