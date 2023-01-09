using Microsoft.Win32;
using Panuon.WPF.UI;
using QuartzCryptoTools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QuartzCryptoTools
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

        private void StartUp_ExitApplication(object sender, RoutedEventArgs e) // 退出程序
        { Close(); }

        private void StartUp_BrowseFile(object sender, RoutedEventArgs e) // 浏览文件
        {
            OpenFileDialog selFile = new OpenFileDialog
            { Multiselect = false, Title = "请选择文件", Filter = "所有文件(*.*)|*.*" };
            if ((bool)selFile.ShowDialog())
            { DataBind.FileLocation = selFile.FileName; }
        }

        private void StartUp_LoadFile(object sender, RoutedEventArgs e) // 加载文件
        {
            if (DataBind.FileLocation == "")
            { DataBind.FileLocationBorder = DataBind.FileLocationColor = new SolidColorBrush(Color.FromArgb(255, 255, 119, 119)); }
            else
            { File_LoadInfo(DataBind.FileLocation); }
        }

        private void Hide_MessageBox(object sender, RoutedEventArgs e) // 隐藏消息框
        { WindowOverlayer.Hide_MessageBox(); }

        #endregion

        #region 页面切换部分代码

        private void SwitchTo_MainMenu(object sender, RoutedEventArgs e)
        { MainCarousel.CurrentIndex = 0; }

        private void SwitchTo_HexViewer(object sender, RoutedEventArgs e)
        { MainCarousel.CurrentIndex = 1; File_LoadHex(); }

        private void SwitchTo_ZipFakeCrypto(object sender, RoutedEventArgs e)
        {
            if (HeaderDict[0] != "zip")
            { WindowOverlayer.Show_MessageBox("请选择zip格式压缩文件"); }
            else
            { MainCarousel.CurrentIndex = 2; Zip_LoadDirectory(); }
        }

        private void SwitchTo_LsbSteganography(object sender, RoutedEventArgs e)
        {
            if (HeaderDict[0] == "jpg" || HeaderDict[0] == "png")
            { MainCarousel.CurrentIndex = 3; }
            else
            { WindowOverlayer.Show_MessageBox("请选择图像格式(jpg/png)文件"); }
        }

        #endregion

        #region 主页面部分代码

        private static bool IsFileTypeRecorded;
        private static List<byte> FileHex;
        private string[] HeaderDict;

        private async void File_LoadInfo(String path)
        {
            WindowOverlayer.Hide_StartUp();
            // 载入文件字节
            WindowOverlayer.Show_Loading();
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
                HeaderDict = new string[1];
                DataBind.FileType = "Unrecorded File Type[" + FileHead.ToString() + "]";
                DataBind.FileDescription = "No Description";
                IsFileTypeRecorded = false;
            }
            // 载入文件名
            DataBind.FileName = path.Split(new char[] { '\\' }).Last().ToString();
        }

        private void File_LoadNew(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selFile = new OpenFileDialog
            { Multiselect = false, Title = "请选择文件", Filter = "所有文件(*.*)|*.*" };
            if ((bool)selFile.ShowDialog())
            { GC.Collect(); File_LoadInfo(selFile.FileName); IsHexLoaded = false; IsZipDictLoaded = false; }
        }

        private void File_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.FileName = DataBind.FileLocation.Split(new char[] { '\\' }).Last();
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
                    { HexDataTable.Columns.Add(Header, typeof(byte)); }
                    int ColumnIndex = 0; // 表格中的列号
                    DataRow row = HexDataTable.NewRow();
                    for (int i = 0; i < (int)Math.Ceiling((double)FileHex.Count / 16) * 16; i++)
                    {
                        // 是否写满一行
                        if (ColumnIndex != 16)
                        {
                            try
                            { row[ColumnHeader[ColumnIndex]] = FileHex[i]; }
                            catch (Exception)
                            { row[ColumnHeader[ColumnIndex]] = DBNull.Value; }
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
                            { row[ColumnHeader[ColumnIndex]] = FileHex[i]; }
                            catch (Exception)
                            { row[ColumnHeader[ColumnIndex]] = " "; }
                            ColumnIndex++;
                        }
                    }
                    // 向表格中添加最后一行
                    HexDataTable.Rows.Add(row);
                    // 修改待绑定数据
                    DataBind.HexTableBind = HexDataTable.DefaultView;
                    DataBind._isHexTableCHanged = false;
                    IsHexLoaded = true;
                });
                // 绑定数据
                HexTable.ItemsSource = DataBind.HexTableBind;
                WindowOverlayer.Hide_Loading();
            }
        }

        private void File_SaveHex(object sender, RoutedEventArgs e)
        {
            List<byte> bytes = new List<byte>();
            // 读取Hex表中每个Hex值并转换为Byte
            for (int i = 0; i < DataBind.HexTableBind.Count; i++)
            {
                for (int j = 0; j <= 15; j++)
                {
                    try
                    { bytes.Add(Convert.ToByte(DataBind.HexTableBind[i].Row[j].ToString().Substring(0, 2), 16)); }
                    catch (Exception) { }
                }
            }
            SaveFileDialog file = new SaveFileDialog();
            file.FileName = DataBind.FileLocation.Split(new char[] { '\\' }).Last();
            if (IsFileTypeRecorded) // 使用源文件后缀构造文件类型选项
            { file.Filter = file.FileName.Split(new char[] { '.' }).Last() + "格式文件(" + HeaderDict[2] + ")|" + HeaderDict[2] + "|所有文件(*.*)|*.*"; }
            else
            { file.Filter = "文件(*.*)|*.*"; }
            if ((bool)file.ShowDialog())
            { HexHelper.WriteHex(file.FileName, bytes); }
        }

        private void File_ReloadHex(object sender, RoutedEventArgs e)
        {
            IsHexLoaded = false;
            File_LoadHex();
        }

        #endregion

        #region zip目录修改器部分代码

        private static List<byte> ZipModifierTempFile = new List<byte>();
        private static bool IsZipDictLoaded = false;

        private async void Zip_LoadDirectory()
        {
            // 判断zip目录是否已经加载
            if (!IsZipDictLoaded)
            {
                ZipModifierTempFile = FileHex;
                String[] ColumnHeader = new string[] { "Name", "LastModify", "SuprressedSize", "PreviousSize", "Encrypted", "EncryptedIndex", "CRC-32" };
                byte[] DictFileHead = new byte[] { Convert.ToByte("50", 16), Convert.ToByte("4B", 16), Convert.ToByte("01", 16), Convert.ToByte("02", 16) };
                DataTable ArchiveDictTable = new DataTable();
                // 添加绑定项
                foreach (String Header in ColumnHeader)
                { ArchiveDictTable.Columns.Add(Header, typeof(String)); }
                // 载入压缩文件目录信息
                WindowOverlayer.Show_Loading();
                // 异步载入压缩文件信息
                await Task.Run(() =>
                {
                    for (int i = 0; i < ZipModifierTempFile.Count; i++)
                    {
                        // 匹配十六进制数据 50
                        if (ZipModifierTempFile[i] == DictFileHead[0])
                        {
                            // 匹配目录中文件文件头标记 50 4B 01 02
                            bool IsMatch = true;
                            int HeadIndex = 0;
                            foreach (byte oneByte in DictFileHead)
                            {
                                if (ZipModifierTempFile[i + HeadIndex] == DictFileHead[HeadIndex])
                                { HeadIndex++; }
                                else
                                { IsMatch = false; break; }
                            }
                            // 获取目录信息
                            if (IsMatch)
                            {
                                DataRow row = ArchiveDictTable.NewRow();
                                List<byte> DictHeader = new List<byte>();
                                for (int j = i; j < i + 46; j++)
                                { DictHeader.Add(ZipModifierTempFile[j]); }
                                // 加载加密信息
                                row["Encrypted"] = DictHeader[8].ToString("X2");
                                // 加载加密信息位置
                                row["EncryptedIndex"] = i + 8;
                                // 加载 CRC-32
                                row["CRC-32"] = DictHeader[19].ToString("X2") + DictHeader[18].ToString("X2") + DictHeader[17].ToString("X2") + DictHeader[16].ToString("X2");
                                // 加载压缩后大小
                                String SuppressedSizeHex = DictHeader[23].ToString("X2") + DictHeader[22].ToString("X2") + DictHeader[21].ToString("X2") + DictHeader[20].ToString("X2");
                                row["SuprressedSize"] = Convert.ToInt32(SuppressedSizeHex, 16);
                                // 加载压缩前大小
                                String PreviousSizeHex = DictHeader[27].ToString("X2") + DictHeader[26].ToString("X2") + DictHeader[25].ToString("X2") + DictHeader[24].ToString("X2");
                                row["PreviousSize"] = Convert.ToInt32(PreviousSizeHex, 16);
                                // 加载文件名长度
                                int FileNameLength = Convert.ToInt32(DictHeader[28]);
                                // 截取文件名
                                List<byte> FileNameByte = new List<byte>();
                                for (int j = i + 46; j < i + 46 + FileNameLength; j++)
                                { FileNameByte.Add(ZipModifierTempFile[j]); }
                                row["Name"] = Encoding.UTF8.GetString(FileNameByte.ToArray());
                                // 加载修改日期时间
                                String DosDateTimeHex = DictHeader[15].ToString("X2") + DictHeader[14].ToString("X2") + DictHeader[13].ToString("X2") + DictHeader[12].ToString("X2");
                                DateTime dateTime = DateTimeExtensions.ToDateTime((int)Convert.ToInt64(DosDateTimeHex, 16));
                                row["LastModify"] = dateTime.ToString();
                                // 加入文件信息
                                ArchiveDictTable.Rows.Add(row);
                                // 移动至该目录信息结束处
                                i = i + 45 + FileNameLength;
                            }
                        }
                    }
                    // 修改待绑定数据
                    DataBind.ArchiveDictTableBind = ArchiveDictTable.DefaultView;
                    DataBind._isArchiveTableChanged = false;
                    IsZipDictLoaded = true;
                });
                // 绑定数据
                ArchiveDict.ItemsSource = DataBind.ArchiveDictTableBind;
                WindowOverlayer.Hide_Loading();
            }
        }

        private void Zip_SaveArchive(object sender, RoutedEventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.FileName = DataBind.FileLocation.Split(new char[] { '\\' }).Last();
            if (IsFileTypeRecorded) // 使用源文件后缀构造文件类型选项
            { file.Filter = file.FileName.Split(new char[] { '.' }).Last() + "格式文件(" + HeaderDict[2] + ")|" + HeaderDict[2] + "|所有文件(*.*)|*.*"; }
            else
            { file.Filter = "文件(*.*)|*.*"; }
            if ((bool)file.ShowDialog())
            { HexHelper.WriteHex(file.FileName, ZipModifierTempFile); }
        }

        private void Zip_ReloadDirectory(object sender, RoutedEventArgs e)
        {
            IsZipDictLoaded = false;
            Zip_LoadDirectory();
        }

        private void Zip_FakeEncrypt(object sender, RoutedEventArgs e)
        {
            if (ArchiveDict.SelectedIndex != -1)
            {
                ZipModifierTempFile[Convert.ToInt32(DataBind.ArchiveDictTableBind[ArchiveDict.SelectedIndex].Row[2].ToString())] = Convert.ToByte("09", 16);
                DataBind.ArchiveDictTableBind[ArchiveDict.SelectedIndex].Row[1] = "09";
            }

        }

        private void Zip_FakeDecrypt(object sender, RoutedEventArgs e)
        {
            if (ArchiveDict.SelectedIndex != -1)
            {
                ZipModifierTempFile[Convert.ToInt32(DataBind.ArchiveDictTableBind[ArchiveDict.SelectedIndex].Row[2].ToString())] = Convert.ToByte("00", 16);
                DataBind.ArchiveDictTableBind[ArchiveDict.SelectedIndex].Row[1] = "00";
            }
        }

        #endregion

        #region Lsb隐写部分代码

        private void LoadLsbData()
        {

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

        public static Visibility MessageBoxOverlayerVisible
        {
            get => _messageBoxOverlayerVisible;
            set
            {
                _messageBoxOverlayerVisible = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MessageBoxOverlayerVisible)));
            }
        }
        private static Visibility _messageBoxOverlayerVisible = Visibility.Collapsed;

        public static String MessageBoxString
        {
            get => _messageBoxString;
            set
            {
                _messageBoxString = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MessageBoxString)));
            }
        }
        private static String _messageBoxString = "";

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

        public static String FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FileName)));
            }
        }
        private static String _fileName = "";

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
                _isHexTableCHanged = true;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(HexTableBind)));
            }
        }
        private static DataView _hexTableBind = new DataView();
        public static bool _isHexTableCHanged = false;

        #endregion

        #region Zip目录修改器部分绑定

        public static DataView ArchiveDictTableBind
        {
            get => _archiveDictTableBind;
            set
            {
                _archiveDictTableBind = value;
                _isArchiveTableChanged = true;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(ArchiveDictTableBind)));
            }
        }
        private static DataView _archiveDictTableBind = new DataView();
        public static bool _isArchiveTableChanged = false;

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

        public static void Show_MessageBox(string message)
        { DataBind.OverlayerVisible = true; DataBind.MessageBoxOverlayerVisible = Visibility.Visible; DataBind.MessageBoxString = message; }

        public static void Hide_MessageBox()
        { DataBind.OverlayerVisible = false; DataBind.MessageBoxOverlayerVisible = Visibility.Collapsed; DataBind.MessageBoxString = ""; }
    }
}
