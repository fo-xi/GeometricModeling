using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace LaboratoryWork1.Service
{
    public interface IContrastWindowService
    {
        bool DialogResult { get; set; }

        void Open();

        RelayCommand OKCommand { get; set; }

        RelayCommand CancelCommand { get; set; }

        int Value { get; set; }
    }
}
