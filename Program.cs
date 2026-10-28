using DatingApp.Data;
using DatingAppApi.Interfaces;
using DatingAppApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DatingAppApi.Middleware;
using DatingAppApi.Data;
using DatingAppApi.Helpers;
using DatingApp.Entities;
using Microsoft.AspNetCore.Identity;
using DatingAppApi.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<AppDbContext>(option=>
{
       option.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
// builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
// builder.Services.AddScoped<ILikeRepository, LikesRepository>();
// builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<LogUserActivity>();
builder.Services.Configure<CaludinarySettings>(builder.Configuration.GetSection("CaludinarySettings"));
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();
builder.Services.AddIdentityCore<AppUser>(opt=>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;

}).AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token Key is missing in configuration");
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        options.Events= new JwtBearerEvents
        {
            OnMessageReceived = context=>
            {
                var accessToken= context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;

                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(opt=>
{
    opt.AddPolicy("RequireAdminRole", policy=>policy.RequireRole("Admin"));
    opt.AddPolicy("ModeratePhotoRole", policy=>policy.RequireRole("Admin", "Moderator"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
 
}

app.UseHttpsRedirection();

app.UseCors(options=>options
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200", " https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/messages");
using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await context.Database.MigrateAsync();
    await context.Connections.ExecuteDeleteAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch(System.Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex.Message, "An error occured during migration");
}
app.Run();
