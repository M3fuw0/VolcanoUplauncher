using System;
using System.Windows.Forms;

namespace Uplauncher.MultiCompte2
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();

            if (!Form1.IsOpen)
            {
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("Une instance de MultiDofus est déjà ouverte.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
