using BusinessData.ofDataAccessLayer.ofCommon;
using BusinessLogic.ofEntityManager.ofGeneric.ofFileFactory;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;

namespace SellerData.ofDataAccessLayer.ofFileFactory
{
    public class FileForm : Entity
    {
        public List<ExcelFileTitlePosition> excelFilePositions { get; set; }
    }
    public class ExcelFileTitlePosition
    {
        public string Value { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
    public static class SellerSMCommodityFile
    {
        public const string RegisteringCommodity = "RegisteringCommodity";
    }
    public class SellerSMCommodityExcelFileFactory : IEntityExcelFileFactory<SellerSMCommodity>
    {
        protected Application Application { get; set; }
        protected Workbook Workbook { get; set; }
        protected Worksheet Worksheet { get; set; }
        public async Task<FileStream> ConvertToFileStream(SellerSMCommodity entity, string nameofFile)
        {
            FileStream fileStream;
            switch(nameofFile)
            {
                case SellerSMCommodityFile.RegisteringCommodity:
                    fileStream = await ConvertToRegisteringCommodityFile(entity);
                    break;
                 default: throw new ArgumentException(nameofFile);  
            }
            return fileStream;
        }

        private async Task<FileStream> ConvertToRegisteringCommodityFile(SellerSMCommodity sellerSMCommodity)
        {
            if(sellerSMCommodity.SellerMCommodity == null) { throw new ArgumentNullException("SellerMCommodity Is null"); }
            throw new NotImplementedException();
            try
            {
                Application = new Application();
                Workbook = Application.Workbooks.Add();
                Worksheet = Workbook.Worksheets.Add();
            }
            finally
            {

            }
        }
    }
}
