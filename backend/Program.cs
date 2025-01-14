using Backend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/events", async () => {
    Console.WriteLine("Hello, world!");

    var events = await Events.GetAllEvents();
    return events;
});

app.Run();

// todo: create database that stores all types of events, INCLUDING BLOCK NUMBER
// todo: when new request, only request events from latest block number in database
// todo: probably rate limit not the API, but calls to blockchain somehow, when API is called (stale data is fine to some extent)
// todo: rebase and remove url with API key to alchemy
// todo: allow query from-block=blockNumber&to-block=blockNumber