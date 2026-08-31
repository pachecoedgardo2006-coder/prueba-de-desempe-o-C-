using System.Text.Json;
using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public MemberRepository(string relativePath = "data/members.json")
    {
        _filePath = Path.IsPathRooted(relativePath)
            ? relativePath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        EnsureFileExists();
    }

    public List<Member> GetAll()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Member>();
        }

        string json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Member>();
        }

        return JsonSerializer.Deserialize<List<Member>>(json, _jsonOptions) ?? new List<Member>();
    }

    public Member? GetById(int id)
    {
        return GetAll().FirstOrDefault(m => m.Id == id);
    }

    public void Add(Member member)
    {
        var members = GetAll();
        members.Add(member);
        SaveAll(members);
    }

    public void Update(Member member)
    {
        var members = GetAll();
        int index = members.FindIndex(m => m.Id == member.Id);
        if (index >= 0)
        {
            members[index] = member;
            SaveAll(members);
        }
    }

    public void Delete(int id)
    {
        var members = GetAll();
        members.RemoveAll(m => m.Id == id);
        SaveAll(members);
    }

    public int GetNextId()
    {
        var members = GetAll();
        return members.Count == 0 ? 1 : members.Max(m => m.Id) + 1;
    }

    private void SaveAll(List<Member> members)
    {
        string json = JsonSerializer.Serialize(members, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    private void EnsureFileExists()
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
}
