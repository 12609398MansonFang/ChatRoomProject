using ChatRoom.Dto;
using ChatRoom.Entities;

namespace ChatRoom.Mapping;

public static class RoomMapping
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto(room.Id, room.Name, room.Description, room.Users, room.Admin);
    }
}