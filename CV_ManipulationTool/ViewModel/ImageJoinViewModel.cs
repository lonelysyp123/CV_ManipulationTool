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
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CV_ManipulationTool.ViewModel
{
    public class ImageJoinViewModel : ObservableObject
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

        private BitmapSource _imageLTSource;
        public BitmapSource ImageLTSource
        {
            get => _imageLTSource;
            set
            {
                SetProperty(ref _imageLTSource, value);
            }
        }

        private BitmapSource _imageRTSource;
        public BitmapSource ImageRTSource
        {
            get => _imageRTSource;
            set
            {
                SetProperty(ref _imageRTSource, value);
            }
        }

        private BitmapSource _imageLBSource;
        public BitmapSource ImageLBSource
        {
            get => _imageLBSource;
            set
            {
                SetProperty(ref _imageLBSource, value);
            }
        }

        private BitmapSource _imageRBSource;
        public BitmapSource ImageRBSource
        {
            get => _imageRBSource;
            set
            {
                SetProperty(ref _imageRBSource, value);
            }
        }

        private ComboBoxItem _joinModel;
        public ComboBoxItem JoinModel
        {
            get => _joinModel;
            set
            {
                SetProperty(ref _joinModel, value);
            }
        }
        
        public RelayCommand<string> LoadImageCommand { get; private set; }
        public RelayCommand SaveJoinedImageCommand { get; private set; }
        public RelayCommand JoinImageCommand { get; private set; }

        private Mat SrcImage;

        public ImageJoinViewModel()
        {
            LoadImageCommand = new RelayCommand<string>(LoadImage);
            SaveJoinedImageCommand = new RelayCommand(SaveJoinedImage);
            JoinImageCommand = new RelayCommand(JoinImage);
        }

        private void JoinImage()
        {
            if (JoinModel != null)
            {
                if (JoinModel.Content.ToString() == "横向拼接")
                {
                    if (_imageLTSource != null && _imageRTSource != null)
                    {
                        if (_imageLTSource.Height == _imageRTSource.Height)
                        {
                            ImageSource = HorizontalJoinImage(_imageLTSource, _imageRTSource);
                        }
                        else
                        {
                            MessageBox.Show("横向拼接模式需要图像高度相等");
                        }
                    }
                    else
                    {
                        MessageBox.Show("请加载左上角和右上角的图像文件");
                    }
                }
                else if (JoinModel.Content.ToString() == "纵向拼接")
                {
                    if (_imageLTSource != null && _imageLBSource != null)
                    {
                        if (_imageLTSource.Width == _imageLBSource.Width)
                        {
                            ImageSource = VerticalJoinImage(_imageLTSource, _imageLBSource);
                        }
                        else
                        {
                            MessageBox.Show("纵向拼接模式需要图像宽度相等");
                        }
                    }
                    else
                    {
                        MessageBox.Show("请加载左上角和左下角的图像文件");
                    }
                }
                else if (JoinModel.Content.ToString() == "四向拼接")
                {
                    // 先横向拼接，再纵向拼接
                    if (_imageLTSource != null && _imageLBSource != null && _imageRTSource != null && _imageRBSource != null)
                    {
                        if (_imageLTSource.Height == _imageRTSource.Height && (_imageLTSource.Width + _imageRTSource.Width)==(_imageLBSource.Width + _imageRBSource.Width) )
                        {
                            var out1 = HorizontalJoinImage(_imageLTSource, _imageRTSource);
                            var out2 = HorizontalJoinImage(_imageLBSource, _imageRBSource);
                            ImageSource = VerticalJoinImage(out1, out2);
                        }
                        else
                        {
                            MessageBox.Show("图像的宽高不符合四向拼接的要求");
                        }
                    }
                    else
                    {
                        MessageBox.Show("请加载所有图像文件");
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择拼接模式");
            }
        }

        private BitmapSource HorizontalJoinImage(BitmapSource map1, BitmapSource map2)
        {
            try
            {
                var out1 = Bitmap2Mat(map1);
                var out2 = Bitmap2Mat(map2);
                SrcImage = new Mat();
                Cv2.HConcat(new Mat[] { out1, out2 }, SrcImage);
                var hBitmap = SrcImage.ToBitmap().GetHbitmap();
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                        hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());
                return bitmapSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private BitmapSource VerticalJoinImage(BitmapSource map1, BitmapSource map2)
        {
            try
            {
                var out1 = Bitmap2Mat(map1);
                var out2 = Bitmap2Mat(map2);
                SrcImage = new Mat();
                Cv2.VConcat(new Mat[] { out1, out2 }, SrcImage);
                var hBitmap = SrcImage.ToBitmap().GetHbitmap();
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                        hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());
                return bitmapSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private void SaveJoinedImage()
        {
            if (SrcImage != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "*.png|*.bmp";
                dialog.ShowDialog();
                if (dialog.FileName != null && dialog.FileName != "")
                {
                    SrcImage.SaveImage(dialog.FileName);
                }
            }
            else
            {
                MessageBox.Show("请勿保存空文件");
            }
        }

        private void LoadImage(string name)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            if (file.FileName != null && file.FileName != "")
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(file.FileName, UriKind.Absolute);
                bi.EndInit();

                switch (name)
                {
                    case "MapLT":
                        ImageLTSource = bi;
                        break;
                    case "MapLB":
                        ImageLBSource = bi;
                        break;
                    case "MapRT":
                        ImageRTSource = bi;
                        break;
                    case "MapRB":
                        ImageRBSource = bi;
                        break;
                    default:
                        break;
                }

            }
        }

        private Mat Bitmap2Mat(BitmapSource Input)
        {
            Bitmap bitmap;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Input));
                encoder.Save(ms);
                bitmap = new Bitmap(ms);
            }
            return BitmapConverter.ToMat(bitmap);
        }
    }
}
