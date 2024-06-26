﻿using CV_ManipulationTool.ViewModel;
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

namespace CV_ManipulationTool.View.Detection
{
    /// <summary>
    /// HoughView.xaml 的交互逻辑
    /// </summary>
    public partial class HoughView : Window
    {
        HoughViewModel viewmodel;
        public HoughView()
        {
            InitializeComponent();

            viewmodel = new HoughViewModel();
            this.DataContext = viewmodel;
        }
    }
}
