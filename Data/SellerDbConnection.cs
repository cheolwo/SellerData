using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.Data
{
    public static class SellerDbConnectionString
    {
        public const string MarketDbConnection= "Data Source=DESKTOP-HKC31JI\\SQLEXPRESS;Integrated Security=True; Database=SellerMarketDb; Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public const string OpenMarketDbConnection = "Data Source=DESKTOP-HKC31JI\\SQLEXPRESS;Integrated Security=True; Database=OpenMarketDb; Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public const string OnChannelDbConnection = "";
    }
}
