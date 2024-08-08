using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Uplauncher
{
    /// <summary>
    /// Logique d'interaction pour SelectDofusClientsWindow.xaml
    /// </summary>
    public partial class SelectDofusClientsWindow : Window
    {
        public SelectDofusClientsWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UplauncherModelView uplauncherModelView)
            {
                if (int.TryParse(ClientsNumberComboBox.SelectedValue.ToString(), out int numberOfClients))
                {
                    // Add 1 to numberOfClients if it is 2 or more
                    if (numberOfClients >= 2)
                    {
                        numberOfClients += 1;
                    }

                    // Now numberOfClients will be incremented by 1 if it was 2 or more
                    uplauncherModelView.NumberOfClientsToStart = numberOfClients;
                    Close();
                    // Uncomment if needed for debugging
                    // MessageBox.Show($"Nombre de clients sélectionné : {numberOfClients}");
                }
            }
        }
    }
}
