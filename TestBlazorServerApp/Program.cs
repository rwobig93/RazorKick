using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure();

var app = builder.Build();

app.ConfigureWebServer();

app.Run();
