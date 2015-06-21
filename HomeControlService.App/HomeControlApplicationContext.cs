using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeControlService.App
{
    public class HomeControlApplicationContext : ApplicationContext
    {
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private HomeControlService tc;

        private const int BaloonTooltipTimeoutMS = 2000;
    
        public HomeControlApplicationContext(HomeControlService tc)
        {
            this.tc = tc;
            tc.OnPersonArrived += tc_OnPersonArrived;
            tc.OnPersonLeft += tc_OnPersonLeft;
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Home Control";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(199, 67);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(198, 30);
            this.toolStripMenuItem1.Text = "Exit";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);

            tc.Start();
        }

        private void ShowTip(string title, string tip = null)
        {
            notifyIcon1.ShowBalloonTip(BaloonTooltipTimeoutMS, tip, tip ?? title, ToolTipIcon.Info);
        }
        private void ShowTip(PersonPresenceChangedEventArgs e, bool arrived)
        {
            string action = arrived ? "Arrived" : "Left";
            ShowTip(string.Format("{0} has {1} at {2}", e.Name, action, e.ChangeTimeUtc.ToLocalTime().ToShortTimeString()));
        }

        void tc_OnPersonLeft(object sender, PersonPresenceChangedEventArgs e)
        {
            ShowTip(e, false);
        }

        void tc_OnPersonArrived(object sender, PersonPresenceChangedEventArgs e)
        {
            ShowTip(e, true);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }
    }
}
