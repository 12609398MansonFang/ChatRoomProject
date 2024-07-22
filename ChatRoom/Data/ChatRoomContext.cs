using Microsoft.EntityFrameworkCore;
using ChatRoom.Entities;

namespace ChatRoom.Data;

public class ChatRoomContext(DbContextOptions<ChatRoomContext> options)
                    : DbContext(options)
{

    public DbSet<User> Users => Set<User>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "frontend", Email = "frontend@test.com", Password = "frontend" },
            new User { Id = 2, Name = "backend", Email = "backend@test.com", Password = "backend" }
        );
        modelBuilder.Entity<Message>().HasData(
            new Message { Id = 1, Text = "Test message 1", UserId = 1, RoomId = 1 },
            new Message { Id = 2, Text = "Test message 2", UserId = 2, RoomId = 1 }
        );
        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, Name = "Room 1", Description = "Room 1", Users = [1], Admin = 1 },
            new Room { Id = 2, Name = "Room 2", Description = "Room 2", Users = [1, 2], Admin = 2 }
        );
        modelBuilder.Entity<Notification>().HasData(
            new Notification { Id = 1, Type = "Room", Text = "Notification 1", Users = [1] },
            new Notification { Id = 2, Type = "Room", Text = "Notification 2", Users = [2] }
        );
    }

}