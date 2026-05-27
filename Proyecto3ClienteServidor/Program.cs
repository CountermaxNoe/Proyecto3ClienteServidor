using Proyecto3ClienteServidor.Hubs;
using Proyecto3ClienteServidor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<TurnoService>();


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapHub<TurnoHub>("/turnoHub");





app.Run();
