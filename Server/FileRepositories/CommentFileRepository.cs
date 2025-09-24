using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private readonly string _dir = Path.Combine(AppContext.BaseDirectory, "Data");
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public CommentFileRepository(string? fileName = null)
    {
        Directory.CreateDirectory(_dir);
        _filePath = Path.Combine(_dir, fileName ?? "comments.json");
        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        SeedIfEmptyAsync().GetAwaiter().GetResult();
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            comment.Id = list.Count == 0 ? 1 : list.Max(c => c.Id) + 1;
            list.Add(comment);
            await SaveAllAsync(list);
            return comment;
        }
        finally { _gate.Release(); }
    }

    public async Task UpdateAsync(Comment comment)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            var idx = list.FindIndex(c => c.Id == comment.Id);
            if (idx < 0) return; // or throw
            list[idx] = comment;
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
            list.RemoveAll(c => c.Id == id);
            await SaveAllAsync(list);
        }
        finally { _gate.Release(); }
    }

    public async Task<Comment?> GetSingleAsync(int id)
    {
        var list = await LoadAllAsync();
        return list.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// Non-async per interface: synchronously unwrap async file read (acceptable for CLI).
    /// </summary>
    public IQueryable<Comment> GetMany()
    {
        string json = File.ReadAllTextAsync(_filePath).Result;
        var list = JsonSerializer.Deserialize<List<Comment>>(json, JsonOpts) ?? new();
        return list.AsQueryable();
    }

    private async Task<List<Comment>> LoadAllAsync()
    {
        string json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Comment>>(json, JsonOpts) ?? new();
    }

    private Task SaveAllAsync(List<Comment> list)
    {
        string json = JsonSerializer.Serialize(list, JsonOpts);
        return File.WriteAllTextAsync(_filePath, json);
    }

    private async Task SeedIfEmptyAsync()
    {
        var list = await LoadAllAsync();
        if (list.Count > 0) return;
        list = new List<Comment>
        {
            new() { Id = 1, PostId = 1, UserId = 2, Body = "Nice work!" },
            new() { Id = 2, PostId = 1, UserId = 3, Body = "Welcome!"   }
        };
        await SaveAllAsync(list);
    }
}