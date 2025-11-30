using System.Text.Json;
using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient _httpClient;

    public HttpUserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("Users", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task<List<User>> GetAllUsers()
    { 
        List<User> response = await _httpClient.GetFromJsonAsync<List<User>>("Users");
        return response;
    }

    public async Task<User> GetUserById(int id)
    {
       User user =  await _httpClient.GetFromJsonAsync<User>("Users/" + id);
       return user;
    }
}