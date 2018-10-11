using Bithumb.NetCore.Data;
using Bithumb.NetCore.Data.Interface;
using Bithumb.NetCore.Entities;
using FileRepository;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Bithumb.NetCore.Tests
{
    public class BithumbRepositoryTests : IDisposable
    {
        private ApiInformation _exchangeApi = null;
        private IBithumbRepository _repo;
        private string configPath = "config.json";
        private string apiKey = string.Empty;
        private string apiSecret = string.Empty;

        /// <summary>
        /// Constructor for tests
        /// </summary>
        public BithumbRepositoryTests()
        {
            IFileRepository _fileRepo = new FileRepository.FileRepository();
            if (_fileRepo.FileExists(configPath))
            {
                _exchangeApi = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
            }
            if (_exchangeApi != null || !string.IsNullOrEmpty(apiKey))
            {
                _repo = new BithumbRepository(_exchangeApi.apiKey, _exchangeApi.apiSecret, true);
            }
            else
            {
                _repo = new BithumbRepository();
            }
        }

        public void Dispose()
        {

        }
        
        [Fact]
        public void GetTickerTest()
        {
            // Arrange
            var symbol = "BTC";

            // Act
            var ticker = _repo.GetTicker(symbol).Result;

            // Assert
            Assert.NotNull(ticker);
        }

        [Fact]
        public void GetOrderBookTests()
        {
            // Arrange
            var symbol = "BTC";

            // Act
            var orderBook = _repo.GetOrderBook(symbol).Result;

            // Assert
            Assert.NotNull(orderBook);
        }

        [Fact]
        public void GetTransactionHistoryTests()
        {
            // Arrange
            var symbol = "BTC";

            // Act
            var txnHistory = _repo.GetTransactionHistory(symbol).Result;

            // Assert
            Assert.NotNull(txnHistory);
        }
    }
}
