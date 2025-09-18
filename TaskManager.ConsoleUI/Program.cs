using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Application usings
using TaskManager.Application.Behaviors;
using TaskManager.Application.Commands.AddTask;
using TaskManager.Application.Commands.CompleteTask;
using TaskManager.Application.Queries.GetTasks;

// Domain/Infrastructure usings
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Repositories;

// Worker
using TaskManager.ConsoleUI.Workers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        // MediatR (v12+)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AddTaskCommand>();
        });

        // Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionLoggingBehavior<,>));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<AddTaskCommand>();

        // Repo (InMemory)
        services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();

        // Worker
        services.AddHostedService<OverdueTaskWorker>();
    })
    .Build();

await host.StartAsync();

var mediator = host.Services.GetRequiredService<IMediator>();

try
{
    var soon = DateTime.UtcNow.AddSeconds(5);
    var ok = await mediator.Send(new AddTaskCommand("Demo zadatak koji će uskoro biti overdue", soon));
    Console.WriteLine($"✅ Dodat: {ok.Description} | Due: {ok.DueDate}");

    // Pokušaj nevalidnog unosa (da vidiš validaciju)
    try
    {
        await mediator.Send(new AddTaskCommand("", DateTime.UtcNow.AddDays(1)));
    }
    catch (ValidationException vex)
    {
        Console.WriteLine("⚠️ Validaciona greška:");
        foreach (var err in vex.Errors)
            Console.WriteLine($"   - {err.ErrorMessage}");
    }

    // Ispiši listu
    var all = await mediator.Send(new GetAllTasksQuery());
    Console.WriteLine("📋 Trenutni zadaci:");
    foreach (var t in all)
    {
        var status = t.IsCompleted ? "✔" : "•";
        var due = t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "no due";
        Console.WriteLine($"  {status} {t.Id} | {t.Description} | due: {due}");
    }

    Console.WriteLine();
    Console.WriteLine("⏳ Worker radi u pozadini. Ostavite aplikaciju upaljenu 15-20 sekundi da vidite overdue log poruke...");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Neočekivana greška: {ex.Message}");
}

// ostavi aplikaciju da radi dok ne pritisneš Ctrl+C (ili dok se host ne ugasi)
await host.WaitForShutdownAsync();
