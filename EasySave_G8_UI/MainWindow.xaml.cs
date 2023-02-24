using EasySave_G8_UI.View_Models;
using EasySave_G8_UI.Views;
using EasySave_G8_UI.Views.Works;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace EasySave_G8_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Mutex _mutex = null;
        public Loading Loading1;
        private View_Model ViewModel;
        public bool ApplicationOn = true;
        public static MainWindow MainWindow1;

        public MainWindow()
        {
            InitializeComponent();
            CheckInstance();
            ViewModel = new View_Model();
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave";
            if (!Directory.Exists(fileName)) //Check if mandatory files are present or not. If not, creates them
            {
                ViewModel.VM_Init();
            }

            translate();

            MainWindow1 = Application.Current.MainWindow as MainWindow;

            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;

            this.Background = Brushes.Transparent;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            Main.Content = new Dashboard();
            if ($"{View_Model.VM_GetString_Language("lang")}" == "en") { FRradio.IsChecked = true; }
            else { ENradio.IsChecked = true; }

            Loading1 = new Loading();
            Thread _threadblacklist = new Thread(Loading1.BlacklistPause);
            _threadblacklist.Start();
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CheckInstance()
        {
            bool createdNew;
            _mutex = new Mutex(true, "EasySave G8", out createdNew);
            if (!createdNew)
            {
                ApplicationOn = false;
                Application.Current.Shutdown();
                MessageBox.Show($"{View_Model.VM_GetString_Language("app_running")}", $"{View_Model.VM_GetString_Language("error")}", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }
        }

        private void translate()
        {
            ViewModel.VM_Update_Language(); //Get the language from app_conf JSON file
            Main.Content = new Dashboard();
            Dashboard_btn.Content = $"{View_Model.VM_GetString_Language("dashboard")}";
            Classics_btn.Content = $"{View_Model.VM_GetString_Language("classics")}";
            Works_btn.Content = $"{View_Model.VM_GetString_Language("works")}";
            Logs_btn.Content = $"{View_Model.VM_GetString_Language("logs")}";
            Loading_btn.Content = $"{View_Model.VM_GetString_Language("loading")}";
            Settings_btn.Content = $"{View_Model.VM_GetString_Language("settings")}";
            Shutdown_btn.Content = $"{View_Model.VM_GetString_Language("shutdown")}";
            Remote_btn.Content = $"{View_Model.VM_GetString_Language("remote")}";



        }

        private void Classics_Click(object sender, RoutedEventArgs e)
        {
            Main.Content= new Classics();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Dashboard();
        }

        private void Works_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Works();
        }

        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Logs();
        }

        private void Loading_btn_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = Loading1;
        }

        private void Settings_btn_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Settings();
        }


        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            ApplicationOn = false;
            Application.Current.Shutdown();
        }

        private void EngButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.VM_Change_Language("en");
            translate();
        }

        private void FraButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.VM_Change_Language("fr");
            translate();
        }

        private void Remote_btn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.VM_RemoteLaunch();
        }
    }
}
