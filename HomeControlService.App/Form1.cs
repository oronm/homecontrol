using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace HomeControlService.App
{
    public partial class Form1 : Form
    {
        private HomeControlService tc;
        public Form1(HomeControlService tc)
        {
            this.tc = tc;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }


        private void runService()
        {
            tc.Start();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
