using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    //This list acts as our "database table" for Comments
    private readonly List<Comment> comments = new();
    
    
    /// <summary>
    /// Add a new Comment. Sets a new Id and returns the created Comment.
    /// </summary>
    public Task<Comment> AddAsync(Comment comment)
    {
        // Generate new Id (auto-increment style)
        comment.Id = comments.Any() ? comments.Max(p => p.Id) + 1 : 1;
        
        //Add to in-memory "database"
        comments.Add(comment);
        
        //Return wrapped in a Task
        return Task.FromResult(comment);
    }
    
    
    /// <summary>
    /// Update an existing Comment. Throws exception if not found.
    /// </summary>
    public Task UpdateAsync(Comment comment)
    {
        //Try to find existing Comment with the same Id
        Comment? existingComment = comments.SingleOrDefault(p => p.Id == comment.Id);
        if (existingComment == null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' does not exist in the database.");
        }
        
        //Replace by removing and adding again
        comments.Remove(existingComment);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    
    /// <summary>
    /// Delete a Comment by Id. Throws exception if not found.
    /// </summary>
    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(p => p.Id == id);
        if (commentToRemove == null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' does not exist in the database.");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    
    /// <summary>
    /// Get a single Comment by Id. Throws exception if not found
    /// </summary>
    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(p => p.Id == id);
        if (comment == null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' does not exist in database.");
        }

        return Task.FromResult(comment);
    }

    
    /// <summary>
    /// Get all Comments as IQueryable for filtering with LINQ.
    /// </summary>
    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}