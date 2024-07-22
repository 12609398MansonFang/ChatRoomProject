using ChatRoom.Dto;
using ChatRoom.Entities;

namespace ChatRoom.Mapping;

public static class NotificationMapping
{
    public static NotificationDto ToDto(this Notification notification)
    {
        return new NotificationDto(notification.Id, notification.Type, notification.Text, notification.Users);
    }
}