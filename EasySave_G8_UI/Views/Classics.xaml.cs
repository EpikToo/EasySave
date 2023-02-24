using EasySave_G8_UI.View_Models;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System;
using System.Threading;
using System.ComponentModel;

namespace EasySave_G8_UI.Views
{
    /// <summary>
    /// Logique d'interaction pour Classics.xaml
    /// </summary>
    public partial class Classics : Page
    {   
        private MainWindow MainWindow1;
        private View_Model ViewModel;

        private string ClassicName;
        private string Source;
        private string Dest;
        private bool Type;

        public Classics()
        {
            InitializeComponent();
            translate();
            MainWindow1 = Application.Current.MainWindow as MainWindow;
            ViewModel = new View_Model();
        }

        private void translate()
        {
            Classics_Title.Text = Name.Text = $"{View_Model.VM_GetString_Language("classics_title")}";
            Name.Text = $"{View_Model.VM_GetString_Language("name")}";
            Source_Path.Text = $"{View_Model.VM_GetString_Language("source_path")}";
            Dest_Path.Text = $"{View_Model.VM_GetString_Language("dest_path")}";
            LaunchBtn.Content = $"{View_Model.VM_GetString_Language("launch_save")}";
            Complete.Content = $"{View_Model.VM_GetString_Language("complete")}";
            Differential.Content = $"{View_Model.VM_GetString_Language("differential")}";
            Browse.Content = $"{View_Model.VM_GetString_Language("browse")}";
            Browse2.Content = $"{View_Model.VM_GetString_Language("browse")}";
        }

        private void Button_Click_LaunchSave(object sender, RoutedEventArgs e)
        {            
            bool blacklist_state = ViewModel.VM_BlackListTest();
            string appPath = Directory.GetCurrentDirectory() + @"\cryptosoft.exe";


            if (blacklist_state == false)
            {
                if (File.Exists(appPath))
                {
                    ClassicName = this.textBox1.Text;
                    try
                    {
                        if (ViewModel.VM_StateLogsExists(ClassicName)) { System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("error_work_name")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                        else if (ClassicName == "")
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
                    }
                    catch { Exception ex; }

                    Source = this.textBox2.Text;
                    Dest = this.textBox3.Text;
                    int indexType = this.comboBox1.SelectedIndex;
                    if (indexType == 0) { Type = true; }
                    else { Type = false; }

                    try
                    {
                        Thread thread_pgbar = new Thread(MainWindow1.Loading1.ProgressBar_Add);
                        thread_pgbar.Name = ClassicName;
                        thread_pgbar.Start();

                        MainWindow1.Main.Content = MainWindow1.Loading1;

                        BackgroundWorker backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork += BackgroundWorker_DoWork;
                        backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                        backgroundWorker.RunWorkerAsync();
                    }
                    catch (Exception) { System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("error_parameters")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
                }
                else
                {
                    System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("error_cryptosoft")}" + appPath, $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show($"{View_Model.VM_GetString_Language("msgbox_blacklist")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ViewModel.VM_Classic(ClassicName, Source, Dest, Type, sender);
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
