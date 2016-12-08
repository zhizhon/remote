using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace szxlc.remote
{
    public partial class FrmRemote : Form
    {
        BackgroundWorker BW;
        static string companyURL = Properties.Resources.companyURL;
        IPAddress ipaddr = (IPAddress)Dns.GetHostEntry(companyURL).AddressList.GetValue(0);
        int startPort = int.Parse(Properties.Resources.remoteStartPort);
        int endPort = int.Parse(Properties.Resources.remoteEndPort);
        public FrmRemote()
        {
            InitializeComponent();
            InitializeBackgroudWorker();

        }

        private void InitializeBackgroudWorker()
        {

            BW = new BackgroundWorker();
            BW.WorkerSupportsCancellation = true;
            BW.WorkerReportsProgress = true;
            BW.DoWork += new DoWorkEventHandler(BW_DoWork);
            BW.ProgressChanged += new ProgressChangedEventHandler(BW_ProgressChanged);
        }

        private void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker bwtemp = sender as BackgroundWorker;
            //portstatus p = (portstatus)e.Argument;
            for (int i = startPort; i <=endPort; i++)
            {
                if (BW.CancellationPending)
                {
                    e.Cancel = true;
                }
               bool  status =IsPortOpen(ipaddr, i);
                BW.ReportProgress(i, status);
            }

            
           
        }

        private void BW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int p = e.ProgressPercentage;
            bool status = bool.Parse(e.UserState.ToString());
            int line = p % startPort + 1;
            string btnName = "btnLine" + line;
            if(FindControl(Controls.Owner,btnName)!=null)
            {
                Button btn = (Button)this.FindControl(Controls.Owner, btnName);
                btn.Enabled = bool.Parse(e.UserState.ToString());
                btn.Update();
            }
           
           
        }
        private Control FindControl(Control control,string controlName)
        {
            Control c1;
            foreach (Control c in control.Controls)
            {
                if (c.Name == controlName)
                {
                    return c;
                }
                else if (c.Controls.Count > 0)
                {
                    c1 = FindControl(c, controlName);
                    if (c1 != null)
                    {
                        return c1;
                    }
                }
            }
            return null;
        }

        private void remote_Load(object sender, EventArgs e)
        {
            PortScan();

        }

        private void PortScan()
        {

            //portstatus ps = new portstatus();
            //ps.port = startPort;

            //while (ps.port <= endPort)
            //{
                BW.RunWorkerAsync();
            //    ps.port++;
            //}
        }
        private   bool IsPortOpen(IPAddress ip, int port)       //判断某一计算机端口是否处于打开状态
        {
            try
            {
                TcpClient client = new TcpClient();       //创建一个TcpClient实例
                client.ReceiveTimeout = client.SendTimeout = 100;
                //IPAddress address = IPAddress.Parse(ip);  //转化string类型的ip地址到IpAddress
                client.Connect(ip, port);            //连接服务器address的port端口
                client.Close();                           //连接成功立即断开
                return true;
            }

            catch (Exception e)
            {
                return false;
            }
        }

        private void FrmRemote_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(BW.IsBusy)
            { BW.CancelAsync(); }
            
        }

        private void FrmRemote_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (BW.IsBusy)
                { BW.CancelAsync(); }
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                if (!BW.IsBusy)
                { 
                PortScan();
            }
            }
            
        }

        private void btnLine3_Click(object sender, EventArgs e)
        {
            //三号线被点击
            //关闭端口扫描;
            //最小化主窗口；
            //
        }
    }
    //public class portstatus
    //{
    //    public int port
    //    { get; set; }
    //    public bool status
    //    { get; set; }
    //}
}
