using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;
/// <summary>
/// File-based repository for Users. Stores JSON at Data/users.json.
/// </summary>
public class UserFileRepository : IUserRepository
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private readonly string _dir = Path.Combine(AppContext.BaseDirectory, "Data");
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public UserFileRepository(string? fileName = null)
    {
        Directory.CreateDirectory(_dir);
        _filePath = Path.Combine(_dir, fileName ?? "users.json");
        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        SeedIfEmptyAsync().GetAwaiter().GetResult();
    }

    public async Task<User> AddAsync(User user)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            user.Id = list.Count == 0 ? 1 : list.Max(u => u.Id) + 1;
            list.Add(user);
            await SaveAllAsync(list);
            return user;
        }
        finally { _gate.Release(); }
    }

    public async Task UpdateAsync(User user)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            var idx = list.FindIndex(u => u.Id == user.Id);
            if (idx < 0) return;
            list[idx] = user;
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
            list.RemoveAll(u => u.Id == id);
            await SaveAllAsync(list);
        }
        finally { _gate.Release(); }
    }

    public async Task<User?> GetSingleAsync(int id)
    {
       //Read-only: no need to lock
       var list = await LoadAllAsync();
       return list.FirstOrDefault(u => u.Id == id);
    }
    
    // Interface demands non-async. I synchronously unwrap the async read.
    // In a console app this is acceptable, but generally prefer await.
    public IQueryable<User> GetMany()
    {
        string json = File.ReadAllTextAsync(_filePath).Result;
        var list = JsonSerializer.Deserialize<List<User>>(json, JsonOpts) ?? new();
        return list.AsQueryable();
    }

    private async Task<List<User>> LoadAllAsync()
    {
        string json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<User>>(json, JsonOpts) ?? new();
    }

    private Task SaveAllAsync(List<User> list)
    {
        string json = JsonSerializer.Serialize(list, JsonOpts);
        return File.WriteAllTextAsync(_filePath, json);
    }

    private async Task SeedIfEmptyAsync()
    {
        var list = await LoadAllAsync();
        if (list.Count > 0) return;
        list = new List<User>()
        {
            new() { Id = 1, Username = "alice", Password = "secret" },
            new() { Id = 2, Username = "bob", Password = "12345678" },
            new() { Id = 3, Username = "carol", Password = "qwerty" }
        };
        await SaveAllAsync(list);
    }
}