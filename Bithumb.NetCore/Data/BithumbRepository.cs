using Bithumb.NetCore.Data.Interface;
using Bithumb.NetCore.Entities;
using DateTimeHelpers;
using FileRepository;
using RESTApiAccess;
using RESTApiAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bithumb.NetCore.Data
{
    public class BithumbRepository : IBithumbRepository
    {
        private IRESTRepository _restRepo;
        private string baseUrl;
        private ApiInformation _apiInfo = null;
        private DateTimeHelper _dtHelper;
        private bool testApi = false;

        /// <summary>
        /// Constructor for non-signed endpoints
        /// </summary>
        public BithumbRepository()
        {
            LoadRepository();
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        public BithumbRepository(string apiKey, string apiSecret)
        {
            _apiInfo = new ApiInformation
            {
                apiKey = apiKey,
                apiSecret = apiSecret
            };
            LoadRepository();
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="configPath">String of path to configuration file</param>
        public BithumbRepository(string configPath)
        {
            IFileRepository _fileRepo = new FileRepository.FileRepository();

            if (_fileRepo.FileExists(configPath))
            {
                _apiInfo = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
                LoadRepository();
            }
            else
            {
                throw new Exception("Config file not found");
            }
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        /// <param name="test">Testing api?</param>
        public BithumbRepository(string apiKey, string apiSecret, bool test)
        {
            _apiInfo = new ApiInformation
            {
                apiKey = apiKey,
                apiSecret = apiSecret
            };
            testApi = test;
            LoadRepository();
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="configPath">String of path to configuration file</param>
        /// <param name="test">Testing api?</param>
        public BithumbRepository(string configPath, bool test)
        {
            IFileRepository _fileRepo = new FileRepository.FileRepository();

            if (_fileRepo.FileExists(configPath))
            {
                _apiInfo = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
                testApi = test;
                LoadRepository();
            }
            else
            {
                throw new Exception("Config file not found");
            }
        }

        /// <summary>
        /// Load repository
        /// </summary>
        /// <param name="key">Api key value (default = "")</param>
        /// <param name="secret">Api secret value (default = "")</param>
        private void LoadRepository(string key = "", string secret = "")
        {
            //security = new Security();
            _restRepo = new RESTRepository();
            baseUrl = "https://api.bithumb.com";
            _dtHelper = new DateTimeHelper();
        }

        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        public bool ValidateExchangeConfigured()
        {
            var ready = _apiInfo == null || string.IsNullOrEmpty(_apiInfo.apiKey) ? false : true;
            if (!ready)
                return false;

            return string.IsNullOrEmpty(_apiInfo.apiSecret) ? false : true;
        }

        #region Public Api

        public async Task<Ticker> GetTicker(string symbol)
        {
            var endpoint = $"/public/ticker/{symbol}";

            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApiStream<ApiResponse<Ticker>>(url);

            return response.data;
        }

        public async Task<OrderBook> GetOrderBook(string symbol)
        {
            var endpoint = $"/public/orderbook/{symbol}";

            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApiStream<ApiResponse<OrderBook>>(url);
            
            return response.data;
        }

        public async Task<RecentTransaction> GetRecentTransactions(string symbol)
        {
            var endpoint = $"/public/recent_transactions/{symbol}";

            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApiStream<ApiResponse<RecentTransaction>>(url);

            return response.data;
        }

        public async Task<Transaction> GetTransactionHistory(string symbol)
        {
            var endpoint = $"/public/transaction_history/{symbol}";

            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApiStream<ApiResponse<Transaction>>(url);

            return response.data;
        }

        #endregion Public Api

        #region Private Api
        
        public async Task<Account> GetAccount(string symbol)
        {
            var endpoint = $"/info/account";

            var url = baseUrl + endpoint;

            var parameters = GetPostData(symbol);

            var response = await _restRepo.PostApi<ApiResponse<Account>, Dictionary<string, object>>(url, parameters);

            return response.data;
        }

        public async Task<Balance[]> GetBalance(string symbol)
        {
            var endpoint = $"/info/account";
            
            var url = baseUrl + endpoint;
            var lowSymbol = symbol.ToLower();

            var parameters = GetPostData(symbol);

            var response = await _restRepo.PostApi<ApiResponse<Dictionary<string, decimal>>, Dictionary<string, object>>(url, parameters);

            var btcBalance = new Balance
            {
                symbol = "BTC",
                total = response.data["total_btc"],
                inUse = response.data["in_use_btc"],
                available = response.data["available_btc"],
            };

            var symbolBalance = new Balance
            {
                symbol = symbol,
                total = response.data[$"total_{lowSymbol}"],
                inUse = response.data[$"in_use_{lowSymbol}"],
                available = response.data[$"available_{lowSymbol}"],
                lastTrx = response.data["xcoin_last"]
            };

            var balances = new Balance[] { btcBalance, symbolBalance };

            return balances;
        }

        public async Task<string> GetDepositAddress(string symbol)
        {
            var endpoint = $"/info/wallet_address";

            var url = baseUrl + endpoint;

            var parameters = GetPostData(symbol);

            var response = await _restRepo.PostApi<ApiResponse<Dictionary<string, string>>, Dictionary<string, object>>(url, parameters);

            return response.data["wallet_address"];
        }

        public async Task<PrivateTicker> GetPrivateTicker(string pair)
        {
            var endpoint = $"/info/wallet_address";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            parameters.Add("order_currency", GetSymbol(pair));
            parameters.Add("payment_currency", GetTradingSymbol(pair));

            var response = await _restRepo.PostApi<ApiResponse<PrivateTicker>, Dictionary<string, object>>(url, parameters);

            return response.data;
        }


        #endregion Private Api

        private Dictionary<string, object> GetBasePostData()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("apiKey", _apiInfo.apiKey);
            parameters.Add("secretKey", _apiInfo.apiSecret);

            return parameters;
        }

        private Dictionary<string, object> GetPostData(string symbol)
        {
            var parameters = GetBasePostData();

            parameters.Add("currency", symbol);

            return parameters;
        }

        private string GetSymbol(string pair)
        {
            return pair.Substring(0, pair.IndexOf("-") - 1);
        }

        private string GetTradingSymbol(string pair)
        {
            return pair.Substring(pair.IndexOf("-") + 1);
        }
    }
}
