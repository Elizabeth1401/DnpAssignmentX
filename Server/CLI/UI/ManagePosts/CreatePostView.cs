using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;
/// <summary>
/// Create a new post (title, body, userId). Validates user exists.
/// </summary>
public class CreatePostView
{
    private readonly IPostRepository _posts;
    private readonly IUserRepository _users;
    
    public CreatePostView(IPostRepository posts, IUserRepository users)
        {
        _posts = posts;
        _users = users;
        }

    public async Task RunAsync()
    {
        Console.WriteLine("Create Post");
        var title = ReadRequired("Title: ");
        var body = ReadRequired("Body: ");
        var userId = ReadInt("UserId: ");
        
        //Check user existence using my API
        var user = await _users.GetSingleAsync(userId);
        if (user is null)
        {
            Console.WriteLine("User not found");
            return;
        }
        
        var post = new Post { Title = title, Body = body, UserId = userId};
        await _posts.AddAsync(post);
        Console.WriteLine($"Post created with id {post.Id}");
    }

    private static string ReadRequired(string lable)
    {
        while (true)
        {
            Console.Write($"{lable}: ");
            var value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
            Console.WriteLine("Value is required");
        }
    }

    private static int ReadInt(string lable)
    {
        while (true)
        {
            Console.Write($"{lable}: ");
            if (int.TryParse(Console.ReadLine(), out var value))
            {
                return value;
            }
            Console.WriteLine("Enter a valid integer");
        }
    }

}