using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;
/// <summary>
/// File-based repository for Posts. Stores JSON at Data/posts.json.
/// </summary>
public class PostFileRepository : IPostRepository
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private readonly string _dir = Path.Combine(AppContext.BaseDirectory, "Data");
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    
    public PostFileRepository(string? fileName = null)
    {
        Directory.CreateDirectory(_dir);
        _filePath = Path.Combine(_dir, fileName ?? "posts.json");
        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        SeedIfEmptyAsync().GetAwaiter().GetResult();
    }
    
    public async Task<Post> AddAsync(Post post)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            post.Id = list.Count == 0 ? 1 : list.Max(p => p.Id) + 1;
            list.Add(post);
            await SaveAllAsync(list);
            return post;
        }
        finally { _gate.Release(); }
    }
    
    public async Task UpdateAsync(Post post)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            var idx = list.FindIndex(p => p.Id == post.Id);
            if (idx < 0) return; // or throw
            list[idx] = post;
            await SaveAllAsync(list);
        }
        finally { _gate.Release(); }
    }
    
    public async Task DeleteAsync(int id)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            list.RemoveAll(p => p.Id == id);
            await SaveAllAsync(list);
        }
        finally { _gate.Release(); }
    }

    public async Task<Post?> GetSingleAsync(int id)
    {
        var list = await LoadAllAsync();
        return list.FirstOrDefault(p => p.Id == id);
    }

    public IQueryable<Post> GetMany()
    {
        string json = File.ReadAllTextAsync(_filePath).Result;
        var list = JsonSerializer.Deserialize<List<Post>>(json, JsonOpts) ?? new();
        return list.AsQueryable();
    }

    private async Task<List<Post>> LoadAllAsync()
    {
        string json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Post>>(json, JsonOpts) ?? new();
    }

    private Task SaveAllAsync(List<Post> list)
    {
        string json = JsonSerializer.Serialize(list, JsonOpts);
        return File.WriteAllTextAsync(_filePath, json);
    }

    private async Task SeedIfEmptyAsync()
    {
        var list = await LoadAllAsync();
        if (list.Count > 0) return;
        list = new List<Post>
        {
            new() { Id = 1, Title = "Hello World", Body = "My first post", UserId = 1 },
            new() { Id = 2, Title = "Second Post", Body = "More content here", UserId = 2 }
        };
        await SaveAllAsync(list);
    }
}