using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CV_ManipulationTool.ViewModel
{
    public class PackColorViewModel : ObservableObject
    {
        private string _rValue;
        public string RValue
        {
            get => _rValue;
            set
            {
                SetProperty(ref _rValue, value);
            }
        }

        private string _gValue;
        public string GValue
        {
            get => _gValue;
            set
            {
                SetProperty(ref _gValue, value);
            }
        }

        private string _bValue;
        public string BValue
        {
            get => _bValue;
            set
            {
                SetProperty(ref _bValue, value);
            }
        }

        private string _xValue;
        public string XValue
        {
            get => _xValue;
            set
            {
                SetProperty(ref _xValue, value);
            }
        }

        private string _yValue;
        public string YValue
        {
            get => _yValue;
            set
            {
                SetProperty(ref _yValue, value);
            }
        }

        private BitmapSource _imageSource;
        public BitmapSource ImageSource
        {
            get => _imageSource;
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        private SolidColorBrush _packColor;
        public SolidColorBrush PackColor
        {
            get => _packColor;
            set
            {
                SetProperty(ref _packColor, value);
            }
        }

        public double XScale = 0;
        public double YScale = 0;

        public RelayCommand LoadImageCommand { get; set; }
        public RelayCommand OpenListenerCommand { get; set; }
        public RelayCommand CloseListenerCommand { get; set; }

        public Mat SrcImage;

        public PackColorViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            OpenListenerCommand = new RelayCommand(OpenListener);
            CloseListenerCommand = new RelayCommand(CloseListener);

            PackColor = new SolidColorBrush(Colors.Black);
        }

        bool isListenerRun = false;
        public void CloseListener()
        {
            isListenerRun = false;
        }

        private void OpenListener()
        {
            Thread listener = new Thread(GetColor);
            isListenerRun = true;
            listener.Start();
        }

        private void GetColor()
        {
            while(true)
            {
                if (isListenerRun)
                {
                    if (XValue != null && YValue != null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            var rgb = SrcImage.At<Vec3b>((int)(double.Parse(YValue) * YScale), (int)(double.Parse(XValue) * XScale));
                            BValue = rgb[0].ToString();
                            GValue = rgb[1].ToString();
                            RValue = rgb[2].ToString();
                            PackColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, rgb[2], rgb[1], rgb[0]));
                        });
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void LoadImage()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            if (file.FileName != null && file.FileName != "")
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(file.FileName, UriKind.Absolute);
                bi.EndInit();

                ImageSource = bi;
                Bitmap bitmap;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bi));
                    encoder.Save(ms);
                    bitmap = new Bitmap(ms);
                }
                SrcImage = BitmapConverter.ToMat(bitmap);
                XScale = 0;
                YScale = 0;
            }
        }
    }
}
