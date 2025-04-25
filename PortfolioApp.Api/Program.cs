using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://black-ground-05c9fc41e.6.azurestaticapps.net")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com"; // Google's token issuer
        options.Audience = builder.Configuration["Google:ClientId"]; // Your Google Client ID
        options.RequireHttpsMetadata = true; // Ensure HTTPS in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Google:ClientId"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Recommended
        };
    });

builder.Services.AddAuthorization(); // Enable authorization

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
