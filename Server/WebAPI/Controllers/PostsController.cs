using ApiContracts.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")] 
public class PostsController : ControllerBase
{
    private readonly IPostRepository  _postRepository;

    public PostsController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<PostDTO>>> GetAll()
    {
        var posts = _postRepository.GetMany();
        if(!posts.Any())
        {
            return NotFound();
        }
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDTO>> GetPost(int id)
    {
        var post = await _postRepository.GetSingleAsync(id);
        if (post == null) 
            return NotFound();
        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<PostDTO>> AddPost([FromBody] CreatePostDTO request)
    {
        bool exists = await _postRepository.DoesPostExistAsync(request.Title);

        if (exists)
        {
            // Return a 409 Conflict (resource already exists)
            return Conflict(new { message = "A post with this title already exists." });
        }

        Post post = new(request.Title, request.Body, request.UserId);
        await _postRepository.AddAsync(post);

        return Created(string.Empty, new PostDTO(post));
    }

    [HttpDelete]
    public async Task<ActionResult<PostDTO>> DeletePost(int id)
    {
        var deletePost = await _postRepository.DeleteAsync(id);
        if (deletePost == null)
        {
          return  NotFound($"Post with id {id} not found.");
        }
        return Ok(deletePost);
    }

    [HttpPatch]
    public async Task<ActionResult<PostDTO>> UpdatePost([FromBody] string title, int id)
    {
        var updatedPost = await _postRepository.PatchAsync(id, title);
        if (updatedPost == null)
        {
            return NotFound($"Post with id {id} not found.");
        }
        return Ok(updatedPost);
    }
}