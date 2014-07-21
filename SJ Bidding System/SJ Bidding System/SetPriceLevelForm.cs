using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bidding;
using UtilityLibrary;

namespace SJ_Bidding_System
{
    public partial class SetPriceLevelForm : Form
    {
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        #endregion

        #region Member Variables
        #endregion

        #region Properties
        #endregion

        #region Constructors and Finalizers
        public SetPriceLevelForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Windows Form Events
        private void SetPriceLevelForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < PriceLevels.levels.Count; i++)
            {
                PriceLevel pl = PriceLevels.levels[i];

                ListViewItem newLvi = new ListViewItem();
                newLvi.Text = pl.down.ToString("c");
                newLvi.SubItems.Add(pl.up.ToString("c"));
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < pl.increments.Count; j++)
                {
                    sb.Append(pl.increments[j].ToString("c"));
                    if (j != pl.increments.Count - 1)
                        sb.Append("  ");
                }
                newLvi.SubItems.Add(sb.ToString());
                priceLevelListView.Items.Add(newLvi);
            }
        }

        private void priceLevelListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (priceLevelListView.SelectedItems.Count == 0)
            {
                downTextBox.Text = upTextBox.Text = levelTextBox.Text = "";
                return;
            }

            if (priceLevelListView.SelectedItems.Count != 1 || priceLevelListView.Items.Count == 0)
                return;

            ListViewItem lvi = priceLevelListView.SelectedItems[0];
            downTextBox.Text = lvi.Text;
            upTextBox.Text = lvi.SubItems[1].Text;
            levelTextBox.Text = lvi.SubItems[2].Text;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string downStr = downTextBox.Text;
            string upStr = upTextBox.Text;
            string lvStr = levelTextBox.Text;
            if (downStr == "" || upStr == "" || lvStr == "")
            {
                MessageBox.Show("請輸入資料!");
                return;
            }

            int down = Utility.ParseCurrencyToInt(downStr, true);
            int up = Utility.ParseCurrencyToInt(upStr, true);
            if (GetPriceLevel(down, up) != -1)
            {
                MessageBox.Show("請使用儲存!");
                return;
            }

            PriceLevel pl = new PriceLevel(down, up);
            string[] lvs = lvStr.Split(' ');
            for (int i = 0; i < lvs.Length; i++)
            {
                pl.increments.Add(Utility.ParseCurrencyToInt(lvs[i], true));
            }
            PriceLevels.levels.Add(pl);

            ListViewItem lvi = new ListViewItem();
            lvi.Text = downStr;
            lvi.SubItems.Add(upStr);
            lvi.SubItems.Add(lvStr);
            priceLevelListView.Items.Add(lvi);
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (priceLevelListView.SelectedItems.Count != 1)
                return;

            string downStr = downTextBox.Text;
            string upStr = upTextBox.Text;
            string lvStr = levelTextBox.Text;

            ListViewItem lvi = priceLevelListView.SelectedItems[0];
            int down = Utility.ParseCurrencyToInt(lvi.Text, true);
            int up = Utility.ParseCurrencyToInt(lvi.SubItems[1].Text, true);
            int plId = GetPriceLevel(down, up);

            PriceLevel pl = PriceLevels.levels[plId];
            pl.down = Utility.ParseCurrencyToInt(downTextBox.Text, true);
            pl.up = Utility.ParseCurrencyToInt(upTextBox.Text, true);
            pl.increments = GetIncrements(levelTextBox.Text);
            PriceLevels.levels[plId] = pl;

            ListViewItem newLvi = new ListViewItem();
            newLvi.Text = downStr;
            newLvi.SubItems.Add(upStr);
            newLvi.SubItems.Add(lvStr);
            int id = priceLevelListView.SelectedIndices[0];
            priceLevelListView.Items[id] = newLvi;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in priceLevelListView.SelectedItems)
            {
                int down = Utility.ParseCurrencyToInt(lvi.Text, true);
                int up = Utility.ParseCurrencyToInt(lvi.SubItems[1].Text, true);
                int id = GetPriceLevel(down, up);
                if (id != -1)
                    PriceLevels.levels.RemoveAt(id);
                priceLevelListView.Items.Remove(lvi);
            }
        }

        private void downTextBox_TextChanged(object sender, EventArgs e)
        {
            if (downTextBox.Text == "")
                return;
            if (Utility.ParseCurrencyToInt(downTextBox.Text, false) == -1)
                downTextBox.Text = "";
        }

        private void upTextBox_TextChanged(object sender, EventArgs e)
        {
            if (upTextBox.Text == "")
                return;
            if (Utility.ParseCurrencyToInt(upTextBox.Text, false) == -1)
                upTextBox.Text = "";
        }

        private void levelTextBox_TextChanged(object sender, EventArgs e)
        {
            string lvStr = levelTextBox.Text;
            string[] lvs = lvStr.Split(' ');
            for (int i = 0; i < lvs.Length; i++)
            {
                if (lvs[i] == "")
                    continue;

                if (Utility.ParseCurrencyToInt(lvs[i], true) == -1)
                {
                    levelTextBox.Text = "";
                    return;
                }
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private int GetPriceLevel(int down, int up)
        {
            for (int i = 0; i < PriceLevels.levels.Count; i++)
            {
                PriceLevel pl = PriceLevels.levels[i];
                if (pl.down == down && pl.up == up)
                {
                    return i;
                }
            }
            return -1; // not found
        }

        private List<int> GetIncrements(string text)
        {
            List<int> increments = new List<int>();
            string[] lvs = text.Split(' ');
            foreach (string lv in lvs)
            {
                if (lv == "")
                    continue;
                increments.Add(Utility.ParseCurrencyToInt(lv, true));
            }
            return increments;
        }
        #endregion
    }
}
