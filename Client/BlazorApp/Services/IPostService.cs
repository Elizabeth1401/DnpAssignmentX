using ApiContracts.DTOs;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<PostDTO> CreatePostAsync(CreatePostDTO request);
}