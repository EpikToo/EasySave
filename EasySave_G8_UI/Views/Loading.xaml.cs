using EasySave_G8_UI.View_Models;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasySave_G8_UI.Views
{
    /// <summary>
    /// Logique d'interaction pour Loading.xaml
    /// </summary>
    public partial class Loading : Page
    {
        private MainWindow currentMainWindow;
        private Loading currentLoading;
        private View_Model ViewModel;

        private bool isRunning;
        private bool isPaused;

        public Loading()
        {
            InitializeComponent();
            ViewModel = new View_Model();
            translate();
            currentMainWindow = Application.Current.MainWindow as MainWindow;
            currentLoading = currentMainWindow.Main.Content as Loading;
        }

        private void translate()
        {
            Loading_Title.Text = $"{View_Model.VM_GetString_Language("loading")}";
        }

        public async void ProgressBar_Add()
        {
            Thread currentThread = Thread.CurrentThread;
            ProgressBar progressBar = null;
            Label middle_label = null;
            Label left_label = null;
            Button pause_button = null;
            Button stop_button = null;
            MainStackPanel.Dispatcher.Invoke(() =>
            {
                progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Margin = new Thickness(20, 20, 20, 20);
                progressBar.Width = 400;
                progressBar.Height = 30;
                progressBar.Foreground = Brushes.DarkCyan;
                progressBar.Name = currentThread.Name + "pgbar";
                MainStackPanel.Children.Add(progressBar);

                middle_label = new Label();
                middle_label.Height = 50;
                middle_label.HorizontalAlignment = HorizontalAlignment.Center;
                middle_label.VerticalAlignment = VerticalAlignment.Center;
                middle_label.FontSize = 20;
                middle_label.Margin = new Thickness(20, -57, 20, 0);
                middle_label.Name = currentThread.Name + "label1";
                MainStackPanel.Children.Add(middle_label);

                left_label = new Label();
                left_label.Height = 50;
                left_label.HorizontalAlignment = HorizontalAlignment.Left;
                left_label.VerticalAlignment = VerticalAlignment.Center;
                left_label.FontSize = 20;
                left_label.Margin = new Thickness(50, -58, 20, 0);
                left_label.Content = currentThread.Name;
                left_label.Name = currentThread.Name + "label3";
                left_label.Foreground = Brushes.White;
                MainStackPanel.Children.Add(left_label);

                stop_button = new Button();
                stop_button.Height = 15;
                stop_button.HorizontalAlignment = HorizontalAlignment.Right;
                stop_button.VerticalAlignment = VerticalAlignment.Center;
                stop_button.FontSize = 8;
                stop_button.Width= 100;
                stop_button.Margin = new Thickness(20, -85, 40, 0);
                stop_button.Content = "Stop";
                stop_button.Name = currentThread.Name + "stop_btn";
                stop_button.Click += StopSpecific_btn_Click;
                MainStackPanel.Children.Add(stop_button);

                pause_button = new Button();
                pause_button.Height = 15;
                pause_button.Width = 100;
                pause_button.HorizontalAlignment = HorizontalAlignment.Right;
                pause_button.VerticalAlignment = VerticalAlignment.Center;
                pause_button.FontSize = 8;
                pause_button.Margin = new Thickness(20, -55, 40, 0);
                pause_button.Content = "Pause";
                pause_button.Name = currentThread.Name + "pause_btn";
                pause_button.Click += PauseSpecific_btn_Click;
                MainStackPanel.Children.Add(pause_button);
            });
        }

        public void UpdatePGBar(ProgressChangedEventArgs e)
        {
            ProgressBar progressBar = null;
            Label label1 = null;
            Label label3 = null;

            foreach (var child in MainStackPanel.Children)
            {
                if ((child as FrameworkElement)?.Name == e.UserState.ToString()+ "pgbar")
                {
                    progressBar = child as ProgressBar;
                    progressBar.Value = e.ProgressPercentage;
                }
                if ((child as FrameworkElement)?.Name == e.UserState.ToString() + "label1")
                {
                    label1 = child as Label;
                    label1.Content = e.ProgressPercentage + "%";
                }
                if ((child as FrameworkElement)?.Name == e.UserState.ToString() + "label3")
                {
                    if (e.ProgressPercentage == 100) { label3 = child as Label; label3.Content = ""; }
                }
            }
            if (e.ProgressPercentage == 100)
            {
                MainStackPanel.Children.Remove(label1);
                MainStackPanel.Children.Remove(label3);
                MainStackPanel.Children.Remove(progressBar);
            }
        }

        private void Pause_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Pause_btn.Content == "Continue") { Pause_btn.Content = "Pause"; }
            else { Pause_btn.Content = "Continue"; }
            Button btn = sender as Button;
            foreach (var child in MainStackPanel.Children)
            {
                if (((child as FrameworkElement)?.Name).Contains("pause_btn"))
                {
                    btn = child as Button;
                    if (btn.Content == "Pause") { btn.Content = "Continue"; }
                    else { btn.Content = "Pause"; }
                }
            }
            ViewModel.VM_PauseThreads();
        }

        private void Stop_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Pause_btn.Content == "Continue") { Pause_btn.Content = "Pause"; }
            ViewModel.VM_StopThreads();
        }

        private void PauseSpecific_btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == "Pause") { btn.Content = "Continue"; }
            else { btn.Content = "Pause"; }
            string WorkName = btn.Name.Replace("pause_btn", "");
            ViewModel.VM_PauseSpecificThread(WorkName);
        }

        private void StopSpecific_btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string WorkName = btn.Name.Replace("stop_btn", "");
            ViewModel.VM_StopSpecificThread(WorkName);
        }

        public void BlacklistPause()
        {
            while (currentMainWindow.ApplicationOn)
            {
                while (ViewModel.VM_SaveOngoing() && currentMainWindow.ApplicationOn)
                {
                    if (ViewModel.VM_BlackListTest())
                    {
                        ViewModel.VM_ForcePause();
                        while (ViewModel.VM_BlackListTest() && currentMainWindow.ApplicationOn) { Thread.Sleep(1000); }
                        ViewModel.VM_ForcePause();
                    }
                    Thread.Sleep(500);
                }
            }
        }
    }
}
