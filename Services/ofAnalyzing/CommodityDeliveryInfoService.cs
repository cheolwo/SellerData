using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SellerData.Services.ofAnalyzing
{
    public interface ICommodityDeliveryInfoService
    {
        Task SetDeliveryCodeByDeliveryInfo(SellerSMCommodity sellerSMCommodity);
    }
    public class CommodityDeliveryInfoService : ICommodityDeliveryInfoService
    {
        private readonly OpenMarketDataContext _openMarketDataContext;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        public CommodityDeliveryInfoService(OpenMarketDataContext openMarketDataContext,
                                                    SellerMarketDataContext sellerMarketDataContext)
        {
            _openMarketDataContext = openMarketDataContext;
            _sellerMarketDataContext = sellerMarketDataContext;
        }
        /// <summary>
        /// 1. DeliveryCode 를 로드하는 단계
        /// 2. SellerMCommodity의 DeliveryInfo 를 분석하여 택배명을 도출하는 단계
        /// 3. 상기 택배명과 일치하는 택배Code를 리턴하는 단계
        /// 4. 상기 Code 정보로 SellerMCommodity를 Post 또는 Put 하는 단계를 포함한다.
        /// </summary>
        /// <param name="sellerMCommodity"></param>
        /// <returns></returns>
        public async Task SetDeliveryCodeByDeliveryInfo(SellerSMCommodity sellerSMCommodity)
        {
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            if (sellerSMCommodity.SellerMCommodity.DeliveryContent == null) { throw new ArgumentNullException("DeliveryContent"); }
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
                    foreach(var charvalue in value.ToCharArray())
                    {
                        if(charvalue == ',')
                        {
                            continue;
                        }
                        stringBuilder.Append(charvalue);
                    }
                    CommonDeliveryFee = int.Parse(stringBuilder.ToString());
                    sellerSMCommodity.CommonDeliveryFee = CommonDeliveryFee;
                    Console.WriteLine(CommonDeliveryFee);
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
                    Console.WriteLine(JejudoDeliveryFee);
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
                    Console.WriteLine(MountainDeliveryFee);
                    sellerSMCommodity.MountainDeliveryFee = MountainDeliveryFee;
                    continue;
                }
                if (SplitDeliveryInfos[i].Contains("/"))
                {
                    var CommonDeliveryFeeKwds = SplitDeliveryInfos[i].Split(" ");
                    var value = CommonDeliveryFeeKwds[CommonDeliveryFeeKwds.Length - 1];
                    if (value.Length >= 10)
                    {
                        var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("롯데택배")).FirstOrDefault();
                        sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                        continue;
                    }
                    var DeliveryAgencyValue = Regex.Replace(value, @" ", "");
                    if(value.Contains("한진택배"))
                    {
                        var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("한진택배")).FirstOrDefault();
                        sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                        continue;
                    }
                    if (value.Contains("롯데택배"))
                    {
                        var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("롯데택배")).FirstOrDefault();
                        sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                        continue;
                    }
                    if (value.Contains("대한통운"))
                    {
                        var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("대한통운")).FirstOrDefault();
                        sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                        continue;
                    }
                    if (value.Contains("로젠"))
                    {
                        var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("로젠")).FirstOrDefault();
                        sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                        continue;
                    }
                    if (value.Contains("로젠"))
                    {
                        var LotteCode = CommodityDeliveryCodes.Where(e => e.Name.Contains("로젠")).FirstOrDefault();
                        sellerSMCommodity.DeliveryCode = LotteCode?.Code ?? "";
                        continue;
                    }
                }
            }
            if(sellerSMCommodity.CommonDeliveryFee > 0)
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
