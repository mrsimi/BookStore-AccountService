using System.Net;
using Contoso_BookStore_AccountService.Data;
using Contoso_BookStore_AccountService.DTO.Requests;
using Contoso_BookStore_AccountService.Services.Implementations;
using Contoso_BookStore_AccountService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Contoso-BookStore.db";


builder.Services.AddSqlite<ContosoBookStoreDbContext>(connectionString);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountService, AccountService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", ( ) => "Contoso-BookStore AccountService");

app.MapPost("api/account/register", async (RegisterRequest request, IAccountService accountService) =>
{
    var result = await accountService.Register(request);

    switch (result.StatusCode)
    {
        case (int)HttpStatusCode.Created:
            return Results.Created("",result);
        case (int)HttpStatusCode.Conflict:
            return Results.Conflict(result);
        default:
            return Results.Problem("An error occured.", statusCode: result.StatusCode);
    }
});

app.MapPost("/api/account/login", async (LoginRequest request, IAccountService accountService) =>
{
    var result = await accountService.Login(request);

    switch (result.StatusCode)
    {
        case (int)HttpStatusCode.OK:
            return Results.Ok(result);
        case (int)HttpStatusCode.BadRequest:
            return Results.BadRequest(result);
        case (int)HttpStatusCode.NotFound:
            return Results.NotFound(result);
        default:
            return Results.Problem("An error occured.", statusCode: result.StatusCode);
    }
});

app.Run();
