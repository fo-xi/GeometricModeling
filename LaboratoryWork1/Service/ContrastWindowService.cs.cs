using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using LaboratoryWork1.Windows;

namespace LaboratoryWork1.Service
{
    public class ContrastWindowService : IContrastWindowService
    {
        private ContrastWindow _contrastWindow;

        public bool DialogResult { get; set; }

        public void Open()
        {
            _contrastWindow = new ContrastWindow(new ContrastWindowVM(this));
            _contrastWindow.ShowDialog();
        }

        public RelayCommand OKCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public int Value { get; set; }

        private void OK()
        {
            var dataContext = _contrastWindow.DataContext as ContrastWindowVM;
            Value = dataContext.Value;
            DialogResult = true;
            Close();
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private void Close()
        {
            _contrastWindow.Close();
        }

        public ContrastWindowService()
        {
            OKCommand = new RelayCommand(OK);
            CancelCommand = new RelayCommand(Cancel);
        }
	}
}
