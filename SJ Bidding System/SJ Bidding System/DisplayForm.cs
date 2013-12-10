using System;
using System.Drawing;
using System.Windows.Forms;
using Bidding;
using UtilityLibrary;

namespace SJ_Bidding_System
{
    public partial class DisplayForm : Form
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
        private ControlState m_rmbLabelState;
        private ControlState m_ntdLabelState;
        private ControlState m_usdLabelState;
        private ControlState m_ntdPriceLabelState;
        private ControlState m_rmbPriceLabelState;
        private ControlState m_usdPriceLabelState;
        private ControlState m_artworkTextBoxState;
        private ControlState m_hkPriceLabelState;
        private ControlState m_hkLabelState;
        private Bitmap m_logo_S = Properties.Resources.LOGO_S_big;
        private Bitmap m_logo_A = Properties.Resources.LOGO_A;
        private Bitmap m_logo_M = Properties.Resources.LOGO_M;
        #endregion

        #region Properties
        #endregion

        #region Constructors and Finalizers
        public DisplayForm()
        {
            InitializeComponent();
            splitContainer1.Panel1.Controls.Add(lotLabel);
            splitContainer1.Panel1.Controls.Add(logoPictureBox);
            splitContainer1.Panel1.Controls.Add(lotNumLabel);
            splitContainer1.Panel1.Controls.Add(artistLabel);
            splitContainer1.Panel1.Controls.Add(rmbLabel);
            splitContainer1.Panel1.Controls.Add(ntdLabel);
            splitContainer1.Panel1.Controls.Add(usdLabel);
            splitContainer1.Panel1.Controls.Add(ntdPriceLabel);
            splitContainer1.Panel1.Controls.Add(rmbPriceLabel);
            splitContainer1.Panel1.Controls.Add(usdPriceLabel);
            splitContainer1.Panel1.Controls.Add(artworkTextBox);
            splitContainer1.Panel1.Controls.Add(hkPriceLabel);
            splitContainer1.Panel1.Controls.Add(hkLabel);
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
            Control cRmbLabel = (Control)rmbLabel;
            m_rmbLabelState = new ControlState(rmbLabel.Location, rmbLabel.Size, rmbLabel.Font.Size, ref cRmbLabel);
            Control cNtdLabel = (Control)ntdLabel;
            m_ntdLabelState = new ControlState(ntdLabel.Location, ntdLabel.Size, ntdLabel.Font.Size, ref cNtdLabel);
            Control cUsdLabel = (Control)usdLabel;
            m_usdLabelState = new ControlState(usdLabel.Location, usdLabel.Size, usdLabel.Font.Size, ref cUsdLabel);
            Control cNtdPriceLabel = (Control)ntdPriceLabel;
            m_ntdPriceLabelState = new ControlState(ntdPriceLabel.Location, ntdPriceLabel.Size, ntdPriceLabel.Font.Size, ref cNtdPriceLabel);
            Control cRmbPriceLabel = (Control)rmbPriceLabel;
            m_rmbPriceLabelState = new ControlState(rmbPriceLabel.Location, rmbPriceLabel.Size, rmbPriceLabel.Font.Size, ref cRmbPriceLabel);
            Control cUsdPriceLabel = (Control)usdPriceLabel;
            m_usdPriceLabelState = new ControlState(usdPriceLabel.Location, usdPriceLabel.Size, usdPriceLabel.Font.Size, ref cUsdPriceLabel);
            Control cArtworkTextBox = (Control)artworkTextBox;
            m_artworkTextBoxState = new ControlState(artworkTextBox.Location, artworkTextBox.Size, artworkTextBox.Font.Size, ref cArtworkTextBox);
            Control cHkPriceLabel = (Control)hkPriceLabel;
            m_hkPriceLabelState = new ControlState(hkPriceLabel.Location, hkPriceLabel.Size, hkPriceLabel.Font.Size, ref cHkPriceLabel);
            Control cHkLabel = (Control)hkLabel;
            m_hkLabelState = new ControlState(hkLabel.Location, hkLabel.Size, hkLabel.Font.Size, ref cHkLabel);
            //label6.Font = new Font("HelveticaNeueLT Pro 45 Lt", 36);
        }
        #endregion

        #region Windows Form Events
        private void DisplayForm_Load(object sender, EventArgs e)
        {
            //CargoPrivateFontCollection();
            //CargoEtiqueta(font);
            if (m_formSize.Width == 0 || m_formSize.Height == 0)
                return;
            float xRatio = (float)this.Width / m_formSize.Width;
            float yRatio = (float)this.Height / m_formSize.Height;
            ReArrangeAll(xRatio, yRatio);
        }

        private void DisplayForm_Resize(object sender, EventArgs e)
        {
            if (m_formSize.Width == 0 || m_formSize.Height == 0)
                return;
            float xRatio = (float)this.Width / m_formSize.Width;
            float yRatio = (float)this.Height / m_formSize.Height;
            ReArrangeAll(xRatio, yRatio);

            Console.WriteLine("DisplayForm_Resize -----------");
            Console.WriteLine("this: " + this.Size + "; m_formSize: " + m_formSize);
            Console.WriteLine("logo Location: " + this.logoPictureBox.Location + "; Size: " + this.logoPictureBox.Size);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set auction on this Form by Auction object.
        /// </summary>
        /// <param name="auction">Auction object</param>
        public void SetAuctionOnForm(Auction auction)
        {
            lotNumLabel.Text = auction.lot;
            artistLabel.Text = auction.artist;
            artworkTextBox.Text = auction.artwork;
            SetNewPrice(auction.nowPrice);
            if (auction.photo != null)
                Utility.SetImageNoStretch(ref auctionPictureBox, ref auction.photo);
            else
                auctionPictureBox.Image = Properties.Resources.loading;
            SetLogo(Utility.ToEnum<Auctioneer>(auction.auctioneer));
        }

        /// <summary>
        /// Set new price on display form.
        /// </summary>
        /// <param name="newNtd">new ntd price now</param>
        public void SetNewPrice(int newNtd)
        {
            ntdPriceLabel.Text = newNtd.ToString("c");
            rmbPriceLabel.Text = ExchangeRate.NtdToRmb(newNtd).ToString("c");
            usdPriceLabel.Text = ExchangeRate.NtdToUsd(newNtd).ToString("c");
            hkPriceLabel.Text = ExchangeRate.NtdToHk(newNtd).ToString("c");
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        /*private void CargoPrivateFontCollection()
        {
            // Create the byte array and get its length

            byte[] fontArray = SJ_Bidding_System.Properties.Resources.arial;
            int dataLength = SJ_Bidding_System.Properties.Resources.arial.Length;


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
            m_rmbLabelState.ReArrange(xRatio, yRatio);
            m_ntdLabelState.ReArrange(xRatio, yRatio);
            m_usdLabelState.ReArrange(xRatio, yRatio);
            m_ntdPriceLabelState.ReArrange(xRatio, yRatio);
            m_rmbPriceLabelState.ReArrange(xRatio, yRatio);
            m_usdPriceLabelState.ReArrange(xRatio, yRatio);
            m_artworkTextBoxState.ReArrange(xRatio, yRatio);
            m_hkPriceLabelState.ReArrange(xRatio, yRatio);
            m_hkLabelState.ReArrange(xRatio, yRatio);
        }

        private void SetLogo(Auctioneer auctioneer)
        {
            switch (auctioneer)
            {
                case Auctioneer.S:
                    logoPictureBox.Image = m_logo_S;
                    break;
                case Auctioneer.A:
                    logoPictureBox.Image = m_logo_A;
                    break;
                case Auctioneer.M:
                    logoPictureBox.Image = m_logo_M;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
