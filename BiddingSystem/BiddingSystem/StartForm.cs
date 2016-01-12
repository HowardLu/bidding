using System;
using System.Windows.Forms;
using BiddingLibrary;

namespace BiddingSystem
{
    public partial class StartForm : Form
    {
        private Form mForm;
        private bool mIsStartServer;

        public StartForm()
        {
            InitializeComponent();
#if M
            Auction.DefaultAuctioneer = Auctioneer.M;
            this.Text = "沐春堂拍賣系統" + Application.ProductVersion;
#elif S
            Auction.DefaultAuctioneer = Auctioneer.S;
            this.Text = "世家拍賣系統" + Application.ProductVersion;
#elif N
            Auction.DefaultAuctioneer = Auctioneer.N;
            this.Text = "新象拍賣系統" + Application.ProductVersion;
#elif SFJ
            Auction.DefaultAuctioneer = Auctioneer.SFJ;
            this.Text = "禪機拍賣系統" + Application.ProductVersion;
#elif DS
            Auction.DefaultAuctioneer = Auctioneer.DS;
            this.Text = "大行拍賣系統 " + Application.ProductVersion;
#else
            Auction.DefaultAuctioneer = Auctioneer.S;
            this.Text = "世家拍賣系統" + Application.ProductVersion;
#endif
            if (DialogResult.Yes == MessageBox.Show("請問本機是否為主機?", "是否主機?", MessageBoxButtons.YesNo))
            {
                string exePath = "C:\\mongodb-win32-i386-2.4.6\\bin\\run.bat";
                if (System.IO.File.Exists(exePath))
                {
                    System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(exePath));
                    System.Diagnostics.Process.Start(exePath);
                    System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
                }
                else
                    MessageBox.Show("找不到mongo db資料夾，請確認以下檔案是否存在：\n" + exePath);
            }
        }

        private void accountingButton_Click(object sender, EventArgs e)
        {
            mForm = new Accounting.AccountingForm();
            mForm.Show();
        }

        private void biddingButton_Click(object sender, EventArgs e)
        {
            mForm = new Bidding.ControlForm();
            mForm.Show();
        }

        private void checkoutButton_Click(object sender, EventArgs e)
        {
            mForm = new Checkout.CheckoutForm();
            mForm.Show();
        }

        private void setAuctionButton_Click(object sender, EventArgs e)
        {
            mForm = new SetAuction.SetAuctionForm();
            mForm.Show();
        }

        private void bidderButton_Click(object sender, EventArgs e)
        {
            string exePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) +
                "\\BidderDataInput\\exe\\BidderDataInput.exe";
            System.Diagnostics.Process.Start(exePath);
        }

        private void dealerButton_Click(object sender, EventArgs e)
        {
            string exePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) +
                "\\DealerDataInput\\exe\\DealerDataInput.exe";
            System.Diagnostics.Process.Start(exePath);
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }
    }
}
