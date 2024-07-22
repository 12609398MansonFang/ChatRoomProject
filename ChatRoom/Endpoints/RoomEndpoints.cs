using ChatRoom.Data;
using ChatRoom.Dto;
using ChatRoom.Entities;
using ChatRoom.Mapping;

namespace ChatRoom.Endpoints;

public static class RoomEndpoints
{
    public static RouteGroupBuilder MapRoomEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("room");

        //CRUD to get all Rooms
        group.MapGet("/", (ChatRoomContext dbContext) =>
        {
            var rooms = dbContext.Rooms.ToList();
            var roomDto = rooms.Select(room => room.ToDto());
            return Results.Ok(roomDto);
        });

        //CRUD to get rooms specific to a user
        group.MapGet("/{Id}", (int Id, ChatRoomContext dbContext) =>
        {
            var rooms = dbContext.Rooms.Where(room => room.Users != null && room.Users.Contains(Id)).ToList();
            var roomDto = rooms.Select(room => room.ToDto());
            return Results.Ok(roomDto);
        });

        //CRUD to create a room
        group.MapPost("/", (CreateRoomDto createRoomDto, ChatRoomContext dbContext) =>
        {
            if (createRoomDto == null || string.IsNullOrEmpty(createRoomDto.Name) || string.IsNullOrEmpty(createRoomDto.Description) || createRoomDto.Users == null || !createRoomDto.Users.Any() || createRoomDto.Admin == 0)
            {
                return Results.BadRequest("Invalid Message Data");
            };
            var room = new Room
            {
                Name = createRoomDto.Name,
                Description = createRoomDto.Description,
                Users = createRoomDto.Users,
                Admin = createRoomDto.Admin
            };

            dbContext.Rooms.Add(room);
            dbContext.SaveChanges();

            var roomDto = room.ToDto();
            return Results.Created($"/{room.Id}", roomDto);
        });

        //CRUD to update a room
        group.MapPut("/{id}", (int id, CreateRoomDto updateRoomDto, ChatRoomContext dbContext) =>
        {
            var room = dbContext.Rooms.Find(id);
            if (room == null)
            {
                return Results.NotFound();
            }
            if (updateRoomDto == null || string.IsNullOrEmpty(updateRoomDto.Name) || string.IsNullOrEmpty(updateRoomDto.Description) || updateRoomDto.Users == null || !updateRoomDto.Users.Any() || updateRoomDto.Admin == 0)
            {
                return Results.BadRequest("Invalid update data.");
            }

            room.Name = updateRoomDto.Name;
            room.Description = updateRoomDto.Description;
            room.Users = updateRoomDto.Users;
            room.Admin = updateRoomDto.Admin;

            dbContext.Rooms.Update(room);
            dbContext.SaveChanges();

            var roomDto = room.ToDto();

            return Results.Ok(roomDto);
        });

        //CRUD to delete a room
        group.MapDelete("/{id}", (int id, ChatRoomContext dbContext) =>
        {
            var room = dbContext.Rooms.Find(id);
            if (room == null)
            {
                return Results.NotFound();
            }

            dbContext.Rooms.Remove(room);
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        return group;
    }
}