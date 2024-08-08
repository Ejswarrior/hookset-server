global using Azure.Storage.Blobs;

using hookset_server;
using hookset_server.DBHelpers;
using hookset_server.JWTManager;
using hookset_server.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.\
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtOption =>
{
    var key = builder.Configuration.GetValue<string>("JwtConfig:key");
    var keyBytes = Encoding.ASCII.GetBytes(key);
    jwtOption.SaveToken = true;
    jwtOption.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true
    };
});

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobConnectionString")));
builder.Services.AddScoped<IUserDBHelper, UserDBHelper>();
builder.Services.AddScoped<IJWTManager, JWTManager>();
builder.Services.AddScoped<IPostsDBHelper, PostsDBHelper>();
builder.Services.AddScoped<ICommentsDBHelper, CommentsDBHelper>();
builder.Services.AddScoped<IUserRelationsDBHelper, UserRelationsDBHelper>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<UserRelationsQueries>();
builder.Services.AddScoped<SaltManager>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
