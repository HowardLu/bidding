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
            checkoutDealerButton.Visible = takebackDealerButton.Visible = false;
#if M
            Auction.DefaultBiddingCompany = BiddingCompany.M;
            this.Text = "沐春堂拍賣系統" + Application.ProductVersion;
#elif S
            Auction.DefaultBiddingCompany = BiddingCompany.S;
            this.Text = "世家拍賣系統" + Application.ProductVersion;
#elif N
            Auction.DefaultBiddingCompany = BiddingCompany.N;
            this.Text = "新象拍賣系統" + Application.ProductVersion;
#elif SFJ
            Auction.DefaultBiddingCompany = BiddingCompany.SFJ;
            this.Text = "禪機拍賣系統" + Application.ProductVersion;
#elif DS
            Auction.DefaultBiddingCompany = BiddingCompany.DS;
            this.Text = "大行拍賣系統 " + Application.ProductVersion;
#elif G
            Auction.DefaultBiddingCompany = BiddingCompany.G;
            this.Text = "吉祥門拍賣系統 " + Application.ProductVersion;
            checkoutDealerButton.Visible = takebackDealerButton.Visible = true;
#else
            Auction.DefaultBiddingCompany = BiddingCompany.S;
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

        private void checkoutDealerButton_Click(object sender, EventArgs e)
        {
            mForm = new CheckoutDealer.CheckoutDealerForm();
            mForm.Show();
        }

        private void takebackDealerButton_Click(object sender, EventArgs e)
        {
            mForm = new TakeBackDealer.MainForm();
            mForm.Show();
        }
    }
}
