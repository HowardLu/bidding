namespace CheckoutDealer
{
    partial class CheckoutDealerForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDealerName = new System.Windows.Forms.Label();
            this.textBoxDealerName = new System.Windows.Forms.TextBox();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.listViewResult = new System.Windows.Forms.ListView();
            this.columnLotId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // labelDealerName
            // 
            this.labelDealerName.AutoSize = true;
            this.labelDealerName.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelDealerName.Location = new System.Drawing.Point(12, 9);
            this.labelDealerName.Name = "labelDealerName";
            this.labelDealerName.Size = new System.Drawing.Size(72, 16);
            this.labelDealerName.TabIndex = 0;
            this.labelDealerName.Text = "賣家姓名";
            this.labelDealerName.Click += new System.EventHandler(this.labelDealerName_Click);
            // 
            // textBoxDealerName
            // 
            this.textBoxDealerName.Location = new System.Drawing.Point(88, 6);
            this.textBoxDealerName.Name = "textBoxDealerName";
            this.textBoxDealerName.Size = new System.Drawing.Size(100, 22);
            this.textBoxDealerName.TabIndex = 1;
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(194, 5);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(75, 23);
            this.buttonQuery.TabIndex = 2;
            this.buttonQuery.Text = "查詢";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // listViewResult
            // 
            this.listViewResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLotId});
            this.listViewResult.GridLines = true;
            this.listViewResult.Location = new System.Drawing.Point(12, 34);
            this.listViewResult.Name = "listViewResult";
            this.listViewResult.Size = new System.Drawing.Size(587, 393);
            this.listViewResult.TabIndex = 3;
            this.listViewResult.UseCompatibleStateImageBehavior = false;
            // 
            // columnLotId
            // 
            this.columnLotId.Text = "拍品Lot編號";
            // 
            // CheckoutDealerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 430);
            this.Controls.Add(this.listViewResult);
            this.Controls.Add(this.buttonQuery);
            this.Controls.Add(this.textBoxDealerName);
            this.Controls.Add(this.labelDealerName);
            this.Name = "CheckoutDealerForm";
            this.Text = "賣家結帳";
            this.Load += new System.EventHandler(this.CheckoutDealerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDealerName;
        private System.Windows.Forms.TextBox textBoxDealerName;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.ListView listViewResult;
        private System.Windows.Forms.ColumnHeader columnLotId;
    }
}

