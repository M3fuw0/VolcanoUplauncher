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
    /// Logique d'interaction pour NumberOfDofusWindows.xaml
    /// </summary>
    public partial class NumberOfDofusWindows : Window
    {
        public NumberOfDofusWindows()
        {
            InitializeComponent();
        }

        private void OpenDofusWindows(object sender, RoutedEventArgs e)
        {
            // Ici, ajoute la logique pour ouvrir le nombre de fenêtres Dofus sélectionné
            // Exemple : int numberOfWindows = Convert.ToInt32(NumberComboBox.SelectedValue);
        }
    }
}
