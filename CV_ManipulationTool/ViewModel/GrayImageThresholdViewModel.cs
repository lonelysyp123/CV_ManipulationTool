using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;

namespace CV_ManipulationTool.ViewModel
{
    public class GrayImageThresholdViewModel : ObservableObject
    {
        private int _minValue;
        public int MinValue
        {
            get => _minValue;
            set
            {
                SetProperty(ref _minValue, value);
                // 阈值处理
                ImageThreshold();
            }
        }

        private int _maxValue;
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                SetProperty(ref _maxValue, value);
                // 阈值处理
                ImageThreshold();
            }
        }

        private BitmapSource _mainImage;
        public BitmapSource MainImage
        {
            get => _mainImage;
            set
            {
                SetProperty(ref _mainImage, value);
            }
        }

        public RelayCommand LoadImageCommand { get; private set; }

        private Mat SrcImage, ResImage;

        public GrayImageThresholdViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            MaxValue = 255;
            ResImage = new Mat();
        }

        private void ImageThreshold()
        {
            if (SrcImage != null)
            {
                Cv2.Threshold(SrcImage, ResImage, MinValue, MaxValue, ThresholdTypes.Binary);
                MainImage = Mat2BitmapSource(ResImage);
            }
        }

        private void LoadImage()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "图像文件(*.png, *.bmp, *.jpg) | *.png; *.bmp; *.jpg";
            file.ShowDialog();
            if (file.FileName != null && file.FileName != "")
            {
                SrcImage = new Mat(file.FileName);
                Cv2.CvtColor(SrcImage, SrcImage, ColorConversionCodes.BGR2GRAY);
                MainImage = Mat2BitmapSource(SrcImage);
            }
        }

        private BitmapSource Mat2BitmapSource(Mat mat)
        {
            var bitmap = BitmapConverter.ToBitmap(mat);
            var hBitmap = bitmap.GetHbitmap();
            BitmapSource bitmaps = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hBitmap);
            bitmap.Dispose();
            return bitmaps;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
