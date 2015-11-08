namespace Bidding
{
    partial class SetPriceLevelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.priceLevelListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addButton = new System.Windows.Forms.Button();
            this.modifyButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.downTextBox = new System.Windows.Forms.TextBox();
            this.upTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.levelTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // priceLevelListView
            // 
            this.priceLevelListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.priceLevelListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.priceLevelListView.FullRowSelect = true;
            this.priceLevelListView.GridLines = true;
            this.priceLevelListView.Location = new System.Drawing.Point(12, 12);
            this.priceLevelListView.Name = "priceLevelListView";
            this.priceLevelListView.Size = new System.Drawing.Size(465, 366);
            this.priceLevelListView.TabIndex = 0;
            this.priceLevelListView.UseCompatibleStateImageBehavior = false;
            this.priceLevelListView.View = System.Windows.Forms.View.Details;
            this.priceLevelListView.SelectedIndexChanged += new System.EventHandler(this.priceLevelListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "下限";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "上限";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "價階";
            this.columnHeader3.Width = 300;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(508, 152);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(44, 21);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "新增";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // modifyButton
            // 
            this.modifyButton.Location = new System.Drawing.Point(558, 152);
            this.modifyButton.Name = "modifyButton";
            this.modifyButton.Size = new System.Drawing.Size(54, 21);
            this.modifyButton.TabIndex = 2;
            this.modifyButton.Text = "儲存";
            this.modifyButton.UseVisualStyleBackColor = true;
            this.modifyButton.Click += new System.EventHandler(this.modifyButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(663, 152);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(44, 21);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "刪除";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // downTextBox
            // 
            this.downTextBox.Location = new System.Drawing.Point(536, 27);
            this.downTextBox.Name = "downTextBox";
            this.downTextBox.Size = new System.Drawing.Size(124, 22);
            this.downTextBox.TabIndex = 4;
            this.downTextBox.TextChanged += new System.EventHandler(this.downTextBox_TextChanged);
            // 
            // upTextBox
            // 
            this.upTextBox.Location = new System.Drawing.Point(536, 55);
            this.upTextBox.Name = "upTextBox";
            this.upTextBox.Size = new System.Drawing.Size(124, 22);
            this.upTextBox.TabIndex = 5;
            this.upTextBox.TextChanged += new System.EventHandler(this.upTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(493, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "下限";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(493, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "上限";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(493, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "價階";
            // 
            // levelTextBox
            // 
            this.levelTextBox.Location = new System.Drawing.Point(536, 100);
            this.levelTextBox.Name = "levelTextBox";
            this.levelTextBox.Size = new System.Drawing.Size(210, 22);
            this.levelTextBox.TabIndex = 10;
            this.levelTextBox.TextChanged += new System.EventHandler(this.levelTextBox_TextChanged);
            // 
            // SetPriceLevelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 390);
            this.Controls.Add(this.levelTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.upTextBox);
            this.Controls.Add(this.downTextBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.modifyButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.priceLevelListView);
            this.Name = "SetPriceLevelForm";
            this.Text = "設定跳階";
            this.Load += new System.EventHandler(this.SetPriceLevelForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView priceLevelListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button modifyButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.TextBox downTextBox;
        private System.Windows.Forms.TextBox upTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox levelTextBox;
    }
}