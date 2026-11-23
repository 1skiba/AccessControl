using AccessControlApi.DTOs;

namespace AccessControlApi.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
}