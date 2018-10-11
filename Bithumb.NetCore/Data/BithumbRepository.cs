using Bithumb.NetCore.Core;
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
        private Helper _helper;
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
            _helper = new Helper();
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

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<OrderBook> GetOrderBook(string symbol)
        {
            var endpoint = $"/public/orderbook/{symbol}";

            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApiStream<ApiResponse<OrderBook>>(url);

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<Transaction> GetTransactionHistory(string symbol)
        {
            var endpoint = $"/public/transaction_history/{symbol}";

            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApiStream<ApiResponse<Transaction>>(url);

            _helper.HandleErrors(response);

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

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<Balance[]> GetBalance(string symbol)
        {
            var endpoint = $"/info/account";
            
            var url = baseUrl + endpoint;
            var lowSymbol = symbol.ToLower();

            var parameters = GetPostData(symbol);

            var response = await _restRepo.PostApi<ApiResponse<Dictionary<string, decimal>>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

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

            _helper.HandleErrors(response);

            return response.data["wallet_address"];
        }

        public async Task<PrivateTicker> GetPrivateTicker(string pair)
        {
            var endpoint = $"/info/wallet_address";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            parameters.Add("order_currency", _helper.GetSymbol(pair));
            parameters.Add("payment_currency", _helper.GetTradingSymbol(pair));

            var response = await _restRepo.PostApi<ApiResponse<PrivateTicker>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<OrderInformation> GetOrder(string orderId)
        {
            var orders = await OnGetOrders(string.Empty, orderId, null, 0, null);

            return orders.Length > 0 ? orders[0] : null;
        }

        public async Task<OrderInformation[]> GetOrders(string symbol)
        {
            return await OnGetOrders(symbol, string.Empty, null, 0, null);
        }

        public async Task<OrderInformation[]> GetOrders(string symbol, TransactionType type)
        {
            return await OnGetOrders(symbol, string.Empty, type, 0, null);
        }

        public async Task<OrderInformation[]> GetOrders(TransactionType type)
        {
            return await OnGetOrders(string.Empty, string.Empty, type, 0, null);
        }

        public async Task<OrderInformation[]> GetOrders(string symbol, TransactionType type, int count, DateTime after)
        {
            return await OnGetOrders(symbol, string.Empty, type, count, after);
        }

        private async Task<OrderInformation[]> OnGetOrders(string symbol, string orderId, TransactionType? type, int count, DateTime? after)
        {
            var endpoint = $"/info/orders";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            if (!string.IsNullOrEmpty(orderId))
                parameters.Add("order_id", orderId);
            if (type != null)
                parameters.Add("type", type.ToString());
            if (count != 100 && count > 0)
                parameters.Add("count", count);
            if (after != null)
                parameters.Add("after", _dtHelper.LocalToUnixTime((DateTime)after));
            if (!string.IsNullOrEmpty(symbol))
                parameters.Add("currency", symbol);

            var response = await _restRepo.PostApi<ApiResponse<OrderInformation[]>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<Dictionary<string, object>[]> GetUserTransactions(string symbol)
        {
            return await OnGetUserTransactions(null, 0, 20, symbol);
        }

        public async Task<Dictionary<string, object>[]> GetUserTransactions(Search search)
        {
            return await OnGetUserTransactions(search, 0, 20, string.Empty);
        }

        public async Task<Dictionary<string, object>[]> GetUserTransactions(Search search, int offset, int count, string symbol)
        {
            return await OnGetUserTransactions(search, offset, count, symbol);
        }

        private async Task<Dictionary<string, object>[]> OnGetUserTransactions(Search? search, int offset, int count, string symbol)
        {
            count = count > 50 ? 50 : count;
            count = count < 1 ? 1 : count;
            var endpoint = $"/info/user_transactions";

            var url = baseUrl + endpoint;
            var lowSymbol = symbol.ToLower();

            var parameters = GetBasePostData();
            if (offset > 0)
                parameters.Add("offset", offset);
            if (count != 20)
                parameters.Add("count", count);
            if (search != null)
            {
                var searchVal = (Search)search;
                parameters.Add("searchGb", searchVal.ToString("D"));
            }
            if (!string.IsNullOrEmpty(symbol))
                parameters.Add("currency", symbol);
            
            var response = await _restRepo.PostApi<ApiResponse<Dictionary<string, object>[]>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<OrderResponse<OrderSettlement[]>> PlaceOrder(string pair, decimal quantity, decimal price, TransactionType type)
        {
            var endpoint = $"/trade/place";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            parameters.Add("order_currency", _helper.GetSymbol(pair));
            parameters.Add("Payment_currency", _helper.GetTradingSymbol(pair));
            parameters.Add("units", quantity);
            parameters.Add("price", price);
            parameters.Add("type", type.ToString());

            var response = await _restRepo.PostApi<OrderResponse<OrderSettlement[]>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response;
        }

        public async Task<OrderResponse<OrderSettlement[]>> MarketOrder(string pair, decimal quantity, TransactionType type)
        {
            var endpoint = type == TransactionType.ask ? $"/trade/market_sell" : $"/trade/market_buy";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            parameters.Add("currency", _helper.GetSymbol(pair));
            parameters.Add("units", quantity);

            var response = await _restRepo.PostApi<OrderResponse<OrderSettlement[]>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response;
        }

        public async Task<OrderDetail> GetOrderDetail(string orderId)
        {
            var orders = await OnGetOrderDetail(orderId, string.Empty, null);

            return orders.Length > 0 ? orders[0] : null;
        }

        public async Task<OrderDetail[]> GetOrderDetails(string symbol)
        {
            return await OnGetOrderDetail(string.Empty, symbol, null);
        }

        public async Task<OrderDetail[]> GetOrderDetails(string symbol, TransactionType type)
        {
            return await OnGetOrderDetail(string.Empty, symbol, type);
        }

        public async Task<OrderDetail[]> GetOrderDetails(TransactionType type)
        {
            return await OnGetOrderDetail(string.Empty, string.Empty, type);
        }

        private async Task<OrderDetail[]> OnGetOrderDetail(string orderId, string symbol, TransactionType? type)
        {
            var endpoint = $"/info/order_detail";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            if (!string.IsNullOrEmpty(orderId))
                parameters.Add("order_id", orderId);
            if (type != null)
                parameters.Add("type", type.ToString());
            if (!string.IsNullOrEmpty(symbol))
                parameters.Add("currency", symbol);

            var response = await _restRepo.PostApi<ApiResponse<OrderDetail[]>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response.data;
        }

        public async Task<bool> CancelOrder(string orderId)
        {
            return await OnCancelOrder(orderId, string.Empty, null);
        }

        public async Task<bool> CancelOrders(string symbol)
        {
            return await OnCancelOrder(string.Empty, symbol, null);
        }

        public async Task<bool> CancelOrders(string symbol, TransactionType type)
        {
            return await OnCancelOrder(string.Empty, symbol, type);
        }

        private async Task<bool> OnCancelOrder(string orderId, string symbol, TransactionType? type)
        {
            var endpoint = $"/trade/cancel";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            if (type != null)
                parameters.Add("type", type.ToString());
            if (!string.IsNullOrEmpty(orderId))
                parameters.Add("order_id", orderId);
            if (!string.IsNullOrEmpty(symbol))
                parameters.Add("currency", symbol);

            var response = await _restRepo.PostApi<Dictionary<string, string>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response["status"] == "0000" ? true : false;
        }

        public async Task<bool> Withdraw(string symbol, decimal quantity, string address)
        {
            return await OnWithdraw(symbol, quantity, address, string.Empty);
        }

        public async Task<bool> Withdraw(string symbol, decimal quantity, string address, string destination)
        {
            return await OnWithdraw(symbol, quantity, address, destination);
        }

        private async Task<bool> OnWithdraw(string symbol, decimal quantity, string address, string destination)
        {
            var endpoint = $"/trade/btc_withdrawal";

            var url = baseUrl + endpoint;

            var parameters = GetBasePostData();
            parameters.Add("units", quantity);
            parameters.Add("address", address);
            if (!string.IsNullOrEmpty(destination))
            {
                if(symbol.Equals("XMR"))
                    parameters.Add("destination", destination);
                else
                    parameters.Add("destination", int.Parse(destination));
            }
            parameters.Add("currency", symbol);

            var response = await _restRepo.PostApi<Dictionary<string, string>, Dictionary<string, object>>(url, parameters);

            _helper.HandleErrors(response);

            return response["status"] == "0000" ? true : false;
        }

        #endregion Private Api

        #region Parameter Helpers

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

        #endregion Parameter Helpers
    }
}
