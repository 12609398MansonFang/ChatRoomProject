using Chatroom.Dto;
using ChatRoom.Entities;

namespace ChatRoom.Mapping;

public static class MessageMapping
{
	public static MessageDto ToDto(this Message message)
	{
		return new MessageDto(message.Id, message.Text, message.UserId, message.RoomId);
	}
}