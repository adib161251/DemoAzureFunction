using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IsolatedProcess
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(builder =>
                {
                    builder.UseMiddleware<LoggingMiddleware>();
                })
                .Build();

            host.Run();
        }

    }

    public class LoggingMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var logger = context.GetLogger<LoggingMiddleware>();

            var funcationName = context.FunctionDefinition.Name;

            logger.LogInformation("LOG: Before excuting in middleware {funcationName}", funcationName);

            await next(context);

            logger.LogInformation("LOG: After excuted in middleware {funcationName}", funcationName);
        }
    }
}