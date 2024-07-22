using Chatroom.Dto;
using ChatRoom.Data;
using ChatRoom.Entities;
using ChatRoom.Mapping;

namespace ChatRoom.Endpoints;

public static class MessageEndpoints
{
    public static RouteGroupBuilder MapMessageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("message");

        //CRUD to get all messages
        group.MapGet("/", (ChatRoomContext dbContext) =>
        {
            var messages = dbContext.Messages.ToList();
            var messageDto = messages.Select(message => message.ToDto());
            return Results.Ok(messageDto);
        });

        //CRUD to get messages specific to a room
        group.MapGet("/{Id}", (int Id, ChatRoomContext dbContext) =>
        {
            var messages = dbContext.Messages.Where(message => message.RoomId == Id).ToList();
            var messageDto = messages.Select(message => message.ToDto());
            return Results.Ok(messageDto);
        });

        //CRUD to create a message
        group.MapPost("/", (CreateMessageDto createMessageDto, ChatRoomContext dbContext) =>
        {
            if (createMessageDto == null || string.IsNullOrEmpty(createMessageDto.Text) || createMessageDto.UserId == 0 || createMessageDto.RoomId == 0)
            {
                return Results.BadRequest("Invalid Message Data");
            };
            var message = new Message
            {
                Text = createMessageDto.Text,
                UserId = createMessageDto.UserId,
                RoomId = createMessageDto.RoomId
            };

            dbContext.Messages.Add(message);
            dbContext.SaveChanges();

            var messageDto = message.ToDto();
            return Results.Created($"/{message.Id}", messageDto);
        });

        //CRUD to update a message
        group.MapPut("/{id}", (int id, CreateMessageDto updateMessageDto, ChatRoomContext dbContext) => {
            var message = dbContext.Messages.Find(id);
            if (message == null)
            {
                return Results.NotFound();
            }
            if (string.IsNullOrEmpty(updateMessageDto.Text) || updateMessageDto.UserId == 0 || updateMessageDto.RoomId == 0)
            {
                return Results.BadRequest("Invalid update data.");
            }

            message.Text = updateMessageDto.Text;
            message.UserId = updateMessageDto.UserId;
            message.RoomId = updateMessageDto.RoomId;

            dbContext.Messages.Update(message);
            dbContext.SaveChanges();

            var messageDto = message.ToDto();

            return Results.Ok(messageDto);
        });

        //CRUD to delete a message
        group.MapDelete("/{id}", (int id, ChatRoomContext dbContext) =>
        {
            var message = dbContext.Messages.Find(id);
            if (message == null)
            {
                return Results.NotFound();
            }

            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        return group;
    }
}