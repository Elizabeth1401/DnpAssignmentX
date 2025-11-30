using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public interface IUserService
{
    public Task AddUserAsync(CreateUserDto request);
    public Task <List<User>> GetAllUsers();
    public Task<User> GetUserById(int id);
}