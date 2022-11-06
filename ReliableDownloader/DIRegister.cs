using Microsoft.Extensions.DependencyInjection;

namespace ReliableDownloader
{
    public static class DIRegister
    {
        public static ServiceProvider RegisterServices()
        {
            return new ServiceCollection()
           .AddScoped<IFileDownloader, FileDownloader>()
           .AddScoped<IWebSystemCalls, WebSystemCalls>()
           .BuildServiceProvider();
        }
    }
}
