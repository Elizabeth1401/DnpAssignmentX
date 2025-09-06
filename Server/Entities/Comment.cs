namespace Entities;

public class Comment
{
    public int Id { get; set; } //PK
    public string Body { get; set; } //required
    
    //Foreign keys
    public int UserId { get; set; } // author of the comment
    public int PostId { get; set; } // the post this comment belongs to 
}