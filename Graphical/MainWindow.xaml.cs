using Graphical.Utils;
using Microsoft.Win32;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            this.DataContext = new DataBind();
            WindowOverlayer.Show_StartUp();
        }

        private void Overlayer_MouseMove(object sender, MouseEventArgs e)
        { if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); } }

        #region 开屏弹窗部分代码

        private void StartUp_ExitApplication(object sender, RoutedEventArgs e)
        { Close(); }

        private void StartUp_BrowseFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selFile = new OpenFileDialog
            { Multiselect = false, Title = "请选择文件", Filter = "所有文件(*.*)|*.*" };
            if ((bool)selFile.ShowDialog())
            { DataBind.FileLocation = selFile.FileName; }
        }

        private void StartUp_LoadFile(object sender, RoutedEventArgs e)
        {
            if (DataBind.FileLocation == "")
            { DataBind.FileLocationBorder = DataBind.FileLocationColor = new SolidColorBrush(Color.FromArgb(255, 255, 119, 119)); }
            else
            { File_LoadInfo(DataBind.FileLocation); }
        }

        #endregion

        #region 页面切换部分代码

        private void SwitchTo_MainMenu(object sender, RoutedEventArgs e)
        { MainCarousel.CurrentIndex = 0; }

        private void SwitchTo_HexViewer(object sender, RoutedEventArgs e)
        { MainCarousel.CurrentIndex = 1; File_LoadHex(); }

        private void SwitchTo_ZipFakeCrypto(object sender, RoutedEventArgs e)
        { MainCarousel.CurrentIndex = 2; }

        #endregion

        #region 主页面部分代码

        private static bool IsFileTypeRecorded;
        private static String FileLocation;
        private static List<byte> FileHex;
        private string[] HeaderDict;

        private async void File_LoadInfo(String path)
        {
            WindowOverlayer.Hide_StartUp();
            // 载入文件字节
            WindowOverlayer.Show_Loading();
            FileLocation = path;
            // 异步读取文件字节
            await Task.Run(() => { FileHex = HexHelper.ReadHex(path); });
            WindowOverlayer.Hide_Loading();
            // 载入文件长度
            DataBind.FileLength = FileHex.Count();
            // 载入文件头
            List<byte> FileHeadBytes = FileHex.Take(4).ToList();
            StringBuilder FileHead = new StringBuilder();
            foreach (byte HeaderByte in FileHeadBytes)
            { FileHead.Append(HeaderByte.ToString("X2")); }
            // 判断文件类型
            try
            {
                HeaderDict = FileHeaders.Headers[FileHead.ToString()];
                // 通过预定义的字典构造文件类型选项
                DataBind.FileType = HeaderDict[0] + " (" + path.Split(new char[] { '\\' }).Last().Split(new char[] { '.' }).Last() + ") [" + FileHead.ToString() + "]";
                DataBind.FileDescription = HeaderDict[1];
                IsFileTypeRecorded = true;
            }
            catch (Exception)
            {
                // 未被定义的文件类型
                DataBind.FileType = "Unrecorded File Type[" + FileHead.ToString() + "]";
                DataBind.FileDescription = "No Description";
                IsFileTypeRecorded = false;
            }
        }

        private void File_LoadNew(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selFile = new OpenFileDialog
            { Multiselect = false, Title = "请选择文件", Filter = "所有文件(*.*)|*.*" };
            if ((bool)selFile.ShowDialog())
            { GC.Collect(); File_LoadInfo(selFile.FileName); IsHexLoaded = false; }
        }

        private void File_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.FileName = FileLocation.Split(new char[] { '\\' }).Last();
            if (IsFileTypeRecorded) // 使用源文件后缀构造文件类型选项
            { file.Filter = file.FileName.Split(new char[] { '.' }).Last() + "格式文件(" + HeaderDict[2] + ")|" + HeaderDict[2] + "|所有文件(*.*)|*.*"; }
            else
            { file.Filter = "文件(*.*)|*.*"; }
            if ((bool)file.ShowDialog())
            { HexHelper.WriteHex(file.FileName, FileHex); }
        }

        #endregion

        #region Hex编辑器部分代码

        private static bool IsHexLoaded = false;

        private async void File_LoadHex()
        {
            // 判断文件Hex是否已经加载
            if (!IsHexLoaded)
            {
                // 载入文件Hex信息
                WindowOverlayer.Show_Loading();
                // 异步加载Hex数据
                await Task.Run(() => 
                {
                    String[] ColumnHeader = new string[] { "C00", "C01", "C02", "C03", "C04", "C05", "C06", "C07", "C08", "C09", "C0a", "C0b", "C0c", "C0d", "C0e", "C0f" };
                    DataTable HexDataTable = new DataTable();
                    // 添加绑定项
                    foreach (String Header in ColumnHeader)
                    { HexDataTable.Columns.Add(Header, typeof(String)); }
                    foreach (String Header in ColumnHeader)
                    {
                        HexDataTable.Columns[Header].MaxLength = 2;
                    }
                    int ColumnIndex = 0; // 表格中的列号
                    DataRow row = HexDataTable.NewRow();
                    for (int i = 0; i < (int)Math.Ceiling((double)FileHex.Count / 16) * 16; i++)
                    {
                        // 是否写满一行
                        if (ColumnIndex != 16)
                        {
                            try
                            { row[ColumnHeader[ColumnIndex]] = FileHex[i].ToString("X2"); }
                            catch (Exception)
                            { row[ColumnHeader[ColumnIndex]] = " "; }
                            ColumnIndex++;
                        }
                        else
                        {
                            // 将已写满的一行添加至表格
                            HexDataTable.Rows.Add(row);
                            // 创建新行
                            row = HexDataTable.NewRow();
                            //重置列号
                            ColumnIndex = 0;
                            try
                            { row[ColumnHeader[ColumnIndex]] = FileHex[i].ToString("X2"); }
                            catch (Exception)
                            { row[ColumnHeader[ColumnIndex]] = " "; }
                            ColumnIndex++;
                        }
                    }
                    // 向表格中添加最后一行
                    HexDataTable.Rows.Add(row);
                    // 修改待绑定数据
                    DataBind.HexTableBind = HexDataTable.DefaultView;
                    IsHexLoaded = true;
                });
                // 绑定数据
                HexTable.ItemsSource = DataBind.HexTableBind;
                WindowOverlayer.Hide_Loading();
            }
        }

        #endregion
    }

    public class DataBind //数据绑定类
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        #region 全局绑定

        public static String FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileLocation)));
            }
        }
        private static String _fileLocation = "";

        #endregion

        #region 开屏弹窗部分绑定

        public static bool OverlayerVisible
        {
            get => _overlayerVisible;
            set
            {
                _overlayerVisible = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(OverlayerVisible)));
            }
        }
        private static bool _overlayerVisible = false;

        public static Visibility LoadingOverlayerVisible
        {
            get => _loadingOverlayerVisible;
            set
            {
                _loadingOverlayerVisible = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(LoadingOverlayerVisible)));
            }
        }
        private static Visibility _loadingOverlayerVisible = Visibility.Collapsed;

        public static Visibility StartUpOverlayerVisible
        {
            get => _startUpOverlayerVisible;
            set
            {
                _startUpOverlayerVisible = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(StartUpOverlayerVisible)));
            }
        }
        private static Visibility _startUpOverlayerVisible = Visibility.Collapsed;

        public static Brush FileLocationBorder
        {
            get => _fileLocationBorder;
            set
            {
                _fileLocationBorder = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileLocationBorder)));
            }
        }
        private static Brush _fileLocationBorder = new SolidColorBrush(Color.FromArgb(255, 187, 187, 187));

        public static Brush FileLocationColor
        {
            get => _fileLocationColor;
            set
            {
                _fileLocationColor = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileLocationColor)));
            }
        }
        private static Brush _fileLocationColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        #endregion

        #region 主菜单部分绑定

        public static String FileType
        {
            get => _fileType;
            set
            {
                _fileType = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileType)));
            }
        }
        private static String _fileType = "";

        public static String FileDescription
        {
            get => _fileDescription;
            set
            {
                _fileDescription = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileDescription)));
            }
        }
        private static String _fileDescription = "";

        public static int FileLength
        {
            get => _fileLength;
            set
            {
                _fileLength = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileLength)));
            }
        }
        private static int _fileLength = 0;


        #endregion

        #region Hex查看器部分绑定

        public static DataView HexTableBind
        {
            get => _hexTableBind;
            set
            {
                _hexTableBind = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(HexTableBind)));
            }
        }
        private static DataView _hexTableBind = new DataView();

        #endregion
    }

    public class WindowOverlayer //遮罩层操作类
    {
        public static void Show_Loading()
        { DataBind.OverlayerVisible = true; DataBind.LoadingOverlayerVisible = Visibility.Visible; }

        public static void Hide_Loading()
        { DataBind.OverlayerVisible = false; DataBind.LoadingOverlayerVisible = Visibility.Collapsed; }

        public static void Show_StartUp()
        { DataBind.OverlayerVisible = true; DataBind.StartUpOverlayerVisible = Visibility.Visible; }

        public static void Hide_StartUp()
        { DataBind.OverlayerVisible = false; DataBind.StartUpOverlayerVisible = Visibility.Collapsed; }
    }
}
