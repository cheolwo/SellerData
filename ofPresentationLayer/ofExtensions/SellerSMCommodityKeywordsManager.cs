namespace SellerCommon.SellerData.ofPresentationLayer.ofExtensions
{
    public static class SellerSMCommodityKeywordsManager
    {
        public static List<string> SplitKeywords(this string SellerSMCommodityKeywords)
        {
            List<string> strings = new();
            List<string> SplitsValues = new();
            var SplitsByComma = SellerSMCommodityKeywords.Split(',');
            foreach(var splits in SplitsByComma)
            {
                var splitsByVoid = splits.Split(' ');
                foreach(var splitvalueByVoid in splitsByVoid)
                {
                    SplitsValues.Add(splitvalueByVoid);
                }
            }
            foreach (var value in SplitsValues)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value[0] == '#')
                    {
                        if (value[value.Length - 1] == ',')
                        {
                            var subvalue2 = value.Substring(1, value.Length - 2);
                            strings.Add(subvalue2);
                            continue;
                        }
                        var subValue = value.Substring(1, value.Length - 1);
                        strings.Add(subValue);
                        continue;
                    }
                    if (value[value.Length - 1] == ',')
                    {
                        var subvalue3 = value.Substring(0, value.Length - 2);
                        strings.Add(subvalue3);
                        continue;
                    }
                    strings.Add(value);
                }
            }
            return strings;
        }
    }
}