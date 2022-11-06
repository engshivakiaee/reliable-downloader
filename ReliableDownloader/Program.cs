using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReliableDownloader;

namespace ReliableDownloader
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Downloading started...");
            Console.WriteLine("Press Ctrl+C to cancel");

            var serviceProvider = DIRegister.RegisterServices();
            var fileDownloader = serviceProvider.GetService<IFileDownloader>();

            Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) => {
                fileDownloader.CancelDownloads();
                Console.WriteLine("Downloading was canceled!");

            });

            // If this url 404's, you can get a live one from https://installerstaging.accurx.com/chain/latest.json.
            var exampleUrl = "https://installerstaging.accurx.com/chain/3.182.57641.0/accuRx.Installer.Local.msi";
            var exampleFilePath = "C:/Users/[USER]/myfirstdownload.msi";
            await fileDownloader.DownloadFile(exampleUrl, exampleFilePath, progress => { Console.WriteLine($"Percent progress is {progress.ProgressPercent}"); });
        }
    }
}
