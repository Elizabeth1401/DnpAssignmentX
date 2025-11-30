using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public class HttpCommentService  : ICommentService
{
    private readonly HttpClient _httpClient;
    
    public HttpCommentService(HttpClient httpClient)
        {
        _httpClient = httpClient;
        }

    public async Task AddCommentAsync(CommentDTO request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("Comments", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task<List<Comment>> GetAllComments()
    {
        List<Comment> response = await _httpClient.GetFromJsonAsync<List<Comment>>("Comments");
        return response;
    }
}