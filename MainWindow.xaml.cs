using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using static IXMRawViewer.MyWpfExtensions;

namespace IXMRawViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> fileList = new List<string>();
        public static BackAndForthList<string> fileListObj = new BackAndForthList<string>();

        public MainWindow()
        {
            InitializeComponent();
            FileNameTextBox.Focusable = false;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog(this.GetIWin32Window());
            FileNameTextBox.Text = dlg.SelectedPath;
            Load(FileNameTextBox.Text);
            //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        }


        public void Load(string Path)
        {
            var data = GetFileList(Path);
            if (data.Count() > 0)
            {
                lblCount.Content = $"{data.Count()} RAW Images Found";

                fileList.AddRange(data);
                fileListObj.AddRange(fileList);
                //foreach (var item in data)
                //{
                //    TextBlock1.Text = TextBlock1.Text + Environment.NewLine + item;
                //}
                if (fileListObj.Count > 0)
                    loadImageToViewer(fileListObj.Current);
            }
            else
            {
                fileList = new List<string>();
                fileListObj = new BackAndForthList<string>();
                lblCount.Content = $"No RAW Images Found";
                imageViewer.Source = null;
                FileNameLabel.Content = "";

                //TextBlock1.Text = "";
            }
        }
        public void Delete(string Path)
        {
            if (File.Exists(Path))
                File.Delete(Path);
            MoveNext();
        }
        public void MoveNext()
        {
            fileListObj.MoveNext();
            loadImageToViewer(fileListObj.Current);
        }
        public void MovePrevious()
        {
            fileListObj.MovePrevious();
            loadImageToViewer(fileListObj.Current);
        }
        private IEnumerable<string> GetFileList(string Path)
        {
            try
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    return Directory.EnumerateFiles(Path, "*.raw", SearchOption.AllDirectories);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private void loadImageToViewer(string selectedFileName)
        {
            if (File.Exists(selectedFileName))
            {

                var ImageData = File.ReadAllBytes(selectedFileName);
                FileNameLabel.Content = selectedFileName;
                FileInfo f = new FileInfo(selectedFileName);
                FileSize.Content = $"{f.Length.ToSize(MyExtension.SizeUnits.MB)} MB";




                var bmp = ToGrayscale(ImageData, 1536, 2048);
                var converter = new ImageConverter();
                var newImage = new Bitmap(1536, 2048);
                using (var gr = Graphics.FromImage(newImage))
                {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, 1536, 2048));
                }

                imageViewer.Source = BitmapToBitmapSource(newImage);
                //imageViewer.Stretch = Stretch.UniformToFill;
                //imageViewer.StretchDirection =StretchDirection.Both;
            }
        }


        public static Bitmap ToGrayscale(byte[] data, int width, int height)
        {
            var bm = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var dimension = new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height);
            var picData = bm.LockBits(dimension, ImageLockMode.ReadWrite, bm.PixelFormat);
            var pixelStateAddress = picData.Scan0;

            var stride = 4 * (int)Math.Ceiling((3 * width) / 4.0);
            var pixels = new byte[stride * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var grey = data[(y * width) + x];
                    pixels[(y * stride) + (3 * x)] = grey;
                    pixels[(y * stride) + (3 * x) + 1] = grey;
                    pixels[(y * stride) + (3 * x) + 2] = grey;
                }
            }

            Marshal.Copy(pixels, 0, pixelStateAddress, pixels.Length);
            bm.UnlockBits(picData);
            return bm;
        }

        public BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }

            return bitSrc;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Load(FileNameTextBox.Text);

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            MoveNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            MovePrevious();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {

                case Key.Enter:
                    break;

                case Key.Delete:
                    Delete(fileListObj.Current);
                    break;


                case Key.Left:
                    MovePrevious();
                    break;

                case Key.Right:
                    MoveNext();
                    break;

                default:
                    break;
            }

        }
    }
}
