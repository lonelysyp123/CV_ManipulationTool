using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV_ManipulationTool.View;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace CV_ManipulationTool.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand AffineCommand { get; set; }
        public RelayCommand HistogramCommand { get; set; }

        public MainViewModel()
        {
            AffineCommand = new RelayCommand(AffineFunc);
            HistogramCommand = new RelayCommand(Histogram);
        }

        private void Histogram()
        {
            HistogramView view = new HistogramView();
            view.ShowDialog();
        }

        private void AffineFunc()
        {
            //! TODO 展示独立界面
            AffineView view = new AffineView();
            view.ShowDialog();
        }
    }
}
