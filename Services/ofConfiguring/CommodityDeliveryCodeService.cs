using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SellerData.Services.ofConfiguring
{
    public interface ICommodityDeliveryCodeService
    {
        Task SetCodeByExcelData(string path);
        Task SetCode(SellerSMCommodity sellerSMCommodity);
    }
    public class CommodityDeliveryCodeService : ICommodityDeliveryCodeService
    {
        private readonly OpenMarketDataContext _openMarketDataContext;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private Application excelApp;
        public CommodityDeliveryCodeService(OpenMarketDataContext openMarketDataContext, SellerMarketDataContext sellerMarketDataContext)
        {
            _openMarketDataContext = openMarketDataContext;
            _sellerMarketDataContext = sellerMarketDataContext;
        }
        private CommodityDeliveryCode commodityDeliveryCode = new();
        public async Task SetCodeByExcelData(string path)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(path);
            Worksheet workSheet = excelApp.ActiveSheet;
            Console.WriteLine(workSheet.Cells[1, 1].Value);
            for (int i = 2; i <= 161; i++)
            {
                commodityDeliveryCode.Code = workSheet.Cells[i, 1].Value?.ToString() ?? "";
                commodityDeliveryCode.Name = workSheet.Cells[i, 2].Value?.ToString() ?? "";
                Console.WriteLine(commodityDeliveryCode.Name);
                await commodityDeliveryCode.PostAsync(_openMarketDataContext);
                commodityDeliveryCode.Id = null;
            }
        }

        public async Task SetCode(SellerSMCommodity sellerSMCommodity)
        {
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            if (sellerSMCommodity.SellerMCommodity.DeliveryContent == null)
            {
                return;
            }
            var CommodityDeliveryCodes = await _openMarketDataContext.GetsAsync<CommodityDeliveryCode>();
            var DeliveryInfo = sellerSMCommodity.SellerMCommodity.DeliveryContent;
            var SplitDeliveryInfos = DeliveryInfo.Split("원");
            int cnt = 0;
            int CommonDeliveryFee;
            int JejudoDeliveryFee;
            int MountainDeliveryFee;
            for (int i = 0; i < SplitDeliveryInfos.Length; i++)
            {
                if (SplitDeliveryInfos[i].Contains("일반"))
                {
                    var CommonDeliveryFeeKwds = SplitDeliveryInfos[i].Split(" ");
                    var value = CommonDeliveryFeeKwds[CommonDeliveryFeeKwds.Length - 1];
                    StringBuilder stringBuilder = new();
                    foreach (var charvalue in value.ToCharArray())
                    {
                        if (charvalue == ',')
                        {
                            continue;
                        }
                        stringBuilder.Append(charvalue);
                    }
                    CommonDeliveryFee = int.Parse(stringBuilder.ToString());
                    sellerSMCommodity.CommonDeliveryFee = CommonDeliveryFee;
                    continue;
                }
                if (SplitDeliveryInfos[i].Contains("제주도"))
                {
                    var CommonDeliveryFeeKwds = SplitDeliveryInfos[i].Split(" ");
                    var value = CommonDeliveryFeeKwds[CommonDeliveryFeeKwds.Length - 1];
                    StringBuilder stringBuilder = new();
                    foreach (var charvalue in value.ToCharArray())
                    {
                        if (charvalue == ',')
                        {
                            continue;
                        }
                        stringBuilder.Append(charvalue);
                    }
                    JejudoDeliveryFee = int.Parse(stringBuilder.ToString());
                    sellerSMCommodity.JejudoDeliveryFee = JejudoDeliveryFee;
                    continue;
                }
                if (SplitDeliveryInfos[i].Contains("도서산간"))
                {
                    var CommonDeliveryFeeKwds = SplitDeliveryInfos[i].Split(" ");
                    var value = CommonDeliveryFeeKwds[CommonDeliveryFeeKwds.Length - 1];
                    StringBuilder stringBuilder = new();
                    foreach (var charvalue in value.ToCharArray())
                    {
                        if (charvalue == ',')
                        {
                            continue;
                        }
                        stringBuilder.Append(charvalue);
                    }
                    MountainDeliveryFee = int.Parse(stringBuilder.ToString());
                    sellerSMCommodity.MountainDeliveryFee = MountainDeliveryFee;
                    continue;
                }
                var CommonDeliveryFeeKwd = SplitDeliveryInfos[i].Split(" ");
                var Buffervalue = CommonDeliveryFeeKwd[CommonDeliveryFeeKwd.Length - 1];
                var DeliveryAgencyValue = Regex.Replace(Buffervalue, @" ", "");
                if (DeliveryAgencyValue.Contains("한진"))
                {
                    var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("한진택배")).FirstOrDefault();
                    sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                    continue;
                }
                if (DeliveryAgencyValue.Contains("롯데"))
                {
                    var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("롯데택배")).FirstOrDefault();
                    sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                    continue;
                }
                if (DeliveryAgencyValue.Contains("대한통운") || DeliveryAgencyValue.Contains("CJ") || DeliveryAgencyValue.Contains("cj") || DeliveryAgencyValue.Contains("Cj"))
                {
                    var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("대한통운")).FirstOrDefault();
                    sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                    continue;
                }
                if (DeliveryAgencyValue.Contains("로젠"))
                {
                    var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("로젠")).FirstOrDefault();
                    sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                    continue;
                }
            }
            if (sellerSMCommodity.CommonDeliveryFee > 0)
            {
                if (sellerSMCommodity.CommonDeliveryFee >= sellerSMCommodity.MountainDeliveryFee)
                {
                    sellerSMCommodity.MountainDeliveryFee += sellerSMCommodity.CommonDeliveryFee;
                }
                if (sellerSMCommodity.CommonDeliveryFee >= sellerSMCommodity.JejudoDeliveryFee)
                {
                    sellerSMCommodity.JejudoDeliveryFee += sellerSMCommodity.CommonDeliveryFee;
                }
            }
            await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
        }
    }
}
