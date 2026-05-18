using AuthSystem.Identity.Extensions;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Accounts.Extensions;
using Common.Core.IdsGenerator;
using Common.Core.Jwt;
using HotelRoomMS.Application;
using HotelRoomMS.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Module
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddAccountsModule(builder.Configuration);

//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentityModuleSwagger();


SnowFlakIdGenerator.Configure(generatorId: 1);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});




var app = builder.Build();

//data seeder
await app.SeedIdentityModuleAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseHttpsRedirection(); 
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
