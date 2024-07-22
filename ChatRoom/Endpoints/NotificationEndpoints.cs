using ChatRoom.Data;
using ChatRoom.Dto;
using ChatRoom.Entities;
using ChatRoom.Mapping;

namespace ChatRoom.Endpoints;

public static class NotificationEndpoints
{
    public static RouteGroupBuilder MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("notification");

        //CRUD to get all Notifications
        group.MapGet("/", (ChatRoomContext dbContext) =>
        {
            var notifications = dbContext.Notifications.ToList();
            var notificationDto = notifications.Select(notification => notification.ToDto());
            return Results.Ok(notificationDto);
        });

        //CRUD to get notification specific to UserId
        group.MapGet("/{Id}", (int Id, ChatRoomContext dbContext) =>
        {
            var notifications = dbContext.Notifications.Where(notification => notification.Users != null && notification.Users.Contains(Id)).ToList();
            var notificationDto = notifications.Select(notification => notification.ToDto());
            return Results.Ok(notificationDto);
        });

        //CRUD to create a notification
        group.MapPost("/", (CreateNotificationDto createNotificationDto, ChatRoomContext dbContext) =>
        {
            if (createNotificationDto == null || string.IsNullOrEmpty(createNotificationDto.Text) || createNotificationDto.Users == null || !createNotificationDto.Users.Any())
            {
                return Results.BadRequest("Invalid Message Data");
            };
            var notification = new Notification
            {
                Type = createNotificationDto.Type,
                Text = createNotificationDto.Text,
                Users = createNotificationDto.Users
            };

            dbContext.Notifications.Add(notification);
            dbContext.SaveChanges();

            var notificationDto = notification.ToDto();
            return Results.Created($"/{notification.Id}", notificationDto);
        });

        //CRUD to update notification
        group.MapPut("/{id}", (int id, UpdateNotificationDto updateNotificationDto, ChatRoomContext dbContext) => 
        {
            var notification = dbContext.Notifications.Find(id);
            if (notification == null)
            {
                return Results.NotFound();
            }

            notification.Users = updateNotificationDto.Users?.Any() == true ? updateNotificationDto.Users : Array.Empty<int>();

            dbContext.Notifications.Update(notification);
            dbContext.SaveChanges();

            var notificationDto = notification.ToDto();

            return Results.Ok(notificationDto);
        });

        //CRUD to delete a notification
        group.MapDelete("/{id}", (int id, ChatRoomContext dbContext) =>
        {
            var notification = dbContext.Notifications.Find(id);
            if (notification == null)
            {
                return Results.NotFound();
            }

            dbContext.Notifications.Remove(notification);
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        return group;
    }
}