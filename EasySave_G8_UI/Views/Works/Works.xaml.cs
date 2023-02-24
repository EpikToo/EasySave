using EasySave_G8_UI;
using EasySave_G8_UI.Models;
using EasySave_G8_UI.View_Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace EasySave_G8_UI.Views.Works
{
    /// <summary>
    /// Logique d'interaction pour Works.xaml
    /// </summary>
    public partial class Works : Page
    {
        private MainWindow MainWindow1;
        private View_Model ViewModel;

        public Works()
        {
            InitializeComponent();
            ViewModel = new View_Model();
            MainWindow1 = Application.Current.MainWindow as MainWindow;
            Works_List();
            translate();
        }

        private void Works_List()
        {
            List_Works.Items.Clear();
            List<Model_PRE>? WorkList = ViewModel.VM_Work_Show(null, true);
            foreach(Model_PRE obj in WorkList)
            {
                List_Works.Items.Add(obj.Name);
            }
        }

        private void Create_btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Main.Content = new Works_Create();
        }

        private void ExecuteAll_btn_Click(object sender, RoutedEventArgs e)
        {
            bool blacklist_state = ViewModel.VM_BlackListTest();
            int i = 0;
            string appPath = Directory.GetCurrentDirectory() + @"\cryptosoft.exe";

            if (blacklist_state == false)
            {
                if (File.Exists(appPath))
                {
                    foreach (string WorkName in List_Works.Items) { i++; }
                    if (i == 0) { MessageBox.Show($"{View_Model.VM_GetString_Language("no_work")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); return; };
                    MainWindow1.Main.Content = MainWindow1.Loading1;
                    foreach (string WorkName in List_Works.Items)
                    {
                        Thread thread_pgbar = new Thread(MainWindow1.Loading1.ProgressBar_Add);
                        thread_pgbar.Name = WorkName;
                        thread_pgbar.Start();

                        BackgroundWorker backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork += BackgroundWorker_DoWork;
                        backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                        backgroundWorker.RunWorkerAsync(argument: WorkName);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("error_cryptosoft")}" + appPath, $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else { MessageBox.Show($"{View_Model.VM_GetString_Language("msgbox_blacklist")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void ExecuteSelected_btn_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            bool blacklist_state = ViewModel.VM_BlackListTest();
            string appPath = Directory.GetCurrentDirectory() + @"\cryptosoft.exe";

            if (blacklist_state == false)
            {
                if (File.Exists(appPath))
                {
                    foreach (string WorkName in List_Works.SelectedItems) { i++; }
                    if (i == 0) { MessageBox.Show($"{View_Model.VM_GetString_Language("choose_work_execute")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

                    MainWindow1.Main.Content = MainWindow1.Loading1;
                    foreach (string WorkName in List_Works.SelectedItems)
                    {
                        Thread thread_pgbar = new Thread(MainWindow1.Loading1.ProgressBar_Add);
                        thread_pgbar.Name = WorkName;
                        thread_pgbar.Start();

                        BackgroundWorker backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork += BackgroundWorker_DoWork;
                        backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                        backgroundWorker.RunWorkerAsync(argument: WorkName);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show($"{View_Model.VM_GetString_Language("error_cryptosoft")}" + appPath, $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else { MessageBox.Show($"{View_Model.VM_GetString_Language("msgbox_blacklist")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ViewModel.VM_Work_Run((string)e.Argument, sender);
        }

        private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            MainWindow1.Loading1.UpdatePGBar(e);
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (string WorkName in List_Works.SelectedItems)
            {
                i++;
                ViewModel.VM_Work_Delete(WorkName);
            }
            if (i == 0) { MessageBox.Show($"{View_Model.VM_GetString_Language("choose_work_delete")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
            Works_List();
        }

        private void Edit_btn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            int i = 0;
            string WorkName = "";
            foreach (string Work in List_Works.SelectedItems)
            {
                i++;
                if (i>1) { MessageBox.Show($"{View_Model.VM_GetString_Language("work_edit_only")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                WorkName = Work;
            }
            if (i == 1) { mainWindow.Main.Content = new Works_Edit(WorkName); }
            else { MessageBox.Show($"{View_Model.VM_GetString_Language("choose_work_edit")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void List_Works_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List_Work_Detail.Text = "";
            if (List_Works.SelectedItems.Count == 0) { return; }

            List<Model_PRE> obj_list = ViewModel.VM_Work_Show((List_Works.Items[List_Works.SelectedIndex].ToString()), false);
            
            foreach(Model_PRE obj in obj_list)
            {
                LabelCurrent.Content= obj.Name;
                List_Work_Detail.Text = $"{View_Model.VM_GetString_Language("name")} : " + obj.Name + "\n";
                List_Work_Detail.Text += "Source : " + obj.Source + "\n";
                List_Work_Detail.Text += "Destination : " + obj.Destination + "\n";
                if (obj.Type) { List_Work_Detail.Text += "Type : Complete \n"; }
                else { List_Work_Detail.Text += "Type : Differential \n"; }
            }
        }

        private void Next_btn_Click(object sender, RoutedEventArgs e)
        {
            if (List_Works.SelectedItems.Count == 0 || List_Works.SelectedItems.Count == 1) { return; }

            string CurrentWork = LabelCurrent.Content.ToString();
            string WorkName = "";
            bool AfterCurrent = false;
            bool WorkNameFound = false;

            while (!WorkNameFound)
            {
                foreach (string Item in List_Works.SelectedItems)
                {
                    if (Item == CurrentWork) { AfterCurrent = true; }
                    if (Item != CurrentWork && AfterCurrent)
                    {
                        WorkName = Item;
                        WorkNameFound = true;
                        break;
                    }
                }
            }

            List_Work_Detail.Text = "";
            List<Model_PRE> obj_list = ViewModel.VM_Work_Show(WorkName, false);

            foreach (Model_PRE obj in obj_list)
            {
                LabelCurrent.Content = obj.Name;
                List_Work_Detail.Text = "Name: " + obj.Name + "\n";
                List_Work_Detail.Text += "Source: " + obj.Source + "\n";
                List_Work_Detail.Text += "Destination: " + obj.Destination + "\n";
                if (obj.Type) { List_Work_Detail.Text += "Type: Complete \n"; }
                else { List_Work_Detail.Text += "Type: Differential \n"; }
            }
        }
        private void translate()
        {
            Works_Title.Text = $"{View_Model.VM_GetString_Language("works_title")}";
            Next_btn.Content = $"{View_Model.VM_GetString_Language("next")}";
            ExecuteAll_btn.Content = $"{View_Model.VM_GetString_Language("execute_all")}";
            ExecuteSelected_btn.Content = $"{View_Model.VM_GetString_Language("execute_selection")}";
            Edit_btn.Content = $"{View_Model.VM_GetString_Language("edit_selection")}";
            Delete_btn.Content = $"{View_Model.VM_GetString_Language("delete_selection")}";
            Create_btn.Content = $"{View_Model.VM_GetString_Language("create_work")}";
        }
    }
}
