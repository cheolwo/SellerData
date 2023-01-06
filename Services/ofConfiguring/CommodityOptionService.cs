using SellerCommon.SellerData.Model;
using System.Text;
using SellerCommon.SellerData.Services.ofExporting;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;

namespace SellerCommon.SellerData.Services.ofConfiguring
{
    public class ExportingOptionInfo
    {
        public string Name {get; set;}
        public string Quantity {get; set;}
        public string Price {get; set;}
    }
    public interface ICommodityOptionService
    {
        ExportingOptionInfo ConfiguringOptionInfo(SellerMCommodity sellerMCommodity, int RepresentatingPrice, double MarginRate);
        ExportingOptionInfo ConfigureOptionInfo(SellerMCommodity sellerMCommodity, double MarginRate);
    }
    public class CommodityOptionService : ICommodityOptionService
    {
        private Dictionary<CommodityOption, bool> DicExportingOptionInfo = new();
        private readonly ILogger<CommodityOptionService> _logger;

        public CommodityOptionService(ILogger<CommodityOptionService> logger)
        {
            _logger = logger;
        }
        // 1. List 형태의 옵션을 사전형태로 변환하는 단계
        // 2. 판매가 대비 옵션가의 조건이 충족되는지 사전을 돌며 확인하는 단계
        // 3. 조건 미충족인 경우 사전에서 제거하는 단계
        // 4. 남은 데이터로 옵션명, 수량, 가격 매핑 데이터를 구성하는 단계
        public ExportingOptionInfo ConfiguringOptionInfo(SellerMCommodity sellerMCommodity, int RepresentatingPrice, double MarginRate)
        {
            DicExportingOptionInfo = new();
            ConvertToDicExportingOptionInfo(sellerMCommodity.CommodityOptions);
            ConfirmOptionName(DicExportingOptionInfo);
            ConfirmOptionPrice(DicExportingOptionInfo, RepresentatingPrice, MarginRate);
            return ConfiguringExportingOptionInfo(DicExportingOptionInfo, RepresentatingPrice, MarginRate);
        }
        public ExportingOptionInfo ConfigureOptionInfo(SellerMCommodity sellerMCommodity, double MarginRate)
        {
            DicExportingOptionInfo = new();
            ConvertToDicExportingOptionInfo(sellerMCommodity.CommodityOptions);
            ConfirmOptionName(DicExportingOptionInfo);
            var Price = ConfigureRepreseintingPrice(sellerMCommodity, MarginRate);
            ConfirmOptionPrice(DicExportingOptionInfo, (int)Price, MarginRate);
            return ConfiguringExportingOptionInfo(DicExportingOptionInfo, (int)Price, MarginRate);
        }
        private double ConfigureRepreseintingPrice(SellerMCommodity sellerMCommodity, double MarginRate)
        {
            var options = sellerMCommodity.CommodityOptions;
            var MinSellerPrice = options.Min(e => e.SellerPrice.ToPrice());
            var MinOption = options.FirstOrDefault(e => e.SellerPrice.ToPrice().Equals(MinSellerPrice));
            var SellerOptionPrice = MinOption.SellerPrice.ToPrice();
            var ConsumerOptionPrice = MinOption.ConsumerPrice.ToPrice();
            var SplitSalesType = MinOption.SalesType.Split("원")[0];
            if (MinOption.SalesType.Contains("준수") || MinOption.SalesType.Equals(SplitSalesType) || MinOption.SalesType.Equals(MinOption.ConsumerPrice)) 
            {
                var CeilingPrice = Math.Ceiling(ConsumerOptionPrice / 10) * 10;
                if (CeilingPrice <= 1000) { throw new ArgumentException("판매불가가격"); }
                return CeilingPrice;
            }
            else
            {
                var Price = SellerOptionPrice * (1 + MarginRate);
                var CeilingPrice = Math.Ceiling(Price / 10) * 10;
                if (CeilingPrice <= 1000) { throw new ArgumentException("판매불가가격"); }
                return CeilingPrice;
            }
        }
        private void ConvertToDicExportingOptionInfo(List<CommodityOption> commodityOptions)
        {
            foreach(var option in commodityOptions)
            {
                DicExportingOptionInfo.Add(option, true);
            }
        }
        private void ConfirmOptionName(Dictionary<CommodityOption, bool> DicExportingOptionInfo)
        {
            List<string> OptionNames = new();
            foreach(var key in DicExportingOptionInfo.Keys)
            {
                if(key.OptionName.Contains("품절"))
                {
                    DicExportingOptionInfo[key] = false;
                }
                if(key.OptionName.Contains("단종"))
                {
                    DicExportingOptionInfo[key] = false;
                }
                if(key.OptionName.Length >= 24)
                {
                    key.OptionName = key.OptionName.Substring(0, 24);
                }
                key.OptionName = key.OptionName.Replace('*', 'X');
                key.OptionName = key.OptionName.Replace(',', ' ');
                OptionNames.Add(key.OptionName);
            }
            var OptionCount = OptionNames.Count;
            _logger.LogInformation(OptionCount.ToString());
            foreach(var optionName in OptionNames)
            {
                _logger.LogInformation(optionName.ToString());
                var Count = OptionNames.Where(e => e.Equals(optionName)).ToList().Count();
                _logger.LogInformation(Count.ToString());
                if (Count > 1) { _logger.LogError("옵션값중복"); throw new ArgumentException("옵션값중복"); }
            }
        }
        // 
        private void ConfirmOptionPrice(Dictionary<CommodityOption, bool> DicExportingOptionInfo, int RepresentatingPrice, double MarginRate)
        {
            foreach(var key in DicExportingOptionInfo.Keys)
            {
                var priceValue = key.SellerPrice.ToPrice();
                var SellerPrice = Math.Ceiling(priceValue / 10) * 10;
                if (RepresentatingPrice < 2000)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 2 < value || value < RepresentatingPrice)
                    {
                        DicExportingOptionInfo[key] = false;
                        continue;
                    }
                }
                if (RepresentatingPrice >= 2000 && RepresentatingPrice < 10000)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 2 < value || value < RepresentatingPrice * 0.5)
                    {
                        DicExportingOptionInfo[key] = false;
                        continue;
                    }
                }
                if (RepresentatingPrice >= 10000)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 1.5 < value || value < RepresentatingPrice * 0.5)
                    {
                        DicExportingOptionInfo[key] = false;
                        continue;
                    }
                }
            }
        }
        private ExportingOptionInfo ConfiguringExportingOptionInfo(Dictionary<CommodityOption, bool> DicExportingOptionInfo, int RepresentatingPrice, double MarginRate)
        {
            List<CommodityOption> options = new();
            StringBuilder optionValueBuilder = new();
            StringBuilder optionQuantityBuilder = new();
            StringBuilder optionPriceBuilder = new();
            ExportingOptionInfo exportingOptionInfo = new();

            foreach(var key in DicExportingOptionInfo.Keys)
            {
                if(DicExportingOptionInfo[key] == true)
                {
                    options.Add(key);
                }
            }
            foreach(var option in options)
            {
                if (optionValueBuilder.Length > 0)
                {
                    optionValueBuilder.Append(", ");
                    optionValueBuilder.Append(option.OptionName);
                }
                else { optionValueBuilder.Append(option.OptionName); }
            }
            foreach(var option in options)
            {
                if (optionQuantityBuilder.Length > 0)
                {
                    optionQuantityBuilder.Append(", ");
                    optionQuantityBuilder.Append("999");
                }
                else { optionQuantityBuilder.Append("999"); }
            }
            List<double> CheckValues = new();
            foreach(var option in options)
            {
                var SellerPrice = option.SellerPrice.ToPrice();
                var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                var AdjustValue = Math.Ceiling(MarginSellerPrice / 10) * 10;
                if (optionPriceBuilder.Length > 0)
                {
                    optionPriceBuilder.Append(", ");
                    CheckValues.Add(AdjustValue - RepresentatingPrice);
                    optionPriceBuilder.Append(AdjustValue - RepresentatingPrice);
                }
                else { optionPriceBuilder.Append(AdjustValue - RepresentatingPrice); }
            }
            if(!CheckValues.Contains(0))
            {
                throw new ArgumentException("옵션가0미포함");
            }
            exportingOptionInfo.Name = optionValueBuilder.ToString();
            exportingOptionInfo.Quantity = optionQuantityBuilder.ToString();
            exportingOptionInfo.Price = optionPriceBuilder.ToString();
            return exportingOptionInfo;
        }
    }
}