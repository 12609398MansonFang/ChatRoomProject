namespace ChatRoom.Entities;

public class Room
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int[]? Users { get; set; }
    public int? Admin { get; set; }
}