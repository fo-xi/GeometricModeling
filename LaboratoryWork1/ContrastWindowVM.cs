using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using LaboratoryWork1.Service;

namespace LaboratoryWork1
{
    public class ContrastWindowVM
    {
        public RelayCommand OKCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public ContrastWindowVM(IContrastWindowService contrastWindowService)
        {
            OKCommand = contrastWindowService.OKCommand;
            CancelCommand = contrastWindowService.CancelCommand;
        }
    }
}
