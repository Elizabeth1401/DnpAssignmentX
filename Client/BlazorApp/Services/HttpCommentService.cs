namespace BlazorApp.Services;

public class HttpCommentService  : ICommentService
{
    private readonly HttpClient _httpClient;
    
    public HttpCommentService(HttpClient httpClient)
        {
        _httpClient = httpClient;
        }
    
}