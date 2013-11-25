using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using Bidding;
using UtilityLibrary;
using InternetLibrary;

namespace SJ_Bidding_System
{
    public partial class ControlForm : Form
    {
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        #endregion

        #region Member Variables
        private DisplayForm m_displayForm;
        private SetAuctionForm m_setAllAuctionForm;
        private List<Auction> m_auctions;
        private int m_auctionIdNow = 0;
        private List<PriceLevel> m_priceLevels;
        private string m_inputNumbers = "";
        //private Thread m_loadPhotoThread;
        private Process m_server;
        private Internet<AuctionEntity> m_aeInternet;
        private Internet<BiddingResultEntity> m_breInternet;
        #endregion

        #region Properties
        #endregion

        #region Constructors and Finalizers
        public ControlForm()
        {
            InitializeComponent();

            CultureInfo ci = new CultureInfo("zh-Tw");
            ci.NumberFormat.CurrencyDecimalDigits = 0;
            ci.NumberFormat.CurrencySymbol = "";
            Thread.CurrentThread.CurrentCulture = ci;
            m_aeInternet = new Internet<AuctionEntity>("127.0.0.1", "test", "entities");
            m_breInternet = new Internet<BiddingResultEntity>("127.0.0.1", "test", "bidding_result");
        }
        #endregion

        #region Windows Form Events
        /// <summary>
        /// Event when Form first load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlForm_Load(object sender, EventArgs e)
        {
            string settingsFP = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.settingsFN);
            Settings.Load(settingsFP);

            ProcessStartInfo pStartInfo = new ProcessStartInfo();
            pStartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, Path.GetDirectoryName(Settings.biddingResultFP));
            pStartInfo.FileName = "cmd.exe";
            pStartInfo.Arguments = @"/k" + Settings.serverFN;
            string serverExePath = Path.Combine(pStartInfo.WorkingDirectory, Settings.serverFN);
            if (Utility.IsFileExist(serverExePath))
                m_server = System.Diagnostics.Process.Start(pStartInfo);

            InitDisplayForm();

            LoadAuctions();
            LoadPrices(Settings.pricesFP);

            LoadPriceLevel(Settings.priceLevelFP);

            ExchangeRate.Load(Settings.exchangeRateFP);
            rmbTextBox.Text = ExchangeRate.ntdToRmbRate.ToString();
            usdTextBox.Text = ExchangeRate.ntdToUsdRate.ToString();
            hkTextBox.Text = ExchangeRate.ntdToHkRate.ToString();

            if (m_auctions.Count != 0)
            {
                SetAuctionOnForm(m_auctions[m_auctionIdNow]);
                m_displayForm.SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            }
            InitializeComboBox();
            nowPriceTextBox.Focus();

            if (m_auctions.Count != 0)
                DoLoadAuctionPhoto(0);

            LoadBiddingResult(Settings.biddingResultFP);
            LoadBackupPath(Path.Combine(Path.GetDirectoryName(Settings.biddingResultFP), "backup_path.ini"));
        }

        /// <summary>
        /// Previous auction button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void prevsBtn_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;

            m_auctionIdNow += (m_auctions.Count - 1);
            m_auctionIdNow %= m_auctions.Count;
            if (m_auctions[m_auctionIdNow].photo == null)
                DoLoadAuctionPhoto(m_auctionIdNow);
            SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            m_displayForm.SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            auctionComboBox.SelectedIndex = m_auctionIdNow;
        }

        /// <summary>
        /// Next auction button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextBtn_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;

            m_auctionIdNow++;
            m_auctionIdNow %= m_auctions.Count;
            if (m_auctions[m_auctionIdNow].photo == null)
                DoLoadAuctionPhoto(m_auctionIdNow);
            SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            m_displayForm.SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            auctionComboBox.SelectedIndex = m_auctionIdNow;
        }

        /// <summary>
        /// Increase price depend on price level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void increaseByLevelBtn_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;

            for (int i = 0; i < m_priceLevels.Count; i++)
            {
                PriceLevel pl = m_priceLevels[i];
                int nowPrice = m_auctions[m_auctionIdNow].nowPrice;
                if (nowPrice < pl.up && nowPrice >= pl.down)
                {
                    int newPrice = 0;
                    if (pl.increments.Count > 1)
                    {
                        int down = pl.down;
                        int interval = (int)Math.Pow(10, (pl.down.ToString().Length - 1));
                        if (interval < Settings.unit)
                            interval = Settings.unit;
                        while (true)
                        {
                            for (int j = 0; j < pl.increments.Count; j++)
                            {
                                newPrice = down + pl.increments[j];
                                if (newPrice > nowPrice)
                                {
                                    SetNewNowPrice(newPrice);
                                    return;
                                }
                            }
                            down += interval;
                            if (down == pl.up)
                            {
                                SetNewNowPrice(down);
                                return;
                            }
                        }
                    }
                    else
                    {
                        int count = (pl.up - pl.down) / pl.increments[0] + 1;
                        for (int j = 0; j < count; j++)
                        {
                            newPrice = pl.down + j * pl.increments[0];
                            if (newPrice > nowPrice)
                            {
                                SetNewNowPrice(newPrice);
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reset price to initialization price.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetBtn_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;

            SetNewNowPrice(m_auctions[m_auctionIdNow].initialPrice);
        }

        /// <summary>
        /// Save price now when form closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePrices(Path.Combine(Application.StartupPath, Settings.saveFolder, Settings.pricesFN));
            ExchangeRate.Save(Path.Combine(Application.StartupPath, Settings.configFolder, Settings.exchangeRateFN));
            //DoSaveBiddingResult(Settings.biddingResultFP);
            if (!m_server.HasExited)
                m_server.CloseMainWindow();
        }

        /// <summary>
        /// Enter price now and some hotkeys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F12)
                Application.Exit();

            if (e.KeyData == Keys.Escape)
            {
                m_inputNumbers = "";
                nowPriceTextBox.Text = m_auctions[m_auctionIdNow].nowPrice.ToString("c");
            }

            if (rmbTextBox.Focused || usdTextBox.Focused || hkTextBox.Focused || increaseByLevelBtn.Focused || resetBtn.Focused ||
                auctionComboBox.Focused || winBidderTextBox.Focused)
                return;

            if (e.KeyData >= Keys.NumPad0 && e.KeyData <= Keys.NumPad9)
            {
                m_inputNumbers += (e.KeyValue - (int)Keys.NumPad0);
                long np = 0;
                if (long.TryParse(m_inputNumbers + "000", out np))
                    nowPriceTextBox.Text = np.ToString("c");
            }

            if (e.KeyData == Keys.Enter)
            {
                int price = 0;
                if (int.TryParse(m_inputNumbers + "000", out price))
                    SetNewNowPrice(price);

                m_inputNumbers = "";
                nowPriceTextBox.BackColor = Color.DarkBlue;
                if (!timer1.Enabled)
                    timer1.Start();
            }
        }

        /// <summary>
        /// Jump to any aution by select auctio number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void auctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_auctionIdNow == auctionComboBox.SelectedIndex)
                return;

            m_auctionIdNow = auctionComboBox.SelectedIndex;
            if (m_auctions[m_auctionIdNow].photo == null)
            {
                DoLoadAuctionPhoto(m_auctionIdNow);
            }
            SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            m_displayForm.SetAuctionOnForm(m_auctions[m_auctionIdNow]);
        }

        /// <summary>
        /// Event handler when user input rmb exchange rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rmbTextBox_TextChanged(object sender, EventArgs e)
        {
            if (rmbTextBox.Text != "" && m_auctions.Count != 0)
            {
                if (float.TryParse(rmbTextBox.Text, out ExchangeRate.ntdToRmbRate))
                    m_displayForm.SetNewPrice(m_auctions[m_auctionIdNow].nowPrice);
            }
        }

        /// <summary>
        /// Event handler when user input usd exchange rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (usdTextBox.Text != "" && m_auctions.Count != 0)
            {
                if (float.TryParse(usdTextBox.Text, out ExchangeRate.ntdToUsdRate))
                    m_displayForm.SetNewPrice(m_auctions[m_auctionIdNow].nowPrice);
            }
        }

        /// <summary>
        /// Avoid original Textbox input handle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nowPriceTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Restrict rmbTextBox to accept only digit and control and punctuation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rmbTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || Char.IsPunctuation(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Restrict usdTextBox to accept only digit and control and punctuation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || Char.IsPunctuation(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Reset all auction to original price.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetAllBtn_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;

            for (int i = 0; i < m_auctions.Count; i++)
                m_auctions[i].nowPrice = m_auctions[i].initialPrice;
            SetNewNowPrice(m_auctions[m_auctionIdNow].initialPrice);
        }

        /// <summary>
        /// When user input price now and press enter, it will coutdown 1 seconds and change nowPriceTextBox.BackColor back to black color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            nowPriceTextBox.BackColor = Color.Black;
            timer1.Stop();
        }

        private void hkTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || Char.IsPunctuation(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void hkTextBox_TextChanged(object sender, EventArgs e)
        {
            if (hkTextBox.Text != "" && m_auctions.Count != 0)
            {
                if (float.TryParse(hkTextBox.Text, out ExchangeRate.ntdToHkRate))
                    m_displayForm.SetNewPrice(m_auctions[m_auctionIdNow].nowPrice);
            }
        }

        private void setAllAuctionButton_Click(object sender, EventArgs e)
        {
            m_setAllAuctionForm = new SetAuctionForm(m_auctions.ToDictionary<Auction, string>(ae => ae.lot), ref m_aeInternet);
            m_setAllAuctionForm.ShowDialog();
        }

        private void confirmBidderButton_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;
            if (winBidderTextBox.BackColor == Color.DarkBlue)
                return;

            int bidderNo = 0;
            if (int.TryParse(winBidderTextBox.Text, out bidderNo))
            {
                m_auctions[m_auctionIdNow].winBidderNo = bidderNo;
                winBidderTextBox.BackColor = Color.DarkBlue;
                DoSaveBiddingResult(Settings.biddingResultFP);
            }
            else
            {
                winBidderTextBox.Text = "";
                MessageBox.Show("請輸入正確的競投牌號!");
            }
        }

        private void clearBidderButton_Click(object sender, EventArgs e)
        {
            if (m_auctions.Count == 0)
                return;
            if (winBidderTextBox.BackColor == Color.Black)
            {
                MessageBox.Show("請確認後再清除!");
                return;
            }

            winBidderTextBox.Text = "";
            m_auctions[m_auctionIdNow].winBidderNo = 0;
            winBidderTextBox.BackColor = Color.Black;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize display form in second monitor.
        /// </summary>
        private void InitDisplayForm()
        {
            Screen otherScreen = Screen.FromControl(this);
            m_displayForm = new DisplayForm();
            //m_displayForm.Size = Settings.displaySize;
            if (Screen.AllScreens.Length > 1)
            {
                otherScreen = Screen.AllScreens[1];
                //if (Settings.displaySize.Width == 0 || Settings.displaySize.Height == 0)
                //    m_displayForm.Size = new Size(otherScreen.WorkingArea.Width, otherScreen.WorkingArea.Height);
            }
            m_displayForm.Left = otherScreen.WorkingArea.Left /*+ Settings.displayPos.X*/;
            m_displayForm.Top = otherScreen.WorkingArea.Top /*+ Settings.displayPos.Y*/;
            m_displayForm.WindowState = FormWindowState.Maximized;
            m_displayForm.Show();
        }

        /// <summary>
        /// Initialize auction ComboBox.
        /// </summary>
        private void InitializeComboBox()
        {
            for (int i = 0; i < m_auctions.Count; i++)
                auctionComboBox.Items.Add(m_auctions[i].lot);
            if (m_auctions.Count != 0)
                auctionComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Load all auctions in "Auctions" folder.
        /// </summary>
        private void LoadAuctions()
        {
            Dictionary<string, AuctionEntity> aeDic = m_aeInternet.GetCollectionList().ToDictionary<AuctionEntity, string>(ae => ae.AuctionId);
            m_auctions = new List<Auction>();
            List<string> illegalFiles = new List<string>();
            string[] filePaths = Directory.GetFiles(Settings.auctionFolder).OrderBy(f => f).ToArray<string>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                Auction auction = new Auction();
                string fp = filePaths[i];
                if (auction.GetInfoFromDictionary(ref aeDic, fp))
                {
                    auction.photofilePath = fp;
                    if (m_auctions.Count < 10)
                        auction.photo = Utility.OpenBitmap(fp);
                    else
                        auction.photo = null;

                    auction.winBidderNo = 0;
                    m_auctions.Add(auction);
                }
                else
                {
                    illegalFiles.Add(fp);
                }
            }

            if (illegalFiles.Count != 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string s in illegalFiles)
                    sb.AppendLine(s);
                MessageBox.Show(this, "不合法的拍品圖檔名，請檢查Auctions:\n" + sb.ToString());
            }
        }

        private void DoLoadAuctionPhoto(int id)
        {
            if (m_auctions[id].photo == null)
                m_auctions[id].photo = Utility.OpenBitmap(m_auctions[id].photofilePath);
            Thread t = new Thread(new ParameterizedThreadStart(this.LoadAuctionPhoto));
            t.IsBackground = true;
            t.Start(id);
        }

        private void LoadAuctionPhoto(object obj)
        {
            int auctionIdNow = (int)obj;
            int min = (auctionIdNow - 10 + m_auctions.Count) % m_auctions.Count;
            int max = (auctionIdNow + 10) % m_auctions.Count;
            for (int i = 0; i < m_auctions.Count; i++)
            {
                Auction auction = m_auctions[i];
                if (min < max && (i >= min && i <= max))
                {
                    if (auction.photo == null)
                        auction.photo = Utility.OpenBitmap(auction.photofilePath);
                }
                else if (min > max && (i >= min || i <= max))
                {
                    if (auction.photo == null)
                        auction.photo = Utility.OpenBitmap(auction.photofilePath);
                }
                else
                {
                    if (i != auctionIdNow && auction.photo != null)
                    {
                        auction.photo.Dispose();
                        auction.photo = null;
                    }
                }
            }
        }

        /// <summary>
        /// Load all prices of auctions in "Auctions" folder.
        /// </summary>
        /// <param name="fn">price file name</param>
        private void LoadPrices(string fn)
        {
            if (!Utility.IsFileExist(fn))
                return;

            using (StreamReader sr = new StreamReader(fn))
            {
                int count = 0;
                string line = sr.ReadLine();
                if (line == null || !int.TryParse(line.Remove(0, "Lot Price---".Length), out count))
                {
                    MessageBox.Show(fn + "檔格式錯誤!");
                    return;
                }

                for (int i = 0; i < count; i++)
                {
                    if (i >= m_auctions.Count)
                        break;

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        MessageBox.Show("Lot " + m_auctions[i].lot + " 價格載入失敗!請檢查" + fn + "檔!");
                        return;
                    }
                    string[] data = line.Split(' ');
                    if (m_auctions[i].lot == data[0])
                    {
                        int price = 0;
                        if (int.TryParse(data[1], out price))
                        {
                            m_auctions[i].nowPrice = price * Settings.unit;
                        }
                        else
                        {
                            MessageBox.Show("Lot " + m_auctions[i].lot + " 價格載入失敗!請檢查" + fn + "檔!");
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save price at fn.
        /// </summary>
        /// <param name="fn">file name</param>
        private void SavePrices(string fn)
        {
            using (StreamWriter sw = new StreamWriter(fn, false))
            {
                sw.WriteLine("Lot Price---{0}", m_auctions.Count);
                for (int i = 0; i < m_auctions.Count; i++)
                {
                    sw.WriteLine("{0} {1}", m_auctions[i].lot, m_auctions[i].nowPrice / Settings.unit);
                }
            }
        }

        /// <summary>
        /// Load price level.
        /// </summary>
        /// <param name="fn">price level file name</param>
        private void LoadPriceLevel(string fn)
        {
            if (!Utility.IsFileExist(fn))
                return;

            m_priceLevels = new List<PriceLevel>();
            using (StreamReader sr = new StreamReader(fn))
            {
                int count = int.Parse(sr.ReadLine().Remove(0, "Down Up Increment---".Length));
                for (int i = 0; i < count; i++)
                {
                    string[] data = sr.ReadLine().Split(' ');
                    PriceLevel pl = new PriceLevel();
                    pl.down = int.Parse(data[0]) * Settings.unit;
                    pl.up = int.Parse(data[1]) * Settings.unit;
                    string[] s = data[2].Split(',');
                    pl.increments = new List<int>();
                    for (int j = 0; j < s.Length; j++)
                    {
                        float level = 0.0f;
                        if (float.TryParse(s[j], out level))
                        {
                            pl.increments.Add(Convert.ToInt32(level * Settings.unit));
                        }
                        else
                        {
                            MessageBox.Show(fn + "格式錯誤");
                            return;
                        }
                    }
                    m_priceLevels.Add(pl);
                }
            }
        }

        /// <summary>
        /// Set auction on this Form by Auction object.
        /// </summary>
        /// <param name="auction">Auction object</param>
        private void SetAuctionOnForm(Auction auction)
        {
            LotNumberLabel.Text = auction.lot;
            ArtistNameLabel.Text = auction.artist;
            ArtworkNameLabel.Text = auction.artwork;
            initPriceLabel.Text = auction.initialPrice.ToString("c");
            nowPriceTextBox.Text = auction.nowPrice.ToString("c");
            if (auction.photo != null)
                Utility.SetImageNoStretch(ref auctionPictureBox, ref auction.photo);
            else
                auctionPictureBox.Image = Properties.Resources.loading;

            if (auction.winBidderNo != 0)
            {
                winBidderTextBox.BackColor = Color.DarkBlue;
                winBidderTextBox.Text = auction.winBidderNo.ToString();
            }
            else
            {
                winBidderTextBox.BackColor = Color.Black;
                winBidderTextBox.Text = "";
            }
        }

        /// <summary>
        /// Set new price and refresh display form.
        /// </summary>
        /// <param name="newPrice">new price now</param>
        private void SetNewNowPrice(int newPrice)
        {
            if (m_auctions.Count == 0)
                return;

            nowPriceTextBox.Text = newPrice.ToString("c");
            m_displayForm.SetNewPrice(newPrice);
            m_auctions[m_auctionIdNow].nowPrice = newPrice;
        }

        private void LoadBiddingResult(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                return;
            }

            Dictionary<string, int> results = new Dictionary<string, int>();
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    return;
                }

                while (line != null)
                {
                    string[] items = line.Split('\t');
                    if (items.Length != 4)
                    {
                        MessageBox.Show(path + "檔格式錯誤!");
                        return;
                    }
                    int bidderNo = 0;
                    if (!int.TryParse(items[2], out bidderNo))
                    {
                        MessageBox.Show(path + "檔格式錯誤!");
                        return;
                    }
                    results.Add(items[0], bidderNo);
                    line = sr.ReadLine();
                }
            }

            for (int i = 0; i < m_auctions.Count; i++)
            {
                if (results.ContainsKey(m_auctions[i].lot))
                {
                    m_auctions[i].winBidderNo = results[m_auctions[i].lot];
                }
            }
        }

        /// <summary>
        /// lot---AuctionName---BidderNo---HammerPrice
        /// </summary>
        /// <returns></returns>
        private void SaveBiddingResult(object obj)
        {
            string fp = (string)obj;
            if (!File.Exists(fp))
            {
                MessageBox.Show(fp + "不存在!");
                return;
            }
            using (StreamWriter sw = new StreamWriter(fp, false))
            {
                foreach (Auction auction in m_auctions)
                {
                    if (auction.winBidderNo != 0)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", auction.lot, auction.artwork, auction.winBidderNo, auction.nowPrice);
                        BiddingResultEntity bre = new BiddingResultEntity(auction.winBidderNo.ToString(), auction.lot, auction.nowPrice);
                        m_breInternet.Insert(bre);
                    }
                }
            }

            File.Copy(fp, Path.Combine(Settings.backupFP, Settings.biddingResultFN), true);
        }

        private void DoSaveBiddingResult(string fp)
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(SaveBiddingResult);
            Thread thread = new Thread(pts);
            thread.Start(fp);
        }

        private void LoadBackupPath(string path)
        {
            if (!Utility.IsFileExist(path))
                return;

            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    MessageBox.Show(path + "檔格式錯誤!");
                    return;
                }
                Settings.backupFP = line;
            }
        }
        #endregion
    }
}