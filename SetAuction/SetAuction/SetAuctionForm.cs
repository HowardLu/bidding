using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Bidding;
using InternetLibrary;
using UtilityLibrary;

namespace SetAuction
{
    public partial class SetAuctionForm : Form
    {
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        #endregion

        #region Member Variables
        private Dictionary<string, Auction> m_auctions;
        private Size m_listViewSize;
        private int[] m_lvColWidths;
        private int m_sortColumnId = -1;
        private ImageList m_largeImgList = new ImageList();
        private ImageList m_smallImgList = new ImageList();
        private string m_addImgFP;
        private Internet<AuctionEntity> m_aeInternet;
        #endregion

        #region Properties
        #endregion

        #region Constructors and Finalizers
        public SetAuctionForm()
        {
            InitializeComponent();
            m_listViewSize = this.auctionsListView.Size;
            m_lvColWidths = new int[auctionsListView.Columns.Count];
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                m_lvColWidths[i] = auctionsListView.Columns[i].Width;
        }
        #endregion

        #region Windows Form Events
        private void SetAuctionForm_Load(object sender, EventArgs e)
        {
            string ip = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入Server IP", "127.0.0.1", -1, -1);
            if (ip.Length == 0)
            {
                MessageBox.Show("IP不可為空!!!");
                Application.Exit();
                return;
            }
            m_aeInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            List<Auction> auctions = null;
            try
            {
                Auction.LoadAuctions(ref auctions, ref m_aeInternet, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
            m_auctions = auctions.ToDictionary<Auction, string>(auc => auc.lot);
            LoadAuctionPhotos();
            InitAuctioneerComboBox();
        }

        private void SetAuctionForm_Resize(object sender, EventArgs e)
        {
            if (m_listViewSize.Width == 0 || m_listViewSize.Height == 0)
                return;
            float xRatio = (float)this.auctionsListView.Width / m_listViewSize.Width;
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                auctionsListView.Columns[i].Width = Convert.ToInt32(m_lvColWidths[i] * xRatio);
        }

        private void SetAuctionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void auctionsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != m_sortColumnId)
            {
                // Set the sort column to the new column.
                m_sortColumnId = e.Column;
                // Set the sort order to ascending by default.
                auctionsListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (auctionsListView.Sorting == SortOrder.Ascending)
                    auctionsListView.Sorting = SortOrder.Descending;
                else
                    auctionsListView.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            auctionsListView.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.auctionsListView.ListViewItemSorter = new ListViewItemComparer(e.Column, auctionsListView.Sorting);
        }

        private void auctionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (auctionsListView.SelectedItems.Count != 1 || auctionsListView.Items.Count == 0)
                return;

            ListViewItem lvi = auctionsListView.SelectedItems[0];
            lotTextBox.Text = lvi.Text;
            artistTextBox.Text = lvi.SubItems[1].Text;
            artworkTextBox.Text = lvi.SubItems[2].Text;
            initialPriceTextBox.Text = int.Parse(lvi.SubItems[3].Text, System.Globalization.NumberStyles.Currency).ToString();
            m_addImgFP = Path.Combine(Application.StartupPath, m_auctions[auctionsListView.SelectedItems[0].Text].photofilePath);
            photoTextBox.Text = Path.GetFileName(m_addImgFP);
            int index = Utility.ToEnumInt<Auctioneer>(lvi.SubItems[4].Text);
            auctioneerComboBox.SelectedIndex = index < 0 ? 0 : index;
        }

        private void lotTextBox_TextChanged(object sender, EventArgs e)
        {
            if (lotTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(lotTextBox.Text, false))
            {
                lotTextBox.Text = "";
            }
        }

        private void artistTextBox_TextChanged(object sender, EventArgs e)
        {
            if (artistTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(artistTextBox.Text, false))
            {
                artistTextBox.Text = "";
            }
        }

        private void artworkTextBox_TextChanged(object sender, EventArgs e)
        {
            if (artworkTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(artworkTextBox.Text, false))
            {
                artworkTextBox.Text = "";
            }
        }

        private void initialPriceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (initialPriceTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(initialPriceTextBox.Text, false) ||
                Utility.ParseToInt(initialPriceTextBox.Text, false) == -1)
            {
                initialPriceTextBox.Text = "";
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (lotTextBox.Text == "" || artistTextBox.Text == "" || artworkTextBox.Text == "" ||
                initialPriceTextBox.Text == "" || photoTextBox.Text == "")
            {
                MessageBox.Show("請輸入資料!!!");
                return;
            }
            if (m_auctions.ContainsKey(lotTextBox.Text) || m_aeInternet.FineOne<string>(ae => ae.AuctionId, lotTextBox.Text) != null)
            {
                MessageBox.Show("重複Lot，請重新輸入!");
                lotTextBox.Text = "";
                return;
            }
            if (int.Parse(initialPriceTextBox.Text) < 1000)
            {
                MessageBox.Show("起拍價小於1000，請重新輸入!");
                initialPriceTextBox.Text = "";
                return;
            }

            Auction auction = new Auction();
            auction.lot = lotTextBox.Text;
            auction.artist = artistTextBox.Text;
            auction.artwork = artworkTextBox.Text;
            auction.initialPrice = int.Parse(initialPriceTextBox.Text);
            auction.auctioneer = Utility.GetEnumString(typeof(Auctioneer), auctioneerComboBox.SelectedIndex);
            CopyPhotoToAuctionsFolder(ref auction);
            string fp = Path.Combine(Application.StartupPath, auction.photofilePath);
            auction.photo = Utility.OpenBitmap(fp);
            m_auctions.Add(auction.lot, auction);
            m_aeInternet.Insert(auction.ToAuctionEntity());

            m_largeImgList.Images.Add(Utility.SizeImage(ref auction.photo, 100, 100));
            m_smallImgList.Images.Add(Utility.SizeImage(ref auction.photo, 50, 50));
            auctionsListView.BeginUpdate();
            AddItemToListView(m_auctions.Count - 1, lotTextBox.Text, artistTextBox.Text, artworkTextBox.Text,
               int.Parse(initialPriceTextBox.Text, System.Globalization.NumberStyles.Currency).ToString("c"),
               Utility.GetEnumString(typeof(Auctioneer), auctioneerComboBox.SelectedIndex));
            auctionsListView.LargeImageList = m_largeImgList;
            auctionsListView.SmallImageList = m_smallImgList;
            auctionsListView.EndUpdate();
            ClearAllTextBox();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (auctionsListView.SelectedIndices.Count != 1 || lotTextBox.Text == "" ||
                initialPriceTextBox.Text == "")
                return;

            if (m_auctions.ContainsKey(lotTextBox.Text) && lotTextBox.Text != auctionsListView.SelectedItems[0].Text)
            {
                MessageBox.Show("重複Lot，請重新輸入!");
                lotTextBox.Text = "";
                return;
            }
            if (int.Parse(initialPriceTextBox.Text) < 1000)
            {
                MessageBox.Show("起拍價小於1000，請重新輸入!");
                initialPriceTextBox.Text = "";
                return;
            }
            string lot = auctionsListView.SelectedItems[0].Text;
            if (!m_auctions.ContainsKey(lot))
            {
                MessageBox.Show("沒有" + lot + "拍品!");
                return;
            }

            Auction auc = m_auctions[lot];
            int id = auctionsListView.SelectedIndices[0];
            auctionsListView.Items[id].Text = auc.lot = lotTextBox.Text;
            auctionsListView.Items[id].SubItems[1].Text = auc.artist = artistTextBox.Text;
            auctionsListView.Items[id].SubItems[2].Text = auc.artwork = artworkTextBox.Text;
            auc.initialPrice = int.Parse(initialPriceTextBox.Text);
            auctionsListView.Items[id].SubItems[3].Text = int.Parse(initialPriceTextBox.Text,
                System.Globalization.NumberStyles.Currency).ToString("c");
            auctionsListView.Items[id].SubItems[4].Text = auc.auctioneer = Utility.GetEnumString(typeof(Auctioneer), auctioneerComboBox.SelectedIndex);
            CopyPhotoToAuctionsFolder(ref auc);
            m_auctions.Remove(lot);
            m_auctions[lotTextBox.Text] = auc;
            
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.Artist, auc.artist);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.Artwork, auc.artwork);
            m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, lot, ae => ae.InitialPrice, auc.initialPrice);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.Auctioneer, auc.auctioneer);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.AuctionId, auc.lot);

            photoTextBox.Text = Path.GetFileName(auc.photofilePath);
            ClearAllTextBox();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in auctionsListView.SelectedItems)
            {
                string lot = lvi.Text;
                /*if (File.Exists(m_auctions[lot].photofilePath))
                    File.Delete(m_auctions[lot].photofilePath);*/
                m_auctions.Remove(lot);
                m_aeInternet.Remove<string>(ae => ae.AuctionId, lot);
                auctionsListView.Items.Remove(lvi);
            }
        }

        private void openPhotoButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.DefaultExt = "jpg";
            openFileDialog1.Filter = "JPEG(*.jpg)|*.jpg|點陣圖檔案(*.bmp)|*.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                photoTextBox.Text = Path.GetFileName(openFileDialog1.FileName);
                m_addImgFP = openFileDialog1.FileName;
            }
            else
            {
                photoTextBox.Text = "";
            }
        }

        private void largeIconRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = View.LargeIcon;
        }

        private void detailsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = View.Details;
        }

        private void smallIconRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = View.SmallIcon;
        }

        private void listRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = View.List;
        }

        private void tileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = View.Tile;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void LoadAuctionPhotos()
        {
            string[] filePaths = Directory.GetFiles(Settings.auctionFolder).OrderBy(f => f).ToArray<string>();
            m_largeImgList.ImageSize = new Size(100, 100);
            m_smallImgList.ImageSize = new Size(10, 10);

            auctionsListView.Items.Clear();
            auctionsListView.BeginUpdate();
            foreach (Auction auction in m_auctions.Values)
            {
                //m_largeImgList.Images.Add(Utility.OpenBitmap(auction.photofilePath).GetThumbnailImage(100, 100, null, new IntPtr()));
                //m_smallImgList.Images.Add(Utility.OpenBitmap(auction.photofilePath).GetThumbnailImage(50, 50, null, new IntPtr()));
                if (auction.photo == null)
                {
                    auction.photo = Utility.OpenBitmap(auction.photofilePath);
                }
                m_largeImgList.Images.Add(Utility.SizeImage(ref auction.photo, 100, 100));
                m_smallImgList.Images.Add(Utility.SizeImage(ref auction.photo, 50, 50));
                AddItemToListView(m_largeImgList.Images.Count - 1, auction.lot, auction.artist, auction.artwork,
                    auction.initialPrice.ToString("c"), auction.auctioneer);
            }
            auctionsListView.LargeImageList = m_largeImgList;
            auctionsListView.SmallImageList = m_smallImgList;
            auctionsListView.EndUpdate();
        }

        private void AddItemToListView(int imgId, string text, string subItem1, string subItem2, string subItem3, string subItem4)
        {
            ListViewItem newLvi = new ListViewItem();
            newLvi.ImageIndex = imgId;
            newLvi.Text = text;
            newLvi.SubItems.Add(subItem1);
            newLvi.SubItems.Add(subItem2);
            newLvi.SubItems.Add(subItem3);
            newLvi.SubItems.Add(subItem4);
            auctionsListView.Items.Add(newLvi);
        }

        private void ClearAllTextBox()
        {
            lotTextBox.Text = artistTextBox.Text = artworkTextBox.Text = initialPriceTextBox.Text =
                photoTextBox.Text = "";
        }

        private void CopyPhotoToAuctionsFolder(ref Auction auction)
        {
            //string newFileName = auction.CreateFileName(Path.GetExtension(photoTextBox.Text));
            string newFileName = auction.lot + Path.GetExtension(photoTextBox.Text);
            auction.photofilePath = Path.Combine(Settings.auctionFolder, newFileName);
            string newFilePath = Path.Combine(Application.StartupPath, auction.photofilePath);
            if (File.Exists(newFilePath))
                return;

            if (File.Exists(m_addImgFP))
            {
                File.Copy(m_addImgFP, newFilePath, true);
            }
            else
            {
                photoTextBox.Text = "";
                MessageBox.Show(m_addImgFP + "圖片來源不存在! ");
            }
        }

        private void InitAuctioneerComboBox()
        {
            for (int i = 0; i < (int)AuctioneerName.Count; i++)
            {
                auctioneerComboBox.Items.Add(Utility.GetEnumString(typeof(AuctioneerName), i));
            }
        }
        #endregion
    }
}
