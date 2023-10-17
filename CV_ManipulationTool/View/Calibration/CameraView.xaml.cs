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

namespace CV_ManipulationTool.View.Calibration
{
    /// <summary>
    /// CameraView.xaml 的交互逻辑
    /// </summary>
    public partial class CameraView : Window
    {
        CameraViewModel viewmodel;

        public CameraView()
        {
            InitializeComponent();
            viewmodel = new CameraViewModel();
            this.DataContext = viewmodel;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (viewmodel != null)
            {
                if (viewmodel.m_CameraOpen_flag)
                {
                    viewmodel.CloseCameraCommand.Execute(null);
                }
            }
        }
    }
}
