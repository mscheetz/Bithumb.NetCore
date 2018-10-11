using Bithumb.NetCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bithumb.NetCore.Data.Interface
{
    public interface IBithumbRepository
    {
        #region Public Api

        Task<Ticker> GetTicker(string symbol);

        Task<OrderBook> GetOrderBook(string symbol);

        Task<Transaction> GetTransactionHistory(string symbol);

        #endregion Public Api

        #region Private Api

        Task<Account> GetAccount(string symbol);

        Task<Balance[]> GetBalance(string symbol);

        Task<string> GetDepositAddress(string symbol);

        Task<PrivateTicker> GetPrivateTicker(string pair);

        Task<OrderInformation> GetOrder(string orderId);

        Task<OrderInformation[]> GetOrders(string symbol);

        Task<OrderInformation[]> GetOrders(string symbol, TransactionType type);

        Task<OrderInformation[]> GetOrders(TransactionType type);

        Task<OrderInformation[]> GetOrders(string symbol, TransactionType type, int count, DateTime after);
        
        Task<Dictionary<string, object>[]> GetUserTransactions(string symbol);

        Task<Dictionary<string, object>[]> GetUserTransactions(Search search);

        Task<Dictionary<string, object>[]> GetUserTransactions(Search search, int offset, int count, string symbol);

        Task<OrderResponse<OrderSettlement[]>> PlaceOrder(string pair, decimal quantity, decimal price, TransactionType type);

        Task<OrderResponse<OrderSettlement[]>> MarketOrder(string pair, decimal quantity, TransactionType type);

        Task<OrderDetail> GetOrderDetail(string orderId);

        Task<OrderDetail[]> GetOrderDetails(string symbol);

        Task<OrderDetail[]> GetOrderDetails(string symbol, TransactionType type);

        Task<OrderDetail[]> GetOrderDetails(TransactionType type);

        Task<bool> CancelOrder(string orderId);

        Task<bool> CancelOrders(string symbol);

        Task<bool> CancelOrders(string symbol, TransactionType type);

        Task<bool> Withdraw(string symbol, decimal quantity, string address);

        Task<bool> Withdraw(string symbol, decimal quantity, string address, string destination);

        #endregion Private Api
    }
}
