using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FixedIncomeService
{
    internal class FixedincomeService(ILogger<FixedincomeService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogWarning($"starting {GetType().Name}");
                while(!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
            catch (OperationCanceledException)
            {
                // stopping service
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "something doesnt work...");
                Environment.Exit(1);
            }
        }
    }
}
