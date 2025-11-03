namespace Entities;

public class Post
{
    public int Id { get; set; } //PK
    public string Title { get; set; } //required
    public string Body { get; set; } //required
    
    //Foreign key to User
    public int UserId { get; set; }
    
    public Post(){}
    public Post(string title, string body)
        {
        Title = title;
        Body = body;
        }
}