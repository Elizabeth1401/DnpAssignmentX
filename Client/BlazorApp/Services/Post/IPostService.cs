using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public interface IPostService
{
    Task CreatePostAsync(CreatePostDTO request);
    Task<List<Post>> GetAllPostsAsync();
}