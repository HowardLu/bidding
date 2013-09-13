using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace SJ_Bidding_System
{
    public partial class SetAuctionForm : Form
    {
        private Dictionary<string, Auction> m_auctions;
        private Size m_listViewSize;
        private int[] m_lvColWidths;
        private int m_sortColumnId = -1;
        private ImageList m_largeImgList = new ImageList();
        private ImageList m_smallImgList = new ImageList();
        private string m_addImgFP;

        public SetAuctionForm()
        {
            InitializeComponent();
            m_listViewSize = this.auctionsListView.Size;
            m_lvColWidths = new int[auctionsListView.Columns.Count];
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                m_lvColWidths[i] = auctionsListView.Columns[i].Width;
        }

        #region Windows Form event handler
        private void SetAuctionForm_Load(object sender, EventArgs e)
        {
            LoadAuctions();
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
        }

        private void lotTextBox_TextChanged(object sender, EventArgs e)
        {
            if (lotTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(lotTextBox.Text) || Utility.IsNumber(lotTextBox.Text) == -1)
            {
                lotTextBox.Text = "";
            }
        }

        private void artistTextBox_TextChanged(object sender, EventArgs e)
        {
            if (artistTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(artistTextBox.Text))
            {
                artistTextBox.Text = "";
            }
        }

        private void artworkTextBox_TextChanged(object sender, EventArgs e)
        {
            if (artworkTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(artworkTextBox.Text))
            {
                artworkTextBox.Text = "";
            }
        }

        private void initialPriceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (initialPriceTextBox.Text.Length == 0)
                return;

            if (!Utility.IsValidFileName(initialPriceTextBox.Text) ||
                Utility.IsNumber(initialPriceTextBox.Text) == -1)
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
            if (m_auctions.ContainsKey(lotTextBox.Text))
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
            string newFileName = auction.CreateFileName(Path.GetExtension(photoTextBox.Text));
            auction.photofilePath = Path.Combine(Settings.auctionFolder, newFileName);
            if (File.Exists(m_addImgFP))
            {
                File.Copy(m_addImgFP, Path.Combine(Application.StartupPath, auction.photofilePath), true);
            }
            else
            {
                photoTextBox.Text = "";
                MessageBox.Show(m_addImgFP + "圖片來源不存在! ");
                return;
            }
            m_auctions.Add(auction.lot, auction);

            string fp = Path.Combine(Application.StartupPath, auction.photofilePath);
            m_largeImgList.Images.Add(Utility.OpenBitmap(fp).GetThumbnailImage(100, 100, null, new IntPtr()));
            m_smallImgList.Images.Add(Utility.OpenBitmap(fp).GetThumbnailImage(50, 50, null, new IntPtr()));
            auctionsListView.BeginUpdate();
            AddItemToListView(m_auctions.Count - 1, lotTextBox.Text, artistTextBox.Text, artworkTextBox.Text,
               int.Parse(initialPriceTextBox.Text, System.Globalization.NumberStyles.Currency).ToString("c"));
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
            auc.lot = lotTextBox.Text;
            auc.artist = artistTextBox.Text;
            auc.artwork = artworkTextBox.Text;
            auc.initialPrice = int.Parse(initialPriceTextBox.Text);
            string newFileName = Path.Combine(Path.GetDirectoryName(auc.photofilePath),
                auc.CreateFileName(Path.GetExtension(auc.photofilePath)));
            if (m_addImgFP == Path.Combine(Application.StartupPath, newFileName))   // didn't change anything, just press save.
                return;

            try
            {
                if (File.Exists(m_addImgFP))
                {
                    File.Copy(m_addImgFP, Path.Combine(Application.StartupPath, newFileName), true);
                    File.Delete(m_addImgFP);
                    auc.photofilePath = newFileName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            m_auctions.Remove(lot);
            m_auctions[lotTextBox.Text] = auc;

            int id = auctionsListView.SelectedIndices[0];
            auctionsListView.Items[id].Text = lotTextBox.Text;
            auctionsListView.Items[id].SubItems[1].Text = artistTextBox.Text;
            auctionsListView.Items[id].SubItems[2].Text = artworkTextBox.Text;
            auctionsListView.Items[id].SubItems[3].Text = int.Parse(initialPriceTextBox.Text,
                System.Globalization.NumberStyles.Currency).ToString("c");
            photoTextBox.Text = Path.GetFileName(auc.photofilePath);
            ClearAllTextBox();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in auctionsListView.SelectedItems)
            {
                if (File.Exists(m_auctions[lvi.Text].photofilePath))
                    File.Delete(m_auctions[lvi.Text].photofilePath);
                m_auctions.Remove(lvi.Text);
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

        #region Methods
        private void LoadAuctions()
        {
            m_auctions = new Dictionary<string, Auction>();
            string[] filePaths = Directory.GetFiles(Settings.auctionFolder).OrderBy(f => f).ToArray<string>();
            m_largeImgList.ImageSize = new Size(100, 100);
            m_smallImgList.ImageSize = new Size(10, 10);

            auctionsListView.Items.Clear();
            auctionsListView.BeginUpdate();
            for (int i = 0; i < filePaths.Length; i++)
            {
                Auction auction = new Auction();
                if (auction.GetInfoFromFileName(filePaths[i]))
                {
                    auction.photofilePath = filePaths[i];
                    m_largeImgList.Images.Add(Utility.OpenBitmap(filePaths[i]).GetThumbnailImage(100, 100, null, new IntPtr()));
                    m_smallImgList.Images.Add(Utility.OpenBitmap(filePaths[i]).GetThumbnailImage(50, 50, null, new IntPtr()));
                    if (!m_auctions.ContainsKey(auction.lot))
                    {
                        m_auctions.Add(auction.lot, auction);
                    }
                    else
                    {
                        MessageBox.Show("重複Lot: " + auction.photofilePath + " 請檢查Auctions!");
                        this.Close();
                    }
                    AddItemToListView(m_auctions.Count - 1, auction.lot, auction.artist, auction.artwork,
                        auction.initialPrice.ToString("n0"));
                }
            }
            auctionsListView.LargeImageList = m_largeImgList;
            auctionsListView.SmallImageList = m_smallImgList;
            auctionsListView.EndUpdate();
        }

        private void AddItemToListView(int imgId, string text, string subItem1, string subItem2, string subItem3)
        {
            ListViewItem newLvi = new ListViewItem();
            newLvi.ImageIndex = imgId;
            newLvi.Text = text;
            newLvi.SubItems.Add(subItem1);
            newLvi.SubItems.Add(subItem2);
            newLvi.SubItems.Add(subItem3);
            auctionsListView.Items.Add(newLvi);
        }

        private void ClearAllTextBox()
        {
            lotTextBox.Text = artistTextBox.Text = artworkTextBox.Text = initialPriceTextBox.Text =
                photoTextBox.Text = "";
        }
        #endregion
    }
}
