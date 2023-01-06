using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofExporting
{
    public class ExportingCoupangViewModel : OpenMarketObservableObject
    {
        private readonly OpenMarketDataContext _openMarketDataContext;
        public ExportingCoupangViewModel(OpenMarketDataContext openMarketDataContext)
                : base(openMarketDataContext)
        {
        }
       
        public override Task InitLoadAsync()
        {
            return base.InitLoadAsync();
        }
        /// <summary>
        ///  현재 등록된 엑셀 상품 파일을 순회하며 가격 및 필요정보를 기입하여 엑셀 파일로 출력한다
        /// </summary>
        /// <param name="filePath">현재 등록된 쿠팡 상품에 대한 엑셀 파일 경로</param>
        /// <returns></returns>
        public async Task ExportingToCoupang(string filePath)
        {

        }
    }
}
