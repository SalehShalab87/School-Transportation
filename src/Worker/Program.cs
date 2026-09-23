using Application;
using Infrastructure;
using Optimization;
using Worker.Outbox;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOptimization();
builder.Services.AddHostedService<OutboxBackgroundService>();

var host = builder.Build();
host.Run();
