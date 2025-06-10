using Contoso.Application.Abstractions;
using Contoso.Application.Matching;
using Contoso.Application.Mentors;
using Contoso.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IMentorRepository, InMemoryMentorRepository>();
builder.Services.AddSingleton<IMatchRequestRepository, InMemoryMatchRequestRepository>();
builder.Services.AddScoped<MentorSearchService>();
builder.Services.AddScoped<MatchRequestService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.MapHealthChecks("/healthz");

await app.RunAsync();

public partial class Program;
