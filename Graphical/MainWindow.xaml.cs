using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Graphical
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext= this;
        }

        private void SwitchTo_HexViewer(object sender, RoutedEventArgs e)
        { MainCarousel.CurrentIndex = 1; }

        public static void Show_LoadingOverlayer()
        {
            OverlayerVisible = true;
            StartUpOverlayerVisible = Visibility.Collapsed;
            LoadingOverlayerVisible = Visibility.Visible;
        }

        public static void Show_StartUpOverlayer()
        {
            OverlayerVisible = true;
            StartUpOverlayerVisible = Visibility.Visible;
            LoadingOverlayerVisible = Visibility.Collapsed;
        }

        #region DataBinds

            #region MainWindow

            public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

            private static bool _overlayerVisible = false;
            public static bool OverlayerVisible
            {
                get => _overlayerVisible;
                set
                {
                    _overlayerVisible = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(OverlayerVisible)));
                }
            }

            private static Visibility _loadingOverlayerVisible = Visibility.Collapsed;
            public static Visibility LoadingOverlayerVisible
            {
                get => _loadingOverlayerVisible;
                set
                {
                    _loadingOverlayerVisible = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(LoadingOverlayerVisible)));
                }
            }

            private static Visibility _startUpOverlayerVisible = Visibility.Collapsed;
            public static Visibility StartUpOverlayerVisible
            {
                get => _startUpOverlayerVisible;
                set
                {
                    _startUpOverlayerVisible = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(StartUpOverlayerVisible)));
                }
            }

            #endregion

        #endregion
    }
}
