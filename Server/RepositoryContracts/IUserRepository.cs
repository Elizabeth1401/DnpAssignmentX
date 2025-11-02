using Entities;

namespace RepositoryContracts;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task<User> DeleteAsync(int id);
    Task<User> GetSingleAsync(int id);
    IQueryable<User> GetMany();
    Task<bool> DoesUsernameExistAsync(string username);
    Task<User> PatchAsync(int id, string password);
}