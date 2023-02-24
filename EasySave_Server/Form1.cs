using SimpleTCP;
using System;
using System.Text;
using System.Windows.Forms;
using EasySave_G8_UI;
using EasySave_G8_UI.View_Models;
using EasySave_G8_UI.Models;
using System.ComponentModel;

namespace EasySave_Server
{
    public partial class ES_Server : Form
    {
        SimpleTcpServer server;
        private View_Model ViewModel;
        private int PercentSpacer;

        public ES_Server()
        {
            InitializeComponent();
            ViewModel = new View_Model();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;//enter
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            //Update mesage to txtStatus
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Clear();
                txtStatus.Text += e.MessageString;
                e.ReplyLine(string.Format("Connection etablished"));
            });

            if (e.MessageString == "WorkList")
            {
                WorkList();
            }

            if (e.MessageString.Contains("WorkExec#"))
            {
                Work_Exec(e.MessageString.Replace("WorkExec#", ""));
            }

            if (e.MessageString.Contains("WorkPause#"))
            {
                Work_Pause(e.MessageString.Replace("WorkPause#", ""));
            }

            if (e.MessageString.Contains("WorkStop#"))
            {
                Work_Stop(e.MessageString.Replace("WorkStop#", ""));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Start server host
            txtStatus.Text += "Server starting...";
            try
            {
                System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtHost.Text);
                server.Start(ip, Convert.ToInt32(txtPort.Text));
            }
            catch (Exception ex) { txtStatus.Clear(); txtStatus.Text += "Invalid IP adress"; }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
                txtStatus.Text += "Server stopping...";
            server.BroadcastLine("Server is stopped");
            server.Stop();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            server.Broadcast(txtMessage.Text);
        }

        private void WorkList()
        {
            Model_Works model_Works = new Model_Works();
            List<Model_PRE> obj_list = model_Works.Get_Work(null, true);
            foreach (Model_PRE obj in obj_list)
            {
                server.Broadcast(obj.Name);
                Thread.Sleep(100);
            }
            server.Broadcast("stop");
        }

        private void Work_Exec(string WorkName)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerAsync(argument: WorkName);
        }

        private void Work_Pause(string WorkName)
        {
            ViewModel.VM_PauseSpecificThread(WorkName);
        }

        private void Work_Stop(string WorkName) 
        { 
            ViewModel.VM_StopSpecificThread(WorkName);
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ViewModel.VM_Work_Run((string)e.Argument, sender);
        }

        private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 101)
            {
                txtStatus.Invoke((MethodInvoker)delegate ()
                {
                    txtStatus.Clear();
                    txtStatus.Text = e.ProgressPercentage.ToString() + "%#" + e.UserState.ToString();
                });
                PercentSpacer += 50;

                Thread.Sleep(PercentSpacer);
                server.Broadcast(e.ProgressPercentage.ToString() + "%#" + e.UserState.ToString());
            }
        }
    }
}