using Microsoft.EntityFrameworkCore;
using ChatRoom.Data;
using ChatRoom.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Configure Database
var connString = builder.Configuration.GetConnectionString("ChatRoom");
builder.Services.AddSqlite<ChatRoomContext>(connString);

var app = builder.Build();

// Apply CORS policy
app.UseCors("AllowAll");

// Add Middleware for logging and error handling
app.UseDeveloperExceptionPage();
app.UseExceptionHandler("/error"); // Customize as needed
app.UseHttpsRedirection();

// Ensure database migration
app.MigrateDb();

// Endpoints
app.MapMessageEndpoints();
app.MapNotificationEndpoints();
app.MapRoomEndpoints();
app.MapUserEndpoints();

app.Run();