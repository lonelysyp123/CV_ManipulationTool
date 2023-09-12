using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using OpenCvSharp;
using System.Windows.Interop;

namespace CV_ManipulationTool.ViewModel
{
    public class ImageDifferViewModel : ObservableObject
    {
        private BitmapSource _image1Source;
        public BitmapSource Image1Source
        {
            get => _image1Source;
            set
            {
                SetProperty(ref _image1Source, value);
            }
        }

        private BitmapSource _image2Source;
        public BitmapSource Image2Source
        {
            get => _image2Source;
            set
            {
                SetProperty(ref _image2Source, value);
            }
        }

        private BitmapSource _image3Source;
        public BitmapSource Image3Source
        {
            get => _image3Source;
            set
            {
                SetProperty(ref _image3Source, value);
            }
        }

        private string _imageHeight;
        public string ImageHeight
        {
            get => _imageHeight;
            set
            {
                SetProperty(ref _imageHeight, value);
            }
        }

        private string _imageWidth;
        public string ImageWidth
        {
            get => _imageWidth;
            set
            {
                SetProperty(ref _imageWidth, value);
            }
        }

        public RelayCommand LoadImage1Command { get; set; }
        public RelayCommand LoadImage2Command { get; set; }
        public RelayCommand ImageDifferCommand { get; set; }
        public RelayCommand SaveImageCommand { get; set; }

        private Mat SrcImage1;
        private Mat SrcImage2;
        private Mat SrcImage3;

        public ImageDifferViewModel()
        {
            LoadImage1Command = new RelayCommand(LoadImage1);
            LoadImage2Command = new RelayCommand(LoadImage2);
            ImageDifferCommand = new RelayCommand(ImageDiffer);
            SaveImageCommand = new RelayCommand(SaveImage);
        }

        private void SaveImage()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "*.png|*.bmp";
            dialog.ShowDialog();
            if (dialog.FileName != null && dialog.FileName != "")
            {
                if (SrcImage3 != null)
                {
                    SrcImage3.SaveImage(dialog.FileName);
                }
            }

        }

        private void ImageDiffer()
        {
            if (SrcImage1 != null && SrcImage2 != null)
            {
                if (SrcImage1.Height == SrcImage2.Height && SrcImage1.Width == SrcImage2.Width)
                {
                    SrcImage3 = SrcImage1 - SrcImage2;
                    var hBitmap = SrcImage3.ToBitmap().GetHbitmap();
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                            hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                            BitmapSizeOptions.FromEmptyOptions());
                    Image3Source = bitmapSource;
                    ImageHeight = Image3Source.Height.ToString();
                    ImageWidth = Image3Source.Width.ToString();
                }
                else
                {
                    MessageBox.Show("图像大小不一致");
                }
            }
            else
            {
                MessageBox.Show("参数图像不能为空");
            }
        }

        private void LoadImage2()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            if (file.FileName != null && file.FileName != "")
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(file.FileName, UriKind.Absolute);
                bi.EndInit();

                Image2Source = bi;
                Bitmap bitmap;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bi));
                    encoder.Save(ms);
                    bitmap = new Bitmap(ms);
                }
                SrcImage2 = BitmapConverter.ToMat(bitmap);
            }
        }

        private void LoadImage1()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            if (file.FileName != null && file.FileName != "")
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(file.FileName, UriKind.Absolute);
                bi.EndInit();

                Image1Source = bi;
                Bitmap bitmap;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bi));
                    encoder.Save(ms);
                    bitmap = new Bitmap(ms);
                }
                SrcImage1 = BitmapConverter.ToMat(bitmap);
            }
        }
    }
}
