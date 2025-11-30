namespace ApiContracts.DTOs;

public class CommentDTO
{
    public int Id { get; set; } //PK
    public string Body { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
}