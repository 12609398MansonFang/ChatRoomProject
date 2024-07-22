namespace ChatRoom.Entities;

public class Message
{
	public int Id { get; set; }
	public string? Text { get; set; }
	public int UserId { get; set; }
	public int RoomId { get; set; }
}