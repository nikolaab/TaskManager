using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Commands.AddTask;
using TaskManager.Application.Commands.CompleteTask;
using TaskManager.Application.Queries.GetTasks;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR (v12+)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<AddTaskCommand>();
});

// Pipeline behaviors (validacija + logovanje grešaka)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionLoggingBehavior<,>));

// FluentValidation: registruj validatore iz Application sklopa
builder.Services.AddValidatorsFromAssemblyContaining<AddTaskCommand>();

// Repozitorijum (u memoriji)
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Minimal API endpointi

// GET /tasks – lista svih zadataka
app.MapGet("/tasks", async (IMediator mediator) =>
{
    var tasks = await mediator.Send(new GetAllTasksQuery());
    // Projekcija u lagani DTO da odgovor bude čist
    return Results.Ok(tasks.Select(t => new TaskDto(
        t.Id, t.Description, t.CreatedAt, t.DueDate, t.IsCompleted
    )));
});

// POST /tasks – dodaj zadatak
app.MapPost("/tasks", async (CreateTaskDto dto, IMediator mediator) =>
{
    var created = await mediator.Send(new AddTaskCommand(dto.Description, dto.DueDate));
    return Results.Created($"/tasks/{created.Id}", new TaskDto(
        created.Id, created.Description, created.CreatedAt, created.DueDate, created.IsCompleted
    ));
});

// POST /tasks/{id}/complete – označi kao završen
app.MapPost("/tasks/{id:guid}/complete", async (Guid id, IMediator mediator) =>
{
    var ok = await mediator.Send(new CompleteTaskCommand(id));
    return ok ? Results.NoContent() : Results.NotFound();
});

app.Run();

// DTO zapisi (jednostavni “view modeli” za HTTP)
public record CreateTaskDto(string Description, DateTime? DueDate);
public record TaskDto(Guid Id, string Description, DateTime CreatedAt, DateTime? DueDate, bool IsCompleted);
