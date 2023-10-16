using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CV_ManipulationTool.ViewModel
{
    public class GrayEqualizationViewModel : ObservableObject
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

        private BitmapSource _secondImage;
        public BitmapSource SecondImage
        {
            get => _secondImage;
            set
            {
                SetProperty(ref _secondImage, value);
            }
        }

        public RelayCommand LoadImageCommand { get; private set; }
        public RelayCommand SaveImageCommand { get; private set; }
        public RelayCommand GrayEquailzationCommand { get; private set; }

        public GrayEqualizationViewModel() 
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            SaveImageCommand = new RelayCommand(SaveImage);
            GrayEquailzationCommand = new RelayCommand(GrayEquailzation);
        }

        private void GrayEquailzation()
        {
            throw new NotImplementedException();
        }

        private void SaveImage()
        {
            throw new NotImplementedException();
        }

        private void LoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "*.png|*.bmp|*.gif|*.tif";
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                BitmapImage bi = new BitmapImage();
            }
        }
    }
}
