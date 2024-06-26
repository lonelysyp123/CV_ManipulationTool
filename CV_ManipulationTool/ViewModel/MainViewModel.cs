﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV_ManipulationTool.Common;
using CV_ManipulationTool.View;
using CV_ManipulationTool.View.Calibration;
using CV_ManipulationTool.View.Detection;
using CV_ManipulationTool.View.Pretreatment;
using CV_ManipulationTool.View.Processing;
using CV_ManipulationTool.View.Transformation;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;

namespace CV_ManipulationTool.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand AffineCommand { get; private set; }
        public RelayCommand HistogramCommand { get; private set; }
        public RelayCommand DemoCommand { get; private set; }
        public RelayCommand CannyCommand { get; private set; }
        public RelayCommand HoughCommand { get; private set; }
        public RelayCommand PackColorCommand { get; private set; }
        public RelayCommand ImageDifferCommand { get; private set; }
        public RelayCommand ImageJoinCommand { get; private set; }
        public RelayCommand GrayImageEqualizationCommand { get; private set; }
        public RelayCommand OnCameraCommand { get; private set; }
        public RelayCommand GrayImageThresholdCommand { get; private set; }
        public RelayCommand GrayImageFilterCommand { get; private set; }

        public MainViewModel()
        {
            AffineCommand = new RelayCommand(AffineFunc);
            HistogramCommand = new RelayCommand(Histogram);
            DemoCommand = new RelayCommand(Demo);
            CannyCommand = new RelayCommand(Canny);
            HoughCommand = new RelayCommand(Hough);
            PackColorCommand = new RelayCommand(PackColor);
            ImageDifferCommand = new RelayCommand(ImageDiffer);
            ImageJoinCommand = new RelayCommand(ImageJoin);
            GrayImageEqualizationCommand = new RelayCommand(GrayImageEqualization);
            OnCameraCommand = new RelayCommand(OnCamera);
            GrayImageThresholdCommand = new RelayCommand(GrayImageThreshold);
            GrayImageFilterCommand = new RelayCommand(GrayImageFilter);
        }

        private void GrayImageFilter()
        {
            GrayImageFilterView view = new GrayImageFilterView();
            view.ShowDialog();
        }

        private void GrayImageThreshold()
        {
            GrayImageThresholdView view = new GrayImageThresholdView();
            view.ShowDialog();
        }

        private void OnCamera()
        {
            CameraView view = new CameraView();
            view.ShowDialog();
        }

        private void GrayImageEqualization()
        {
            GrayEqualizationView view = new GrayEqualizationView();
            view.ShowDialog();
        }

        private void ImageJoin()
        {
            ImageJoinView view = new ImageJoinView();
            view.ShowDialog();
        }

        private void ImageDiffer()
        {
            ImageDifferView view = new ImageDifferView();
            view.ShowDialog();
        }

        private void PackColor()
        {
            PackColorView view = new PackColorView();
            view.ShowDialog();
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
