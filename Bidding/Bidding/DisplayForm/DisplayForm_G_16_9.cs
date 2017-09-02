//#define MCT
//#define SJ

using System;
using System.Drawing;
using System.Windows.Forms;
using BiddingLibrary;
using UtilityLibrary;

namespace Bidding
{
    public partial class DisplayForm_G_16_9 : DisplayFormBase
    {
        //[DllImport("gdi32.dll")]
        //private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        //FontFamily ff;
        //Font font;
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        #endregion

        #region Member Variables
        private Size m_formSize;
        private ControlState m_lotLabelState;
        private ControlState m_logoPictureBoxState;
        private ControlState m_lotNumLabelState;
        private ControlState m_artistLabelState;
        private ControlState m_mainCurNameLabelState;
        private ControlState m_mainPriceLabelState;
        private ControlState m_er1LabelState;
        private ControlState m_er2LabelState;
        private ControlState m_er3LabelState;
        private ControlState m_er1PriceLabelState;
        private ControlState m_er2PriceLabelState;
        private ControlState m_er3PriceLabelState;
        private ControlState m_artworkTextBoxState;
        private ControlState m_progressLabelState;
        private Bitmap m_logo_S = Bidding.Properties.Resources.LOGO_S_big;
        private Bitmap m_logo_A = Bidding.Properties.Resources.LOGO_A;
        private Bitmap m_logo_M = Bidding.Properties.Resources.LOGO_M;
        private Bitmap m_logo_N = Bidding.Properties.Resources.LOGO_N_big;
        private Bitmap m_logo_G = Bidding.Properties.Resources.LOGO_G;
        #endregion

        #region Properties
        public bool ChangeLogoEnable { get; set; }
        #endregion

        #region Constructors and Finalizers
        public DisplayForm_G_16_9()
        {
            InitializeComponent();
            splitContainer1.Panel1.Controls.Add(lotLabel);
            splitContainer1.Panel1.Controls.Add(logoPictureBox);
            splitContainer1.Panel1.Controls.Add(lotNumLabel);
            splitContainer1.Panel1.Controls.Add(artistLabel);
            splitContainer1.Panel1.Controls.Add(er1NameLabel);
            splitContainer1.Panel1.Controls.Add(mainCurNameLabel);
            splitContainer1.Panel1.Controls.Add(er2NameLabel);
            splitContainer1.Panel1.Controls.Add(mainPriceLabel);
            splitContainer1.Panel1.Controls.Add(er1PriceLabel);
            splitContainer1.Panel1.Controls.Add(er2PriceLabel);
            splitContainer1.Panel1.Controls.Add(artworkTextBox);
            splitContainer1.Panel1.Controls.Add(er3PriceLabel);
            splitContainer1.Panel1.Controls.Add(er3NameLabel);
            splitContainer1.Panel2.Controls.Add(auctionPictureBox);

            m_formSize = this.Size;
            Control cLotLabel = (Control)lotLabel;
            m_lotLabelState = new ControlState(lotLabel.Location, lotLabel.Size, lotLabel.Font.Size, ref cLotLabel);
            Control cLogoPictureBox = (Control)logoPictureBox;
            m_logoPictureBoxState = new ControlState(logoPictureBox.Location, logoPictureBox.Size, 0, ref cLogoPictureBox);
            Control cLotNumLabel = (Control)lotNumLabel;
            m_lotNumLabelState = new ControlState(lotNumLabel.Location, lotNumLabel.Size, lotNumLabel.Font.Size, ref cLotNumLabel);
            Control cArtistLabel = (Control)artistLabel;
            m_artistLabelState = new ControlState(artistLabel.Location, artistLabel.Size, artistLabel.Font.Size, ref cArtistLabel);
            Control cMainCurNameLabel = (Control)mainCurNameLabel;
            m_mainCurNameLabelState = new ControlState(mainCurNameLabel.Location, mainCurNameLabel.Size, mainCurNameLabel.Font.Size, ref cMainCurNameLabel);
            Control cMainPriceLabel = (Control)mainPriceLabel;
            m_mainPriceLabelState = new ControlState(mainPriceLabel.Location, mainPriceLabel.Size, mainPriceLabel.Font.Size, ref cMainPriceLabel);
            Control cEr1Label = (Control)er1NameLabel;
            m_er1LabelState = new ControlState(er1NameLabel.Location, er1NameLabel.Size, er1NameLabel.Font.Size, ref cEr1Label);
            Control cEr2Label = (Control)er2NameLabel;
            m_er2LabelState = new ControlState(er2NameLabel.Location, er2NameLabel.Size, er2NameLabel.Font.Size, ref cEr2Label);
            Control cEr3Label = (Control)er3NameLabel;
            m_er3LabelState = new ControlState(er3NameLabel.Location, er3NameLabel.Size, er3NameLabel.Font.Size, ref cEr3Label);
            Control cEr1PriceLabel = (Control)er1PriceLabel;
            m_er1PriceLabelState = new ControlState(er1PriceLabel.Location, er1PriceLabel.Size, er1PriceLabel.Font.Size, ref cEr1PriceLabel);
            Control cEr2PriceLabel = (Control)er2PriceLabel;
            m_er2PriceLabelState = new ControlState(er2PriceLabel.Location, er2PriceLabel.Size, er2PriceLabel.Font.Size, ref cEr2PriceLabel);
            Control cEr3PriceLabel = (Control)er3PriceLabel;
            m_er3PriceLabelState = new ControlState(er3PriceLabel.Location, er3PriceLabel.Size, er3PriceLabel.Font.Size, ref cEr3PriceLabel);
            Control cArtworkTextBox = (Control)artworkTextBox;
            m_artworkTextBoxState = new ControlState(artworkTextBox.Location, artworkTextBox.Size, artworkTextBox.Font.Size, ref cArtworkTextBox);
            Control cProgressLabel = (Control)progressLabel;
            m_progressLabelState = new ControlState(cProgressLabel.Location, cProgressLabel.Size, cProgressLabel.Font.Size, ref cProgressLabel);
            //label6.Font = new Font("HelveticaNeueLT Pro 45 Lt", 36);
        }
        #endregion

        #region Windows Form Events
        private void DisplayForm_G_16_9_Load(object sender, EventArgs e)
        {
            //CargoPrivateFontCollection();
            //CargoEtiqueta(font);
            if (m_formSize.Width == 0 || m_formSize.Height == 0)
                return;
            float xRatio = (float)this.Width / m_formSize.Width;
            float yRatio = (float)this.Height / m_formSize.Height;
            ReArrangeAll(xRatio, yRatio);
            ChangeLogoCheck();
            if (BiddingCompany.G == Auction.DefaultBiddingCompany)
                this.logoPictureBox.Image = m_logo_G;
        }

        private void DisplayForm_G_16_9_Resize(object sender, EventArgs e)
        {
            if (m_formSize.Width == 0 || m_formSize.Height == 0)
                return;
            float xRatio = (float)this.Width / m_formSize.Width;
            float yRatio = (float)this.Height / m_formSize.Height;
            ReArrangeAll(xRatio, yRatio);

            Console.WriteLine("DisplayForm_G_16_9_Resize -----------");
            Console.WriteLine("this: " + this.Size + "; m_formSize: " + m_formSize);
            Console.WriteLine("logo Location: " + this.logoPictureBox.Location + "; Size: " + this.logoPictureBox.Size);
        }

        private void DisplayForm_G_16_9_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_lotLabelState = null;
            m_logoPictureBoxState = null;
            m_lotNumLabelState = null;
            m_artistLabelState = null;
            m_mainCurNameLabelState = null;
            m_mainPriceLabelState = null;
            m_er1LabelState = null;
            m_er2LabelState = null;
            m_er3LabelState = null;
            m_er1PriceLabelState = null;
            m_er2PriceLabelState = null;
            m_er3PriceLabelState = null;
            m_artworkTextBoxState = null;
            m_progressLabelState = null;
            m_logo_S = null;
            m_logo_A = null;
            m_logo_M = null;
            m_logo_N = null;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set auction on this Form by Auction object.
        /// </summary>
        /// <param name="auction">Auction object</param>
        public override void SetAuctionOnForm(Auction auction)
        {
            lotNumLabel.Text = auction.lot;
            artistLabel.Text = auction.artist;
            artworkTextBox.Text = auction.artwork;
            SetNewPrice(auction.nowPrice);
            if (auction.photo != null)
                Utility.SetImageNoStretch(ref auctionPictureBox, ref auction.photo);
            else
                auctionPictureBox.Image = Bidding.Properties.Resources.loading;

            //if (ChangeLogoEnable)
            //SetLogo(Utility.ToEnum<Auctioneer>(auction.auctioneer));
        }

        /// <summary>
        /// Set new price on display form.
        /// </summary>
        /// <param name="newMainPrice">new ntd price now</param>
        public override void SetNewPrice(int newMainPrice)
        {
            mainPriceLabel.Text = newMainPrice.ToString("c");
            er1PriceLabel.Text = ExchangeRate.MainToCurrency(newMainPrice, 0).ToString("c");
            er2PriceLabel.Text = ExchangeRate.MainToCurrency(newMainPrice, 1).ToString("c");
            er3PriceLabel.Text = ExchangeRate.MainToCurrency(newMainPrice, 2).ToString("c");
        }

        public override void SetSession(string sessionStr)
        {
            sessionLabel.Text = String.Format(@"第{0}場", sessionStr);
        }

        public override void SetProgress(int currentId, int totalCount)
        {
            progressLabel.Text = String.Format(@"{0}/{1}", currentId, totalCount);
        }

        public override void SetRateName(int rateId, string name)
        {
            switch (rateId)
            {
                case 0:
                    er1NameLabel.Text = name;
                    break;
                case 1:
                    er2NameLabel.Text = name;
                    break;
                case 2:
                    er3NameLabel.Text = name;
                    break;
                default:
                    mainCurNameLabel.Text = name;
                    break;
            }
        }

        public override void ShowExchangeRate(int erId, bool isShow)
        {
            switch (erId)
            {
                case 0:
                    {
                        er1NameLabel.Visible = isShow;
                        er1PriceLabel.Visible = isShow;
                    }
                    break;
                case 1:
                    {
                        er2NameLabel.Visible = isShow;
                        er2PriceLabel.Visible = isShow;
                    }
                    break;
                case 2:
                    {
                        er3NameLabel.Visible = isShow;
                        er3PriceLabel.Visible = isShow;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        /*private void CargoPrivateFontCollection()
        {
            // Create the byte array and get its length

            byte[] fontArray = BiddingSystem.Properties.Resources.arial;
            int dataLength = BiddingSystem.Properties.Resources.arial.Length;


            // ASSIGN MEMORY AND COPY  BYTE[] ON THAT MEMORY ADDRESS
            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);

            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection pfc = new PrivateFontCollection();
            //PASS THE FONT TO THE  PRIVATEFONTCOLLECTION OBJECT
            pfc.AddMemoryFont(ptrData, dataLength);
            //pfc.AddFontFile("D:\\Helvetica\\HelveticaNeue\\HelveticaNeueLTPro-LtCn.otf");

            //FREE THE  "UNSAFE" MEMORY
            Marshal.FreeCoTaskMem(ptrData);

            ff = pfc.Families[0];
            font = new Font(ff, 15f, FontStyle.Regular);
        }
        private void CargoEtiqueta(Font font)
        {
            FontStyle fontStyle = FontStyle.Regular;
            this.label6.Font = new Font(ff, 36, fontStyle);
        }*/

        private void ReArrangeAll(float xRatio, float yRatio)
        {
            m_lotLabelState.ReArrange(xRatio, yRatio);
            m_logoPictureBoxState.ReArrange(xRatio, yRatio);
            m_lotNumLabelState.ReArrange(xRatio, yRatio);
            m_artistLabelState.ReArrange(xRatio, yRatio);
            m_er1LabelState.ReArrange(xRatio, yRatio);
            m_mainCurNameLabelState.ReArrange(xRatio, yRatio);
            m_er2LabelState.ReArrange(xRatio, yRatio);
            m_mainPriceLabelState.ReArrange(xRatio, yRatio);
            m_er1PriceLabelState.ReArrange(xRatio, yRatio);
            m_er2PriceLabelState.ReArrange(xRatio, yRatio);
            m_artworkTextBoxState.ReArrange(xRatio, yRatio);
            m_er3PriceLabelState.ReArrange(xRatio, yRatio);
            m_er3LabelState.ReArrange(xRatio, yRatio);
            m_progressLabelState.ReArrange(xRatio, yRatio);
        }

        private void SetLogo(BiddingCompany auctionCompany)
        {
            switch (auctionCompany)
            {
                case BiddingCompany.S:
                    logoPictureBox.Image = m_logo_S;
                    break;
                /*case Auctioneer.A:
                    logoPictureBox.Image = m_logo_A;
                    break;*/
                case BiddingCompany.M:

                    logoPictureBox.Image = m_logo_M;
                    break;
                case BiddingCompany.N:
                    logoPictureBox.Image = m_logo_N;
                    break;
                case BiddingCompany.G:
                    logoPictureBox.Image = m_logo_G;
                    break;
                default:
                    break;
            }
        }

        private string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            textBox.UseSystemPasswordChar = true;
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.ShowDialog();
            return textBox.Text;
        }

        private void ChangeLogoCheck()
        {
            this.ChangeLogoEnable = false;
            string today = DateTime.Now.ToShortDateString();
            if (today == @"2013/12/21" || today == @"2013/12/22" ||
                today == @"2015/1/18")
            {
                string password = ShowDialog("", "請輸入啟動密碼");
                if (password == "superwaser55667878")
                    this.ChangeLogoEnable = true;
            }
        }
        #endregion
    }
}
