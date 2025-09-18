using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Queries.GetTasks;

namespace TaskManager.ConsoleUI.Workers;

public class OverdueTaskWorker : BackgroundService
{
    private readonly ILogger<OverdueTaskWorker> _logger;
    private readonly IMediator _mediator;

    public OverdueTaskWorker(ILogger<OverdueTaskWorker> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OverdueTaskWorker started.");

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var overdue = await _mediator.Send(new GetOverdueTasksQuery(), stoppingToken);
                if (overdue.Count > 0)
                {
                    _logger.LogWarning("⚠️ {Count} overdue task(s):", overdue.Count);
                    foreach (var t in overdue)
                    {
                        _logger.LogWarning(" - {Desc} (Id: {Id}, Due: {Due})", t.Description, t.Id, t.DueDate);
                    }
                }
                else
                {
                    _logger.LogInformation("No overdue tasks at {Now}.", DateTime.UtcNow);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // normalno na shutdown
        }

        _logger.LogInformation("OverdueTaskWorker stopping.");
    }
}
