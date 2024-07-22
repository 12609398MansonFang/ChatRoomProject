namespace Chatroom.Dto;

public record class MessageDto(int Id, string? Text, int UserId, int RoomId);

public record class CreateMessageDto(string? Text, int UserId, int RoomId);