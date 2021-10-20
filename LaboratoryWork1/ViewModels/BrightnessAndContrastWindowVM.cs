using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LaboratoryWork1.Service;

namespace LaboratoryWork1
{
    public class BrightnessAndContrastWindowVM : ViewModelBase
    {
        private int _contrastValue;

        private int _brightnessValue;

        public int ContrastValue
        {
            get
            {
                return _contrastValue;
            }
            set
            {
                _contrastValue = value;
                RaisePropertyChanged(nameof(ContrastValue));
            }
        }

        public int BrightnessValue
        {
            get
            {
                return _brightnessValue;
            }
            set
            {
                _brightnessValue = value;
                RaisePropertyChanged(nameof(BrightnessValue));
            }
        }

        public RelayCommand OKCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public BrightnessAndContrastWindowVM(IBrightnessAndContrastWindowService contrastWindowService)
        {
            OKCommand = contrastWindowService.OKCommand;
            CancelCommand = contrastWindowService.CancelCommand;
        }
    }
}
