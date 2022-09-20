using CV_ManipulationTool.Tool;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CV_ManipulationTool.ViewModel
{
    public class SobelViewModel : ObservableObject
    {
        private BitmapSource _imageSource;
        public BitmapSource ImageSource
        {
            get => _imageSource;
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        private BitmapSource _imageSource1;
        public BitmapSource ImageSource1
        {
            get => _imageSource1;
            set
            {
                SetProperty(ref _imageSource1, value);
            }
        }

        public RelayCommand LoadImageCommand { get; set; }
        public RelayCommand RGB2GrayCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        public RelayCommand SobelCommand { get; set; }

        private Mat SrcImage;
        InputArray SobelKernelX = InputArray.Create<float>(new float[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } });
        InputArray SobelKernelY = InputArray.Create<float>(new float[3, 3] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } });

        public SobelViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            RGB2GrayCommand = new RelayCommand(RGB2Gray);
            FilterCommand = new RelayCommand(Filter);
            SobelCommand = new RelayCommand(Sobel);
        }

        private void Sobel()
        {
            Mat temp = new Mat();
            Cv2.Sobel(SrcImage, temp, MatType.CV_8UC1, 1, 1);
            var hBitmap = temp.ToBitmap().GetHbitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
            ImageSource = bitmapSource;
            GDIHelper.DeleteObject(hBitmap);
            SrcImage = temp;
        }

        private void Filter()
        {
            Mat temp = SrcImage.GaussianBlur(new OpenCvSharp.Size(3, 3), 0);
            var hBitmap = temp.ToBitmap().GetHbitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
            ImageSource = bitmapSource;
            GDIHelper.DeleteObject(hBitmap);
            SrcImage = temp;
        }

        private void RGB2Gray()
        {
            if (SrcImage != null)
            {
                Mat temp = new Mat();
                Cv2.CvtColor(SrcImage, temp, ColorConversionCodes.RGB2GRAY);

                var hBitmap = temp.ToBitmap().GetHbitmap();
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                    hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());
                ImageSource = bitmapSource;
                GDIHelper.DeleteObject(hBitmap);
                SrcImage = temp;
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

            }
        }
    }
}
