using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Uplauncher.MultiCompte2
{
   partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.multiCompteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capturerLesFenêtresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lancerDofusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            //this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.aProposToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lancerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lancerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renommerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fermerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.toggle1 = new Uplauncher.MultiCompte2.Composants.Toggle();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            //this.multiDofusV2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.multiCompteToolStripMenuItem,
            //this.multiDofusV2ToolStripMenuItem,
            //this.toolStripMenuItem2,
            this.aProposToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(686, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // multiCompteToolStripMenuItem
            // 
            this.multiCompteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.capturerLesFenêtresToolStripMenuItem,
            this.lancerDofusToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.quitterToolStripMenuItem});
            this.multiCompteToolStripMenuItem.Name = "multiCompteToolStripMenuItem";
            this.multiCompteToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.multiCompteToolStripMenuItem.Text = "MultiCompte";
            // 
            // capturerLesFenêtresToolStripMenuItem
            // 
            this.capturerLesFenêtresToolStripMenuItem.Name = "capturerLesFenêtresToolStripMenuItem";
            this.capturerLesFenêtresToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.capturerLesFenêtresToolStripMenuItem.Text = "Capturer les fenêtres";
            this.capturerLesFenêtresToolStripMenuItem.Click += new System.EventHandler(this.capturerLesFenêtresToolStripMenuItem_Click);
            // 
            // lancerDofusToolStripMenuItem
            // 
            this.lancerDofusToolStripMenuItem.Name = "lancerDofusToolStripMenuItem";
            this.lancerDofusToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.lancerDofusToolStripMenuItem.Text = "Lancer Dofus";
            this.lancerDofusToolStripMenuItem.Click += new System.EventHandler(this.lancerDofusToolStripMenuItem_Click);
            // 
            // quitterToolStripMenuItem
            // 
            this.quitterToolStripMenuItem.Name = "quitterToolStripMenuItem";
            this.quitterToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.quitterToolStripMenuItem.Text = "Quitter";
            this.quitterToolStripMenuItem.Click += new System.EventHandler(this.quitterToolStripMenuItem_Click);
            //// 
            //// toolStripMenuItem2
            //// 
            //this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this.toolStripMenuItem3});
            //this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            //this.toolStripMenuItem2.Size = new System.Drawing.Size(147, 20);
            //this.toolStripMenuItem2.Text = "Organisation des Clients";
            //// 
            //// toolStripMenuItem3
            //// 
            //this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            //this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            //this.toolStripMenuItem3.Text = "Activer";
            //this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // aProposToolStripMenuItem
            // 
            this.aProposToolStripMenuItem.Name = "aProposToolStripMenuItem";
            this.aProposToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.aProposToolStripMenuItem.Text = "À propos";
            this.aProposToolStripMenuItem.Click += new System.EventHandler(this.aProposToolStripMenuItem_Click);
            // 
            // lancerToolStripMenuItem
            // 
            this.lancerToolStripMenuItem.Name = "lancerToolStripMenuItem";
            this.lancerToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lancerToolStripMenuItem1,
            this.renommerToolStripMenuItem,
            this.fermerToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(134, 70);
            // 
            // lancerToolStripMenuItem1
            // 
            this.lancerToolStripMenuItem1.Name = "lancerToolStripMenuItem1";
            this.lancerToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
            this.lancerToolStripMenuItem1.Text = "Lancer";
            this.lancerToolStripMenuItem1.Click += new System.EventHandler(this.lancerToolStripMenuItem1_Click);
            // 
            // renommerToolStripMenuItem
            // 
            this.renommerToolStripMenuItem.Name = "renommerToolStripMenuItem";
            this.renommerToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.renommerToolStripMenuItem.Text = "Renommer";
            this.renommerToolStripMenuItem.Click += new System.EventHandler(this.renommerToolStripMenuItem_Click);
            // 
            // fermerToolStripMenuItem
            // 
            this.fermerToolStripMenuItem.Name = "fermerToolStripMenuItem";
            this.fermerToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.fermerToolStripMenuItem.Text = "Fermer";
            this.fermerToolStripMenuItem.Click += new System.EventHandler(this.fermerToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(686, 366);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.DragOver += new System.Windows.Forms.DragEventHandler(this.tabControl1_DragOver);
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
            this.tabControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseMove);
            // 
            // toggle1
            // 
            this.toggle1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggle1.Checked = false;
            this.toggle1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(132)))), ((int)(((byte)(188)))));
            this.toggle1.Icons = false;
            this.toggle1.Location = new System.Drawing.Point(638, 5);
            this.toggle1.Name = "toggle1";
            this.toggle1.Size = new System.Drawing.Size(38, 16);
            this.toggle1.TabIndex = 2;
            this.toggle1.CheckedChanged += new Uplauncher.MultiCompte2.Composants.Toggle.CheckedChangedEventHandler(this.toggle1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(549, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Suivre perso actif";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // multiDofusV2ToolStripMenuItem
            // 
            //this.multiDofusV2ToolStripMenuItem.Name = "multiDofusV2ToolStripMenuItem";
            //this.multiDofusV2ToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            //this.multiDofusV2ToolStripMenuItem.Text = "MultiDofus V2";
            //this.multiDofusV2ToolStripMenuItem.Click += new System.EventHandler(this.multiDofusV2ToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 390);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toggle1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = global::Uplauncher.Icon.pyrasis_m;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multi-Compte Pyrasis";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        [AccessedThroughProperty("tabControl1")]
        private System.Windows.Forms.TabControl tabControl1;
        private ToolStripMenuItem lancerToolStripMenuItem;
        private ToolStripMenuItem configurationToolStripMenuItem;
        private Uplauncher.MultiCompte2.Composants.Toggle toggle1;
        private Label label1;
        private Timer timer1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem lancerToolStripMenuItem1;
        private ToolStripMenuItem fermerToolStripMenuItem;
        private ToolStripMenuItem renommerToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem aProposToolStripMenuItem;
        private ToolStripMenuItem multiCompteToolStripMenuItem;
        private ToolStripMenuItem capturerLesFenêtresToolStripMenuItem;
        private ToolStripMenuItem lancerDofusToolStripMenuItem;
        private ToolStripMenuItem optionToolStripMenuItem;
        private ToolStripMenuItem quitterToolStripMenuItem;
        //private ToolStripMenuItem multiDofusV2ToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        //private ContextMenuStrip ContextMenuStrip1;
        //private ToolStripMenuItem AProposToolStripMenuItem;
        //private ToolStripMenuItem CapturerFenetresToolStripMenuItem;
        //private ToolStripMenuItem AjouterToolStripMenuItem;
        //private ToolStripMenuItem SupprimerToolStripMenuItem;
        //private ToolStripMenuItem RenommerToolStripMenuItem;
        //private MenuStrip MenuStrip1;
        //private ToolStripMenuItem OptionsToolStripMenuItem;
        //private ToolStripMenuItem LancerDofusToolStripMenuItem;
        //private ToolStripMenuItem OptionsToolStripMenuItem1;
        //private ToolStripMenuItem QuitterToolStripMenuItem;
    }
}

