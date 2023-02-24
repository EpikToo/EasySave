using System;
using System.Text;
using SimpleTCP;

namespace EasySave_Client
{
    public partial class ES_Client : Form
    {
        private bool WriteToListbox = false;
        SimpleTcpClient client;

        public ES_Client()
        {
            InitializeComponent();
            btnExe.Enabled= false;
            btnPause.Enabled= false;
            btnStop.Enabled= false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnect.Enabled = false;
                //Connect to server
                client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
                client.WriteLineAndGetReply("Client Connected !", TimeSpan.FromSeconds(3));
            }
            catch (Exception ex) 
            { 
                txtStatus.Text = "Host Unknown";
                btnConnect.Enabled = true;
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            int index;
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                if (WriteToListbox)
                {
                    if (e.MessageString == "stop") { WriteToListbox = false; }
                    else if(e.MessageString != "Connection etablished\u0013") { listBox1.Items.Add(e.MessageString); listBox2.Items.Add(""); }
                    if (listBox1.Items.Count > 0)
                    {
                        btnExe.Enabled = true;
                        btnPause.Enabled = true;
                        btnStop.Enabled = true;
                    }
                }
                else
                {
                    if (e.MessageString.Contains("%#"))
                    {
                        foreach(string item in listBox1.Items) 
                        {
                            if (e.MessageString.Contains(item))
                            {
                                index = listBox1.Items.IndexOf(item);
                                if (e.MessageString.Replace("#" + item, "").Contains("100%")) { listBox2.Items[index] = ""; }
                                else { listBox2.Items[index] = e.MessageString.Replace("#" + item, ""); }
                            }
                        }
                    }
                    else
                    {
                        txtStatus.Clear();
                        txtStatus.Text = e.MessageString;
                    }
                }
            });
        }

        private void btnExe_Click(object sender, EventArgs e)
        {
            foreach (string work in listBox1.SelectedItems)
            {
                client.Write("WorkExec#" + work);
                Thread.Sleep(50);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            foreach (string work in listBox1.SelectedItems)
            {
                int selectedIndex = listBox1.SelectedIndex;
                client.Write("WorkPause#" + work);
                Thread.Sleep(50);
                listBox2.Items[selectedIndex] = "Paused";

            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            foreach (string work in listBox1.SelectedItems)
            {
                client.Write("WorkStop#" + work);
                Thread.Sleep(50);
                int selectedIndex = listBox1.SelectedIndex;
                listBox2.Items[selectedIndex] = "";

            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            client.Write("WorkList");
            WriteToListbox = true;
            btnExe.Enabled = true;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
        }
    }
}