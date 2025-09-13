using RepositoryContracts;

namespace CLI.UI.ManagePosts;
/// <summary>
/// View a specific post: title, body, and all comments on it (with usernames)
/// </summary>
public class SinglePostView
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;
    private readonly IUserRepository _users;

    public SinglePostView(IPostRepository posts, ICommentRepository comments, IUserRepository users)
        {
        _posts = posts;
        _comments = comments;
        _users = users;
        }

    public async Task RunAsync()
    {
        Console.WriteLine("Viewing single post");
        var postId = ReadInt("Post ID: ");
        
        var post = await _posts.GetSingleAsync(postId);
        if (post is null)
        {
            Console.WriteLine("Post not found");
            return;
        }
        
        Console.WriteLine($"\nTitle: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        
        //Use GetMany() to filter comments by PostId
        var comments = _comments.GetMany()
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.Id)
            .ToList();
        Console.WriteLine("\nComments:");
        if (!comments.Any())
        {
            Console.WriteLine("No comments found");
            return;
        }
        
        //Print each comment with author's username
        foreach (var c in comments)
        {
            var author = await _users.GetSingleAsync(c.UserId);
            var name = author?.Username ?? $"User {c.UserId}";
            Console.WriteLine($"- {name}: {c.Body}");
        }
    }

    private int ReadInt(string label)
    {
        while (true)
        {
            Console.Write(label);
            if (int.TryParse(Console.ReadLine(), out var input))
            {
                return input;
            }
            Console.WriteLine("Enter a valid integer");
        }
    }
}