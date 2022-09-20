using CV_ManipulationTool.ViewModel;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CV_ManipulationTool.View
{
    /// <summary>
    /// HistogramView.xaml 的交互逻辑
    /// </summary>
    public partial class HistogramView : Window
    {
        HistogramViewModel viewmodel;
        private bool IsTrans = true;
        private Point RectStartPoint;
        private Point RectRunPoint;
        private Point RectStopPoint;
        private Rect ROIRect = Rect.Empty;

        public HistogramView()
        {
            InitializeComponent();

            viewmodel = new HistogramViewModel();
            this.DataContext = viewmodel;
        }

        private void Simple_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ROIBtn.IsChecked != true)
            {
                if (viewmodel.ImageSource != null)
                {
                    PlotView plotView = Chart;
                    Image image = Map;
                    Rectangle rectangle = ROI;

                    if (IsTrans)
                    {
                        Complex.Children.Clear();
                        Simple.Children.Clear();
                        Complex.Children.Add(plotView);
                        Simple.Children.Add(image);
                        Simple.Children.Add(rectangle);
                    }
                    else
                    {
                        Complex.Children.Clear();
                        Simple.Children.Clear();
                        Complex.Children.Add(image);
                        Complex.Children.Add(rectangle);
                        Simple.Children.Add(plotView);
                    }

                    IsTrans = !IsTrans;
                }
            }
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ROIBtn.IsChecked == true)
            {
                RectStartPoint = e.GetPosition((Grid)((Image)sender).Parent); //获得鼠标按下的pictureBox上坐标
            }
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (ROIBtn.IsChecked == true)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    RectRunPoint = e.GetPosition((Grid)((Image)sender).Parent); //获得鼠标按下的pictureBox上坐标
                                                                                //! 实时更新图像，持续更新矩形
                    Rect rect = new Rect(RectStartPoint, RectRunPoint);
                    ROI.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
                    ROI.Width = rect.Width;
                    ROI.Height = rect.Height;

                    int ImageX = (int)(1088 / ((Grid)((Image)sender).Parent).ActualWidth * rect.X);
                    int ImageY = (int)(374 / ((Grid)((Image)sender).Parent).ActualHeight * rect.Y);
                    int Width = (int)(1088 / ((Grid)((Image)sender).Parent).ActualWidth * rect.Width);
                    int Height = (int)(374 / ((Grid)((Image)sender).Parent).ActualHeight * rect.Height);

                    // 刷新信息
                    //viewmodel.ImageInfo = "Rect  X:" + ImageX + " Y:" + ImageY + " Height:" + Height + " Width:" + Width;
                }
            }
        }

        private void ROI_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ROIBtn.IsChecked == true)
            {
                RectStopPoint = e.GetPosition((Grid)((Rectangle)sender).Parent); //获得鼠标按下的pictureBox上坐标
                Rect rect = new Rect(RectStartPoint, RectStopPoint);
                ROI.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
                ROI.Width = rect.Width;
                ROI.Height = rect.Height;

                int ImageX = (int)(viewmodel.ImageSource.PixelWidth / ((Grid)((Rectangle)sender).Parent).ActualWidth * rect.X);
                int ImageY = (int)(viewmodel.ImageSource.PixelHeight / ((Grid)((Rectangle)sender).Parent).ActualHeight * rect.Y);
                int Width = (int)(viewmodel.ImageSource.PixelWidth / ((Grid)((Rectangle)sender).Parent).ActualWidth * rect.Width);
                int Height = (int)(viewmodel.ImageSource.PixelHeight / ((Grid)((Rectangle)sender).Parent).ActualHeight * rect.Height);

                // 刷新信息
                //viewmodel.ImageInfo = "Rect  X:" + ImageX + " Y:" + ImageY + " Height:" + Height + " Width:" + Width;

                //! 根据画的矩形去截图做直方图
                ROIRect = new Rect(ImageX, ImageY, Width, Height);
                viewmodel.GetROIHistogramHSV(ROIRect);
            }
        }

        private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                Rect rect = new Rect(new Point(0,0), new Point(0,0));
                ROI.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
                ROI.Width = rect.Width;
                ROI.Height = rect.Height;
                ROIRect = Rect.Empty;
                viewmodel.GetROIHistogramHSV(ROIRect);
            }
        }

        private void RBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as RadioButton).Name;
            if (name == "RBtn")
            {
                viewmodel.RGBIndex = 0;
            }
            else if (name == "GBtn")
            {
                viewmodel.RGBIndex = 1;
            }
            else if (name == "BBtn")
            {
                viewmodel.RGBIndex = 2;
            }

            viewmodel.GetROIHistogramHSV(ROIRect);
        }
    }
}
