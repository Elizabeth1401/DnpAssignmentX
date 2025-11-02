using ApiContracts.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController] //Enables automatic validation and consistent API responses
[Route("[controller]")] //Replaces [controller] with the class name minus "Controller"-> /api/users
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    //Constructor dependency injection
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDTO>>> GetUsers()
    {
        var users = _userRepository.GetMany();
        if (!users.Any())
            return NotFound();
        return Ok(users);
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser([FromBody] CreateUserDto request)
    {
        await VerifyUserNameIsAvailableAsync(request.Username);

        User user = new(request.Username, request.Password);
        await _userRepository.AddAsync(user);
        return Created();
    }

    [HttpDelete]
    public async Task<ActionResult<UserDTO>> DeleteUser(int id)
    {
        var deleteUser = await _userRepository.DeleteAsync(id);
        if (deleteUser == null)
        {
            return NotFound($"User with id {id} not found");
        }
        return Ok(deleteUser);
    }
    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
       var currentUsersInTheDatabase = _userRepository.GetMany();
       foreach (var currentUser in currentUsersInTheDatabase)
       { if (currentUser.Username == userName)
               {
                  throw new InvalidOperationException($"User {userName} already exists");
               }
       }
    }
}