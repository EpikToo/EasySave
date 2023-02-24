using EasySave_G8_UI.View_Models;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using EasySave_G8_UI.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace EasySave_G8_UI.Views.Works
{
    /// <summary>
    /// Logique d'interaction pour Works_Edit.xaml
    /// </summary>
    public partial class Works_Edit : Page
    {
        private string Original_WorkName { get; set; }
        private View_Model ViewModel;
        private MainWindow MainWindow1;

        public Works_Edit(string? WorkName)
        {
            InitializeComponent();
            Original_WorkName = WorkName;
            ViewModel = new View_Model();
            MainWindow1 = Application.Current.MainWindow as MainWindow;
            Get_Work_Data();
            translate();
        }

        private void translate()
        {
            Work_Edit.Text = $"{View_Model.VM_GetString_Language("work_edit")}";
            Name.Text = $"{View_Model.VM_GetString_Language("name")}";
            Source_Path.Text = $"{View_Model.VM_GetString_Language("source_path")}";
            Dest_Path.Text = $"{View_Model.VM_GetString_Language("dest_path")}";
            Browse.Content = $"{View_Model.VM_GetString_Language("browse")}";
            Browse2.Content = $"{View_Model.VM_GetString_Language("browse")}";
            Save_btn.Content = $"{View_Model.VM_GetString_Language("save_work")}";
            Execute_Now.Text = $"{View_Model.VM_GetString_Language("execute_now")}";
            Complete.Content = $"{View_Model.VM_GetString_Language("complete")}";
            Differential.Content = $"{View_Model.VM_GetString_Language("differential")}";
            Yes.Content = $"{View_Model.VM_GetString_Language("yes")}";
            No.Content = $"{View_Model.VM_GetString_Language("no")}";
        }

        private void Get_Work_Data()
        {
            textBox1.Text = Original_WorkName;
            List<Model_PRE> Work_List = ViewModel.VM_Work_Show(Original_WorkName, false);
            foreach(Model_PRE obj in Work_List) 
            {
                textBox2.Text = obj.Source;
                textBox3.Text = obj.Destination;
                if (obj.Type) { comboBox1.SelectedIndex = 0; }
                else { comboBox1.SelectedIndex = 1;}
            }
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            bool Type;
            if (comboBox1.SelectedIndex == 0) { Type = true; }
            else { Type = false; }

            bool ExeNow;
            if (comboBox2.SelectedIndex == 0) { ExeNow = true; }
            else { ExeNow = false; }

            if (textBox1.Text == "")
            {
                MessageBox.Show($"Name cannot be empty.", $"Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (textBox2.Text == "")
            {
                MessageBox.Show($"Source cannot be empty.", $"Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (textBox3.Text == "")
            {
                MessageBox.Show($"Destination cannot be empty.", $"Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ViewModel.VM_Work_Delete(Original_WorkName);
            ViewModel.VM_Work_New(textBox1.Text, textBox2.Text, textBox3.Text, Type);

            if (ExeNow)
            {
                bool blacklist_state = ViewModel.VM_BlackListTest();
                string appPath = Directory.GetCurrentDirectory() + @"\cryptosoft.exe";

                if (!blacklist_state)
                {
                    if (File.Exists(appPath))
                    {
                        Thread thread_pgbar = new Thread(MainWindow1.Loading1.ProgressBar_Add);
                    thread_pgbar.Name = textBox1.Text;
                    thread_pgbar.Start();

                    MainWindow1.Main.Content = MainWindow1.Loading1;

                    BackgroundWorker backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork += BackgroundWorker_DoWork;
                    backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                    backgroundWorker.RunWorkerAsync(argument: textBox1.Text);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("error_cryptosoft")}" + appPath, $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else { MessageBox.Show($"{View_Model.VM_GetString_Language("msgbox_blacklist")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            }
            else
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.Main.Content = new Works();
            }
        }
        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ViewModel.VM_Work_Run((string)e.Argument, sender);
        }

        private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            MainWindow1.Loading1.UpdatePGBar(e);
        }

        private void Button_Click_Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ValidateNames = false;
            openFileDialog.FileName = $"{View_Model.VM_GetString_Language("select_directory")}";
            openFileDialog.Filter = $"{View_Model.VM_GetString_Language("directories")}";
            openFileDialog.InitialDirectory = @"C:\";

            if (openFileDialog.ShowDialog() == true)
            {
                textBox2.Text = Path.GetDirectoryName(openFileDialog.FileName);
            }
        }
        private void Button_Click_Browse2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ValidateNames = false;
            openFileDialog.FileName = $"{View_Model.VM_GetString_Language("select_directory")}";
            openFileDialog.Filter = $"{View_Model.VM_GetString_Language("directories")}";
            openFileDialog.InitialDirectory = @"C:\";

            if (openFileDialog.ShowDialog() == true)
            {
                textBox3.Text = Path.GetDirectoryName(openFileDialog.FileName);
            }
        }
    }
}
