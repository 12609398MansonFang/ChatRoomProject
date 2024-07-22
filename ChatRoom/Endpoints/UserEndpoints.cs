using ChatRoom.Data;
using ChatRoom.Dto;
using ChatRoom.Entities;
using ChatRoom.Mapping;


namespace ChatRoom.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("user");

        //CRUD to get all users
        group.MapGet("/", (ChatRoomContext dbContext) =>
        {
            var users = dbContext.Users.ToList();
            var userDto = users.Select(user => user.ToDto());
            return Results.Ok(userDto);
        });

        //CRUD to post user specific to a user name and password and return Id and name
        group.MapPost("/login", (LogInDto logInDto, ChatRoomContext dbContext) =>
        {
            if (logInDto == null || string.IsNullOrEmpty(logInDto.Email) || string.IsNullOrEmpty(logInDto.Password))
            {
                return Results.BadRequest("Invalid Login Data");
            }

            var user = dbContext.Users.SingleOrDefault(user => user.Email == logInDto.Email && user.Password == logInDto.Password);

            if (user == null)
            {
                return Results.Unauthorized();
            }
            var result = new { user.Id, user.Name };
            return Results.Ok(result);
        });

        //CRUD to create a user
        group.MapPost("/", (CreateUserDto createUserDto, ChatRoomContext dbContext) =>
        {
            if (createUserDto == null || string.IsNullOrEmpty(createUserDto.Name) || string.IsNullOrEmpty(createUserDto.Email) || string.IsNullOrEmpty(createUserDto.Password))
            {
                return Results.BadRequest("Invalid Message Data");
            };
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = createUserDto.Password
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var userDto = user.ToDto();
            return Results.Created($"/{user.Id}", userDto);
        });

        //CRUD to update a user
        group.MapPut("/{id}", (int id, CreateUserDto updateUserDto, ChatRoomContext dbContext) =>
        {
            var user = dbContext.Users.Find(id);
            if (user == null)
            {
                return Results.NotFound();
            }
            if (updateUserDto == null || string.IsNullOrEmpty(updateUserDto.Name) || string.IsNullOrEmpty(updateUserDto.Email) || string.IsNullOrEmpty(updateUserDto.Password))
            {
                return Results.BadRequest("Invalid update data.");
            }

            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;
            user.Password = updateUserDto.Password;

            dbContext.Users.Update(user);
            dbContext.SaveChanges();

            var userDto = user.ToDto();

            return Results.Ok(userDto);
        });

        //CRUD to delete a user
        group.MapDelete("/{id}", (int id, ChatRoomContext dbContext) =>
        {
            var user = dbContext.Users.Find(id);
            if (user == null)
            {
                return Results.NotFound();
            }

            dbContext.Users.Remove(user);
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        return group;
    }
}