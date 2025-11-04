using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    //This list acts as our "database table" for Comments
    private readonly List<Comment> comments = new();

    public CommentInMemoryRepository()
    {
        SeedComments();
    }

    private void SeedComments()
    {
        comments.Add(new Comment { Id = 1, Body = "Nice post!", PostId = 1, UserId = 2 });
        comments.Add(new Comment { Id = 2, Body = "Thanks for sharing!", PostId = 1, UserId = 3 });
        comments.Add(new Comment { Id = 3, Body = "I agree with you.", PostId = 2, UserId = 1 });
    }
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
    public Task<Comment> DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(p => p.Id == id);
        if (commentToRemove == null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' does not exist in the database.");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask as Task<Comment>;
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

    public Task<bool> DoesCommentExistAsync(string body)
    {
        throw new NotImplementedException();
    }

    public Task<Comment> PatchAsync(int id, string body)
    {
        throw new NotImplementedException();
    }
}