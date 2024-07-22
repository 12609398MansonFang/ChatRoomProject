namespace ChatRoom.Dto;

public record class UserDto(int Id, string? Name, string? Email, string? Password);

public record class CreateUserDto(string? Name, string? Email, string? Password);

public record class LogInDto(string? Email, string? Password);