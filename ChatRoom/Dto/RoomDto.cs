namespace ChatRoom.Dto;

public record class RoomDto(int Id, string? Name, string? Description, int[]? Users, int? Admin);

public record class CreateRoomDto(string? Name, string? Description, int[]? Users, int? Admin);

