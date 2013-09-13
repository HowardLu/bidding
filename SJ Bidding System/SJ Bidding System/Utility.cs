using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SJ_Bidding_System
{
    /// <summary>
    /// Auction class to store information of the auction.
    /// </summary>
    public class Auction
    {
        public string lot;
        public string artist;
        public string artwork;
        public int initialPrice;
        public int nowPrice;
        public Bitmap photo;
        public string photofilePath;
        public int winBidderNo;

        /// <summary>
        /// Get auction's info from its file name.
        /// </summary>
        /// <param name="fileName">auction's file name</param>
        /// <param name="auction">auction object</param>
        public bool GetInfoFromFileName(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string[] infos = fileName.Split('_');
            if (infos.Length != 4)
                return false;
            this.lot = infos[0];
            this.artist = infos[1];
            this.artwork = infos[2];
            int price = 0;
            if (int.TryParse(infos[3], out price))
                this.initialPrice = this.nowPrice = price * Settings.unit;
            else
                return false;
            return true;
        }

        public string CreateFileName(string extension)
        {
            if (extension != "")
                return lot + "_" + artist + "_" + artwork + "_" + (initialPrice / Settings.unit).ToString() + extension;
            else
                return lot + "_" + artist + "_" + artwork + "_" + (initialPrice / Settings.unit).ToString();
        }
    }

    /// <summary>
    /// Price level structure.
    /// </summary>
    public struct PriceLevel
    {
        public int down;
        public int up;
        public System.Collections.Generic.List<int> increments;
    }

    /// <summary>
    /// Exchange rate static class, store rate now and provide convertion.
    /// </summary>
    public static class ExchangeRate
    {
        public static float ntdToRmbRate = 0.1f;
        public static float ntdToUsdRate = 0.1f;
        public static float ntdToHkRate = 0.1f;

        /// <summary>
        /// Load exchange rate.
        /// </summary>
        /// <param name="fn">exchange rate file name</param>
        public static void Load(string fn)
        {
            if (!Utility.IsFileExist(fn))
                return;

            using (StreamReader sr = new StreamReader(fn))
            {
                string line = sr.ReadLine();
                if (line != null && line.Substring(0, 3) == "rmb")
                {
                    float.TryParse(line.Remove(0, 4), out ntdToRmbRate);
                }
                line = sr.ReadLine();
                if (line != null && line.Substring(0, 3) == "usd")
                {
                    float.TryParse(line.Remove(0, 4), out ntdToUsdRate);
                }
                line = sr.ReadLine();
                if (line != null && line.Substring(0, 2) == "hk")
                {
                    float.TryParse(line.Remove(0, 3), out ntdToHkRate);
                }
            }
        }

        /// <summary>
        /// Save exchange rate at fn.
        /// </summary>
        /// <param name="fn">file name</param>
        public static void Save(string fn)
        {
            using (StreamWriter sw = new StreamWriter(fn))
            {
                sw.WriteLine("rmb {0}", ntdToRmbRate);
                sw.WriteLine("usd {0}", ntdToUsdRate);
                sw.WriteLine("hk {0}", ntdToHkRate);
            }
        }

        /// <summary>
        /// Convert NTD to RMB.
        /// </summary>
        /// <param name="ntd">NTD to convert</param>
        /// <returns>RMB</returns>
        public static int NtdToRmb(int ntd)
        {
            int rmb = (int)Math.Round((decimal)(ntd * ntdToRmbRate / 100), MidpointRounding.AwayFromZero) * 100;
            return rmb;
        }

        /// <summary>
        /// Convert NTD to USD.
        /// </summary>
        /// <param name="ntd">NTD to convert</param>
        /// <returns>USD</returns>
        public static int NtdToUsd(int ntd)
        {
            int usd = (int)Math.Round((decimal)(ntd * ntdToUsdRate / 10), MidpointRounding.AwayFromZero) * 10;
            return usd;
        }

        /// <summary>
        /// Convert NTD to HK.
        /// </summary>
        /// <param name="ntd">NTD to convert</param>
        /// <returns>HK</returns>
        public static int NtdToHk(int ntd)
        {
            int hk = (int)Math.Round((decimal)(ntd * ntdToHkRate / 10), MidpointRounding.AwayFromZero) * 10;
            return hk;
        }
    }

    /// <summary>
    /// Utility static class.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Resize new image.
        /// </summary>
        /// <param name="b">Bitmap to resize</param>
        /// <param name="newWidth">new width after resize</param>
        /// <param name="newHeight">new heigh after resize</param>
        /// <returns></returns>
        public static Bitmap SizeImage(ref Bitmap b, int newWidth, int newHeight)
        {
            Bitmap newBmp = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newBmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(b, 0, 0, newWidth, newHeight);
            g.Dispose();
            return newBmp;
        }

        public static void SetImageNoStretch(ref System.Windows.Forms.PictureBox pb, ref Bitmap bmp)
        {
            float scaleX = (float)pb.Width / bmp.Width;
            float scaleY = (float)pb.Height / bmp.Height;
            float scale = (scaleX >= scaleY) ? scaleY : scaleX;
            if (pb.Image != null)
                pb.Image.Dispose();
            pb.Image = Utility.SizeImage(ref bmp, (int)(bmp.Width * scale), (int)(bmp.Height * scale));
            pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        }

        public static bool IsFileExist(string filePath)
        {
            if (File.Exists(filePath))
                return true;
            else
                MessageBox.Show(Path.GetFileName(filePath) + " 檔案不存在!");

            return false;
        }

        public static bool IsValidFileName(string input)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            int index = input.IndexOfAny(invalidChars);
            if (index == -1)
            {
                return true;
            }
            else
            {
                MessageBox.Show("不可輸入無效字元: " + input[index]);
                return false;
            }
        }

        public static int IsNumber(string input)
        {
            int number = 0;
            if (int.TryParse(input, out number))
            {
                return number;
            }
            else
            {
                MessageBox.Show("請輸入有效數字");
                return -1;
            }
        }

        public static Bitmap OpenBitmap(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new Bitmap(stream);
            }
        }
    }

    public static class Settings
    {
        public static string auctionFolder = "Auctions";
        public static string configFolder = "Config";
        public static string saveFolder = "Save";
        public static string pricesFN = "Prices.txt";
        public static string pricesFP = "Prices.txt";
        public static string priceLevelFN = "PriceLevel.txt";
        public static string priceLevelFP = "PriceLevel.txt";
        public static string exchangeRateFN = "ExchangeRate.txt";
        public static string exchangeRateFP = "ExchangeRate.txt";
        public static string settingsFN = "settings.txt";
        public static string biddingResultFN = "bid_items.txt";
        public static string biddingResultFP = "bid_items.txt";
        public static string backupFP = @"C:\";
        public static string serverFN = "server.exe";
        public static string serverDir = @"BidderDataServer\exe";
        public static int unit = 1000;
        //public static Point displayPos = Point.Empty;
        //public static Size displaySize = Size.Empty;

        public static void Load(string fn)
        {
            pricesFP = Path.Combine(Application.StartupPath, Settings.saveFolder, Settings.pricesFN);
            priceLevelFP = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.priceLevelFN);
            exchangeRateFP = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.exchangeRateFN);

            if (!Utility.IsFileExist(fn))
                return;

            string[] settingName = { "DisplayPos", "DisplaySize", "Server", "Unit" };
            using (StreamReader sr = new StreamReader(fn))
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    MessageBox.Show(fn + "檔格式錯誤!");
                    return;
                }

                while (line != null)
                {
                    string[] sections = line.Split(' ');
                    string name = sections[0];
                    string data = sections[1];
                    if (name == settingName[0])
                    {
                        string[] num = data.Split(',');
                        int x = 0, y = 0;
                        if (!int.TryParse(num[0], out x) || !int.TryParse(num[1], out y))
                        {
                            MessageBox.Show(fn + "檔格式錯誤!");
                            return;
                        }
                        //displayPos = new Point(x, y);
                    }
                    if (name == settingName[1])
                    {
                        string[] num = data.Split(',');
                        int w = 0, h = 0;
                        if (!int.TryParse(num[0], out w) || !int.TryParse(num[1], out h))
                        {
                            MessageBox.Show(fn + "檔格式錯誤!");
                            return;
                        }
                        //displaySize = new Size(w, h);
                    }
                    if (name == settingName[2])
                    {
                        string path = Path.Combine(Application.StartupPath, data);
                        if (Directory.Exists(path))
                            serverDir = path;
                        biddingResultFP = Path.Combine(serverDir, biddingResultFN);
                    }
                    if (name == settingName[3])
                    {
                        int u = 0;
                        if (!int.TryParse(data, out u))
                        {
                            MessageBox.Show(fn + "檔格式錯誤!");
                            return;
                        }
                        unit = u;
                    }

                    line = sr.ReadLine();
                }
            }
        }
    }

    // Implements the manual sorting of items by columns.
    public class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                returnVal *= -1;    // Invert the value returned by String.Compare.
            return returnVal;
        }
    }

    public class ControlState
    {
        public Point location;
        public Size size;
        public float fontSize;
        public Control control;

        public ControlState(Point _location, Size _size, float _fontSize, ref Control _control)
        {
            location = _location;
            size = _size;
            fontSize = _fontSize;
            control = _control;
        }

        private void Relocate(float xRatio, float yRatio)
        {
            Point newLocation = new Point();
            newLocation.X = Convert.ToInt32(location.X * xRatio);
            newLocation.Y = Convert.ToInt32(location.Y * yRatio);
            control.Location = newLocation;
        }

        private void Resize(float xRatio, float yRatio)
        {
            float smallerRatio = xRatio < yRatio ? xRatio : yRatio;
            Size newSize = new Size();
            newSize.Width = Convert.ToInt32(size.Width * xRatio);
            newSize.Height = Convert.ToInt32(size.Height * yRatio);
            control.Size = newSize;
        }

        private void ResizeFont(float xRatio, float yRatio)
        {
            float smallerRatio = xRatio < yRatio ? xRatio : yRatio;
            float newSize = fontSize * smallerRatio;
            control.Font = new Font(control.Font.FontFamily.Name, newSize, control.Font.Style);
        }

        public void ReArrange(float xRatio, float yRatio)
        {
            Relocate(xRatio, yRatio);
            Resize(xRatio, yRatio);
            if (fontSize != 0)
                ResizeFont(xRatio, yRatio);
        }
    }
}