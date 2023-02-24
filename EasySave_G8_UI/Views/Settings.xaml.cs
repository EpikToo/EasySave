using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasySave_G8_UI.Models;
using EasySave_G8_UI.View_Models;
using EasySave_G8_UI.Views.Works;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EasySave_G8_UI.Views
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private View_Model ViewModel;
        public Settings()
        {
            InitializeComponent();
            ViewModel= new View_Model();
            ListRefresh();
            translate();
        }

        private void translate()
        {
            Settings_Title.Text = $"{View_Model.VM_GetString_Language("settings")}";
            txtBlacklistAdd.Text = $"{View_Model.VM_GetString_Language("blacklist_add")}";
            Blacklist_add_btn.Content = $"{View_Model.VM_GetString_Language("add")}";
            txtBlacklistRemove.Text = $"{View_Model.VM_GetString_Language("blacklist_rm")}";
            Blacklist_rm_btn.Content = $"{View_Model.VM_GetString_Language("remove")}";
            Prioritylist_Name.Text = $"{View_Model.VM_GetString_Language("prioritylist_add")}";
            Prioritylist_add_btn1.Content = $"{View_Model.VM_GetString_Language("add")}";
            Prioritylist_rm.Text = $"{View_Model.VM_GetString_Language("prioritylist_rm")}";
            Prioritylist_rm_btn1.Content = $"{View_Model.VM_GetString_Language("remove")}";
            Extensionlist_add1_txt.Text = $"{View_Model.VM_GetString_Language("extensionlist_add")}";
            Extensionlist_add_btn.Content = $"{View_Model.VM_GetString_Language("add")}";
            Extensionlist_rm_txt.Text = $"{View_Model.VM_GetString_Language("extensionlist_rm")}";
            Extensionlist_rm_btn.Content = $"{View_Model.VM_GetString_Language("remove")}";
            Size_add_txt.Text = $"{View_Model.VM_GetString_Language("size_add")}";
            Size_add_btn.Content = $"{View_Model.VM_GetString_Language("add")}";
            Actual_Size.Text = $"{View_Model.VM_GetString_Language("actual_size")}";
        }

        private void Blacklist_add_btn_Click(object sender, RoutedEventArgs e)
        {
            string ProcessName = Blacklist_add.Text;
            if (ProcessName == "") { return; }
            ViewModel.VM_BlackListAdd(ProcessName);
            PageRefresh();
        }

        private void Blacklist_rm_btn_Click(object sender, RoutedEventArgs e)
        {
            string ProcessNameRm = Blacklist_rm_combobox.Text;
            ViewModel.VM_BlackListRemove(ProcessNameRm);
            ListRefresh();
            PageRefresh();
        }

        private void ListRefresh()
        {
            List<string> blacklist = ViewModel.MV_Blacklist();
            int i;
            for (i = 0; i < blacklist.Count; i++)
            {
                Blacklist_rm_combobox.Items.Add(blacklist[i]);
            }
            List<string> prioritylist = ViewModel.MV_PriorityListRe();
            for (i = 0; i < prioritylist.Count; i++)
            {
                PriorityNumer_combobox.Items.Add(i);
                Prioritylist_rm_combobox1.Items.Add(prioritylist[i]);
            }
            PriorityNumer_combobox.Items.Add(prioritylist.Count);

            List<string> CSExtList = ViewModel.MV_ExtensionListRe();
            for (i = 0; i < CSExtList.Count; i++)
            {
                Extensionlist_rm_combobox1.Items.Add(CSExtList[i]);
            }
            Display_Size.Text = ViewModel.MV_NbKoReturn().ToString();
        }

        private void PageRefresh()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Main.Content = new Settings();
        }


        private void Prioritylist_add_btn_Click(object sender, RoutedEventArgs e)
        {
            string ExtensionName = Prioritylist_add1.Text;
            if (ExtensionName == "") { return; }
            int index = PriorityNumer_combobox.SelectedIndex;
            if (index == -1) { index = 0; }
            ViewModel.VM_PriorityListAdd(ExtensionName, index);
            PageRefresh();

        }
        private void Prioritylist_rm_btn_Click(object sender, RoutedEventArgs e)
        {
            string ExtensionName = Prioritylist_rm_combobox1.Text;
            ViewModel.VM_PriorityListRemove(ExtensionName);
            ListRefresh();
            PageRefresh();
        }

        private void Extensionlist_rm_btn_Click(object sender, RoutedEventArgs e)
        {
            string CSExtensionName = Extensionlist_rm_combobox1.Text;
            ViewModel.Extensionlist_rm_btn_Click2(CSExtensionName);
            ListRefresh();
            PageRefresh();
        }
        private void Extensionlist_add_btn_Click(object sender, RoutedEventArgs e)
        {
            string CSExtensionName = Extensionlist_add1.Text;
            if (CSExtensionName == "") { return; }
            ViewModel.VM_ExtensionListAdd(CSExtensionName);
            PageRefresh();
        }


        private void Size_add_btn_Click(object sender, RoutedEventArgs e)
        {
            double Size;
            if (Double.TryParse(Size_txbx.Text, out Size))
            {
                ViewModel.VM_NbKoSet(Size);
                MessageBox.Show("Size is Saved !");
                PageRefresh();
            }
        }
        public double MV_NbKoReturn()
        {
            Model_NBKO modelNbko = new Model_NBKO();
            double nbKo = modelNbko.NbKoReturn();
            return nbKo;
        }

        public void VM_NbKoSet(double nbko)
        {
            Model_NBKO modelNbko = new Model_NBKO();
            modelNbko.NbKoSet(nbko);
        }


    }
}
