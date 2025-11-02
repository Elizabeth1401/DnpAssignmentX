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
    //JsonSerializerOptions → configuration object for System.Text.Json.
    // 
    // WriteIndented = true → makes your JSON file formatted with line breaks and spaces instead of one long line.
    private readonly string _dir = Path.Combine(AppContext.BaseDirectory, "Data");
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    //This is a thread-safety lock that ensures only one operation (read or write) accesses the file at a time.
    // 
    // SemaphoreSlim → lightweight synchronization tool.
    // 
    // The two 1 values mean:
    // 
    // initialCount = 1 → one thread allowed at a time.
    // 
    // maxCount = 1 → maximum one thread.

    public UserFileRepository(string? fileName = null)
    {
        Directory.CreateDirectory(_dir); //Ensure Data/ exists
        _filePath = Path.Combine(_dir, fileName ?? "users.json");
        if (!File.Exists(_filePath)) //Ensure the JSON file exists
            File.WriteAllText(_filePath, "[]");
        SeedIfEmptyAsync().GetAwaiter().GetResult(); //Optional: add dummy data on first run
    }

    public async Task<bool> DoesUsernameExistAsync(string username)
    {
        await _gate.WaitAsync(); //acquire write lock
        try
        {
            var allUsers = await LoadAllAsync(); //read & deserialize full list
            foreach (var currentUser in allUsers)
            {
                if (currentUser.Username == username)
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            _gate.Release();
        } //release write lock
    }
    public async Task<User> AddAsync(User user)
    {
        await _gate.WaitAsync(); //acquire write lock
        try
        {
            var list = await LoadAllAsync(); //read & deserialize full list
            user.Id = list.Count == 0 ? 1 : list.Max(u => u.Id) + 1; //next id
            list.Add(user); //mutate list in memory
            await SaveAllAsync(list); //serialize & write back
            return user; //return with assigned id
        }
        finally { _gate.Release(); } //release write lock
    }

    public async Task<User> PatchAsync(int id, string password)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            var idx = list.FindIndex(u => u.Id == id);
            if (idx == -1)
            {
                return null;
            }
            list[idx].Password = password;
            await SaveAllAsync(list);
            return list[idx];
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task UpdateAsync(User user)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            var idx = list.FindIndex(u => u.Id == user.Id);
            if (idx < 0) return; //or throw KeyNotFoundException
            list[idx] = user; //overwrite the item
            await SaveAllAsync(list);
        }
        finally { _gate.Release(); }
    }

    public async Task<User?> DeleteAsync(int id)
    {
        await _gate.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            
            var userToDelete = list.FirstOrDefault(u => u.Id == id);
            if (userToDelete == null) { return null; }
            
            list.Remove(userToDelete);
            await SaveAllAsync(list);
            return userToDelete;
            
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
        string json = File.ReadAllTextAsync(_filePath).Result; //sync unwrap
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