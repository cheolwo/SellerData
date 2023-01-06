using BusinessData.ofDataContext;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.ViewModel.Common
{
    public class CommonObservableObject : ObservableObject
    {
        public CommonObservableObject()
        {

        }
        private bool _IsLoad;
        public bool IsLoad
        {
            get => _IsLoad;
            set => SetProperty(ref _IsLoad, value);
        }
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }
    }
}
