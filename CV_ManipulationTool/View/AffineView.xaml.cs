using CV_ManipulationTool.Tool;
using CV_ManipulationTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CV_ManipulationTool.View
{
    /// <summary>
    /// AffineView.xaml 的交互逻辑
    /// </summary>
    public partial class AffineView : Window
    {
        private Storyboard mStoryboard0;
        private Storyboard mStoryboard1;
        private AffineViewModel viewmodel;
        private bool isShowedMenu = false;
        public AffineView()
        {
            InitializeComponent();

            viewmodel = new AffineViewModel();
            this.DataContext = viewmodel;
            InitAnimation();
            
        }

        private void InitAnimation()
        {
            //! 展示
            DoubleAnimation da = new DoubleAnimation();
            da.From = null;
            da.To = 100;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            HideMenu.RenderTransform = new TranslateTransform();
            mStoryboard0 = new Storyboard();
            mStoryboard0.Children.Add(da);
            Storyboard.SetTarget(da, HideMenu);
            Storyboard.SetTargetProperty(da, new PropertyPath("RenderTransform.X"));

            //！隐藏
            DoubleAnimation doa = new DoubleAnimation();
            doa.From = null;
            doa.To = 0;
            doa.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            mStoryboard1 = new Storyboard();
            mStoryboard1.Children.Add(doa);
            Storyboard.SetTarget(doa, HideMenu);
            Storyboard.SetTargetProperty(doa, new PropertyPath("RenderTransform.X"));
        }

        private void HideMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isShowedMenu)
            {
                mStoryboard0.Begin(this);
                isShowedMenu = true;
                BtnBo.Visibility = Visibility.Collapsed;
            }
            
        }
        
        private void HideMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isShowedMenu)
            {
                mStoryboard1.Begin(this);
                isShowedMenu = false;
                BtnBo.Visibility = Visibility.Visible;
            }
        }
    }
}
