using ApiContracts.DTOs;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<PostDTO> CreatePostAsync(CreatePostDTO request);
    Task<IEnumerable<PostDTO>> GetAllPostsAsync();
    Task<PostDTO> GetPostByIdAsync(int id);
}