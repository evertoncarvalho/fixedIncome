using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace FixedIncomeService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(
                service =>
                {
                    service.ServiceName.Equals("FixedIncomeService");
                });
            LoggerProviderOptions.RegisterProviderOptions<
                EventLogSettings, EventLogLoggerProvider>(builder.Services);

            //builder.Services.AddSingleton<FixedIncomeManager>();
            builder.Services.AddHostedService<FixedIncomeService.FixedincomeService>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}
