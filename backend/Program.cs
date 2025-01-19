using Backend;

await DatabaseSetup.SetupTeardownDatabase();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/events", async () => await new Events().GetAllEvents());

app.Run();

// todo: allow query from-block=blockNumber&to-block=blockNumber
// todo: move Events.ContractAddress in to appsettings.json
// todo: hide Events.AlchemyApiKey apiKey somehow
