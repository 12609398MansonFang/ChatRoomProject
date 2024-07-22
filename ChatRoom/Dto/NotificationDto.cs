namespace ChatRoom.Dto;

public record class NotificationDto(int Id, string? Type, string? Text, int[]? Users);

public record class CreateNotificationDto(string? Type, string? Text, int[]? Users);

public record class UpdateNotificationDto(int[]? Users);
