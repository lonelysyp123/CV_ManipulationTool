using CV_ManipulationTool.ViewModel;
using OxyPlot.Axes;
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
    /// PackColorView.xaml 的交互逻辑
    /// </summary>
    public partial class PackColorView : Window
    {
        private PackColorViewModel viewmodel;

        public PackColorView()
        {
            InitializeComponent();
            viewmodel = new PackColorViewModel();
            this.DataContext = viewmodel;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewmodel.XScale == 0 && viewmodel.YScale == 0)
            {
                viewmodel.XScale = viewmodel.SrcImage.Width / Img.ActualWidth;
                viewmodel.YScale = viewmodel.SrcImage.Height / Img.ActualHeight;
            }
            Point p = Mouse.GetPosition(Img);
            viewmodel.XValue = p.X.ToString();
            viewmodel.YValue = p.Y.ToString();
            var i = Img.ActualHeight;
            Console.WriteLine("Mouse Position: {0} ; {1}", p.X, p.Y);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewmodel.CloseListener();
        }
    }
}
