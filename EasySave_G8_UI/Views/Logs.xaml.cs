using System.IO;
using System;
using EasySave_G8_UI.View_Models;
using System.Windows;
using System.Collections.Generic;
using EasySave_G8_UI.Models;

namespace EasySave_G8_UI.Views
{
    /// <summary>
    /// Logique d'interaction pour Logs.xaml
    /// </summary>
    public partial class Logs : System.Windows.Controls.Page
    {
        private View_Model ViewModel;
        public Logs()
        {
            InitializeComponent();
            ViewModel = new View_Model();
            translate();
            ButtonLogs_Refresh(null, null);
        }

        private void translate()
        {
            Logs_Title.Text = $"{View_Model.VM_GetString_Language("logs_title")}";
            Daily_Logs.Content = $"{View_Model.VM_GetString_Language("daily_logs")}";
            State_Logs.Content = $"{View_Model.VM_GetString_Language("state_logs")}";
        }

        private void ButtonLogs_Refresh(object sender, EventArgs e)
        {
            try
            {
                textBoxLogs.Clear();
                List<Model_AFT> LogsList = ViewModel.MV_Look_Logs(DateTime.Now.ToString("dd-MM-yyyy"));
                foreach (Model_AFT Log in LogsList)
                {
                    textBoxLogs.AppendText("Name: " + Log.Name);
                    textBoxLogs.AppendText("\nSource: " + Log.Source);
                    textBoxLogs.AppendText("\nDestination: " + Log.Destination);
                    textBoxLogs.AppendText("\nType: " + Log.Type);
                    textBoxLogs.AppendText("\nDate: " + Log.utcDateString);
                    textBoxLogs.AppendText("\nDuration: " + Log.millisecondsDuration);
                    textBoxLogs.AppendText("\nTotal files: " + Log.total_files);
                    textBoxLogs.AppendText("\nSize: " + Log.Size);
                    textBoxLogs.AppendText("\n\n");
                }
            }
            catch (Exception) { System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("no_logs")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void ButtonStateLogs_Refresh(object sender, EventArgs e)
        {
            try
            {
                textBoxLogs.Clear();
                List<Model_StateLogs> LogsList = ViewModel.MV_Look_StateLogs();
                foreach (Model_StateLogs Log in LogsList)
                {
                    textBoxLogs.AppendText("Name: " + Log.Name);
                    textBoxLogs.AppendText("\nSource: " + Log.Source);
                    textBoxLogs.AppendText("\nDestination: " + Log.Destination);
                    textBoxLogs.AppendText("\nType: " + Log.Type);
                    textBoxLogs.AppendText("\nDate: " + Log.utcDateString);
                    textBoxLogs.AppendText("\nDuration: " + Log.millisecondsDuration);
                    textBoxLogs.AppendText("\nTotal files: " + Log.total_files);
                    textBoxLogs.AppendText("\nSize: " + Log.Size);
                    textBoxLogs.AppendText("\nProgression: " + Log.progression);
                    textBoxLogs.AppendText("\nState: " + Log.State);
                    textBoxLogs.AppendText("\n\n");
                }
            }
            catch (Exception) { System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("no_statelogs")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
    }
}
