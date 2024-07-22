using ChatRoom.Dto;
using ChatRoom.Entities;

namespace ChatRoom.Mapping;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(user.Id, user.Name, user.Email, user.Password);
    }
}