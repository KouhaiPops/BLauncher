using BLauncher.ViewModels;

using MahApps.Metro.Controls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BLauncher.Helpers
{
    internal class ObservableProgressBar : BaseViewModel, IProgress<float>
    {
        private float progress;


        public float Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }


        public void Report(float value)
        {
            Progress = value;
        }
    }
}
