using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KMPTest
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Process Proc;
        String pn;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Proc = new System.Diagnostics.Process();
            Proc.StartInfo.FileName = "E:/Program Files/The KMPlayer/KMPlayer.exe";
            Proc.StartInfo.Arguments = @"D:/Antonio_mcDyess.flv";
            Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            Proc.Start();
            pn = Proc.ProcessName;
            System.Threading.Thread.Sleep(1000);
            SendKeys.Send("%{Enter}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KMPlayer kmplayer = new KMPlayer();
            kmplayer.OpenFile("E:/Program Files/The KMPlayer/KMPlayer.exe", "D:/Antonio_mcDyess.flv", 10000);
        }
    }
}
