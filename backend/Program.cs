using Backend.Functionality;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddTransient<IEventService, EventService>();
builder.Services.AddTransient<IBlockchainSynchronizer, BlockchainSynchronizer>();

builder.Services.Configure<BlockchainOptions>(
    builder.Configuration.GetSection(BlockchainOptions.Blockchain));

await DatabaseSetup.SetupTeardownDatabase();

builder.Build().Run();
