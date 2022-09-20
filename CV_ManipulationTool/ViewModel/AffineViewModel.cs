using CV_ManipulationTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using CV_ManipulationTool.Tool;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Windows.Interop;
using System.Windows;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CV_ManipulationTool.ViewModel
{
    public class AffineViewModel : ObservableObject
    {
        private ObservableCollection<ImageModel> _imageModels;
        public ObservableCollection<ImageModel> ImageList
        {
            get => _imageModels;
            set
            {
                if (value.Count > 10)
                {
                    value.RemoveAt(0);
                }
                SetProperty(ref _imageModels, value);
            }
        }

        private ImageModel _imageSelected;
        public ImageModel ImageSelected
        {
            get => _imageSelected;
            set
            {
                if (_imageSelected != value)
                {
                    if (value != null)
                    {
                        UpdateImage(value.Image);
                    }
                }
                SetProperty(ref _imageSelected, value);
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

        private string _logContent;
        public string LogContent
        {
            get => _logContent;
            set
            {
                SetProperty(ref _logContent, value);
            }
        }

        private Mat _srcImage;
        public Mat SrcImage
        {
            get => _srcImage;
            set
            {
                SetProperty(ref _srcImage, value);
            }
        }

        private int _imageIndex;
        public int ImageIndex
        {
            get => _imageIndex;
            set
            {
                SetProperty(ref _imageIndex, value);
            }
        }

        private int _vertical;
        public int Vertical
        {
            get => _vertical;
            set
            {
                SetProperty(ref _vertical, value);
            }
        }

        private int _horizontal;
        public int Horizontal
        {
            get => _horizontal;
            set
            {
                SetProperty(ref _horizontal, value);
            }
        }

        private int _angle;
        public int Angle
        {
            get => _angle;
            set
            {
                SetProperty(ref _angle, value);
            }
        }

        private int _bigTimes;
        public int BigTimes
        {
            get => _bigTimes;
            set
            {
                SetProperty(ref _bigTimes, value);
            }
        }

        private int _smalleTimes;
        public int SmalleTimes
        {
            get => _smalleTimes;
            set
            {
                SetProperty(ref _smalleTimes, value);
            }
        }

        public IRelayCommand ClearImageCommand { get; set; }
        public IRelayCommand LoadImageCommand { get; set; }
        public IRelayCommand TranslationCommand { get; set; }
        public IRelayCommand RotateCommand { get; set; }
        public IRelayCommand BiggerCommand { get; set; }
        public IRelayCommand SmallerCommand { get; set; }

        public AffineViewModel()
        {
            ClearImageCommand = new RelayCommand(ClearImage);
            LoadImageCommand = new RelayCommand(LoadImage);
            TranslationCommand = new RelayCommand(Translation);
            RotateCommand = new RelayCommand(Rotate);
            BiggerCommand = new RelayCommand(Bigger);
            SmallerCommand = new RelayCommand(Smaller);

            ImageList = new ObservableCollection<ImageModel>();

            Reecord("仿射变换");
        }

        private void Smaller()
        {
            if (SrcImage != null)
            {
                if (SmalleTimes != 0)
                {
                    OpenCvSharp.Point center = new OpenCvSharp.Point(SrcImage.Cols / 2, SrcImage.Rows / 2);
                    float angle = 0;
                    float scale = 1.0f / SmalleTimes;
                    Mat rot_mat = Cv2.GetRotationMatrix2D(center, angle, scale);
                    Mat warp_rotate_dst = Mat.Zeros(SrcImage.Rows, SrcImage.Cols, SrcImage.Type());
                    Cv2.WarpAffine(SrcImage, warp_rotate_dst, rot_mat, warp_rotate_dst.Size());

                    var hBitmap = warp_rotate_dst.ToBitmap().GetHbitmap();
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                        hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());

                    ImageModel model = new ImageModel()
                    {
                        Image = bitmapSource,
                        Tag = "仿射变换_缩小"
                    };
                    ImageList.Add(model);
                    ImageSource = bitmapSource;
                    ImageIndex = ImageList.Count - 1;

                    GDIHelper.DeleteObject(hBitmap);

                    SrcImage = warp_rotate_dst;
                    Reecord("仿射变换_缩小");
                }
            }
        }

        private void Bigger()
        {
            if (SrcImage != null)
            {
                if (BigTimes != 0)
                {
                    OpenCvSharp.Point center = new OpenCvSharp.Point(SrcImage.Cols / 2, SrcImage.Rows / 2);
                    float angle = 0;
                    float scale = BigTimes;
                    Mat rot_mat = Cv2.GetRotationMatrix2D(center, angle, scale);
                    Mat warp_rotate_dst = Mat.Zeros(SrcImage.Rows, SrcImage.Cols, SrcImage.Type());

                    double new_height = SrcImage.Rows * scale;
                    double new_width = SrcImage.Cols * scale;
                    rot_mat.At<double>(0, 2) += (new_width - SrcImage.Cols) / 2;
                    rot_mat.At<double>(1, 2) += (new_height - SrcImage.Rows) / 2;
                    OpenCvSharp.Rect bbox = new RotatedRect(new Point2f(SrcImage.Cols / 2 * scale, SrcImage.Rows / 2 * scale), new Size2f(SrcImage.Cols * scale, SrcImage.Rows * scale), angle).BoundingRect();
                    Cv2.WarpAffine(SrcImage, warp_rotate_dst, rot_mat, bbox.Size);

                    var hBitmap = warp_rotate_dst.ToBitmap().GetHbitmap();
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                        hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());

                    ImageModel model = new ImageModel()
                    {
                        Image = bitmapSource,
                        Tag = "仿射变换_放大"
                    };
                    ImageList.Add(model);
                    ImageSource = bitmapSource;
                    ImageIndex = ImageList.Count - 1;

                    GDIHelper.DeleteObject(hBitmap);

                    SrcImage = warp_rotate_dst;
                    Reecord("仿射变换_放大");
                }
            }
        }

        private void Rotate()
        {
            if (SrcImage != null)
            {
                OpenCvSharp.Point center = new OpenCvSharp.Point(SrcImage.Cols / 2, SrcImage.Rows / 2);
                float angle = 0.0f - Angle;
                float scale = 1.0f;
                Mat rot_mat = Cv2.GetRotationMatrix2D(center, angle, scale);
                Mat warp_rotate_dst = Mat.Zeros(SrcImage.Rows, SrcImage.Cols, SrcImage.Type());
                //Cv2.WarpAffine(SrcImage, warp_rotate_dst, rot_mat, warp_rotate_dst.Size());
                double sin_angle = Math.Sin(Math.Abs(angle) * Math.PI / 180);
                double cos_angle = Math.Cos(Math.Abs(angle) * Math.PI / 180);
                double new_height = SrcImage.Cols * sin_angle + SrcImage.Rows * cos_angle;
                double new_width = SrcImage.Cols * cos_angle + SrcImage.Rows * sin_angle;
                rot_mat.At<double>(0, 2) += (new_width - SrcImage.Cols) / 2;
                rot_mat.At<double>(1, 2) += (new_height - SrcImage.Rows) / 2;

                OpenCvSharp.Rect bbox = new RotatedRect(new Point2f(SrcImage.Cols / 2, SrcImage.Rows / 2), new Size2f(SrcImage.Cols, SrcImage.Rows), angle).BoundingRect();
                Cv2.WarpAffine(SrcImage, warp_rotate_dst, rot_mat, bbox.Size);

                var hBitmap = warp_rotate_dst.ToBitmap().GetHbitmap();
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                    hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());

                ImageModel model = new ImageModel()
                {
                    Image = bitmapSource,
                    Tag = "仿射变换_旋转"
                };
                ImageList.Add(model);
                ImageSource = bitmapSource;
                ImageIndex = ImageList.Count - 1;

                GDIHelper.DeleteObject(hBitmap);

                SrcImage = warp_rotate_dst;
                Reecord("仿射变换_旋转");
            }
        }

        private void Translation()
        {
            if (SrcImage != null)
            {
                Point2f[] srcTri = new Point2f[3];
                srcTri[0] = new Point2f(0.0f, 0.0f);
                srcTri[1] = new Point2f(SrcImage.Cols - 1.0f, 0.0f);
                srcTri[2] = new Point2f(0.0f, SrcImage.Rows - 1.0f);
                Point2f[] dstTri = new Point2f[3];
                dstTri[0] = new Point2f(Horizontal, Vertical);
                dstTri[1] = new Point2f(SrcImage.Cols - 1.0f + Horizontal, Vertical);
                dstTri[2] = new Point2f(Horizontal, SrcImage.Rows - 1.0f + Vertical);

                Mat warp_mat = Cv2.GetAffineTransform(srcTri, dstTri);
                Mat warp_dst = Mat.Zeros(SrcImage.Rows + Vertical, SrcImage.Cols + Horizontal, SrcImage.Type());

                Cv2.WarpAffine(SrcImage, warp_dst, warp_mat, warp_dst.Size());

                var hBitmap = warp_dst.ToBitmap().GetHbitmap();
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                                    hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());

                ImageModel model = new ImageModel()
                {
                    Image = bitmapSource,
                    Tag = "仿射变换_平移"
                };
                ImageList.Add(model);
                ImageSource = bitmapSource;
                ImageIndex = ImageList.Count - 1;

                GDIHelper.DeleteObject(hBitmap);

                SrcImage = warp_dst;
                Reecord("仿射变换_平移");
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

                ImageModel model = new ImageModel()
                {
                    Image = bi,
                    Tag = "原始图像"
                };
                ImageList.Add(model);
                Reecord("加载图像");
                //SrcImage = new Mat(file.FileName, ImreadModes.AnyColor);
            }
        }

        private void ClearImage()
        {
            if (ImageList != null || ImageList.Count > 0)
            {
                ImageList.Clear();
                ImageSource = null;
            }
        }

        private void UpdateImage(BitmapSource source)
        {
            ImageSource = source;
            Bitmap bitmap;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(ms);
                bitmap = new Bitmap(ms);
            }
            SrcImage = BitmapConverter.ToMat(bitmap);
        }

        private void Reecord(string content)
        {
            string log = DateTime.Now.ToString("hh:mm:ff") + "\t" + content + "\n";
            LogContent += log;
        }
    }
}
