using ApiContracts.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository  _commentRepository;

    public CommentsController(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentDTO>>> GetComments()
    {
        var comments = _commentRepository.GetMany();
        if (!comments.Any())
        {
            return NotFound();
        }
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDTO>> GetComment(int id)
    {
        var comment = await _commentRepository.GetSingleAsync(id);
        if (comment == null)
        { 
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDTO>> AddComment(CommentDTO request)
    {
        await _commentRepository.DoesCommentExistAsync(request.Body);

        Comment comment = new(request.Id, request.Body);
        await _commentRepository.AddAsync(comment);
        return Created();
    }

    [HttpDelete]
    public async Task<ActionResult<CommentDTO>> DeleteComment(int id)
    {
        var deleteComment = await _commentRepository.DeleteAsync(id);
        if (deleteComment == null)
        {
            return NotFound($"Comment with id {id} not found");
        }
        return Ok(deleteComment);
    }

    [HttpPatch]
    public async Task<ActionResult<CommentDTO>> UpdateComment([FromBody] string body, int id)
    {
        var commentToUpdate = await _commentRepository.PatchAsync(id, body);
        if (commentToUpdate == null)
        {
            return NotFound($"Comment with id {id} not found");
        }
        return Ok(commentToUpdate);
    }
}