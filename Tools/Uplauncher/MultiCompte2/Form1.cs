﻿using Mono.Cecil.Cil;
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
//using Uplauncher.Multi.Dofus_2;
//using Uplauncher.Multi.Dofus_2;
using Uplauncher.MultiCompte2.Composants;
//using Uplauncher.MultiDofus.ViewModels;
//using Uplauncher.MultiDofus;
using Logger = Uplauncher.MultiCompte2.Composants.Logger;
using Struct = Uplauncher.MultiCompte2.Composants.Struct;
using System.Threading.Tasks;

namespace Uplauncher.MultiCompte2
{
    public partial class Form1 : Form
    {
		private bool tabPagesOrderChangeAllowed = true;
		private Logger logger = new Logger();
		private string newLine = Environment.NewLine;
        public static Form1 Instance { get; private set; }
        public static bool IsOpen { get; private set; } = false;
        public Form1()
        {
            InitializeComponent();
            Instance = this; // Assigner l'instance actuelle à la propriété statique.
            Resize += Form1_Resize;
            Load += Form1_Load;
            HOTKEY_ACTIVE += Start;
            FormClosing += Form1_FormClosing;
            //InitializeComponent();
            FormClosing += (sender, args) => IsOpen = false; // Assurez-vous de mettre IsOpen à false lorsque le formulaire se ferme
            Load += (sender, args) => IsOpen = true; // Mettre IsOpen à true lorsque le formulaire se charge
            tabControl1.AllowDrop = true; //Permet de bouger les onglets
            tabPagesOrderChangeAllowed = true; //Permet de changer l'ordre
        }

        public delegate void HOTKEY_ACTIVEEventHandler(int id);
        private HOTKEY_ACTIVEEventHandler HOTKEY_ACTIVEEvent;

        public event HOTKEY_ACTIVEEventHandler HOTKEY_ACTIVE
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add => HOTKEY_ACTIVEEvent = (HOTKEY_ACTIVEEventHandler)Delegate.Combine(HOTKEY_ACTIVEEvent, value);
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove => HOTKEY_ACTIVEEvent = (HOTKEY_ACTIVEEventHandler)Delegate.Remove(HOTKEY_ACTIVEEvent, value);
        }

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Core.Mainfct(2, tabControl1, Handle);
            //Instance?.Fermer();
        }

        public void Fermer()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Close));
            }
            else
            {
                Close();
            }
        }

        private void Start(int id)
		{
			checked
			{
				try
				{
					switch (id)
					{
						case 444719:
							{
								int selectedIndex = tabControl1.SelectedIndex;
								if (selectedIndex == tabControl1.Controls.Count - 1)
								{
									selectedIndex = 0;
									tabControl1.SelectedIndex = 0;
								}
								else
								{
									selectedIndex++;
									tabControl1.SelectedIndex = selectedIndex;
								}
								break;
							}
						case 1:
							foreach (Struct.Processus liste_Processu in Struct.Liste_Processus)
							{
								liste_Processu.rename();
							}
							break;
					}
				}
				catch (Exception ex)
				{
					logger.WriteLog("Module30_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
				}
			}
		}

        private void Form1_Load(object sender, EventArgs e)
        {
			Core.Mainfct(1, tabControl1, Handle);
			Api.RegisterHotKey(Handle, 1, (int)HotKey.FsModifiers.None, (int)Keys.F5);
            //HotKey.RegisterGlobalHotKey((int)HotKey.FsModifiers.None, (int)Keys.F5, Handle);
        }

		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;
			if (msg == 786)
			{
				if (m.WParam.ToInt32() == 444719)
				{
					HOTKEY_ACTIVEEvent?.Invoke(444719);
				}
				if (m.WParam.ToInt32() == 1)
				{
					HOTKEY_ACTIVEEvent?.Invoke(1);
				}
			}
			base.WndProc(ref m);
		}

        private void Form1_Resize(object sender, EventArgs e)
        {
			Core.Mainfct(1, tabControl1, Handle);
			Api.RegisterHotKey(Handle, 1, (int)HotKey.FsModifiers.None, (int)Keys.F5);
            //HotKey.RegisterGlobalHotKey((int)HotKey.FsModifiers.None, (int)Keys.F5, Handle);
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Configuration configuration = new Configuration();
			configuration.ShowDialog();
        }

        private void optionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Configuration configuration = new Configuration();
            configuration.ShowDialog();
        }

        private void toggle1_CheckedChanged(object sender)
        {
			if (timer1.Enabled)
			{
				timer1.Stop();
				return;
			}
			MessageBox.Show("Le déplacement auto est fonctionnel, il faut appuyer sur le bouton milieu de la souris.");
			timer1.Start();
		}

        private void timer1_Tick(object sender, EventArgs e)
        {
			Core.Mainfct(8, tabControl1, Handle);
		}

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
			Core.DragStartPosition = new Point(e.X, e.Y);
		}

        private void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
			if (e.Button == MouseButtons.Left)
			{
				Rectangle rectangle = new Rectangle(Core.DragStartPosition, Size.Empty);
				rectangle.Inflate(SystemInformation.DragSize);
				TabPage tabPage = Core.HoverTab(tabControl1);
				if (tabPage != null && !rectangle.Contains(e.X, e.Y))
				{
					tabControl1.DoDragDrop(tabPage, DragDropEffects.All);
				}
				Core.DragStartPosition = Point.Empty;
			}
		}

        private void tabControl1_DragOver(object sender, DragEventArgs e)
        {
			TabPage tabPage = Core.HoverTab(tabControl1);
			if (tabPage == null)
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				if (!e.Data.GetDataPresent(typeof(TabPage)))
				{
					return;
				}
				e.Effect = DragDropEffects.Move;
				TabPage tabPage2 = (TabPage)e.Data.GetData(typeof(TabPage));
				if (tabPage != tabPage2)
				{
					Rectangle tabRect = tabControl1.GetTabRect(tabControl1.TabPages.IndexOf(tabPage));
					tabRect.Inflate(-3, -3);
					TabControl tabControl = tabControl1;
					Point p = new Point(e.X, e.Y);
					if (tabRect.Contains(tabControl.PointToClient(p)))
					{
						Core.SwapTabPages(tabControl, tabPage2, tabPage);
						tabControl1.SelectedTab = tabPage2;
					}
				}
			}
		}

        private void fermerToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Core.Mainfct(4, tabControl1, Handle);
			Core.Mainfct(1, tabControl1, Handle);
		}

        private void lancerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			Core.Mainfct(3, tabControl1, Handle);
			Core.Mainfct(1, tabControl1, Handle);
		}

        private void configurationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			Configuration configuration = new Configuration();
			configuration.ShowDialog();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
			Core.Mainfct(7, tabControl1, Handle);
		}

        private void lancerToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Core.Mainfct(3, tabControl1, Handle);
			Core.Mainfct(1, tabControl1, Handle);
		}

        private void renommerToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Core.Mainfct(5, tabControl1, Handle);
		}

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			//if (!tabPagesOrderChangeAllowed)
			//{
			//	toolStripMenuItem3.Text = "Désactiver";
			//	tabControl1.AllowDrop = true;
			//	tabPagesOrderChangeAllowed = true;
			//}
			//else
			//{
			//	toolStripMenuItem3.Text = "Activer";
			//	tabControl1.AllowDrop = false;
			//	tabPagesOrderChangeAllowed = false;
			//}
		}

        private void aProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Récupérez les informations d'assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyProductAttribute productAttr = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyDescriptionAttribute descriptionAttr = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            AssemblyCompanyAttribute companyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            AssemblyCopyrightAttribute copyrightAttr = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            string version = assembly.GetName().Version.ToString();

            // Construisez le message pour la boîte de dialogue
            string message = $"Multi-Dofus Pyrasis\n\nLe Multi-Dofus a été redéveloppé pour Pyrasis.\n\nVersion : {version}\n\n{copyrightAttr.Copyright}";

            // Affichez le message
            MessageBox.Show(message, "À propos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void capturerLesFenêtresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Votre logique pour capturer les fenêtres ici
            Core.Mainfct(10, tabControl1, Handle);
            Core.Mainfct(9, tabControl1, Handle);
        }

        private void lancerDofusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Mainfct(3, tabControl1, Handle);
            Task.Delay(5000);
            Core.Mainfct(1, tabControl1, Handle);
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Mainfct(2, tabControl1, Handle);
        }

        //private void multiDofusV2ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    MainMultiDofus multi = new MainViewModel(new MainMultiDofus()).View;
        //    multi.Show();
        //    Instance?.Fermer();
        //}

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration configuration = new Configuration();
            configuration.ShowDialog();
        }
    }
}
