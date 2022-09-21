using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV_ManipulationTool.View;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;

namespace CV_ManipulationTool.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand AffineCommand { get; set; }
        public RelayCommand HistogramCommand { get; set; }
        public RelayCommand DemoCommand { get; set; }
        public RelayCommand CannyCommand { get; set; }
        public RelayCommand HoughCommand { get; set; }

        public MainViewModel()
        {
            AffineCommand = new RelayCommand(AffineFunc);
            HistogramCommand = new RelayCommand(Histogram);
            DemoCommand = new RelayCommand(Demo);
            CannyCommand = new RelayCommand(Canny);
            HoughCommand = new RelayCommand(Hough);
        }

        private void Hough()
        {
            HoughView view = new HoughView();
            view.ShowDialog();
        }

        private void Canny()
        {
            CannyView view = new CannyView();
            view.ShowDialog();
        }

        private void Demo()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            if (file.FileName != null && file.FileName != "")
            {
                //BitmapImage bi = new BitmapImage();
                //bi.BeginInit();
                //bi.UriSource = new Uri(file.FileName, UriKind.Absolute);
                //bi.EndInit();

                //ImageSource = bi;
                //Bitmap bitmap;
                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                //{
                //    BitmapEncoder encoder = new BmpBitmapEncoder();
                //    encoder.Frames.Add(BitmapFrame.Create(bi));
                //    encoder.Save(ms);
                //    bitmap = new Bitmap(ms);
                //}
                FileStream fs = new FileStream(file.FileName, FileMode.Open);
                byte[] bmpdata = new byte[fs.Length];
                fs.Read(bmpdata, 0, bmpdata.Length);
                fs.Close();
            }
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
