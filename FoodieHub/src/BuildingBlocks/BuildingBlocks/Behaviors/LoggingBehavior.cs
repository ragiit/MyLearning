﻿namespace BuildingBlocks.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
        (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handling with request: {@Request} - Response={Response} - RequestData={RequestData}", typeof(TRequest).Name, typeof(TResponse).Name, request);

            var timer = new Stopwatch();
            timer.Start();

            var response = await next(cancellationToken);
            timer.Stop();

            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3)
            {
                logger.LogWarning("[PERFORMANCE] The Request {Request} Handling request took {TimeTaken} seconds", typeof(TRequest).Name, timeTaken.Seconds);
            }

            logger.LogInformation("[END] Handled request: {@Request} - Response={Response} - TimeTaken={TimeTaken} seconds", typeof(TRequest).Name, typeof(TResponse).Name, timeTaken.Seconds);

            return response;
        }
    }
}