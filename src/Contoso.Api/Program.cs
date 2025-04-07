using Contoso.Application.Abstractions;
using Contoso.Application.Mentors;
using Contoso.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IMentorRepository, InMemoryMentorRepository>();
builder.Services.AddScoped<MentorSearchService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapControllers();
app.MapHealthChecks("/healthz");

await app.RunAsync();

public partial class Program;
