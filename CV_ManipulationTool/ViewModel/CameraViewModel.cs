using CV_ManipulationTool.Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CV_ManipulationTool.ViewModel
{
    public class CameraViewModel : ObservableObject
    {
        private BitmapSource _mainImage;
        public BitmapSource MainImage
        {
            get => _mainImage;
            set
            {
                SetProperty(ref _mainImage, value);
            }
        }

        private ObservableCollection<string> _cameraList;
        public ObservableCollection<string> CameraList
        {
            get => _cameraList;
            set
            {
                SetProperty(ref _cameraList, value);
            }
        }

        private int _cameraIndex;
        public int CameraIndex
        {
            get => _cameraIndex;
            set
            {
                SetProperty(ref _cameraIndex, value);
            }
        }

        private string _cameraHeight;
        public string CameraHeight
        {
            get => _cameraHeight;
            set
            {
                SetProperty(ref _cameraHeight, value);
            }
        }

        private string _cameraWidth;
        public string CameraWidth
        {
            get => _cameraWidth;
            set
            {
                SetProperty(ref _cameraWidth, value);
            }
        }

        private string _cameraFps;
        public string CameraFps
        {
            get => _cameraFps;
            set
            {
                SetProperty(ref _cameraFps, value);
            }
        }

        private bool _flipX_flag;
        public bool FlipX_flag
        {
            get => _flipX_flag;
            set
            {
                SetProperty(ref _flipX_flag, value);
            }
        }

        private bool _flipY_flag;
        public bool FlipY_flag
        {
            get => _flipY_flag;
            set
            {
                SetProperty(ref _flipY_flag, value);
            }
        }

        private bool _collimator_flag;
        public bool Collimator_flag
        {
            get => _collimator_flag;
            set
            {
                SetProperty(ref _collimator_flag, value);
            }
        }

        private bool _roi_flag;
        public bool ROI_flag
        {
            get => _roi_flag;
            set
            {
                SetProperty(ref _roi_flag, value);
            }
        }

        private string _recordVideoContent;
        public string RecordVideoContent
        {
            get => _recordVideoContent;
            set
            {
                SetProperty(ref _recordVideoContent, value);
            }
        }

        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand OpenCameraCommand { get; private set; }
        public RelayCommand CloseCameraCommand { get; private set; }
        public RelayCommand SaveScreenshotsCommand { get; private set; }
        public RelayCommand RecordVideoCommand { get; private set; }
        public RelayCommand RGB2GrayCommand { get; private set; }
        public RelayCommand Gray2RGBCommand { get; private set; }

        private VideoCapture m_VideoCapture;
        public bool m_CameraOpen_flag;
        private bool m_SaveScreenshots_flag;
        private Mat Screenshots;
        private bool m_RecordVideo_flag;
        private Queue<Mat> VideoQ;
        private bool Gray_flag;

        public CameraViewModel() 
        {
            RefreshCommand = new RelayCommand(Refresh);
            OpenCameraCommand = new RelayCommand(OpenCamera);
            CloseCameraCommand = new RelayCommand(CloseCamera);
            SaveScreenshotsCommand = new RelayCommand(SaveScreenshots);
            RecordVideoCommand = new RelayCommand(RecordVideo);
            RGB2GrayCommand = new RelayCommand(RGB2Gray);
            Gray2RGBCommand = new RelayCommand(Gray2RGB);

            RecordVideoContent = "开始录制";
        }

        private void Gray2RGB()
        {
            Gray_flag = false;
        }

        private void RGB2Gray()
        {
            Gray_flag = true;
        }

        private void RecordVideo()
        {
            if (m_RecordVideo_flag)
            {
                m_RecordVideo_flag = false;
                RecordVideoContent = "开始录制";

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "图像保存";
                dialog.RestoreDirectory = true;
                dialog.Filter = "Video File(*.avi;*.mp4;*.mkv;*.wav;*.rmvb)|*.avi;*.mp4;*.mkv;*.wav;*.rmvb|All File(*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    if (!string.IsNullOrEmpty(dialog.FileName))
                    {
                        VideoWriter writer = new VideoWriter(
                            dialog.FileName, 
                            VideoWriter.FourCC(@"MJPG"), 
                            double.Parse(CameraFps), 
                            new OpenCvSharp.Size(double.Parse(CameraWidth), double.Parse(CameraHeight)));
                        
                        while(VideoQ.Any())
                        {
                            Mat vmat = VideoQ.Dequeue();
                            writer.Write(vmat);
                        }
                        writer.Release();
                    }
                }
            }
            else
            {
                VideoQ = new Queue<Mat>();
                m_RecordVideo_flag = true;
                RecordVideoContent = "停止录制";
            }
        }

        private void SaveScreenshots()
        {
            m_SaveScreenshots_flag = true;
            while (m_SaveScreenshots_flag)
            {
                Thread.Sleep(10);
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "图像保存";
            dialog.RestoreDirectory = true;
            dialog.Filter = "jpg图片|*.JPG|gif图片|*.GIF|png图片|*.PNG|jpeg图片|*.JPEG|BMP图片|*.BMP";
            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    Screenshots.SaveImage(dialog.FileName);
                }
            }
        }

        private void CloseCamera()
        {
            if(m_CameraOpen_flag)
            {
                m_CameraOpen_flag = false;
                m_VideoCapture.Release();
            }
        }

        private void OpenCamera()
        {
            if (!m_CameraOpen_flag)
            {
                try
                {
                    m_VideoCapture = new VideoCapture(CameraIndex);
                    // 相机属性配置
                    //m_VideoCapture.Set(VideoCaptureProperties.FrameWidth, 640);//宽度
                    //m_VideoCapture.Set(VideoCaptureProperties.FrameHeight, 360);//高度
                    m_VideoCapture.Fps = 60;//帧  

                    CameraHeight = m_VideoCapture.FrameHeight.ToString();
                    CameraWidth = m_VideoCapture.FrameWidth.ToString();
                    CameraFps = ((int)m_VideoCapture.Fps).ToString();

                    if (m_VideoCapture.IsOpened())
                    {
                        m_CameraOpen_flag = true;
                        Thread Video_thread = new Thread(Play_Video);
                        Video_thread.IsBackground = true;
                        Video_thread.Start();
                        MainImage = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("OpenCV方式打开摄像头异常：" + ex.ToString());
                }
            }
        }

        Mat mat = new Mat();
        private void Play_Video()
        {
            while (m_CameraOpen_flag)
            {
                if (m_VideoCapture.Read(mat))
                {
                    int sleepTime = (int)Math.Round(1000 / m_VideoCapture.Fps);
                    Cv2.WaitKey(sleepTime);
                    if (mat.Empty())
                    {
                        break;
                    }
                    if (Gray_flag)
                    {
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
                    }
                    if (FlipX_flag)
                    {
                        Cv2.Flip(mat, mat, OpenCvSharp.FlipMode.X);
                    }
                    if (FlipY_flag)
                    {
                        Cv2.Flip(mat, mat, OpenCvSharp.FlipMode.Y);
                    }
                    if (Collimator_flag)
                    {
                        Cv2.Line(mat, new OpenCvSharp.Point(mat.Cols / 2, mat.Rows / 2 + 20), new OpenCvSharp.Point(mat.Cols / 2, mat.Rows / 2 - 20), new Scalar(0, 0, 255), 2, LineTypes.Link8);
                        Cv2.Line(mat, new OpenCvSharp.Point(mat.Cols / 2 + 20, mat.Rows / 2), new OpenCvSharp.Point(mat.Cols / 2 - 20, mat.Rows / 2), new Scalar(0, 0, 255), 2, LineTypes.Link8);
                    }
                    if(ROI_flag)
                    {
                        //int V_width = mat.Width;
                        //int V_height = mat.Height;
                        //int start_x = mat.Width / 4;
                        //int start_y = mat.Height / 4;
                        //OpenCvSharp.Point strat_point = new OpenCvSharp.Point(mat.Width / 2, mat.Height / 2);
                        //OpenCvSharp.Rect ROI_rect = new OpenCvSharp.Rect(start_x, start_y, V_width / 2, V_height / 2);
                        //Cv2.Rectangle(mat, ROI_rect, new Scalar(255, 255, 0), 2);

                        if (Gray_flag)
                        {
                            Cv2.EqualizeHist(mat, mat);
                            CascadeClassifier cascade = new CascadeClassifier("./Resource/haarcascade_frontalface_alt.xml");
                            OpenCvSharp.Rect[] rects = cascade.DetectMultiScale(
                                image: mat,
                                scaleFactor: 1.1,
                                minNeighbors: 1,
                                flags: HaarDetectionTypes.DoRoughSearch | HaarDetectionTypes.ScaleImage,
                                minSize: new OpenCvSharp.Size(30, 30));
                            if (rects.Length > 0)
                            {
                                for (int i = 0; i < rects.Length; i++)
                                {
                                    Cv2.Rectangle(mat, rects[i], new Scalar(255, 255, 0), 2);
                                }
                            }
                        }
                    }
                    if(m_SaveScreenshots_flag)
                    {
                        Screenshots = mat.Clone();
                        m_SaveScreenshots_flag = false;
                    }
                    if(m_RecordVideo_flag)
                    {
                        VideoQ.Enqueue(mat.Clone());
                    }

                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainImage = Mat2BitmapSource(mat);
                    }));
                }
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

        private void Refresh()
        {
            var items = Win32_Api_Helper.GetCameraList();
            if (items != null)
            {
                CameraList = new ObservableCollection<string>(items);
                CameraIndex = 0;
            }
        }

        private void Camera_Devices_Init()
        {
            var frame = Cv2.CreateFrameSource_Camera(0);
            VideoCapture capture = new VideoCapture();
        }
    }
}
