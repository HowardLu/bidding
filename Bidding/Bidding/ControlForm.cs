//#define MCT
//#define SJ
//#define DS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BiddingLibrary;
using InternetLibrary;
using UtilityLibrary;

namespace Bidding
{
    public partial class ControlForm : Form
    {
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        private enum DisplayMode
        {
            純文字 = 0,
            影片,
            圖片
        }
        #endregion

        #region Member Variables
        private DisplayFormBase m_displayForm;
        private string m_sessionIdNow = "";
        private List<string> m_sessions;
        private List<Auction> m_auctions;
        private int m_auctionIdNow = 0;
        //private List<PriceLevel> m_priceLevels;
        private string m_inputNumbers = "";
        //private Thread m_loadPhotoThread;
        //private Process m_server;
        private Internet<AuctionEntity> m_aeInternet;
        private Internet<BidderEntity> m_beInternet;
        private PlayerForm m_playerForm;
        //private Form2 m_form2;
        private DisplayMode m_curDisplayMode;
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
            string ip = Utility.InputIp();
            if (ip != "")
            {
                m_aeInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
                m_beInternet = new Internet<BidderEntity>(ip, "bidding_data", "buyer_table");
            }
            else
            {
                Environment.Exit(Environment.ExitCode);
            }
            m_curDisplayMode = DisplayMode.純文字;
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
            if (Auctioneer.M == Auction.DefaultAuctioneer)
            {
                this.logoPictureBox.Visible = false;
                playButton.Visible = stopButton.Visible = false;
                displayModeLabel.Visible = displayModeComboBox.Visible = false;
            }
            if (Auctioneer.N == Auction.DefaultAuctioneer)
            {
                this.logoPictureBox.Image = Properties.Resources.LOGO_N;
                playButton.Visible = stopButton.Visible = false;
                displayModeLabel.Visible = displayModeComboBox.Visible = false;
            }
            if (Auctioneer.S == Auction.DefaultAuctioneer)
            {
                displayModeLabel.Visible = displayModeComboBox.Visible = false;
            }
            if (Auctioneer.DS == Auction.DefaultAuctioneer)
            {
                this.logoPictureBox.Visible = false;
            }

            Settings.Load();

            /*ProcessStartInfo pStartInfo = new ProcessStartInfo();
            pStartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, Path.GetDirectoryName(Settings.biddingResultFP));
            pStartInfo.FileName = "cmd.exe";
            pStartInfo.Arguments = @"/k" + Settings.serverFN;
            string serverExePath = Path.Combine(pStartInfo.WorkingDirectory, Settings.serverFN);
            if (Utility.IsFileExist(serverExePath))
                m_server = System.Diagnostics.Process.Start(pStartInfo);*/

            PriceLevels.Load(Settings.priceLevelFP);

            InitDisplayForm();

            InitSessionComboBox();

            nowPriceTextBox.Focus();

            ExchangeRate.Load(Settings.exchangeRateFP);
            mainCurrencyTextBox.Text = ExchangeRate.mainRateName;
            er1NameTextBox.Text = ExchangeRate.rateNames[0];
            er2NameTextBox.Text = ExchangeRate.rateNames[1];
            er3NameTextBox.Text = ExchangeRate.rateNames[2];
            er1TextBox.Text = ExchangeRate.mainToExchangeRate[0].ToString();
            er2TextBox.Text = ExchangeRate.mainToExchangeRate[1].ToString();
            er3TextBox.Text = ExchangeRate.mainToExchangeRate[2].ToString();

            //LoadBiddingResult(Settings.biddingResultFP);
            //LoadBackupPath(Path.Combine(Path.GetDirectoryName(Settings.biddingResultFP), "backup_path.ini"));

            //SetPriceLevelForm setPLForm = new SetPriceLevelForm();
            //setPLForm.Show();
            displayModeComboBox.SelectedIndex = 0;
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
            if (Auctioneer.S == Auction.DefaultAuctioneer)
                ShowPlayerForm(m_auctions[m_auctionIdNow].videoPath);
            m_displayForm.SetProgress(m_auctionIdNow + 1, m_auctions.Count);
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
            if (Auctioneer.S == Auction.DefaultAuctioneer)
                ShowPlayerForm(m_auctions[m_auctionIdNow].videoPath);
            m_displayForm.SetProgress(m_auctionIdNow + 1, m_auctions.Count);
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

            int nowPrice = m_auctions[m_auctionIdNow].nowPrice;
            for (int i = 0; i < PriceLevels.levels.Count; i++)
            {
                PriceLevel pl = PriceLevels.levels[i];
                if (nowPrice < pl.up && nowPrice >= pl.down)
                {
                    int newPrice = 0;
                    if (pl.increments.Count > 1)
                    {
                        int down = pl.down;
                        /*int interval = (int)Math.Pow(10, (pl.down.ToString().Length - 1));
                        if (interval < Settings.unit)
                            interval = Settings.unit;*/
                        if (down >= pl.increments[0] && down <= pl.increments[pl.increments.Count - 1])
                            down = 0;
                        int interval = pl.increments[pl.increments.Count - 1];
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
            MessageBox.Show("跳階設定有誤，請檢查跳階!");
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

            if (MessageBox.Show("是否重置此拍品?\n\nNOTE:結帳狀態將一併重置", "警告", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                winBidderTextBox.Text = "";
                winBidderTextBox.BackColor = Color.Black;

                SetNewNowPrice(m_auctions[m_auctionIdNow].initialPrice);
                ClearBidder(m_auctionIdNow);
                ClearCheckoutState(m_auctionIdNow);
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

            if (MessageBox.Show("是否重置全部拍品?\n\nNOTE:結帳狀態將一併重置", "警告", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                winBidderTextBox.Text = "";
                winBidderTextBox.BackColor = Color.Black;

                SetNewNowPrice(m_auctions[m_auctionIdNow].initialPrice);
                for (int i = 0; i < m_auctions.Count; i++)
                {
                    m_auctions[i].ResetPrice();
                    m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, m_auctions[i].lot, ae => ae.NowPrice, m_auctions[i].initialPrice);
                    m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, m_auctions[i].lot, ae => ae.HammerPrice, m_auctions[i].initialPrice);
                    ClearBidder(i);
                    ClearCheckoutState(i);
                }
            }
        }

        /// <summary>
        /// Save price now when form closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SavePrices(Path.Combine(Application.StartupPath, Settings.saveFolder, Settings.pricesFN));
            string erFilePath = Path.Combine(Application.StartupPath, Settings.configFolder, Settings.exchangeRateFN);
            string[] rateNames = { er1NameTextBox.Text, er2NameTextBox.Text, er3NameTextBox.Text };
            string[] rates = { er1TextBox.Text, er2TextBox.Text, er3TextBox.Text };
            ExchangeRate.Save(erFilePath, mainCurrencyTextBox.Text, ref rateNames, ref rates);
            PriceLevels.Save();

            //DoSaveBiddingResult(Settings.biddingResultFP);
            /*if (!m_server.HasExited)
                m_server.CloseMainWindow();*/
        }

        /// <summary>
        /// Enter price now and some hotkeys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F12)
            {
                m_displayForm.Close();
                this.Close();
            }

            if (e.KeyData == Keys.Escape)
            {
                m_inputNumbers = "";
                nowPriceTextBox.Text = m_auctions[m_auctionIdNow].nowPrice.ToString("c");
            }

            if (er1TextBox.Focused || er2TextBox.Focused || er3TextBox.Focused || increaseByLevelBtn.Focused || resetBtn.Focused ||
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
            m_displayForm.SetProgress(m_auctionIdNow + 1, m_auctions.Count);
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
        /// When user input price now and press enter, it will coutdown 1 seconds and change nowPriceTextBox.BackColor back to black color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            nowPriceTextBox.BackColor = Color.Black;
            timer1.Stop();
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
                if (m_beInternet.FineOne<int>(be => be.BidderID_int, bidderNo) == null)
                {
                    if (MessageBox.Show("無此買家牌號：" + bidderNo.ToString() + "\n是否強制輸入此牌號", "警告", MessageBoxButtons.OKCancel) !=
                        System.Windows.Forms.DialogResult.OK)
                    {
                        winBidderTextBox.Text = "";
                        return;
                    }
                }
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
            winBidderTextBox.BackColor = Color.Black;
            ClearBidder(m_auctionIdNow);
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (Auctioneer.S == Auction.DefaultAuctioneer)
                ShowPlayerForm(m_auctions[m_auctionIdNow].videoPath);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            ClosePlayerForm();
        }

        private void sessionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sessionSelected = sessionComboBox.SelectedItem.ToString();
            LoadAuctionsFromSession(sessionSelected);  // load auctions from session.
            m_displayForm.SetSession(sessionSelected);
            m_displayForm.SetProgress(1, m_auctions.Count);
        }

        /// <summary>
        /// Restrict rmbTextBox to accept only digit and control and punctuation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void er1TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckRateInput(e.KeyChar);
        }

        /// <summary>
        /// Restrict usdTextBox to accept only digit and control and punctuation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckRateInput(e.KeyChar);
        }

        private void hkTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckRateInput(e.KeyChar);
        }

        /// <summary>
        /// Event handler when user input rmb exchange rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void er1TextBox_TextChanged(object sender, EventArgs e)
        {
            RateChanged(er1TextBox.Text, 0);
        }

        /// <summary>
        /// Event handler when user input usd exchange rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void er2TextBox_TextChanged(object sender, EventArgs e)
        {
            RateChanged(er2TextBox.Text, 1);
        }

        private void er3TextBox_TextChanged(object sender, EventArgs e)
        {
            RateChanged(er3TextBox.Text, 2);
        }

        private void er1NameTextBox_TextChanged(object sender, EventArgs e)
        {
            m_displayForm.SetRateName(0, er1NameTextBox.Text);
        }

        private void er2NameTextBox_TextChanged(object sender, EventArgs e)
        {
            m_displayForm.SetRateName(1, er2NameTextBox.Text);
        }

        private void er3NameTextBox_TextChanged(object sender, EventArgs e)
        {
            m_displayForm.SetRateName(2, er3NameTextBox.Text);
        }

        private void currencyTextBox_TextChanged(object sender, EventArgs e)
        {
            m_displayForm.SetRateName(-1, mainCurrencyTextBox.Text);
        }

        private void setPriceLevelButton_Click(object sender, EventArgs e)
        {
            SetPriceLevelForm setPLForm = new SetPriceLevelForm();
            setPLForm.Show();
        }

        private void displayModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string modeSelected = displayModeComboBox.SelectedItem.ToString();
            if (modeSelected == m_curDisplayMode.ToString())
                return;

            if (DisplayMode.純文字.ToString() == modeSelected )
            {
                m_displayForm.Close();
                m_displayForm.Dispose();
                m_displayForm = new DisplayForm_DS_TextOnly();
            }
            else if (DisplayMode.影片.ToString() == modeSelected)
            {
                m_displayForm.Close();
                m_displayForm.Dispose();
                m_displayForm = new DisplayForm_DS_Movie();
            }
            else if (DisplayMode.圖片.ToString() == modeSelected)
            {
                m_displayForm.Close();
                m_displayForm.Dispose();
                m_displayForm = new DisplayForm();
                m_displayForm.SetLogo(Auction.DefaultAuctioneer);
            }
            m_displayForm.SetAuctionOnForm(m_auctions[m_auctionIdNow]);

            Screen otherScreen = Screen.FromControl(this);
            if (Screen.AllScreens.Length > 1)
            {
                otherScreen = Screen.AllScreens[1];
            }
            m_displayForm.Left = otherScreen.WorkingArea.Left /*+ Settings.displayPos.X*/;
            m_displayForm.Top = otherScreen.WorkingArea.Top /*+ Settings.displayPos.Y*/;
            m_displayForm.WindowState = FormWindowState.Maximized;
            m_displayForm.Show();
            m_curDisplayMode = (DisplayMode)displayModeComboBox.SelectedIndex;
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
            if (Auctioneer.DS == Auction.DefaultAuctioneer)
            {
                m_displayForm = new DisplayForm_DS_TextOnly();
            }
            else if (Auctioneer.N == Auction.DefaultAuctioneer)
            {
                m_displayForm = new DisplayForm_N_16_9();
            }
            else
            {
                m_displayForm = new DisplayForm();
            }
            //m_displayForm.Size = Settings.displaySize;
            Screen otherScreen = Screen.FromControl(this);
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

        private void LoadAuctionsFromSession(string sessionId)
        {
            if (m_auctions != null)
                m_auctions.Clear();
            Auction.LoadAuctions(sessionId, ref m_auctions, ref m_aeInternet, false);
            //LoadPrices(Settings.pricesFP);

            if (m_auctions.Count != 0)
            {
                m_auctionIdNow = 0;
                SetAuctionOnForm(m_auctions[m_auctionIdNow]);
                m_displayForm.SetAuctionOnForm(m_auctions[m_auctionIdNow]);
            }
            else
            {
                MessageBox.Show(String.Format("請在Auctions/場次 {0} 儲存拍品圖片", sessionId.ToString()), "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            InitAuctionComboBox();

            if (m_auctions.Count != 0)
                DoLoadAuctionPhoto(0);

            m_sessionIdNow = sessionId;
        }

        /// <summary>
        /// Initialize auction ComboBox.
        /// </summary>
        private void InitAuctionComboBox()
        {
            auctionComboBox.Items.Clear();
            for (int i = 0; i < m_auctions.Count; i++)
                auctionComboBox.Items.Add(m_auctions[i].lot);
            if (m_auctions.Count != 0)
                auctionComboBox.SelectedIndex = 0;
        }

        private void DoLoadAuctionPhoto(int id)
        {
            if (m_auctions[id].photo == null)
                m_auctions[id].photo = Utility.OpenBitmap(m_auctions[id].photoFilePath);
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
                        auction.photo = Utility.OpenBitmap(auction.photoFilePath);
                }
                else if (min > max && (i >= min || i <= max))
                {
                    if (auction.photo == null)
                        auction.photo = Utility.OpenBitmap(auction.photoFilePath);
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
            if (!Utility.IsFileExist(fn, false))
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
                auctionPictureBox.Image = Bidding.Properties.Resources.loading;

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
            m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, m_auctions[m_auctionIdNow].lot, ae => ae.NowPrice, newPrice);
            if (m_auctions[m_auctionIdNow].winBidderNo > 0)
                m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, m_auctions[m_auctionIdNow].lot, ae => ae.HammerPrice, newPrice);
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
                        m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, auction.lot, ae => ae.BidderNumber, auction.winBidderNo.ToString());
                        m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, auction.lot, ae => ae.NowPrice, auction.nowPrice);
                        m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, auction.lot, ae => ae.HammerPrice, auction.nowPrice);
                    }
                }
            }

            if (Directory.Exists(Settings.backupFP))
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
            if (!Utility.IsFileExist(path, false))
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

        private void ClearBidder(int auctionId)
        {
            m_auctions[auctionId].winBidderNo = 0;
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, m_auctions[auctionId].lot, ae => ae.BidderNumber, "");
        }

        private void ClearCheckoutState(int auctionId)
        {
            m_auctions[auctionId].checkoutNumber = 0;
            m_auctions[auctionId].checkoutTime = "";
            m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, m_auctions[auctionId].lot, ae => ae.CheckoutNumber, m_auctions[auctionId].checkoutNumber);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, m_auctions[auctionId].lot, ae => ae.CheckoutTime, m_auctions[auctionId].checkoutTime);
        }

        private void ShowPlayerForm(string videoPath)
        {
            ClosePlayerForm();

            if (videoPath == "")
            {
                //m_form2 = new Form2();
                //m_form2.Show();
                return;
            }

            try
            {
                m_playerForm = new PlayerForm(Path.Combine(Application.StartupPath, videoPath));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            Screen otherScreen = Screen.FromControl(this);
            if (Screen.AllScreens.Length > 1)
            {
                otherScreen = Screen.AllScreens[1];
                m_playerForm.Left = otherScreen.WorkingArea.Left /*+ Settings.displayPos.X*/;
                m_playerForm.Top = otherScreen.WorkingArea.Top /*+ Settings.displayPos.Y*/;
            }
            m_playerForm.WindowState = FormWindowState.Maximized;
            m_playerForm.Show();
        }

        private void ClosePlayerForm()
        {
            if (m_playerForm != null)
            {
                m_playerForm.CloseForm();
                m_playerForm = null;
            }
        }

        private void InitSessionComboBox()
        {
            sessionComboBox.Items.Clear();
            m_sessions = Auction.LoadSessions();
            if (m_sessions.Count == 0)
                MessageBox.Show("請在Auctions建立場次資料夾，\n例：1、2...", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                sessionComboBox.Items.AddRange(m_sessions.ToArray());
            if (m_sessions.Count > 0)
            {
                sessionComboBox.SelectedIndex = 0;
            }
        }

        private bool CheckRateInput(char keyChar)
        {
            if (Char.IsDigit(keyChar) || Char.IsControl(keyChar) || Char.IsPunctuation(keyChar))
                return false;
            else
                return true;
        }

        private void RateChanged(string rateText, int rateId)
        {
            float rate = 0f;
            if ("0" == rateText)
            {
                m_displayForm.ShowExchangeRate(rateId, false);
            }
            else
            {
                m_displayForm.ShowExchangeRate(rateId, true);
                if (rateText != "" && m_auctions != null && m_auctions.Count != 0)
                {
                    if (float.TryParse(rateText, out rate))
                    {
                        ExchangeRate.mainToExchangeRate[rateId] = ExchangeRate.Revert(rate);
                        m_displayForm.SetNewPrice(m_auctions[m_auctionIdNow].nowPrice);
                    }
                }
            }
        }
        #endregion
    }
}