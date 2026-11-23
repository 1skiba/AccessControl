using System.ComponentModel.DataAnnotations;

namespace AccessControlApi.DTOs;


public record UserDto(int Id, string Name, DateTime CreatedAt, int? AccessGroupId, string? AccessGroupName, List<string> CardsUids);

public record CreateUserDto([Required, MaxLength(200)] string Name, int? AccessGroupId);

public record UpdateUserDto([Required, MaxLength(200)] string Name, int? AccessGroupId);
