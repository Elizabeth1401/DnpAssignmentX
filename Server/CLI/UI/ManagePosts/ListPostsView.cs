using RepositoryContracts;

namespace CLI.UI.ManagePosts;
/// <summary>
/// Overview = show [id] title for each post.
/// </summary>
public class ListPostsView
{
    private readonly IPostRepository _posts;
    
    public ListPostsView(IPostRepository posts)
        {
        _posts = posts;
        }

    public Task RunAsync()
    {
        Console.WriteLine("Posts Overview");
        var items = _posts.GetMany().OrderBy(p => p.Id).Select(p => new { p.Id, p.Title }).ToList();
        foreach (var p in items)
        {
            Console.WriteLine($"{p.Id} - {p.Title}");
        }

        if (!items.Any())
        {
            Console.WriteLine("No posts found");
        }
        return Task.CompletedTask;
    }

}