using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using MetadataExtractor;

namespace Lab2
{
    public class FileData
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string HorizontalResolution { get; set; }
        public string VerticalResolution { get; set; }
        public string ColorDepth { get; set; }
        public string Compression { get; set; }
    }

    public partial class MainWindow : Window
    {

        private readonly HashSet<string> ImageExtensions = new HashSet<string>
        {
            ".jpg", ".jpeg", ".tiff", ".tif", ".gif", ".bmp", ".png" ,".pcx"
        };

        private readonly Dictionary<long, string> CompressionTypes = new Dictionary<long, string>()
        {
            {1, "No compression"},
            {2, "CCITT modified Huffman RLE"},
            {3, "CCITT Group 3 fax encoding"},
            {4, "CCITT Group 4 fax encoding"},
            {5, "LZW"},
            {6, "JPEG Old"},
            {7, "JPEG New"},
            {8, "DeflateAdobe"},
            {9, "JBIG_85"},
            {10, "JBIG_43"},
            {11, "JPEG"},
            {12, "JPEG"},
            {32766, "RLE_NeXT"},
            {32771, "CCITTRLEW"},
            {32773, "PackBits compression, aka Macintosh RLE"},
            {32809, "RLE_ThunderScan"},
            {32895, "RasterPadding"},
            {32896, "RLE_LW"},
            {32897, "RLE_HC"},
            {32898 ,"IT8BL"},
            {32908,"PIXARFILM"},
            {32909,"PIXARLOG"},
            {32946, "DeflatePKZIP"},
            {32947, "RLE_BL"},
            {34661, "JBIG"},
            {34676, "SGILOG"},
            {34677, "SGILOG24"},
            {34712, "JPEG2000"},
            {34713, "Nikon_NEF"}
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenFileClick(object sender, RoutedEventArgs e)
        {
            lvFiles.Items.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.tiff;*.tif;*.bmp;*.pcx)|*.png;*.jpg;*.jpeg;*.gif;*.tiff;*.tif;*.bmp;*.pcx";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (openFileDialog.ShowDialog() == true)
            {
                SetFilesInfo(openFileDialog.FileNames);
            }
        }

        private void BtnOpenDirectoryClick(object sender, RoutedEventArgs e)
        {
            lvFiles.Items.Clear();
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderDialog.SelectedPath;
                DirectoryInfo dir = new DirectoryInfo(path);
                List<FileInfo> files = dir.GetFiles("*.*")
                                        .Where(file => ImageExtensions.Contains(file.Extension.ToLower()))
                                        .ToList();

                string[] fileNames = new string[files.Count()];
                for (int i = 0; i < files.Count(); i++)
                {
                    fileNames[i] = files[i].FullName;
                }
                SetFilesInfo(fileNames);
            }
        }

        private async void SetFilesInfo(string[] fileNames)
        {
            await Task.Run(() =>
            {
                Parallel.For(0, fileNames.Length, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (i) =>
                {
                    string fileName = fileNames[i];
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.Extension == ".pcx")
                    {
                        SetPCXFileInfo(fileName);
                    }
                    else
                    {
                        Bitmap bitmap = new Bitmap(fileName);
                        Image image = Image.FromFile(fileName);

                        string name = Path.GetFileName(fileName);
                        string size = bitmap.Width + " x " + bitmap.Height;
                        string horizontalResolution = bitmap.HorizontalResolution.ToString();
                        string verticalResolution = bitmap.VerticalResolution.ToString();
                        string colorDepth = GetColorDepth(bitmap.PixelFormat).ToString();
                        string compression = GetCompressionType(image);

                        Dispatcher.Invoke(() => lvFiles.Items.Add(new FileData
                        {
                            Name = name,
                            Size = size,
                            HorizontalResolution = horizontalResolution,
                            VerticalResolution = verticalResolution,
                            ColorDepth = colorDepth,
                            Compression = compression
                        }));
                        image.Dispose();
                        bitmap.Dispose();
                    }
                });
            });
        }

        private void SetPCXFileInfo(string fileName)
        {
            var directories = ImageMetadataReader.ReadMetadata(fileName);
            var pcxFileData = new Dictionary<string, string>();

            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    pcxFileData[$"{directory.Name} - {tag.Name}"] = tag.Description;
                }
            }

            string size = CountPcxFileSize(
                int.Parse(pcxFileData["PCX - X Min"]),
                int.Parse(pcxFileData["PCX - X Max"]),
                int.Parse(pcxFileData["PCX - Y Min"]),
                int.Parse(pcxFileData["PCX - Y Max"]));

            Dispatcher.Invoke(() => lvFiles.Items.Add(new FileData
            {
                Name = Path.GetFileName(fileName),
                Size = size,
                HorizontalResolution = pcxFileData["PCX - Horizontal DPI"],
                VerticalResolution = pcxFileData["PCX - Vertical DPI"],
                ColorDepth = pcxFileData["PCX - Color Planes"],
                Compression = IsPcxFileCompressed(fileName) ? "RLE" : "No compression"
            }));
        }

        private bool IsPcxFileCompressed(string fileName)
        {
            byte[] bytes = new byte[3];
            using (var reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                reader.Read(bytes, 0, 3);
            }
            return bytes[2] == 1;
        }

        private string CountPcxFileSize(int xMin, int xMax, int yMin, int yMax)
        {
            return $"{xMax - xMin + 1} x {yMax - yMin + 1}";
        }

        private int GetColorDepth(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return 1;
                case PixelFormat.Format4bppIndexed:
                    return 4;
                case PixelFormat.Format8bppIndexed:
                    return 8;
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format16bppArgb1555:
                    return 16;
                case PixelFormat.Format24bppRgb:
                    return 24;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    return 32;
                case PixelFormat.Format48bppRgb:
                    return 48;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    return 64;
                default:
                    return -1;
            }
        }

        private string GetCompressionType(Image image)
        {
            int compressionId;
            try
            {
                PropertyItem propItem = image.GetPropertyItem(259);
                compressionId = BitConverter.ToInt16(propItem.Value, 0);
            }
            catch
            {
                return "Не удается получить информацию о сжатии";
            }

            if (!CompressionTypes.TryGetValue(compressionId, out var compressionType))
            {
                return "Неопределенный тип сжатия";
            }
            return compressionType;
        }
    }
}
