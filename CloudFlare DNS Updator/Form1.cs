using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudFlare_DNS_Updator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            textBoxDomain.Text = Properties.Settings.Default.Domain;
            textBoxAPIKey.Text = Properties.Settings.Default.ApiKey;
            textBoxEMail.Text = Properties.Settings.Default.Email;

            GlobalIpChecker_Tick(null, null);
        }

        bool close;

        string ip;

        void ChangeGlobalIpEvent()
        {
            CFAPI ca = new CFAPI(textBoxDomain.Text, textBoxAPIKey.Text, textBoxEMail.Text);

           
            var DNSINFO = ca.getDNSINFO();

            if (ip == DNSINFO.content)
            {
                MessageBox.Show("IP == DNS IP");
            }


            ip = GetPublicIP();

            MessageBox.Show("rec_id:" + DNSINFO.rec_id + "\r\n" + "name: " + DNSINFO.name);
            ca.UpdateDNSIP(DNSINFO.rec_id, ip, DNSINFO.name, DNSINFO.service_mode, DNSINFO.ttl);
      

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Domain = textBoxDomain.Text;
            Properties.Settings.Default.ApiKey = textBoxAPIKey.Text;
            Properties.Settings.Default.Email = textBoxEMail.Text;

            if (!close)
            {
                e.Cancel = true;
                this.Visible = false;
            }
            Properties.Settings.Default.Save();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            close = true;
            this.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == false)
                this.Visible = true;
            else
                this.Activate();
        }

        private void GlobalIpChecker_Tick(object sender, EventArgs e)
        {
            //network connected?
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                string nowgip;

                try
                {
                    nowgip = GetPublicIP();
                }
                catch (Exception)
                {
                    return;
                }
                this.Text = "CloudFlare DNS Updator - " + nowgip;

                if (ip == null)
                {
                    ip = nowgip;
                    return;
                }

                if (ip != nowgip)
                    ChangeGlobalIpEvent();

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
          
        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void selfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeGlobalIpEvent();
            MessageBox.Show("Updated");
        }
    }
}
