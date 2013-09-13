using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace Checkout
{
    /// <summary>
    /// Auction class to store information of the auction.
    /// </summary>
    public class Auction
    {
        public string lot;
        public string name;
        public int hammerPrice;
        public int serviceCharge;
        public int total;
        public string docName = "";
        public _Document paymentDoc;
        public bool isUseCreditCard;

        public Auction()
        {
            hammerPrice = serviceCharge = total = 0;
            lot = name = "";
            paymentDoc = null;
            isUseCreditCard = false;
        }

        /// <summary>
        /// 0 ~ 10,000,000 = 18%
        /// 10,000,000 ~ 20,000,000 = 15%
        /// 20,000,000 ~ = 12%
        /// </summary>
        public void ComputeChargeAndTotal()
        {
            if (hammerPrice == 0)
                return;

            if (hammerPrice > 20000000)
            {
                serviceCharge = Convert.ToInt32((hammerPrice - 20000000) * 0.12f + 10000000 * 0.15f + 10000000 * 0.18f);
                total = hammerPrice + serviceCharge;
            }
            else if (hammerPrice > 10000000)
            {
                serviceCharge = Convert.ToInt32((hammerPrice - 10000000) * 0.15f + 10000000 * 0.18f);
                total = hammerPrice + serviceCharge;
            }
            else
            {
                serviceCharge = Convert.ToInt32(hammerPrice * 0.18f);
                total = hammerPrice + serviceCharge;
            }
        }
    };

    public class Bidder
    {
        public string name;
        public int no;
        public string phone;
        public string fax;
        public string email;
        public string addr;
        public bool isUseCredit;
        public int creditCardFee;
        public int tax;
        public int amountDue;
        public Dictionary<string, Auction> auctions;
    }

    public static class Utility
    {
        public static bool IsFileExist(string filePath, string fileName)
        {
            if (new FileInfo(filePath).Exists)
                return true;
            else
                MessageBox.Show(fileName + " 檔案不存在!");

            return false;
        }

        public static bool IsValidFileName(string input)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            int index = input.IndexOfAny(invalidChars);
            if (index == -1)
            {
                return true;
            }
            else
            {
                MessageBox.Show("不可輸入無效字元: " + input[index]);
                return false;
            }
        }

        public static int IsNumber(string input)
        {
            int number = 0;
            if (int.TryParse(input, out number))
            {
                return number;
            }
            else
            {
                MessageBox.Show("請輸入有效數字");
                return -1;
            }
        }

        public static string GetDirectory(string filePath)
        {
            int slashPos = filePath.LastIndexOf(@"/") == -1 ? filePath.LastIndexOf(@"\") : filePath.LastIndexOf(@"/");
            return filePath.Remove(slashPos + 1);
        }

        public static string GetFileName(string filePath)
        {
            int slashPos = filePath.LastIndexOf(@"/") == -1 ? filePath.LastIndexOf(@"\") : filePath.LastIndexOf(@"/");
            return filePath.Remove(0, slashPos + 1);
        }

        public static string GetFileNameNoExtension(string filePath)
        {
            string fileName = GetFileName(filePath);
            return fileName.Remove(fileName.LastIndexOf('.'));
        }

        public static string GetFileExtension(string filePath)
        {
            return filePath.Remove(0, filePath.LastIndexOf(@".") + 1);
        }
    }

    /// <summary>
    /// Implements the manual sorting of items by columns.
    /// </summary>
    public class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                returnVal *= -1;    // Invert the value returned by String.Compare.
            return returnVal;
        }
    }
}