using SellerData.ofDataAccessLayer.ofDataContext;
using SellerCommon.SellerData.Model;
using Microsoft.Office.Interop.Excel;
using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using System.Text;
using SellerData.Services.ofConfiguring;
using SellerData.ofDataAccessLayer.ofModel;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services.ofConfiguring;
using SellerCommon.SellerData.ofDbContext;
using Microsoft.Extensions.Logging;

namespace SellerCommon.SellerData.Services.ofExporting
{
    public static class SmartStoreCommodityRegisterExcelForm
    {
        // 기본정보
        public const int SellerCommodityCode = 1;
        public const int CategoryCode = 2;
        public const int NameofCommodity = 3;
        public const int StatusofCommodity = 4;
        public const int Price = 5;
        public const int VAT = 6;
        public const int Quantity = 7;
        public const int OptionForm = 8;
        public const int OptionName = 9;
        public const int OptionValue = 10;
        public const int OptionPrice = 11;
        public const int OptionQuantity = 12;
        public const int RepresentativeImgae = 18;
        public const int AdditionalImage = 19;
        public const int DetailImage = 20;

        //추가정보
        public const int ManuFacturingCompany = 22;
        public const int OriginCode = 25;
        public const int ImportingCompany = 26;

        //배송정보
        public const int DeliveryMethod = 31;
        public const int DeliveryCode = 32;
        public const int DeliveryForm = 33; // 무료, 조건부 무료, 유료... 등
        public const int CommonDeliveryFee = 34;
        public const int CommonPayingMethod = 35;
        public const int ConditionFreeDevliery = 36;
        public const int ReturnDeliveryFee = 42;
        public const int ChangeDeliveryFee = 43;
        // A/S
        public const int AsPhoneNumber = 52; // 52 A/S 전화
        public const int AsInfo = 53;// 53 A/S 안내
        // Discount
        public const int PCDiscountPoint = 57;// 55 PC 즉시할인
        public const int MobileDiscountPoint = 58;// 57 모바일 즉시할인
        public const int ConditionQuantityDiscount = 59; // 59 복수구매 즉시할인 조건값
        public const int QuantityDiscountPoint = 61;// 61 복수구매할인 값
        public const int PointofPurchasing = 63;// 63 상품구매시 포인트 지급 값
        public const int TextReviewPoint = 65;// 65 텍스트리뷰 작성시 지급 포인트
        public const int PhotoReviewPoint = 66;// 66 포토/동영상 리뷰 작성 시 지급 포인트
        public const int TextReviewUsingMonth = 67;// 67 한달사용 텍스트리뷰 작성시 지급 포인트
        public const int PohtoReviewUsingMonth = 68;// 68 한달사용 포토/동영상 리뷰 지급시 포인트
        public const int CustomerReviewPoint = 69;// 69 톡톡친구/스토어찜 고객리뷰 작성시 지급 포인트
        public const int InstallmentMonth = 70;// 70 무이자 할부 개월
        public const int OpenEvaluating = 73;// 73 구매평 노출여부
    }
    public static class PriceString
    {
        public static double ToPrice(this string Price)
        {
            StringBuilder stringBuilder = new();
            var kwds = Price.Split(',');
            foreach (var kwd in kwds)
            {
                stringBuilder.Append(kwd);
            }
            return double.Parse(stringBuilder.ToString());
        }
        public static string SplitByKwdAndConcat(this string Input, string Kwd)
        {
            var splitvalues = Input.Split(Kwd);
            StringBuilder stringBuilder = new();
            foreach (var value in splitvalues)
            {
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }
    }
    public interface ICommodityExportingService
    {
        Task ExportingToExcel(SellerSMCommodity sellerSMCommodity, double MarginRate, string NameofOpenMarket, string FilePath, string SavePath);
        Task ToExcelCommodityList(List<SellerSMCommodity> sellerSMCommodities, double MarginRate, string NameofOpenMarket, string FilePath, string SavePath);
    }
    public class CommodityExportingService : ICommodityExportingService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly OpenMarketDataContext _openMarketDataContext;
        private readonly SellerMarketDbContext _sellerMarketDbContext;
        private readonly ICommodityOriginCodeService _commodityOriginCodeService;
        private readonly ICommodityOptionService _commodityOptionService;
        private readonly ISellerMMCommodityRepository _sellerMMCommodityRepository;
        private readonly IConfiguration _configuration;
        private Application excelApp;
        private readonly ILogger<CommodityExportingService> _logger;
        private string _marketDeliveryInfo;
        public CommodityExportingService(SellerMarketDataContext sellerMarketDataContext,
            OpenMarketDataContext openMarketDataContext,
            SellerMarketDbContext sellerMarketDbContext,
            IConfiguration configuration,
            ISellerMMCommodityRepository sellerMMCommodityRepository,
            ICommodityOptionService commodityOptionService,
            ICommodityOriginCodeService commodityOriginCodeService,
            ILogger<CommodityExportingService> logger)
        {
            _sellerMarketDataContext = sellerMarketDataContext;
            _commodityOriginCodeService = commodityOriginCodeService;
            _commodityOptionService = commodityOptionService;
            _sellerMMCommodityRepository = sellerMMCommodityRepository;
            _openMarketDataContext = openMarketDataContext;
            _sellerMarketDbContext = sellerMarketDbContext;
            _configuration = configuration;
            _logger = logger;
            _marketDeliveryInfo = _configuration.GetSection("Market")["DeliveryInfo"];
        }
        public async Task ExportingToExcel(SellerSMCommodity sellerSMCommodity, double MarginRate, string NameofOpenMarket, string FilePath, string SavePath)
        {
            if (sellerSMCommodity.SellerMarket == null) {sellerSMCommodity.SellerMarket = await _sellerMarketDataContext.GetByIdAsync<SellerMarket>(sellerSMCommodity.SellerMarketId);}
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            if (sellerSMCommodity.Name == null) { throw new ArgumentNullException(nameof(sellerSMCommodity.Name)); }
            if (sellerSMCommodity.DeliveryCode == null) { throw new ArgumentNullException(nameof(sellerSMCommodity.DeliveryCode)); }
            var SellerOpenMarkets = await _sellerMarketDataContext.GetsAsync<SellerOpenMarket>();
            var OpenMarket = SellerOpenMarkets.Where(e => e.Name.Equals(NameofOpenMarket)).FirstOrDefault();
            if (OpenMarket == null) { throw new ArgumentNullException(nameof(OpenMarket)); }

            SellerMMCommodity sellerMMCommodity = new();
            sellerMMCommodity.SellerSMCommodityId = sellerSMCommodity.Id;
            sellerMMCommodity.SellerMCommodityId = sellerSMCommodity.SellerMCommodityId;
            sellerMMCommodity.SellerMarketId = sellerSMCommodity.SellerMarketId;
            sellerMMCommodity.SellerOpenMarketId = OpenMarket.Id;
            await sellerMMCommodity.PostAsync(_sellerMarketDataContext);

            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(FilePath);
            Worksheet worksheet = excelApp.ActiveSheet;

            int cntrow = 2;
            while (worksheet.Cells[cntrow, 1].Value != null) { cntrow++; }

            await MappingForBasicCommodityInfo(cntrow, MarginRate, sellerSMCommodity, worksheet);
            await MappingForDetailCommodityInfo(cntrow, sellerSMCommodity, worksheet);
            await MappingForDeliveryInfo(cntrow, sellerSMCommodity, worksheet);

            var SaveDirectoryPath = Path.Combine(SavePath, sellerSMCommodity.SellerMCommodity.Code, NameofOpenMarket);
            Directory.CreateDirectory(SaveDirectoryPath);
            var SaveFilePath = Path.Combine(SaveDirectoryPath, sellerMMCommodity.Id);
            workbook.SaveAs2(SaveFilePath);
            workbook.Close();
            excelApp.Quit();
        }
        public async Task ToExcelCommodityList(List<SellerSMCommodity> sellerSMCommodities, double MarginRate, string NameofOpenMarket, string FilePath, string SavePath)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(FilePath);
            Worksheet worksheet = excelApp.ActiveSheet;
            int cntrow = 2;
            int Count = 1;
            while (worksheet.Cells[cntrow, 1].Value != null) { cntrow++; }
            var OpenMarkets = await _sellerMarketDataContext.GetsAsync<SellerOpenMarket>();
            var OpenMarket = OpenMarkets.FirstOrDefault(e => e.Name.Equals(NameofOpenMarket));
            if (OpenMarket == null) { throw new ArgumentException("존재하지 않는 OpenMarket 입니다."); }
            var SMCommodity = sellerSMCommodities.FirstOrDefault();
            var FilterCategoryCodes = SMCommodity.SellerMarket.FilterCategoryCodes;

            int CurrentStateCount = 1;
            foreach (var sellerSMCommodity in sellerSMCommodities)
            {
                if(FilterCategoryCodes.FirstOrDefault(e=>e.Equals(sellerSMCommodity.Code)) != null) { continue; }
                try
                {
                    _logger.LogInformation(sellerSMCommodities.Count.ToString());
                    _logger.LogInformation(CurrentStateCount.ToString());
                    CurrentStateCount++;
                    if (cntrow == 503)
                    {
                        var savePath = FileListNameBuilder(SavePath, Count);
                        workbook.SaveAs2(savePath);
                        Count++;
                        cntrow = 3;
                    }
                    if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
                    if (sellerSMCommodity.SellerMCommodity == null) { continue; /*throw new ArgumentException("아직 엑셀로 전환하기에는 이릅니다."); */}
                    if (sellerSMCommodity.SellerMCommodity.DetailImages.Count == 0) { continue; }

                    await MappingForBasicCommodityInfo(cntrow, MarginRate, sellerSMCommodity, worksheet);
                    await MappingForDetailCommodityInfo(cntrow, sellerSMCommodity, worksheet);
                    MappingForASAndPoint(cntrow, sellerSMCommodity, worksheet);
                    await MappingForDeliveryInfo(cntrow, sellerSMCommodity, worksheet);
                }
                catch
                {
                    continue;
                }
                cntrow++;
            }
            var Path = FileListNameBuilder(SavePath, Count);
            workbook.SaveAs2(Path);
            workbook.Close();
            excelApp.Quit();
            
        }
        private string FileListNameBuilder(string SavePath, int Count)
        {
            return SavePath + "-" + Count.ToString();
        }
        private async Task MappingForBasicCommodityInfo(int row, double MarginRate, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        {
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.SellerCommodityCode].Value = sellerSMCommodity.GetBasicCode();
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.CategoryCode].Value = sellerSMCommodity.Code;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.NameofCommodity].Value = sellerSMCommodity.SellerMCommodity.Name.Replace('*', 'X');
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.StatusofCommodity].Value = "신상품";

            var options = sellerSMCommodity.SellerMCommodity.CommodityOptions;
            var MinSellerPrice = options.Min(e => e.SellerPrice.ToPrice());
            var MinOption = options.FirstOrDefault(e => e.SellerPrice.ToPrice().Equals(MinSellerPrice));
            var SellerOptionPrice = MinOption.SellerPrice.ToPrice();
            var ConsumerOptionPrice = MinOption.ConsumerPrice.ToPrice();

            if (MinOption.SalesType.Contains("준수") || MinOption.SalesType.Equals(MinOption.ConsumerPrice))
            {
                var CeilingPrice = Math.Ceiling(ConsumerOptionPrice / 10) * 10;
                worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value = CeilingPrice;
                if(CeilingPrice <= 1000) { throw new ArgumentException("판매불가가격"); }
            }
            else
            {
                var Price = SellerOptionPrice * (1 + MarginRate);
                var CeilingPrice = Math.Ceiling(Price / 10) * 10;
                worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value = CeilingPrice;
                if (CeilingPrice <= 1000) { throw new ArgumentException("판매불가가격"); }
            }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.VAT].Value = "과세상품";
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Quantity].Value = 9999;
            // 1. 길이 25자 이상 X
            // 2. 특수문자 미포함
            if(sellerSMCommodity.SellerMCommodity.CommodityOptions.Count > 0)
            {
                worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionForm].Value = "조합형";
                worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionName].Value = "옵션명";
                var sellingPrice = worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value;
                var ExportingCommodityOption = _commodityOptionService.ConfiguringOptionInfo(sellerSMCommodity.SellerMCommodity, (int)sellingPrice, MarginRate);
                if(ExportingCommodityOption.Name.Split(',').Length == 1)
                {
                    worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionForm].Value = "단독형";
                    var OptionPrice = int.Parse(ExportingCommodityOption.Price);
                    worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionPrice].Value = "";
                    var SellerPrice = int.Parse(worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value);
                    var Price = OptionPrice + SellerPrice;
                    worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price] = Price;
                }
                else
                {
                    worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionValue].Value = ExportingCommodityOption.Name;
                    worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionQuantity].Value = ExportingCommodityOption.Quantity;
                    worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionPrice].Value = ExportingCommodityOption.Price;
                }
            }
            else
            {
                worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionForm].Value = "단독형";
            }
            // MappingForOptionValue(row, sellerSMCommodity, worksheet);
            // MappingForOptionPrice(row, MarginRate, sellerSMCommodity, worksheet);
            await MappingForImage(row, sellerSMCommodity, worksheet);
        }
        private void MappingForOptionValue(int row, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        {
            List<string> optionNames = new();
            int QuantityCheck = 0;
            // 품절체크
            foreach (var value in sellerSMCommodity.SellerMCommodity.CommodityOptions)
            {
                optionNames.Add(value.OptionName.SplitByKwdAndConcat("(품절)"));
                QuantityCheck++;
            }
            StringBuilder optionValueBuilder = new();
            foreach (var checkValue in optionNames)
            {
                if (optionValueBuilder.Length > 0)
                {
                    optionValueBuilder.Append(",");
                    optionValueBuilder.Append(checkValue);
                }
                else { optionValueBuilder.Append(checkValue); }
            }

            StringBuilder optionQuantityBuilder = new();
            for (int i = 0; i < QuantityCheck; i++)
            {
                if (i > 0)
                {
                    optionQuantityBuilder.Append(", ");
                    optionQuantityBuilder.Append("999");
                }
                else { optionQuantityBuilder.Append("999"); }
            }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionValue].Value = optionValueBuilder.ToString();
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionQuantity].Value = optionQuantityBuilder.ToString();
        }
        private void MappingForOptionPrice(int row, double MarginRate, SellerSMCommodity sellerSMcommodity, Worksheet worksheet)
        {
            StringBuilder optionPriceBuilder = new();
            var SellingPrice = (int)worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value;
            var options = sellerSMcommodity.SellerMCommodity.CommodityOptions;
            var option = sellerSMcommodity.SellerMCommodity.CommodityOptions.FirstOrDefault();
            if (option.SalesType.Contains("자율"))
            {
                List<double> SellerOptionPrices = new();
                foreach(var value in options)
                {
                    SellerOptionPrices.Add(value.SellerPrice.ToPrice());
                }
                var AnalyzingSellingPrice = GetSatisfyRepresentatingPrice(SellingPrice, MarginRate, SellerOptionPrices);
                worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value = AnalyzingSellingPrice;
                FineMappingOptionPrice(row, MarginRate, sellerSMcommodity, worksheet);
            }
            else
            {
                foreach (var value in sellerSMcommodity.SellerMCommodity.CommodityOptions)
                {
                    var ConsumerPrice = value.ConsumerPrice.ToPrice();
                    var AdjustValue = Math.Ceiling(ConsumerPrice / 10) * 10;
                    if (optionPriceBuilder.Length > 0)
                    {
                        optionPriceBuilder.Append(',');
                        optionPriceBuilder.Append(AdjustValue - SellingPrice);
                    }
                    else { optionPriceBuilder.Append(AdjustValue - SellingPrice); }
                }
            }
        }
        private bool CheckOptionPrice(int RepresentatingPrice, double MarginRate, List<CommodityOption> optionPrices)
        {
            bool IsFine = true;
            List<double> OptionPricesValue = new();
            foreach (var price in optionPrices)
            {
                var pricevalue = price.SellerPrice.ToPrice();
                var value = Math.Ceiling(pricevalue / 10) * 10;
                OptionPricesValue.Add(value);
            }
            if (RepresentatingPrice < 2000)
            {
                foreach (var SellerPrice in OptionPricesValue)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 2 < value || value < RepresentatingPrice)
                    {
                        IsFine = false;
                        return IsFine;
                    }
                }
            }
            if (RepresentatingPrice >= 2000 && RepresentatingPrice < 10000)
            {
                foreach (var SellerPrice in OptionPricesValue)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 2 < value || value * 0.5 < RepresentatingPrice)
                    {
                        IsFine = false;
                        return IsFine;
                    }
                }
            }
            if (RepresentatingPrice >= 10000)
            {
                foreach (var SellerPrice in OptionPricesValue)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 1.5 < value || value * 0.5 < RepresentatingPrice)
                    {
                        IsFine = false;
                        return IsFine;
                    }
                }
            }
            return IsFine;
        }
        private bool CheckOptionPrice(int RepresentatingPrice, double MarginRate, List<double> optionPrices)
        {
            bool IsFine = true;
            if (RepresentatingPrice < 2000)
            {
                foreach (var SellerPrice in optionPrices)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 2 < value || value < RepresentatingPrice)
                    {
                        IsFine = false;
                        return IsFine;
                    }
                }
            }
            if (RepresentatingPrice >= 2000 && RepresentatingPrice < 10000)
            {
                foreach (var SellerPrice in optionPrices)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 2 < value || value * 0.5 < RepresentatingPrice)
                    {
                        IsFine = false;
                        return IsFine;
                    }
                }
            }
            if (RepresentatingPrice >= 10000)
            {
                foreach (var SellerPrice in optionPrices)
                {
                    var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
                    if (RepresentatingPrice * 1.5 < value || value * 0.5 < RepresentatingPrice)
                    {
                        IsFine = false;
                        return IsFine;
                    }
                }
            }
            return IsFine;
        }
        private int GetSatisfyRepresentatingPrice(int RepresentatingPrice, double MarginRate, List<double> optionPrices)
        {
            var DistintValues = optionPrices.Distinct();
            int ResultValue = 0;
            bool IsFine = false;
            foreach(var value in DistintValues)
            {
                IsFine = CheckOptionPrice((int)value, MarginRate, optionPrices);
                if(IsFine == true)
                {
                    ResultValue = (int)value;
                    break;
                }
            }
            if(IsFine == false)
            {
                return RepresentatingPrice;
            }
            return ResultValue;
        }
        private void FineMappingOptionPrice(int row, double MarginRate, SellerSMCommodity sellerSMcommodity, Worksheet worksheet)
        {
            StringBuilder optionPriceBuilder = new();
            var SellingPrice = worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value;
            foreach (var value in sellerSMcommodity.SellerMCommodity.CommodityOptions)
            {
                var SellerPrice = value.SellerPrice.ToPrice();
                var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                var AdjustValue = Math.Ceiling(MarginSellerPrice / 10) * 10;
                if (optionPriceBuilder.Length > 0)
                {
                    optionPriceBuilder.Append(",");
                    optionPriceBuilder.Append(AdjustValue - SellingPrice);
                }
                else { optionPriceBuilder.Append(AdjustValue - SellingPrice); }
            }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OptionPrice].Value = optionPriceBuilder.ToString();
        }
        private async Task MappingForImage(int row, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        {
            if (sellerSMCommodity.SellerMCommodity.RepresentativeImageUrl != null) { worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.RepresentativeImgae].Value = sellerSMCommodity.SellerMCommodity.RepresentativeImageUrl; }

            //StringBuilder AdditionalImagesBuilder = new();

            //int cnt = 0;
            //foreach (var value in AdditionalImagesValues.Distinct())
            //{
            //    if (cnt >= 9) { break; }
            //    if (AdditionalImagesBuilder.Length > 0)
            //    {
            //        AdditionalImagesBuilder.Append('\n');
            //        AdditionalImagesBuilder.Append(value);
            //        cnt++;
            //    }
            //    else
            //    {
            //        AdditionalImagesBuilder.Append(value);
            //        cnt++;
            //    }
            //}
            //worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.AdditionalImage].Value = AdditionalImagesBuilder.ToString();
            StringBuilder DetatilImagesBuilder = new();
            List<string> DetailImagesList = new();
            var DeliveryInfoImgaeSrc = _marketDeliveryInfo;
            if(DeliveryInfoImgaeSrc != null)
            {
                DetailImagesList.Add($"<img src={DeliveryInfoImgaeSrc}>");
            }
            var DetailImagesValues = sellerSMCommodity.SellerMCommodity.DetailImages;
            foreach (var value in DetailImagesValues.Distinct())
            {
                if (value == "") { continue; }
                DetailImagesList.Add($"<img src={value}>");
            }
            foreach (var value in DetailImagesList.Distinct())
            {
                if (DetatilImagesBuilder.Length > 0)
                {
                    DetatilImagesBuilder.Append('\n');
                    DetatilImagesBuilder.Append(value);
                }
                else
                {
                    DetatilImagesBuilder.Append(value);
                }
            }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.DetailImage].Value = DetatilImagesBuilder.ToString();
        }

        private async Task MappingForDetailCommodityInfo(int row, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        {
            if (sellerSMCommodity.OriginCode == null) { await GetOriginCode(sellerSMCommodity); }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.OriginCode].Value = "'" + sellerSMCommodity.OriginCode?.ToString() ?? "0200037";
            var DicDetailInfo = sellerSMCommodity.SellerMCommodity.DetailCommodityInfo;
            var key = DicDetailInfo.Keys.FirstOrDefault(e => e.Contains("제조자") || e.Contains("수입자"));
            string ManufactureAndImporterValue = "";
            if (key != null)
            {
                ManufactureAndImporterValue = DicDetailInfo[key];
            }
            if(ManufactureAndImporterValue.Length >= 50)
            {
                ManufactureAndImporterValue = ManufactureAndImporterValue.Substring(0, 49);
            }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.ManuFacturingCompany].Value = ManufactureAndImporterValue;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.ImportingCompany].Value = ManufactureAndImporterValue;
        }

        private async Task GetOriginCode(SellerSMCommodity sellerSMCommodity)
        {
            var DicDetailInfo = sellerSMCommodity.SellerMCommodity.DetailCommodityInfo;
            var key = DicDetailInfo.Keys.FirstOrDefault(e => e.Contains("제조국"));
            var originValue = DicDetailInfo[key];
            var OriginCodes = await _openMarketDataContext.GetsAsync<CommodityOriginCode>();
            var IsDomestic = false;
            CommodityOriginCode OriginCode = new();
            if (originValue.Contains("대한민국") || originValue.Contains("국산") || originValue.Contains("국내산"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Equals("국산"));
            }
            if (originValue.Contains("중국"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Equals("중국"));
            }
            if (originValue.Contains("의무"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("의무"));
            }
            if (originValue.Contains("상세"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("상세"));
            }
            sellerSMCommodity.OriginCode = OriginCode.Code;
            await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
        }
        private async Task MappingForDeliveryInfo(int row, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        {
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.DeliveryMethod].Value = "택배‚ 소포‚ 등기";
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.DeliveryCode].Value = sellerSMCommodity.DeliveryCode;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.DeliveryForm].Value = "유료";
            if(sellerSMCommodity.CommonDeliveryFee == 0)
            {
                sellerSMCommodity.CommonDeliveryFee = 3000;
                await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
            }
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.CommonDeliveryFee].Value = sellerSMCommodity.CommonDeliveryFee;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.CommonPayingMethod].Value = "선결제";
            //worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.ConditionFreeDevliery].Value = 30000;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.ReturnDeliveryFee].Value = sellerSMCommodity.CommonDeliveryFee;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.ChangeDeliveryFee].Value = sellerSMCommodity.CommonDeliveryFee;
        }
        // // A/S
        // public const int AsPhoneNumber = 52; // 52 A/S 전화
        // public const int AsInfo = 53;// 53 A/S 안내
        // // Discount
        // public const int PCDiscountPoint = 57;// 55 PC 즉시할인
        // public const int MobileDiscountPoint = 58;// 57 모바일 즉시할인
        // public const int ConditionQuantityDiscount = 59; // 59 복수구매 즉시할인 조건값
        // public const int QuantityDiscountPoint = 61;// 61 복수구매할인 값
        // public const int PointofPurchasing = 63;// 63 상품구매시 포인트 지급 값
        // public const int TextReviewPoint = 65;// 65 텍스트리뷰 작성시 지급 포인트
        // public const int PhotoReviewPoint = 66;// 66 포토/동영상 리뷰 작성 시 지급 포인트
        // public const int TextReviewUsingMonth = 67;// 67 한달사용 텍스트리뷰 작성시 지급 포인트
        // public const int PohtoReviewUsingMonth = 68;// 68 한달사용 포토/동영상 리뷰 지급시 포인트
        // public const int CustomerReviewPoint = 69;// 69 톡톡친구/스토어찜 고객리뷰 작성시 지급 포인트
        // public const int InstallmentMonth = 70;// 70 무이자 할부 개월
        // public const int OpenEvaluating = 73;// 73 구매평 노출여부

        private void MappingForASAndPoint(int row, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        {
            /*
             *     var MarginSellerPrice = SellerPrice * (1 + MarginRate);
                    var value = Math.Ceiling(MarginSellerPrice / 10) * 10;
             */
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.AsPhoneNumber].Value = "010-6797-3707";
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.AsInfo].Value = "문자 주세요.";
            var Price = worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.Price].Value;
            //_logger.LogInformation(Price.ToString());
            //var PriceTextPoint = double.Parse(Price) * 0.05;
            //var PricePhotoPoint = double.Parse(Price) * 0.1;
            //worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.TextReviewPoint].Value = Math.Ceiling(PriceTextPoint / 10) * 10;
            //worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.PhotoReviewPoint].Value = Math.Ceiling(PricePhotoPoint / 10) * 10;
            worksheet.Cells[row, SmartStoreCommodityRegisterExcelForm.InstallmentMonth].Value = "'6";
        }
        // // Margin * 0.1
        // private string PointByMargen(int row, SellerSMCommodity sellerSMCommodity, Worksheet worksheet)
        // {
            
        // }
    }
}