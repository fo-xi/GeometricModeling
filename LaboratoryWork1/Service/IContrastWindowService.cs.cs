﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace LaboratoryWork1.Service
{
    public interface IContrastWindowService
    {
        /// <summary>
        /// DialogResult.
        /// </summary>
        bool DialogResult { get; set; }

        /// <summary>
        /// Open a window.
        /// </summary>
        /// <param name="note">Note</param>
        void Open();

        /// <summary>
        /// Returns and sets OK command.
        /// </summary>
        RelayCommand OKCommand { get; set; }

        /// <summary>
        /// Returns and sets Cancel command.
        /// </summary>
        RelayCommand CancelCommand { get; set; }

        int Value { get; set; }
    }
}
