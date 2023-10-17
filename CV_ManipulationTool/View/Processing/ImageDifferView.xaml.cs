using CV_ManipulationTool.ViewModel;
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

namespace CV_ManipulationTool.View.Processing
{
    /// <summary>
    /// ImageDifferView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDifferView : Window
    {
        ImageDifferViewModel viewmodel;
        public ImageDifferView()
        {
            InitializeComponent();

            viewmodel = new ImageDifferViewModel();
            this.DataContext = viewmodel;
        }
    }
}
