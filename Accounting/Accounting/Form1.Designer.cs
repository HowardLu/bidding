namespace Accounting
{
    partial class Form1
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
            this.excelWrapper1 = new EmbeddedExcel.ExcelWrapper();
            this.SuspendLayout();
            // 
            // excelWrapper1
            // 
            this.excelWrapper1.Location = new System.Drawing.Point(244, 39);
            this.excelWrapper1.Name = "excelWrapper1";
            this.excelWrapper1.Size = new System.Drawing.Size(598, 552);
            this.excelWrapper1.TabIndex = 0;
            this.excelWrapper1.ToolBarVisible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 677);
            this.Controls.Add(this.excelWrapper1);
            this.Name = "Form1";
            this.Text = "Accounting";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private EmbeddedExcel.ExcelWrapper excelWrapper1;

    }
}

