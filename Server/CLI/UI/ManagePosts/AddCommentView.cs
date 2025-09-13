using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;
/// <summary>
/// Add comment to an existing post (validates user & post).
/// </summary>
public class AddCommentView
{
    private readonly ICommentRepository _comments;
    private readonly IUserRepository _users;
    private readonly IPostRepository _posts;
    
    public AddCommentView(ICommentRepository comments, IPostRepository posts, IUserRepository users)
        {
        _comments = comments;
        _posts = posts;
        _users = users;
        }

    public async Task RunAsync()
    {
        Console.WriteLine("Add Comment");
        
        var postId = ReadInt("PostId: ");
        var post = await _posts.GetSingleAsync(postId);
        
        if (post is null)
        {
            Console.WriteLine("Post not found");
            return;
        }
        
        var userId = ReadInt("UserId: ");
        var user = await _users.GetSingleAsync(userId);
        if (user is null)
        {
            Console.WriteLine("User not found");
        }
        
        var body = ReadRequired("Comment: ");
        
        var comment = new Comment { PostId = postId, UserId = userId, Body = body };
        await _comments.AddAsync(comment);
        Console.WriteLine("Comment added");
    }

    private string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write(label);
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Console.WriteLine("Value is required.");
        }

    }

    private int ReadInt(string label)
    {
        while (true)
        {
            Console.Write(label);
            if (int.TryParse(Console.ReadLine(), out var v)) return v;
            Console.WriteLine("Enter a valid integer.");
        }
    }
}