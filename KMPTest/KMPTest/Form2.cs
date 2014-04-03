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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.URL = @"C:\Wildlife.wmv";
            this.axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                this.axWindowsMediaPlayer1.fullScreen = true;
            }
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsStopped)
            {
                //this.axWindowsMediaPlayer1.Dispose();
                //this.Close();
                //this.Hide();
                //this.Dispose();
            }
        }

        public void CloseForm()
        {
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                this.axWindowsMediaPlayer1.Dispose();
            }
            this.Close();
        }
    }
}
