using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient _httpClient;

    public HttpPostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task CreatePostAsync(CreatePostDTO request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("Posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        List<Post> response = await _httpClient.GetFromJsonAsync<List<Post>>("Posts");
        return response;
    }
}