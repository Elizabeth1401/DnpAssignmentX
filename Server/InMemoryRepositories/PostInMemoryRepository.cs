using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;


/// <summary>
/// In-memory implementation of IPostRepository.
/// Stores Posts inside a List Post instead of a real database.
/// </summary>
public class PostInMemoryRepository : IPostRepository
{
    //This list acts as our "database table" for Posts
    private readonly List<Post> posts = new();

    public PostInMemoryRepository()
    {
        SeedPosts();
    }

    private void SeedPosts()
    {
        posts.Add(new Post { Id = 1, Title = "Hello World", Body = "This is my first post!", UserId = 1 });
        posts.Add(new Post { Id = 2, Title = "Second Post", Body = "Repository pattern is working!", UserId = 2 });
    }
    /// <summary>
    /// Add a new Post. Sets a new Id and returns the created Post.
    /// </summary>
    public Task<Post> AddAsync(Post post)
    {
        // Generate new Id (auto-increment style)
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        
        //Add to in-memory "database"
        posts.Add(post);
        
        //Return wrapped in a Task
        return Task.FromResult(post);
    }
    
    
    /// <summary>
    /// Update an existing Post. Throws exception if not found.
    /// </summary>
    public Task UpdateAsync(Post post)
    {
        //Try to find existing Post with the same Id
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost == null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' does not exist in the database.");
        }
        
        //Replace by removing and adding again
        posts.Remove(existingPost);
        posts.Add(post);
        return Task.CompletedTask;
    }

    
    /// <summary>
    /// Delete a Post by Id. Throws exception if not found.
    /// </summary>
    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove == null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' does not exist in the database.");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    
    /// <summary>
    /// Get a single Post by Id. Throws exception if not found
    /// </summary>
    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post == null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' does not exist in database.");
        }

        return Task.FromResult(post);
    }

    
    /// <summary>
    /// Get all Posts as IQueryable for filtering with LINQ.
    /// </summary>
    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}