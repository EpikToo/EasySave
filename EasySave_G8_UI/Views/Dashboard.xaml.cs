using EasySave_G8_UI.View_Models;
using System.Windows.Controls;

namespace EasySave_G8_UI.Views
{
    /// <summary>
    /// Logique d'interaction pour Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            translate();
        }

        private void translate()
        {
            DashboardTitle.Text = $"{View_Model.VM_GetString_Language("dashboard_title")}";
        }
    }
}
