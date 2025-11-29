using ApiContracts.DTOs;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient _httpClient;


    public Task<PostDTO> CreatePostAsync(CreatePostDTO request)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDTO>> GetAllPostsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PostDTO> GetPostByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}