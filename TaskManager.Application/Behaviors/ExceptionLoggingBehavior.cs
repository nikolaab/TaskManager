using MediatR;
using Microsoft.Extensions.Logging;

namespace TaskManager.Application.Behaviors;

public class ExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ExceptionLoggingBehavior<TRequest, TResponse>> _logger;

    public ExceptionLoggingBehavior(ILogger<ExceptionLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for request {RequestType}: {@Request}", typeof(TRequest).Name, request);
            throw; // ponovo bacamo – kontrola je centralna, ali ne gutamo greške
        }
    }
}
