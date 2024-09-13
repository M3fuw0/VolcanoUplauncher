using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using Uplauncher.Sound;
using Uplauncher.Sound.UplSound;
using CheckBox = System.Windows.Forms.CheckBox;
using ComboBox = System.Windows.Forms.ComboBox;
using Control = System.Windows.Forms.Control;
using Panel = System.Windows.Forms.Panel;
using TabControl = System.Windows.Forms.TabControl;

namespace Uplauncher.MultiCompte2.Composants
{
    internal class Core
    {
		public static Point DragStartPosition = Point.Empty;

		public const int GWL_STYLE = -16;

		public const int WS_CAPTION = 12582912;

		public const int WS_BORDER = 8388608;

		private static bool SoundLaunched = false;

        private static readonly SoundProxy m_soundProxy = new SoundProxy();
        private static UpServer server = new UpServer();

        private static Process m_regProcess;

        public static Logger logger = new Logger();
		public static string newLine = Environment.NewLine;

		public static long Transforme_Intptr(int loWord, int hiWord)
		{
			return checked(hiWord * 65536L) | (loWord & 0xFFFFL);
		}


		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void Mainfct(int act, TabControl tabControl, IntPtr handle)
		{
			//Discarded unreachable code: IL_01a7, IL_01ff, IL_052d
			switch (act)
			{
				case 1:
					try
					{
						if (Operators.ConditionalCompareObjectEqual(param("path"), "", TextCompare: false))
						{
							reset();
						}
						tabControl.TabPages.Clear();
						foreach (Struct.Processus liste_Processu in Struct.Liste_Processus)
						{
							//liste_Processu.release();
                            //if (!liste_Processu.Process.MainModule.FileName.EndsWith("Reg.exe", StringComparison.OrdinalIgnoreCase))
                            //{
                                liste_Processu.release();
                            //}
                        }
						Struct.Liste_Processus.Clear();
					}
					catch (Exception ex13)
					{
						MessageBox.Show("Erreur : Module 1");
                        //logger.WriteLog("Module1_Error", "Message :" + newLine + ex13.Message + newLine + newLine + "StackTrace :" + newLine + ex13.StackTrace);
                        MessageBox.Show("Message :" + newLine + ex13.Message + newLine + newLine + "StackTrace :" + newLine + ex13.StackTrace,"Erreur : Module 1");
                    }
					try
					{
						Process[] processes = Process.GetProcesses();
						foreach (Process process in processes)
						{
							if (Operators.CompareString(process.ProcessName, "Dofus", TextCompare: false) == 0)
							{
                                TabPage tabPage = new TabPage
                                {
                                    Text = "Dofus",
                                    ToolTipText = process.Id.ToString()
                                };
                                Panel panel = new Panel
                                {
                                    Dock = DockStyle.Fill
                                };
                                Struct.Processus item = new Struct.Processus
                                {
                                    Process = process,
                                    Panel = panel,
                                    Tab = tabPage
                                };
                                tabControl.TabPages.Add(tabPage);
								tabPage.Controls.Add(panel);
								Struct.Liste_Processus.Add(item);
							}
						}
						foreach (Struct.Processus liste_Processu2 in Struct.Liste_Processus)
						{
							//liste_Processu2.rec(@bool: true);
                            liste_Processu2.rec(true);
                        }
					}
					catch (Exception ex15)
					{
						MessageBox.Show("Erreur : Module 2");
                        //logger.WriteLog("Module2_Error", "Message :" + newLine + ex15.Message + newLine + newLine + "StackTrace :" + newLine + ex15.StackTrace);
                        MessageBox.Show("Message :" + newLine + ex15.Message + newLine + newLine + "StackTrace :" + newLine + ex15.StackTrace,"Erreur : Module 2");
                    }
					try
					{
						foreach (Struct.Processus liste_Processu3 in Struct.Liste_Processus)
						{
							liste_Processu3.Redimentionne();
						}
					}
					catch (Exception ex17)
					{
						MessageBox.Show("Erreur : Module 3");
						//logger.WriteLog("Module3_Error", "Message :" + newLine + ex17.Message + newLine + newLine + "StackTrace :" + newLine + ex17.StackTrace);
					}
					break;
				case 2:
					try
					{
                        Api.UnregisterHotKey(handle, 444719); // Utilisez la fonction API pour désenregistrer le raccourci.
                        //HotKey.UnregisterGlobalHotKey(444719, handle);
                        Api.UnregisterHotKey(handle, 1);
                        //HotKey.UnregisterGlobalHotKey(1, handle);
                        foreach (Struct.Processus liste_Processu4 in Struct.Liste_Processus)
						{
							liste_Processu4.Process.Kill();
						}
                        //Application.Exit();
                        //ProjectData.EndApp();
                        // Assurez-vous que 'Form1.Instance' est bien l'instance en cours et non une nouvelle instance.
                        Form1.Instance?.Fermer();
                    }
					catch (Exception ex11)
					{
						MessageBox.Show("Erreur : Module 4");
						////logger.WriteLog("Module4_Error", "Message :" + newLine + ex11.Message + newLine + newLine + "StackTrace :" + newLine + ex11.StackTrace);
					}
					break;
				case 3:
					try
					{
						if (Operators.ConditionalCompareObjectEqual(param("path"), "", TextCompare: false))
						{
                        	DialogResult defaultPathResult = MessageBox.Show("Voulez-vous utiliser le chemin par défaut pour Pyrasis ?", "Chemin par défaut", MessageBoxButtons.YesNo);

                        	if (defaultPathResult == DialogResult.Yes)
                        	{
                            	string defaultPath = @"C:\Program Files(x86)\Pyrasis\pyrasis_app";
                            	if (File.Exists(defaultPath + @"\Dofus.exe"))
                            	{
                                	reset();
                                	wparam("path", defaultPath + @"\Dofus.exe");
                            	}
                            	else
                            	{
                                	MessageBox.Show("Dofus.exe n'est pas trouvé dans le chemin par défaut de Pyrasis.");
                                	// Vous pouvez éventuellement demander à l'utilisateur de choisir un autre chemin ici.
                            	}
                        	}
                        	else
							{
								FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
								folderBrowserDialog.Description = "Veuillez selectionner le dossier contenant Dofus.exe";
								DialogResult dialogResult = folderBrowserDialog.ShowDialog();
								if (dialogResult == DialogResult.OK)
								{
									if (File.Exists(folderBrowserDialog.SelectedPath + "\\Dofus.exe"))
									{
										reset();
										wparam("path", folderBrowserDialog.SelectedPath + "\\Dofus.exe");
										if (File.Exists(folderBrowserDialog.SelectedPath + "\\reg\\Reg.exe"))
										{
											wparam("soundpath", folderBrowserDialog.SelectedPath + "\\reg\\Reg.exe");
										}
									}
									else
									{
										MessageBox.Show("Ce dossier ne semble pas être celui de dofus");
									}
								}
								else
								{
									MessageBox.Show("Aucun dossier selectionné");
								}
							}
						}
						else
						{
							int num = Conversions.ToInteger(param("winOpen"));
							int num2 = num;
							if (Conversions.ToInteger(param("isSoundActivated")) == 1)
							{
								try
								{
									if (!SoundLaunched)
									{
										//Process process = new Process();
                                        //ProcessStartInfo startInfo = process.StartInfo;
										//startInfo.FileName = Conversions.ToString(param("soundpath"));
										//process.Start();
                                        //ManagerSound.Init();
                                        //Server server = new Server();
                                        //server.StartAuthentificate();
                                        server.StartAuthentificate();
                                        Initialization.Init();
                                        m_soundProxy.StartProxy();
                                        SoundLaunched = true;
									}
								}
								catch (Exception ex)
								{
                                    //logger.WriteLog("ModuleSound_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
                                    MessageBox.Show("Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace, "ModuleSound_Error");
                                }
							}
							for (int i = 1; i <= num2; i = checked(i + 1))
							{
                                if (m_soundProxy.Started && (m_regProcess == null || m_regProcess.HasExited))
                                {
                                    StartRegApp();
                                }
                                Process process = new Process
                            	{
                                	StartInfo = new ProcessStartInfo(Conversions.ToString(param("path")), m_soundProxy.Started ? ("--reg-client-port=" + m_soundProxy.ClientPort) : string.Empty)
                            	};
                            	process.Start();
								//Interaction.Shell(Conversions.ToString(param("path")));
								Thread.Sleep(10000);
							}
						}
					}
					catch (Exception ex9)
					{
						MessageBox.Show("Erreur : Module 5");
                        //logger.WriteLog("Module5_Error", "Message :" + newLine + ex9.Message + newLine + newLine + "StackTrace :" + newLine + ex9.StackTrace);
                        MessageBox.Show("Message :" + newLine + ex9.Message + newLine + newLine + "StackTrace :" + newLine + ex9.StackTrace,"Erreur : Module 5");
                    }
					break;
				case 4:
					try
					{
						if (tabControl.Controls.Count != 0)
						{							
							int processIdToKill = Convert.ToInt32(tabControl.SelectedTab.ToolTipText);
							foreach (Struct.Processus liste_Processu5 in Struct.Liste_Processus)
							{
                                if (liste_Processu5.Process.Id == processIdToKill)
                                {
                                    liste_Processu5.Process.Kill();
                                    tabControl.TabPages.Remove(tabControl.SelectedTab);
                                }
							}
						}
						else
						{
							MessageBox.Show("Aucune fenêtre ouverte");
						}
					}
					catch (Exception ex7)
					{
						MessageBox.Show("Erreur : Module 6");
						//logger.WriteLog("Module6_Error", "Message :" + newLine + ex7.Message + newLine + newLine + "StackTrace :" + newLine + ex7.StackTrace);
						MessageBox.Show("Message :" + newLine + ex7.Message + newLine + newLine + "StackTrace :" + newLine + ex7.StackTrace, "Module6_Error");
                    }
					break;
				case 5:
					try
					{
						if (tabControl.Controls.Count != 0)
						{
							string text = Interaction.InputBox("Insérer le texte pour renommer la fenêtre.");
							tabControl.SelectedTab.Text = text;
						}
					}
					catch (Exception ex5)
					{
						MessageBox.Show("Erreur : Module 7");
						//logger.WriteLog("Module7_Error", "Message :" + newLine + ex5.Message + newLine + newLine + "StackTrace :" + newLine + ex5.StackTrace);
					}
					break;
				case 7:
					try
					{
						foreach (Struct.Processus liste_Processu6 in Struct.Liste_Processus)
						{
							liste_Processu6.Redimentionne();
						}
					}
					catch (Exception ex3)
					{
						MessageBox.Show("Erreur : Module 9");
						//logger.WriteLog("Module9_Error", "Message :" + newLine + ex3.Message + newLine + newLine + "StackTrace :" + newLine + ex3.StackTrace);
					}
					break;
				case 8:
					try
					{
						MouseButtons mouseButtons = Control.MouseButtons;
						if (mouseButtons != MouseButtons.Middle)
						{
							break;
						}
						
						Struct.MOVEPOS = tabControl.SelectedTab.PointToClient(Control.MousePosition);
						IntPtr proc = Api.WindowFromPoint(Cursor.Position);
						foreach (Struct.Processus liste_Processu7 in Struct.Liste_Processus)
						{
							liste_Processu7.suivre(proc);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur : Module 10");
						//logger.WriteLog("Module10_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
					}
					break;
                case 9:
                    try
                    {
                        //Mainfct(1); // Utilisez la fonction déjà existante pour capturer les fenêtres.
                        Mainfct(1, null, new IntPtr(0));
                        Interaction.MsgBox("Fenêtres capturées avec succès.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur : Module Capturer");
                    }
                    break;
                case 10:
                    try
                    {
                        if (Operators.ConditionalCompareObjectEqual(param("path"), "", TextCompare: false))
                        {
                            DialogResult defaultPathResult = MessageBox.Show("Voulez-vous utiliser le chemin par défaut pour Pyrasis ?", "Chemin par défaut", MessageBoxButtons.YesNo);

                            if (defaultPathResult == DialogResult.Yes)
                            {
                                string defaultPath = @"C:\Program Files(x86)\Pyrasis\pyrasis_app";
                                if (File.Exists(defaultPath + @"\Dofus.exe"))
                                {
                                    reset();
                                    wparam("path", defaultPath + @"\Dofus.exe");
                                }
                                else
                                {
                                    MessageBox.Show("Dofus.exe n'est pas trouvé dans le chemin par défaut de Pyrasis.");
                                    // Vous pouvez éventuellement demander à l'utilisateur de choisir un autre chemin ici.
                                }
                            }
                        }
                        else
                        {
                            int num = Conversions.ToInteger(param("winOpen"));
                            int num2 = num;
                            for (int i = 1; i <= num2; i = checked(i + 1))
                            {
                                Interaction.Shell(Conversions.ToString(param("path")));
                                Thread.Sleep(280);
                            }
                        }
                        break;
                    }
                    catch (Exception ex9)
                    {
                    	MessageBox.Show("Erreur : Détection");
						//logger.WriteLog("Module9_Error", "Message :" + newLine + ex9.Message + newLine + newLine + "StackTrace :" + newLine + ex9.StackTrace);

                    }
                    break;
                    case 6:
                        break;
            }
		}

		public static void setparam(NumericUpDown numericUpDown, CheckBox Alt, CheckBox Ctrl, CheckBox Windows, ComboBox comboBox)
		{
			try
			{
				numericUpDown.Value = Conversions.ToDecimal(param("winOpen"));
				Alt.CheckState = (CheckState)Conversions.ToInteger(param("Alt"));
				Ctrl.CheckState = (CheckState)Conversions.ToInteger(param("Ctrl"));
				Windows.CheckState = (CheckState)Conversions.ToInteger(param("Windows"));
				comboBox.Text = Conversions.ToString(param("hotkey"));
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erreur : Module 11");
				//logger.WriteLog("Module11_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
			}
		}

		public static object param(string parm)
		{
			object result = default(object);
			try
			{
				string text = Conversions.ToString(Registry.GetValue("HKEY_CURRENT_USER\\Multi-CompteV1\\Settings", parm, ""));
				if (Operators.CompareString(text, "1", TextCompare: false) == 0)
				{
					result = CheckState.Checked;
					return result;
				}
				if (Operators.CompareString(text, "0", TextCompare: false) == 0)
				{
					result = CheckState.Unchecked;
					return result;
				}
				result = text;
				return result;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erreur : Module 12");
				//logger.WriteLog("Module12_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
				return result;
			}
		}

		public static void wparam(string par, string val)
		{
			try
			{
				Registry.SetValue("HKEY_CURRENT_USER\\Multi-CompteV1\\Settings", par, val);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erreur : Module 13");
				//logger.WriteLog("Module13_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
			}
		}

		public static void reset()
		{
			try
			{
				wparam("winOpen", "1");
				wparam("path", "");
				wparam("Alt", "0");
				wparam("Ctrl", "0");
				wparam("Windows", "0");
				wparam("hotkey", "");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erreur : Module 14");
				//logger.WriteLog("Module14_Error", "Message :" + newLine + ex.Message + newLine + newLine + "StackTrace :" + newLine + ex.StackTrace);
			}
		}

		public static TabPage HoverTab(TabControl tabControl)
		{
			checked
			{
				int num = tabControl.TabCount - 1;
				for (int i = 0; i <= num; i++)
				{
					if (tabControl.GetTabRect(i).Contains(tabControl.PointToClient(Cursor.Position)))
					{
						return tabControl.TabPages[i];
					}
				}
				return null;
			}
		}

        private static void StartRegApp()
        {
            NotifyIcon notif = new NotifyIcon();
            if (!File.Exists(Constants.DofusExePath))
            {
                notif.ShowBalloonTip(4000, Constants.ApplicationName, Constants.RegExePath + " est introuvable. Les sons ne seront pas activés.", ToolTipIcon.Warning);
                return;
            }

            m_regProcess = new Process
            {
                StartInfo = new ProcessStartInfo(Conversions.ToString(param("soundpath")), "--reg-engine-port=" + m_soundProxy.RegPort),
            };

            if (!m_regProcess.Start())
            {
                notif.ShowBalloonTip(4000, Constants.ApplicationName, "Impossible de lancer" + Constants.RegExePath + ". Raison inconnue.", ToolTipIcon.Warning);
            }
        }

        public static void SwapTabPages(TabControl tabControl, TabPage tp1, TabPage tp2)
		{
			int index = tabControl.TabPages.IndexOf(tp1);
			int index2 = tabControl.TabPages.IndexOf(tp2);
			tabControl.TabPages[index] = tp2;
			tabControl.TabPages[index2] = tp1;
		}
	}
}
