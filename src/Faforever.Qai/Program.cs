using Faforever.Qai.Core.Database;
using Faforever.Qai.Discord;
using Faforever.Qai.Irc;
using Faforever.Qai.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var botConfig = builder.InitializeBotConfig();

// add services
var services = builder.Services;
services.AddLogging(options => options.AddConsole());
services.SetupBotServices(botConfig);
services.SetupApiAuth(botConfig);

services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Faforever.Qai",
        Version = "v1",
        Description = "The API for the Faforever.Qai and Dostya bots."
    });
});

var app = builder.Build();
var db = app.Services.GetRequiredService<QAIDatabaseModel>();
if (db.Database.GetPendingMigrations().Any())
{
    db.Database.Migrate();
    db.SaveChanges();
}
    
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Register Swagger API Documentation
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Faforever.Qai v1"));

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var ircBot = app.Services.GetRequiredService<QaIrc>();
ircBot.Run();

var discordBot = app.Services.GetRequiredService<DiscordBot>();

await discordBot.InitializeAsync();
await discordBot.StartAsync();

app.Run();