using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BiddingLibrary;
using InternetLibrary;
using UtilityLibrary;
using Microsoft.Office.Interop.Word;

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
        private int m_lastUnitIndex = 0;
        private int[] m_units = { 1, 1000, 10000 };
        private int m_initPriceThreshold = 1000;
        private string m_sessionIdNow = "";
        private List<string> m_sessions;
        private Object m_oMissing = System.Reflection.Missing.Value;
        private string m_saveFolder = "";
        private string m_ip;
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

            string ip = Utility.InputIp();
            if (ip.Length == 0)
            {
                MessageBox.Show("IP不可為空!!!");
                System.Windows.Forms.Application.Exit();
                return;
            }
            m_aeInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_ip = ip;
            if (BiddingCompany.N == Auction.DefaultBiddingCompany)
                m_initPriceThreshold = 0;
            m_saveFolder = Path.Combine(System.Windows.Forms.Application.StartupPath, Settings.saveFolder);
        }
        #endregion

        #region Windows Form Events
        private void SetAuctionForm_Load(object sender, EventArgs e)
        {
            if (!Utility.IsDirectoryExist(Settings.auctionFolder, false))
            {
                Utility.CreateDirectory(Settings.auctionFolder);
            }

            InitSessionComboBox();
            if (BiddingCompany.G == Auction.DefaultBiddingCompany)
            {
                Settings.Load();
                ExchangeRate.Load(Settings.exchangeRateFP);
                unitComboBox.SelectedIndex = 1;
            }
            else
            {
                unitComboBox.SelectedIndex = 2;
                exportDataForAuctioneerButton.Visible = exportAuctionInfoButton.Visible =
                    exportClerkTableButton.Visible = false;
            }
        }

        private void SetAuctionForm_Resize(object sender, EventArgs e)
        {
            if (m_listViewSize.Width == 0 || m_listViewSize.Height == 0)
                return;
            float xRatio = (float)this.auctionsListView.Width / m_listViewSize.Width;
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                auctionsListView.Columns[i].Width = Convert.ToInt32(m_lvColWidths[i] * xRatio);
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
            initialPriceTextBox.Text = lvi.SubItems[3].Text;
            m_addImgFP = Path.Combine(System.Windows.Forms.Application.StartupPath, m_auctions[auctionsListView.SelectedItems[0].Text].photoFilePath);
            photoTextBox.Text = Path.GetFileName(m_addImgFP);
            //int index = Utility.ToEnumInt<Auctioneer>(lvi.SubItems[4].Text);
            //auctioneerComboBox.SelectedIndex = index < 0 ? 0 : index;
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

            int initPrice = int.Parse(initialPriceTextBox.Text) * m_units[unitComboBox.SelectedIndex];
            if (initPrice < m_initPriceThreshold)
            {
                MessageBox.Show(String.Format("起拍價小於{0}，請重新輸入!", m_initPriceThreshold));
                initialPriceTextBox.Text = "";
                return;
            }

            Auction auction = new Auction();
            auction.lot = lotTextBox.Text;
            auction.artist = artistTextBox.Text;
            auction.artwork = artworkTextBox.Text;
            auction.initialPrice = initPrice;
            //auction.auctioneer = Utility.GetEnumString(typeof(Auctioneer), auctioneerComboBox.SelectedIndex);
            CopyPhotoToAuctionsFolder(ref auction);
            string fp = Path.Combine(System.Windows.Forms.Application.StartupPath, auction.photoFilePath);
            m_auctions.Add(auction.lot, auction);
            m_aeInternet.Insert(auction.ToAuctionEntity());

            Bitmap bmp;
            Utility.OpenBitmap(fp, out bmp);
            Bitmap largeBmp, smallBmp;
            Utility.SizeImage(ref bmp, out largeBmp, 100, 100);
            Utility.SizeImage(ref largeBmp, out smallBmp, 50, 50);
            m_largeImgList.Images.Add(largeBmp);
            m_smallImgList.Images.Add(smallBmp);
            bmp.Dispose();

            auctionsListView.BeginUpdate();
            AddItemToListView(m_auctions.Count - 1, lotTextBox.Text, artistTextBox.Text, artworkTextBox.Text,
               initialPriceTextBox.Text/*,
                Utility.GetEnumString(typeof(Auctioneer), auctioneerComboBox.SelectedIndex)*/);
            //auctionsListView.LargeImageList = m_largeImgList;
            //auctionsListView.SmallImageList = m_smallImgList;
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

            int initPrice = int.Parse(initialPriceTextBox.Text) * m_units[unitComboBox.SelectedIndex];
            if (initPrice < m_initPriceThreshold)
            {
                MessageBox.Show(String.Format("起拍價小於{0}，請重新輸入!", m_initPriceThreshold));
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
            if (auc.lot != lotTextBox.Text && null != m_aeInternet.FineOne<string>(ae => ae.AuctionId, lotTextBox.Text))
            {
                MessageBox.Show("已有拍品" + lotTextBox.Text + "，請重新輸入!");
                lotTextBox.Text = auc.lot;
                return;
            }
            auctionsListView.Items[id].Text = auc.lot = lotTextBox.Text;
            auctionsListView.Items[id].SubItems[1].Text = auc.artist = artistTextBox.Text;
            auctionsListView.Items[id].SubItems[2].Text = auc.artwork = artworkTextBox.Text;
            auc.initialPrice = initPrice;
            auctionsListView.Items[id].SubItems[3].Text = initialPriceTextBox.Text;
            //auctionsListView.Items[id].SubItems[4].Text = auc.auctioneer = Utility.GetEnumString(typeof(Auctioneer), auctioneerComboBox.SelectedIndex);
            CopyPhotoToAuctionsFolder(ref auc);
            m_auctions.Remove(lot);
            m_auctions[lotTextBox.Text] = auc;

            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.Artist, auc.artist);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.Artwork, auc.artwork);
            m_aeInternet.UpdateField<string, int>(ae => ae.AuctionId, lot, ae => ae.InitialPrice, auc.initialPrice);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.Auctioneer, auc.auctioneer);
            m_aeInternet.UpdateField<string, string>(ae => ae.AuctionId, lot, ae => ae.AuctionId, auc.lot);

            photoTextBox.Text = Path.GetFileName(auc.photoFilePath);
            ClearAllTextBox();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (auctionsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("請選擇欲刪除的拍品");
                return;
            }

            if (MessageBox.Show("確定刪除?", "警告", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                foreach (ListViewItem lvi in auctionsListView.SelectedItems)
                {
                    string lot = lvi.Text;
                    m_auctions.Remove(lot);
                    m_aeInternet.Remove<string>(ae => ae.AuctionId, lot);
                    auctionsListView.Items.Remove(lvi);
                }
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
            auctionsListView.View = System.Windows.Forms.View.LargeIcon;
        }

        private void detailsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = System.Windows.Forms.View.Details;
        }

        private void smallIconRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = System.Windows.Forms.View.SmallIcon;
        }

        private void listRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = System.Windows.Forms.View.List;
        }

        private void tileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            auctionsListView.View = System.Windows.Forms.View.Tile;
        }

        private void unitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            float scale = 1 / (float)m_units[unitComboBox.SelectedIndex];
            foreach (ListViewItem item in auctionsListView.Items)
            {
                string lot = item.Text;
                int initPrice = m_auctions[lot].initialPrice;
                float scalePrice = initPrice * scale;
                string scalePriceStr = scalePrice.ToString("0.#");
                item.SubItems[3].Text = scalePriceStr;
            }
            m_lastUnitIndex = unitComboBox.SelectedIndex;
        }

        private void sessionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sessionSelected = sessionComboBox.SelectedItem.ToString();
            LoadAuctionsFromSession(sessionSelected);  // load auctions from session.
            LoadAuctionToListView();
        }

        private void exportDataForAuctioneerbutton_Click(object sender, EventArgs e)
        {
            string auctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
            Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" +
                    Settings.docTempFolder + @"\DataForAuctioneerTemp_" + auctioneer + ".dot";
            string docName = "拍賣官翻閱用資料.doc";
            Microsoft.Office.Interop.Word._Application wordApp = new Microsoft.Office.Interop.Word.Application(); ;
            Microsoft.Office.Interop.Word._Document doc = wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing,
                                                                                        ref m_oMissing, ref m_oMissing);
            // duplicate first page to create the pages we need for auctions
            object startPoint = 0;
            object oEoF = WdUnits.wdStory;
            Range pageRange = doc.Range(ref startPoint, ref m_oMissing);
            Range rng = pageRange;
            int pageCount = m_auctions.Count / 2 + m_auctions.Count % 2;
            pageRange.Copy();
            for (int i = 0; i < pageCount - 1; i++)
            {
                rng.SetRange(pageRange.End + 1, pageRange.End + 1);
                pageRange.Paste();
            }
            wordApp.Selection.EndKey(ref oEoF, ref m_oMissing);
            wordApp.Selection.TypeBackspace();  // delete last empty line

            // Fill aution data in tables
            List<Auction> auctions = m_auctions.Values.ToList<Auction>();
            for (int i = 0; i < auctions.Count; i++)
            {
                Microsoft.Office.Interop.Word.Table table = doc.Tables[i + 1];

                table.Cell(1, 1).Range.Text = auctions[i].lot;
                table.Cell(2, 1).Range.Text = Utility.CutStringByLength(auctions[i].artist, 8);
                table.Cell(3, 1).Range.Text = Utility.CutStringByLength(auctions[i].artwork, 8);
                table.Cell(4, 1).Range.Text = "起拍價：" + auctions[i].initialPrice;
            }

            if (!Directory.Exists(m_saveFolder))
            {
                Directory.CreateDirectory(m_saveFolder);
            }
            string filePath = Path.Combine(m_saveFolder, docName);
            if (File.Exists(filePath))
            {
                if (DialogResult.OK == MessageBox.Show("是否覆蓋目前拍賣官翻閱資料", "", MessageBoxButtons.OKCancel))
                {
                    File.Delete(filePath);
                }
            }
            if (!doc.Saved)
            {
                doc.SaveAs(filePath);
                MessageBox.Show("已儲存在Save資料夾", "", MessageBoxButtons.OK);
            }

            CloseDoc(ref doc);
            CloseWordApp(ref wordApp);
        }

        private void exportAuctionInfoButton_Click(object sender, EventArgs e)
        {
            if (0 == ExchangeRate.mainToExchangeRate.Length)
            {
                MessageBox.Show("請至拍賣跳接設定匯率，\n第一個匯率為輸出匯率。", "", MessageBoxButtons.OK);
                return;
            }
            string warning = String.Format("是否以第一個匯率{0}:{1}轉換?", ExchangeRate.rateNames[0],
                ExchangeRate.mainToExchangeRate[0]);
            if (DialogResult.OK != MessageBox.Show(warning, "", MessageBoxButtons.OKCancel))
            {
                return;
            }

            Internet<DealerItemEntity> dealerItemInternet = new Internet<DealerItemEntity>(m_ip, "bidding_data", "dealer_item_table");
            string auctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
            Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" +
                    Settings.docTempFolder + @"\AuctionInfoTemp_" + auctioneer + ".dot";
            string docName = "作品小字卡.doc";
            Microsoft.Office.Interop.Word._Application wordApp = new Microsoft.Office.Interop.Word.Application(); ;
            Microsoft.Office.Interop.Word._Document doc = wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing,
                                                                                        ref m_oMissing, ref m_oMissing);
            // duplicate first page to create the pages we need for auctions
            object startPoint = 0;
            object oEOF = WdUnits.wdStory;
            Range pageRange = doc.Range(ref startPoint, ref m_oMissing);
            Range rng = pageRange;
            bool isNeedOneMorePage = 0 != (m_auctions.Count % 6);
            int pageCount = m_auctions.Count / 6;    // 6 items per page
            if (isNeedOneMorePage)
                pageCount++;
            pageRange.Copy();
            for (int i = 0; i < pageCount - 1; i++)
            {
                rng.SetRange(pageRange.End + 2, pageRange.End + 2);
                pageRange.Paste();
            }
            wordApp.Selection.EndKey(ref oEOF, ref m_oMissing);
            wordApp.Selection.TypeBackspace();  // delete last empty line

            // Fill aution data in tables
            List<Auction> auctions = m_auctions.Values.ToList<Auction>();
            const int itemPerPage = 6;
            for (int i = 0; i < auctions.Count; i++)
            {
                int tableIndex = i / itemPerPage + 1;
                Microsoft.Office.Interop.Word.Table table = doc.Tables[tableIndex];
                int row = (i - (tableIndex - 1) * itemPerPage) / 2 * 12;

                DealerItemEntity dealerItem = dealerItemInternet.FineOne((di => di.LotNO), auctions[i].lot);
                table.Cell(row + 1, 1).Range.Text = "Lot " + auctions[i].lot;
                table.Cell(row + 2, 1).Range.Text = auctions[i].artwork;
                table.Cell(row + 3, 1).Range.Text = auctions[i].artist;
                if (null != dealerItem)
                {
                    table.Cell(row + 5, 1).Range.Text = dealerItem.ItemPS;
                    table.Cell(row + 6, 1).Range.Text = dealerItem.Spec + "cm";
                }
                table.Cell(row + 8, 1).Range.Text = "NT$：" + auctions[i].initialPrice.ToString("n0");
                table.Cell(row + 9, 1).Range.Text = ExchangeRate.rateNames[0] + " : " +
                    ExchangeRate.MainToCurrency(auctions[i].initialPrice, 0).ToString("n0");
                i++;
                if (auctions.Count <= i)
                    break;
                dealerItem = null;
                dealerItem = dealerItemInternet.FineOne((di => di.LotNO), auctions[i].lot);
                table.Cell(row + 1, 2).Range.Text = "Lot " + auctions[i].lot;
                table.Cell(row + 2, 2).Range.Text = auctions[i].artwork;
                table.Cell(row + 3, 2).Range.Text = auctions[i].artist;
                if (null != dealerItem)
                {
                    table.Cell(row + 5, 2).Range.Text = dealerItem.ItemPS;
                    table.Cell(row + 6, 2).Range.Text = dealerItem.Spec + "cm";
                }
                table.Cell(row + 8, 2).Range.Text = "NT$：" + auctions[i].initialPrice.ToString("n0");
                table.Cell(row + 9, 2).Range.Text = ExchangeRate.rateNames[0] + " : " +
                    ExchangeRate.MainToCurrency(auctions[i].initialPrice, 0).ToString("n0");
            }

            if (!Directory.Exists(m_saveFolder))
            {
                Directory.CreateDirectory(m_saveFolder);
            }
            string filePath = Path.Combine(m_saveFolder, docName);
            if (File.Exists(filePath))
            {
                if (DialogResult.OK == MessageBox.Show("是否覆蓋目前作品小字卡", "", MessageBoxButtons.OKCancel))
                {
                    File.Delete(filePath);
                }
            }
            if (!doc.Saved)
            {
                doc.SaveAs(filePath);
                MessageBox.Show("已儲存在Save資料夾", "", MessageBoxButtons.OK);
            }

            CloseDoc(ref doc);
            CloseWordApp(ref wordApp);
        }

        private void exportClerkTablebutton_Click(object sender, EventArgs e)
        {
            string auctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
            Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" +
                    Settings.docTempFolder + @"\ClerkTableTemp_" + auctioneer + ".dot";
            string docName = "書記官登記表.doc";
            Microsoft.Office.Interop.Word._Application wordApp = new Microsoft.Office.Interop.Word.Application(); ;
            Microsoft.Office.Interop.Word._Document doc = wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing,
                                                                                        ref m_oMissing, ref m_oMissing);
            // duplicate first page to create the pages we need for auctions
            object startPoint = 0;
            object oEoF = WdUnits.wdStory;
            Range pageRange = doc.Range(ref startPoint, ref m_oMissing);
            Range rng = pageRange;
            int pageCount = m_auctions.Count / 2 + m_auctions.Count % 2;
            pageRange.Copy();
            for (int i = 0; i < pageCount - 1; i++)
            {
                rng.SetRange(pageRange.End + 1, pageRange.End + 1);
                pageRange.Paste();
            }
            wordApp.Selection.EndKey(ref oEoF, ref m_oMissing);
            wordApp.Selection.TypeBackspace();  // delete last empty line

            // Fill aution data in tables
            List<Auction> auctions = m_auctions.Values.ToList<Auction>();
            for (int i = 0; i < auctions.Count; i++)
            {
                Microsoft.Office.Interop.Word.Table table = doc.Tables[i + 1];

                table.Cell(2, 1).Range.Text = auctions[i].lot;
                table.Cell(2, 2).Range.Text = Utility.CutStringByLength(auctions[i].artist, 8);
                table.Cell(2, 3).Range.Text = Utility.CutStringByLength(auctions[i].artwork, 8);
            }

            if (!Directory.Exists(m_saveFolder))
            {
                Directory.CreateDirectory(m_saveFolder);
            }
            string filePath = Path.Combine(m_saveFolder, docName);
            if (File.Exists(filePath))
            {
                if (DialogResult.OK == MessageBox.Show("是否覆蓋目前拍賣官翻閱資料", "", MessageBoxButtons.OKCancel))
                {
                    File.Delete(filePath);
                }
            }
            if (!doc.Saved)
            {
                doc.SaveAs(filePath);
                MessageBox.Show("已儲存在Save資料夾", "", MessageBoxButtons.OK);
            }

            CloseDoc(ref doc);
            CloseWordApp(ref wordApp);
        }

        private void SetAuctionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_auctions != null)
            {
                m_auctions.Clear();
                m_auctions = null;
            }
            if (null != m_largeImgList)
                m_largeImgList.Dispose();
            if (null != m_smallImgList)
                m_smallImgList.Dispose();
            if (null != m_aeInternet)
                m_aeInternet = null;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void LoadAuctionToListView()
        {
            bool isCached = false;
            if (!Utility.IsDirectoryExist(Settings.cachedFoler, true))
            {
                Utility.CreateDirectory(Settings.cachedFoler);
            }
            else
            {
                isCached = true;
            }

            m_largeImgList.ImageSize = new Size(100, 100);
            m_smallImgList.ImageSize = new Size(10, 10);

            auctionsListView.Items.Clear();
            auctionsListView.BeginUpdate();
            auctionsListView.LargeImageList = m_largeImgList;
            auctionsListView.SmallImageList = m_smallImgList;
            auctionsListView.EndUpdate();
            foreach (Auction auction in m_auctions.Values)
            {
                Bitmap largeBmp, smallBmp;
                string filename = Path.GetFileName(auction.photoFilePath);
                string largeFileName = Path.Combine(Settings.cachedFoler, "100X_" + filename);
                string smallFileName = Path.Combine(Settings.cachedFoler, "10X_" + filename);
                if (isCached)
                {
                    Utility.OpenBitmap(largeFileName, out largeBmp);
                    Utility.OpenBitmap(smallFileName, out smallBmp);
                }
                else
                {
                    Bitmap bmp;
                    Utility.OpenBitmap(auction.photoFilePath, out bmp);
                    Utility.SizeImage(ref bmp, out largeBmp, 100, 100);
                    Utility.SizeImage(ref largeBmp, out smallBmp, 10, 10);
                    largeBmp.Save(largeFileName);
                    smallBmp.Save(smallFileName);
                    bmp.Dispose();
                }
                m_largeImgList.Images.Add(largeBmp);
                m_smallImgList.Images.Add(smallBmp);
                AddItemToListView(m_largeImgList.Images.Count - 1, auction.lot, auction.artist, auction.artwork,
                    auction.initialPrice.ToString()/*, auction.auctioneer*/);
            }
        }

        private void AddItemToListView(int imgId, string lot, string artist, string artwork, string initPrice/*, string subItem4*/)
        {
            ListViewItem newLvi = new ListViewItem();
            newLvi.ImageIndex = imgId;
            newLvi.Text = lot;
            newLvi.SubItems.Add(artist);
            newLvi.SubItems.Add(artwork);
            newLvi.SubItems.Add(initPrice);
            auctionsListView.Items.Add(newLvi);
        }

        private void ClearAllTextBox()
        {
            lotTextBox.Text = artistTextBox.Text = artworkTextBox.Text = initialPriceTextBox.Text =
                photoTextBox.Text = "";
        }

        private void CopyPhotoToAuctionsFolder(ref Auction auction)
        {
            string newFileName = auction.lot + Path.GetExtension(photoTextBox.Text);
            auction.photoFilePath = Path.Combine(Settings.auctionFolder, m_sessionIdNow, newFileName);
            string targetFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, auction.photoFilePath);
            if (File.Exists(targetFilePath))
            {
                if (DialogResult.Cancel == MessageBox.Show(String.Format("{0}\n已存在，是否覆蓋?", targetFilePath),
                                                            "", MessageBoxButtons.OKCancel))
                {
                    return;
                }
            }

            if (File.Exists(m_addImgFP))
            {
                if (m_addImgFP != targetFilePath)
                    File.Copy(m_addImgFP, targetFilePath, true);
                else
                    return;
            }
            else
            {
                photoTextBox.Text = "";
                MessageBox.Show(m_addImgFP + "圖片來源不存在! ");
            }

            // Save cached image
            if (!Utility.IsDirectoryExist(Settings.cachedFoler, true))
            {
                Utility.CreateDirectory(Settings.cachedFoler);
            }
            Bitmap largeBmp, smallBmp;
            string largeFileName = Path.Combine(Settings.cachedFoler, "100X_" + newFileName);
            string smallFileName = Path.Combine(Settings.cachedFoler, "10X_" + newFileName);
            Bitmap bmp;
            Utility.OpenBitmap(m_addImgFP, out bmp);
            Utility.SizeImage(ref bmp, out largeBmp, 100, 100);
            Utility.SizeImage(ref largeBmp, out smallBmp, 10, 10);
            largeBmp.Save(largeFileName);
            smallBmp.Save(smallFileName);
        }

        private void InitAuctioneerComboBox()
        {
            for (int i = 0; i < (int)BiddingCompanyName.Count; i++)
            {
                auctioneerComboBox.Items.Add(Utility.GetEnumString(typeof(BiddingCompanyName), i));
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

        private void LoadAuctionsFromSession(string sessionId)
        {
            if (m_auctions != null)
                m_auctions.Clear();
            List<Auction> auctions = null;
            try
            {
                Auction.LoadAuctions(sessionId, out auctions, ref m_aeInternet, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                System.Windows.Forms.Application.Exit();
            }
            m_auctions = auctions.ToDictionary<Auction, string>(auc => auc.lot);

            if (0 == m_auctions.Count)
            {
                MessageBox.Show(String.Format("請在Auctions/場次 {0} 儲存拍品圖片", sessionId.ToString()), "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            m_sessionIdNow = sessionId;
        }

        private void CloseDoc(ref Microsoft.Office.Interop.Word._Document doc)
        {
            object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            doc.Close(ref saveOption, ref m_oMissing, ref m_oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
            doc = null;
        }

        private void CloseWordApp(ref Microsoft.Office.Interop.Word._Application wordApp)
        {
            if (wordApp != null)
            {
                if (wordApp.NormalTemplate != null)
                    wordApp.NormalTemplate.Saved = true;
                wordApp.Quit(ref m_oMissing, ref m_oMissing, ref m_oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                wordApp = null;
            }
        }
        #endregion
    }
}
