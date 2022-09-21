using CV_ManipulationTool.Model;
using CV_ManipulationTool.Tool;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using TYVisuallibrary;

namespace CV_ManipulationTool.ViewModel
{
    public class HistogramViewModel : ObservableObject
    {
        private PlotModel _linePlotModel;
        public PlotModel LinePlotModel
        {
            get => _linePlotModel;
            set
            {
                SetProperty<PlotModel>(ref _linePlotModel, value);
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

        private Visibility _isShowRGB;
        public Visibility IsShowRGB
        {
            get => _isShowRGB;
            set
            {
                SetProperty(ref _isShowRGB, value);
            }
        }

        public RelayCommand LoadImageCommand { get; set; }
        public RelayCommand RGB2GrayCommand { get; set; }

        private Mat SrcImage;
        int[] Y = new int[256];
        List<int[]> RGB = new List<int[]>();
        private MatOptimization CvOperation = new MatOptimization();//opencvsharp的自定义操作
        public int RGBIndex = 0;

        public HistogramViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            RGB2GrayCommand = new RelayCommand(RGB2Gray);

            LinePlotModel = new PlotModel();
            IsShowRGB = Visibility.Visible;

            for (int i = 0; i < 256; i++)
            {
                Y[i] = i;
            }
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
                IsShowRGB = Visibility.Hidden;
                RGB = Histogram(SrcImage);
                RGBIndex = 0;
                InitChart(RGBIndex);
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

                RGB = Histogram(SrcImage);
                InitChart(RGBIndex);
            }
        }

        private void InitChart(int Index)
        {
            LineSeries lineSeries = new LineSeries() { Title = "Histogram" };
            for (int i = 0; i < Y.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(Y[i], RGB[Index][i]));
            }

            LinePlotModel.Series.Clear();
            LinePlotModel.Series.Add(lineSeries);
            LinePlotModel.InvalidatePlot(true);
        }

        private List<int[]> Histogram(Mat Input)
        {
            List<int[]> objs = new List<int[]>();
            if (Input.Type() == MatType.CV_8UC1)
            {
                objs.Add(MatIntegration(Input));
            }
            else
            {
                Mat matR = new Mat();
                Mat matG = new Mat();
                Mat matB = new Mat();

                //! 在HSV格式下，获取的单通道分别对应像素点的HSV值
                Cv2.ExtractChannel(Input, matR, 0);
                Cv2.ExtractChannel(Input, matG, 1);
                Cv2.ExtractChannel(Input, matB, 2);

                objs.Add(MatIntegration(matR));
                objs.Add(MatIntegration(matG));
                objs.Add(MatIntegration(matB));
            }

            return objs;
        }

        private int[] MatIntegration(Mat Input)
        {
            int[] Y = new int[256];
            for (int i = 0; i < Input.Rows; i++)
            {
                for (int j = 0; j < Input.Cols; j++)
                {
                    byte l = Input.Get<byte>(i, j);
                    Y[l]++;
                }
            }
            return Y;
        }

        public void GetROIHistogramHSV(System.Windows.Rect rect)
        {
            if (SrcImage != null)
            {
                Mat obj;
                if (rect == System.Windows.Rect.Empty || rect == null)
                {
                    obj = SrcImage.Clone();
                }
                else
                {
                    obj = CutImage(SrcImage, rect);
                }

                RGB = Histogram(obj);
                LineSeries lineSeries = new LineSeries() { Title = "Histogram" };
                for (int i = 0; i < Y.Length; i++)
                {
                    lineSeries.Points.Add(new DataPoint(Y[i], RGB[RGBIndex][i]));
                }

                LinePlotModel.Series.Clear();
                LinePlotModel.Series.Add(lineSeries);
                LinePlotModel.InvalidatePlot(true);
            }
        }

        /// <summary>
        /// 裁剪出ROI区域
        /// </summary>
        /// <param name="Image">图像资源</param>
        /// <param name="rect">ROI区域</param>
        /// <returns>裁剪后的图像资源</returns>
        public Mat CutImage(Mat Image, System.Windows.Rect rect)
        {
            OpenCvSharp.Rect CvRect = new OpenCvSharp.Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            if (Image != null)
            {
                return CvOperation.CaptureImageByPosition(Image, CvRect);
            }
            return null;
        }
    }
}
