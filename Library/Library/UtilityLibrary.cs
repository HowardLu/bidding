using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UtilityLibrary
{
    /// <summary>
    /// Exchange rate static class, store rate now and provide convertion.
    /// </summary>
    public static class ExchangeRate
    {
        public static int count = 3;
        public static string mainRateName = "NTD";
        public static string[] rateNames = new string[count];
        public static float[] mainToExchangeRate = new float[count];

        /// <summary>
        /// Load exchange rate.
        /// </summary>
        /// <param name="fn">exchange rate file name</param>
        public static void Load(string fn)
        {
            if (!Utility.IsFileExist(fn, false))
                return;

            using (StreamReader sr = new StreamReader(fn))
            {
                string line = "";
                mainRateName = sr.ReadLine();
                for (int i = 0; i < count; i++)
                {
                    line = sr.ReadLine();
                    if (line != null)
                    {
                        string[] data = line.Split(' ');
                        rateNames[i] = data[1];
                        float.TryParse(data[2], out mainToExchangeRate[i]);   // "er* "
                    }
                }
            }
        }

        /// <summary>
        /// Save exchange rate at fn.
        /// </summary>
        /// <param name="fn">file name</param>
        public static void Save(string fn, string aMainRateName, ref string[] aRateNames, ref string[] aRates)
        {
            using (StreamWriter sw = new StreamWriter(fn))
            {
                sw.WriteLine(aMainRateName);
                for (int i = 0; i < count; i++)
                {
                    sw.WriteLine("er{0} {1} {2}", i, aRateNames[i], aRates[i]);
                }
            }
        }

        /// <summary>
        /// Convert NTD to RMB.
        /// </summary>
        /// <param name="mainNum">NTD to convert</param>
        /// <returns>RMB</returns>
        public static int MainToCurrency(int mainNum, int rateId)
        {
            int numByNewCurrency = (int)Math.Round((decimal)(mainNum * mainToExchangeRate[rateId] / 10), MidpointRounding.AwayFromZero) * 10;   // 四捨五入個位
            return numByNewCurrency;
        }

        public static float Revert(float rate)
        {
            return 1f / rate;
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

        public static bool CreateDirectory(string directoryName)
        {
            try
            {
                Directory.CreateDirectory(directoryName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsDirectoryExist(string directoryPath, bool isSilence)
        {
            if (!Directory.Exists(directoryPath))
            {
                if (!isSilence)
                    MessageBox.Show(directoryPath + " 資料夾不存在!");
                return false;
            }
            return true;
        }

        public static bool IsFileExist(string filePath, bool isSilence)
        {
            if (!File.Exists(filePath))
            {
                if (!isSilence)
                    MessageBox.Show(Path.GetFileName(filePath) + " 檔案不存在!");
                return false;
            }
            return true;
        }

        public static bool IsValidFileName(string input, bool isSilence)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            int index = input.IndexOfAny(invalidChars);
            if (index == -1)
            {
                return true;
            }
            else
            {
                if (!isSilence)
                    MessageBox.Show("不可輸入無效字元: " + input[index]);
                return false;
            }
        }

        public static int ParseToInt(string input, bool isSilence)
        {
            int number = 0;
            if (int.TryParse(input, out number))
            {
                return number;
            }
            else
            {
                if (!isSilence)
                    MessageBox.Show("無效數字: " + input + "\n請輸入有效數字!");

                return -1;
            }
        }

        public static int ParseCurrencyToInt(string input, bool isSilence)
        {
            int result = -1;
            try
            {
                result = int.Parse(input, System.Globalization.NumberStyles.Currency);
            }
            catch (Exception e)
            {
                if (!isSilence)
                    MessageBox.Show("無效數字: " + input + "\n請輸入有效數字!");
            }
            return result;
        }

        public static float ParseToFloat(string input, bool isSilence)
        {
            float number = 0;
            if (float.TryParse(input, out number))
            {
                return number;
            }
            else
            {
                if (!isSilence)
                    MessageBox.Show("無效數字: " + input + "\n請輸入有效數字!");

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

        public static string GetDirectory(string filePath)
        {
            int slashPos = filePath.LastIndexOf(@"/") == -1 ? filePath.LastIndexOf(@"\") : filePath.LastIndexOf(@"/");
            return filePath.Remove(slashPos + 1);
        }

        public static string GetFileName(string filePath, bool hasExtension)
        {
            int slashPos = filePath.LastIndexOf(@"/") == -1 ? filePath.LastIndexOf(@"\") : filePath.LastIndexOf(@"/");
            string fileName = filePath.Remove(0, slashPos + 1);
            if (hasExtension)
                return fileName;
            else
                return fileName.Remove(fileName.LastIndexOf('.'));
        }

        public static string GetFileExtension(string filePath)
        {
            return filePath.Remove(0, filePath.LastIndexOf(@".") + 1);
        }

        public static string GetEnumString(Type enumType, int index)
        {
            string[] names = Enum.GetNames(enumType);
            if (index < names.Length)
                return names[index];
            else
                return "";
        }

        public static int ToEnumInt<TEnum>(string stringToParse) where TEnum : struct, IConvertible
        {
            if (stringToParse == "")
                return -1;
            TEnum e = (TEnum)Enum.Parse(typeof(TEnum), stringToParse);
            return Convert.ToInt32(Enum.Format(typeof(TEnum), e, "d"));
        }

        public static TEnum ToEnum<TEnum>(string stringToParse) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            TEnum e = default(TEnum);
            Enum.TryParse<TEnum>(stringToParse, true, out e);
            return e;
        }

        public static string InputIp()
        {
            string lastInputIp = "127.0.0.1";
            string fileName = "ip_cache.txt";
            string filePath = Path.Combine(Application.StartupPath, Settings.configFolder, fileName);
            if (IsFileExist(filePath, true))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    lastInputIp = sr.ReadLine();
                }
            }

            string ip = InputBox("", "請輸入Server IP:", lastInputIp, -1, -1);
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                sw.WriteLine(ip);
            }
            return ip;
        }

        public static string InputBox(string prompt, string title = "", string defaultResponse = "", int xPos = -1, int yPos = -1)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultResponse, xPos, yPos);
        }
    }

    public static class Settings
    {
        public static string auctionFolder = "Auctions";
        public static string configFolder = "Config";
        public static string saveFolder = "Save";
        public static string docTempFolder = "TempDoc";
        public static string pricesFN = "Prices.txt";
        public static string pricesFP = "Prices.txt";
        public static string priceLevelFN = "PriceLevel.txt";
        public static string priceLevelFP = "PriceLevel.txt";
        public static string exchangeRateFN = "ExchangeRate.txt";
        public static string exchangeRateFP = "ExchangeRate.txt";
        public static string settingsFN = "settings.txt";
        public static string biddingResultFN = "bid_items.txt";
        public static string biddingResultFP = "bid_items.txt";
        public static string backupFP = @"Z:\";
        public static string serverFN = "server.exe";
        public static string serverDir = @"BidderDataServer\exe";
        public static int serverPort = 27017;
        public static int unit = 1000;
        public static string videoFolder = "Video";
        public static Point displayPos = Point.Empty;
        public static Size displaySize = Size.Empty;
        public static int serviceChargeRate = 0;        // for Checkout
        public static int creditCardRate = 0;           // for Checkout
        public static int dealDocPrintCount = 3;        // for Checkout
        public static int cashFlowDocPrintCount = 1;    // for Checkout

        public static void Load()
        {
            string settingsFP = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.settingsFN);
            pricesFP = Path.Combine(Application.StartupPath, Settings.saveFolder, Settings.pricesFN);
            priceLevelFP = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.priceLevelFN);
            exchangeRateFP = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.exchangeRateFN);
            biddingResultFP = Path.Combine(Application.StartupPath, Settings.saveFolder, biddingResultFN);

            if (!Utility.IsFileExist(settingsFP, false))
                return;

            using (StreamReader sr = new StreamReader(settingsFP))
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                    return;
                }

                while (line != null)
                {
                    string[] sections = line.Split(' ');
                    string settingName = sections[0];
                    string data = sections[1];
                    switch (settingName)
                    {
                        case "DisplayPos":
                            {
                                string[] num = data.Split(',');
                                int x = 0, y = 0;
                                if (!int.TryParse(num[0], out x) || !int.TryParse(num[1], out y))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                                displayPos = new Point(x, y);
                            }
                            break;
                        case "DisplaySize":
                            {
                                string[] num = data.Split(',');
                                int w = 0, h = 0;
                                if (!int.TryParse(num[0], out w) || !int.TryParse(num[1], out h))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                                displaySize = new Size(w, h);
                            }
                            break;
                        case "Server":
                            {
                                string path = Path.Combine(Application.StartupPath, data);
                                if (Directory.Exists(path))
                                    serverDir = path;
                            }
                            break;
                        case "Unit":
                            {
                                int u = 0;
                                if (!int.TryParse(data, out u))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                                unit = u;
                            }
                            break;
                        case "port":
                            if (!int.TryParse(data, out serverPort))
                            {
                                MessageBox.Show(settingsFP + "檔格式錯誤!");
                                return;
                            }
                            break;
                        case "service_charge":
                            {
                                if (!int.TryParse(data, out serviceChargeRate))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                            }
                            break;
                        case "card_fee":
                            {
                                if (!int.TryParse(data, out creditCardRate))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                            }
                            break;
                        case "dealDocPrintCount":
                            {
                                if (!int.TryParse(data, out dealDocPrintCount))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                            }
                            break;
                        case "cashFlowDocPrintCount":
                            {
                                if (!int.TryParse(data, out cashFlowDocPrintCount))
                                {
                                    MessageBox.Show(settingsFP + "檔格式錯誤!");
                                    return;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    line = sr.ReadLine();
                }
            }
        }

        public static void Save()
        {
            string fp = Path.Combine(System.Windows.Forms.Application.StartupPath, Settings.configFolder, settingsFN);
            using (StreamWriter sw = new StreamWriter(fp))
            {
                sw.WriteLine("port " + serverPort);
                sw.WriteLine("service_charge " + serviceChargeRate);
                sw.WriteLine("card_fee " + creditCardRate);
                sw.WriteLine("dealDocPrintCount " + dealDocPrintCount);
                sw.WriteLine("cashFlowDocPrintCount " + cashFlowDocPrintCount);
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