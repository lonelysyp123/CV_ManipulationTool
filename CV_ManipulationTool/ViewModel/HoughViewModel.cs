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
    public class HoughViewModel : ObservableObject
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

        private int _rho = 1;
        public int Rho
        {
            get => _rho;
            set
            {
                SetProperty(ref _rho, value);
            }
        }

        private int _theta = 1;
        public int Theta
        {
            get => _theta;
            set
            {
                SetProperty(ref _theta, value);
            }
        }

        private int _thr = 1;
        public int Thr
        {
            get => _thr;
            set
            {
                SetProperty(ref _thr, value);
            }
        }

        public RelayCommand LoadImageCommand { get; set; }
        public RelayCommand HoughCommand { get; set; }

        private Mat SrcImage;

        public HoughViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            HoughCommand = new RelayCommand(Hough);
        }

        private void Hough()
        {
            if (SrcImage != null)
            {

                Mat temp = SrcImage.CvtColor(ColorConversionCodes.GRAY2RGB);
                LineSegmentPolar[] lines = Cv2.HoughLines(SrcImage, Rho, Theta, Thr);
                for (int i = 0; i < lines.Length; i++)
                {
                    double rho = lines[i].Rho;//线长
                    double theta = lines[i].Theta;//角度
                    OpenCvSharp.Point pt1 = new OpenCvSharp.Point();
                    OpenCvSharp.Point pt2 = new OpenCvSharp.Point();
                    double a = Math.Cos(theta);
                    double b = Math.Sin(theta);
                    double x0 = a * rho, y0 = b * rho;
                    pt1.X = (int)Math.Round(x0 + 600 * (-b));
                    pt1.Y = (int)Math.Round(y0 + 600 * a);
                    pt2.X = (int)Math.Round(x0 - 600 * (-b));
                    pt2.Y = (int)Math.Round(y0 - 600 * a);
                    Cv2.Line(temp, pt1, pt2, Scalar.Blue, 1, LineTypes.Link8);

                    var hBitmap = temp.ToBitmap().GetHbitmap();
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                        hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());
                    ImageSource1 = bitmapSource;
                    GDIHelper.DeleteObject(hBitmap);
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

            }
        }
    }
}
