using Bithumb.NetCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Core
{
    public class Helper
    {
        public string GetSymbol(string pair)
        {
            return pair.Substring(0, pair.IndexOf("-") - 1);
        }

        public string GetTradingSymbol(string pair)
        {
            return pair.Substring(pair.IndexOf("-") + 1);
        }

        public void HandleErrors<T>(ApiResponse<T> response)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("status", response.status);
            dictionary.Add("message", response.message);

            HandleErrors(dictionary);
        }

        public void HandleErrors<T>(OrderResponse<T> response)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("status", response.status);
            dictionary.Add("message", response.message);

            HandleErrors(dictionary);
        }

        public void HandleErrors(Dictionary<string, string> response)
        {
            if (!response["status"].Equals("0000"))
            {
                if (!response["status"].Equals("5600"))
                    throw new Exception(GetError(response["status"]));
                else
                    throw new Exception(response["message"]);
            }
        }

        public string GetError(string code)
        {
            switch (code)
            {
                case "5100":
                    return "Bad Request";
                case "5200":
                    return "Not Member";
                case "5300":
                    return "Invalid Apikey";
                case "5302":
                    return "Method Not Allowed";
                case "5400":
                    return "Database Fail";
                case "5500":
                    return "Invalid Parameter";
                case "5600":
                    return "Custom notice";
                case "5900":
                    return "Unknown Error";
                default:
                    return "Unknown Error";
            }
        }
    }
}
