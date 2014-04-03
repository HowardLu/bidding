using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SJ_Bidding_System
{
    public partial class PlayerForm : Form
    {
        public string URL { get; set; }

        public PlayerForm(string url)
        {
            InitializeComponent();
            URL = url;
        }

        private void PlayerForm_Load(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.URL = URL;// @"C:\Wildlife.wmv";
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
