namespace Entities;

public class Post
{
    public int Id { get; set; } //PK
    public string Title { get; set; } //required
    public string Body { get; set; } //required

    public bool IsOpen { get; set; } = false;
    //Foreign key to User
    public int UserId { get; set; }
    
    public Post(){}
    public Post(string title, string body, int userId)
        {
        Title = title;
        Body = body;
        UserId = userId;
        }
}