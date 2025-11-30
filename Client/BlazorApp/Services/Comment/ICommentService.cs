using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public interface ICommentService
{
    public Task AddCommentAsync(CommentDTO  request);
    public Task<List<Comment>> GetAllComments();
}