namespace ChatRoom.Entities;

public class Notification
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? Text { get; set; }
    public int[]? Users { get; set; } = Array.Empty<int>();
}
