namespace EasySave_Client
{
    partial class ES_Client
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblHost = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnExe = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(357, 64);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(91, 27);
            this.txtPort.TabIndex = 10;
            this.txtPort.Text = "8080";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(134, 64);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(125, 27);
            this.txtHost.TabIndex = 9;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(299, 67);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(42, 20);
            this.lblPort.TabIndex = 8;
            this.lblPort.Text = "Port :";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(78, 67);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(47, 20);
            this.lblHost.TabIndex = 7;
            this.lblHost.Text = "Host :";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(577, 65);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 29);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnExe
            // 
            this.btnExe.Location = new System.Drawing.Point(91, 372);
            this.btnExe.Name = "btnExe";
            this.btnExe.Size = new System.Drawing.Size(126, 47);
            this.btnExe.TabIndex = 13;
            this.btnExe.Text = "Execute Work(s)";
            this.btnExe.UseVisualStyleBackColor = true;
            this.btnExe.Click += new System.EventHandler(this.btnExe_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(91, 165);
            this.listBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox1.Size = new System.Drawing.Size(446, 184);
            this.listBox1.TabIndex = 14;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(224, 372);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(126, 47);
            this.btnPause.TabIndex = 15;
            this.btnPause.Text = "Pause Work(s)";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(357, 372);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(126, 47);
            this.btnStop.TabIndex = 16;
            this.btnStop.Text = "Stop Work(s)";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(91, 101);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(579, 52);
            this.txtStatus.TabIndex = 17;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(597, 372);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(74, 47);
            this.btnRefresh.TabIndex = 18;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 20;
            this.listBox2.Location = new System.Drawing.Point(545, 165);
            this.listBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(125, 184);
            this.listBox2.TabIndex = 19;
            // 
            // ES_Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 439);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnExe);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblHost);
            this.Name = "ES_Client";
            this.Text = "EasySave Client";
            this.Load += new System.EventHandler(this.Client_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox txtPort;
        private TextBox txtHost;
        private Label lblPort;
        private Label lblHost;
        private Button btnConnect;
        private Button btnExe;
        private ListBox listBox1;
        private Button btnPause;
        private Button btnStop;
        private TextBox txtStatus;
        private Button btnRefresh;
        private ListBox listBox2;
    }
}