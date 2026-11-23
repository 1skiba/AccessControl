using System.ComponentModel.DataAnnotations;
using AccessControlApi.Data;
using AccessControlApi.DTOs;
using AccessControlApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessControlApi.Services;

public class UserService : IUserService
{
    private readonly AccessControlDbContext _db;
    public UserService(AccessControlDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _db.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserDto(
                u.Id,
                u.Name,
                u.CreatedAt,
                u.AccessGroupId,
                u.AccessGroup != null ? u.AccessGroup.Name : null,
                u.Cards.Select(c => c.Uid).ToList()))
            .ToListAsync();
        
        return users;
    }
    
    public Task<UserDto?> GetAsync(int id)
    {
        var user = _db.Users.Where(u => u.Id == id)
            .Select(u => new UserDto(
                u.Id,
                u.Name,
                u.CreatedAt,
                u.AccessGroupId,
                u.AccessGroup != null ? u.AccessGroup.Name : null,
                u.Cards.Select(c => c.Uid).ToList()))
            .FirstOrDefaultAsync();
        
        return user;
    }
    
    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = new User();
        user.Name = dto.Name.Trim();
        user.AccessGroupId = dto.AccessGroupId;
        user.CreatedAt = DateTime.UtcNow;
        
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        var created = await GetAsync(user.Id);
        
        return created;
    }
    
    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
        {
            return false;
        }

        await EnsureGroupExistsAsync(dto.AccessGroupId);

        user.Name = dto.Name.Trim();
        user.AccessGroupId = dto.AccessGroupId;
        
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user is null)
        {
            return false;
        }
        
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        
        return true;
    }

    private async Task EnsureGroupExistsAsync(int? groupId)
    {
        if (groupId is null)
        {
            return;
        }
        
        var exists = await _db.AccessGroups.AnyAsync(g => g.Id == groupId);
        if (!exists)
        {
            throw new ValidationException("This Access Group does not exist");
        }
    }
}