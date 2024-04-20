#if DEBUG
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rayer.Services;

internal static class LocalLoggerFactory
{
    public static ILogger GetOrCreateLogger(
            IServiceProvider serviceProvider,
            string logCategoryName)
    {

        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

        loggerFactory ??= LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

        return loggerFactory.CreateLogger(logCategoryName);
    }
}
#endif