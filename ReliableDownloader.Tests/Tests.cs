using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ReliableDownloader.Tests
{
    [TestFixture]
    public class Tests
    {
        private IFileDownloader _fileDownloader;
        [SetUp]
        public void Setup()
        {
            var serviceProvider = DIRegister.RegisterServices();
            _fileDownloader = serviceProvider.GetService<IFileDownloader>();
        }

        [Test]
        public async Task FileDownloader_Should_Download_Successfully()
        {
            var exampleUrl = "https://installerstaging.accurx.com/chain/3.182.57641.0/accuRx.Installer.Local.msi";
            var exampleFilePath = "C:/Users/Public/myfirstdownload.msi";

            await _fileDownloader.DownloadFile(exampleUrl, exampleFilePath, progress => { });
            Assert.True(true);
        }

        [Test]
        public void FileDownloader_Should_Throw_Exception()
        {
            var exampleUrl = "https://installerstaging.accurx.com/chain/abc/3.182.57641.0/accuRx.Installer.Local.msi";
            var exampleFilePath = "C:/Users/Public/myfirstdownload.msi";

            var ex = Assert.ThrowsAsync<Exception>(() => _fileDownloader.DownloadFile(exampleUrl, exampleFilePath, progress => { }));
            Assert.AreEqual("Http response is not ok. Status code is NotFound ", ex.Message);
        }

    }
}