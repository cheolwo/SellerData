using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.Services.ofCollecitng
{
    public interface IRelKwdSettingService
    {
        Task<List<CommodityAnalyzedInfo>> GetRelKwd(List<string> ImportantKwds, string NamingType, string SellerMCommodityId, int FilterMonthlyMobileQc, int MaxRelKwds, OpenMarketCommonCategory openMarketCommonCategory);
    }
    public class RelKwdSettingService : IRelKwdSettingService
    {
        private readonly INaverSerachRelKwdAPIService _naverSerachRelKwdAPIService;
        private readonly IPointStringServie _pointStringServie;
        public RelKwdSettingService(INaverSerachRelKwdAPIService naverSerachRelKwdAPIService, IPointStringServie pointStringServie)
        {
            _naverSerachRelKwdAPIService = naverSerachRelKwdAPIService;
            _pointStringServie = pointStringServie; 
        }
        public async Task<List<CommodityAnalyzedInfo>> GetRelKwd(List<string> ImportantKwds, string NamingType, string SellerMCommodityId, int FilterMonthlyMobileQc, int MaxRelKwds, OpenMarketCommonCategory openMarketCommonCategory)
        {
            if(ImportantKwds.Count == 0 || ImportantKwds == null) { throw new ArgumentNullException(nameof(ImportantKwds)); }
            var ImportantKwd = ImportantKwds.FirstOrDefault();
            var HintKwds = ConfigureHinKwds(ImportantKwds, ImportantKwd, NamingType);
            var CommodityAnalyzeInfos = await _naverSerachRelKwdAPIService.GetRelKwdStat(HintKwds, SellerMCommodityId, FilterMonthlyMobileQc);
            if(CommodityAnalyzeInfos.Count == 0)
            {
                List<string> newHintKwds = new();
                int cnt = 0;           
                newHintKwds.Add(ImportantKwd); cnt++;
                if (openMarketCommonCategory.Category3 != null)
                {
                    var values = openMarketCommonCategory.Category3.Split("/");
                    foreach(var value in values)
                    {
                        newHintKwds.Add(value);
                        cnt++;
                    }
                }
                if (openMarketCommonCategory.Category4 != null && openMarketCommonCategory.Category4 != "")
                {
                    var values = openMarketCommonCategory.Category4.Split("/");
                    foreach (var value in values)
                    {
                        if(cnt >= 5) { break; }
                        newHintKwds.Add(value);
                    }
                }
                CommodityAnalyzeInfos = await _naverSerachRelKwdAPIService.GetRelKwdStat(newHintKwds, SellerMCommodityId, FilterMonthlyMobileQc);
                if(CommodityAnalyzeInfos.Count == 0) { throw new ArgumentException("직접분석"); }
            }
            var RelKwds = ConfigureRelKwds(CommodityAnalyzeInfos);
            var ComparingStrings = ConfigureComparingStrings(ImportantKwds, openMarketCommonCategory.Category3, openMarketCommonCategory.Category4);
            var PointNodes = _pointStringServie.GetPointRelKwdsNodeByComparetingStrings(RelKwds, ComparingStrings);
            return FindInfoByNodeStrings(PointNodes, CommodityAnalyzeInfos, MaxRelKwds);
        }
        private List<CommodityAnalyzedInfo> FindInfoByNodeStrings(List<PointNode<string>> pointNodes, List<CommodityAnalyzedInfo> commodityAnalyzedInfos, int MaxRelKwds)
        {
            int cnt = 0;
            List<CommodityAnalyzedInfo> FindcommodityAnalyzeds = new();
            foreach(var node in pointNodes)
            {
                if(cnt >= MaxRelKwds) { break; }
                var Info = commodityAnalyzedInfos.Find(e => e.SeletedKeywords.Equals(node.t));
                if(Info != null) { FindcommodityAnalyzeds.Add(Info); cnt++; }
            }
            return FindcommodityAnalyzeds;
        }
        private List<string> ConfigureHinKwds(List<string> ImportantKwds, string ImportantKwd, string NameType)
        {
            if(NameType == NamingType.MainForm || NameType == NamingType.MixForm)
            {
                return ConfigureHintKwdsByMixAndMainForm(ImportantKwds, ImportantKwd);
            }
            else //NamingType.CombinationForm
            {
                return ConfigureHintKwdsByCombinationForm(ImportantKwds, ImportantKwd);
            }
        }
        private List<string> ConfigureHintKwdsByCombinationForm(List<string> ImportantKwds, string ImportantKwd)
        {
            int cnt = 0;
            List<string> HintKwds = new();
            foreach(var kwd in ImportantKwds)
            {
                if(kwd == ImportantKwd) { continue; }
                if(cnt >= 5) { break; }
                HintKwds.Add(kwd);
            }
            return HintKwds;
        }
        private List<string> ConfigureHintKwdsByMixAndMainForm(List<string> ImportantKwds, string ImportantKwd)
        {
            int cnt = 0;
            List<string> HintKwds = new();
            foreach (var kwd in ImportantKwds)
            {
                if (kwd == ImportantKwd) { continue; }
                if (cnt >= 5) { break; }
                var Point = _pointStringServie.GetPoint(kwd.ToCharArray(), ImportantKwd.ToCharArray());
                if(!kwd.Contains(ImportantKwd) && Point < ImportantKwd.Length)
                {
                    //예를들어 ImportantKwd : 병원
                    // Kwd : 요양원
                    HintKwds.Add(kwd);
                    cnt++;
                    continue;
                }
                if(kwd.Contains(ImportantKwd))
                {
                    HintKwds.Add(kwd);
                    cnt++;
                    continue;
                }
                if(!kwd.Contains(ImportantKwd))
                {
                    HintKwds.Add(kwd + ImportantKwd);
                    cnt++;
                    continue;
                }
            }
            return HintKwds;
        }

        private List<string> ConfigureComparingStrings(List<string> ImportantKwds, string Category3, string Category4)
        {
            List<string> strings = new List<string>();
            foreach(var value in ImportantKwds)
            {
                strings.Add(value);
            }
            if(Category3 != null) { strings.Add(Category3); }
            if(Category4 != null) { strings.Add(Category4); }
            return strings;
        }
        private List<string> ConfigureRelKwds(List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
        {
            List<string> strings = new();
            foreach(var value in commodityAnalyzedInfos)
            {
                strings.Add(value.SeletedKeywords);
            }
            return strings;
        }
    }
    public interface IPointStringServie
    {
        List<PointNode<string>> GetPointRelKwdsNodeByComparetingStrings(List<string> RelKwds, List<string> ComparingStrings, int MaxNode);
        List<PointNode<string>> GetPointRelKwdsNodeByComparetingStrings(List<string> RelKwds, List<string> ComparingStrings);
        int GetPoint(char[] chars, char[] compareChars);
    }
    public class PointStringService : IPointStringServie
    {
        public List<PointNode<string>> GetPointRelKwdsNodeByComparetingStrings(List<string> RelKwds, List<string> ComparingStrings)
        {
            var pointNodes = ConfigurePoingNode(RelKwds);
            GetPointByComparingStrings(pointNodes, ComparingStrings);
            pointNodes.Sort();
            pointNodes.Reverse();
            return pointNodes;
        }
        public List<PointNode<string>> GetPointRelKwdsNodeByComparetingStrings(List<string> RelKwds, List<string> ComparingStrings, int MaxNode)
        {
            var pointNodes = ConfigurePoingNode(RelKwds);
            GetPointByComparingStrings(pointNodes, ComparingStrings);
            pointNodes.Sort();
            pointNodes.Reverse();
            return pointNodes.Take(MaxNode).ToList();
        }
        private List<PointNode<string>> ConfigurePoingNode(List<string> RelKwds)
        {
            List<PointNode<string>> pointNodes = new();
            foreach(var value in RelKwds)
            {
                PointNode<string> pointNode = new(value);
                pointNodes.Add(pointNode);
            }
            return pointNodes;
        }
        private void GetPointByComparingStrings(List<PointNode<string>> pointNodes, List<string> ComparingStrings)
        {
            foreach(var node in pointNodes)
            {
                var nodecharsArray = node.t.ToCharArray();
                foreach(var compareString in ComparingStrings)
                {
                    var compareCharArray = compareString.ToCharArray();    
                    var point = GetPoint(nodecharsArray, compareCharArray);
                    node.Point += point;
                }
            }
        }
        public int GetPoint(char[] chars, char[] compareChars)
        {
            int point = 0;
            foreach(var charvalue in chars)
            {
                var value = compareChars.FirstOrDefault(e => e.Equals(charvalue));
                if(value == charvalue) { point++; }
            }
            return point;
        }
    }
}
