using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    //This list acts as our "database table" for Posts
    private readonly List<User> users = new();

    public UserInMemoryRepository()
    {
        SeedUsers();
    }
    private void SeedUsers()
    {
        users.Add(new User {Id = 1, Username = "alice", Password = "pass123"});
        users.Add(new User {Id = 2, Username = "bob", Password = "qwerty"});
        users.Add(new User {Id = 3, Username = "charlie", Password = "123456"});
    }
    /// <summary>
    /// Add a new User. Sets a new Id and returns the created User.
    /// </summary>
    public Task<User> AddAsync(User user)
    {
        // Generate new Id (auto-increment style)
        user.Id = users.Any() ? users.Max(p => p.Id) + 1 : 1;
        
        //Add to in-memory "database"
        users.Add(user);
        
        //Return wrapped in a Task
        return Task.FromResult(user);
    }
    
    
    /// <summary>
    /// Update an existing User. Throws exception if not found.
    /// </summary>
    public Task UpdateAsync(User user)
    {
        //Try to find existing User with the same Id
        User? existingUser = users.SingleOrDefault(p => p.Id == user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.Id}' does not exist in the database.");
        }
        
        //Replace by removing and adding again
        users.Remove(existingUser);
        users.Add(user);
        return Task.CompletedTask;
    }

    
    /// <summary>
    /// Delete a PUser by Id. Throws exception if not found.
    /// </summary>
    public Task<User> DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(p => p.Id == id);
        if (userToRemove == null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' does not exist in the database.");
        }

        users.Remove(userToRemove);
        return Task.CompletedTask as Task<User>;
    }

    
    /// <summary>
    /// Get a single User by Id. Throws exception if not found
    /// </summary>
    public Task<User> GetSingleAsync(int id)
    {
        User? user = users.SingleOrDefault(p => p.Id == id);
        if (user == null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' does not exist in database.");
        }

        return Task.FromResult(user);
    }

    
    /// <summary>
    /// Get all Users as IQueryable for filtering with LINQ.
    /// </summary>
    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }

    public Task<bool> DoesUsernameExistAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<User> PatchAsync(int id, string password)
    {
        throw new NotImplementedException();
    }
}