﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace TrinityCoreAdmin.Forms
{
    public partial class MainForm : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        public MainForm()
        {
            InitializeComponent();

            // Create an instance of a ListView column sorter and assign it
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.listViewAccounts.ListViewItemSorter = lvwColumnSorter;

            if (Properties.Settings.Default.firstStart)
                new FirstStartForm().ShowDialog();
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void einstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm frmSettings = new SettingsForm();
            frmSettings.ShowInTaskbar = false;
            frmSettings.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServerManager.LoadRealms();
        }

        private void listViewAccounts_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listViewAccounts.Sort();
        }

        public void authDBConn_OnToggleConnectionStateHandler(object sender, OnConnectionStateEventArgs e)
        {
            if (e.connState == ConnectionState.Open)
            {
                this.statusStripChar.ForeColor = Color.Green;
            }
            else if (e.connState == ConnectionState.Closed)
            {
                this.statusStripChar.ForeColor = Color.Red;
            }
        }

        public void sshConn_OnToggleConnectionStateHandler(object sender, OnConnectionStateEventArgs e)
        {
            if (e.connState == ConnectionState.Open)
            {
                this.statusStripSSH.ForeColor = Color.Green;
            }
            else if (e.connState == ConnectionState.Closed)
            {
                this.statusStripSSH.ForeColor = Color.Red;
            }
        }

        private void LoadAccounts()
        {
            listViewAccounts.Items.Clear();
            listViewAccounts.Update();

            foreach (Account acc in Account.GetAccounts())
            {
                ListViewItem item = new ListViewItem(acc.id.ToString());
                item.UseItemStyleForSubItems = false;

                if (acc.online)
                    item.SubItems[0].BackColor = Color.Green;
                else
                    item.SubItems[0].BackColor = Color.Red;

                item.SubItems.Add(acc.username);
                item.SubItems.Add(acc.reg_mail);
                item.SubItems.Add(acc.email);
                item.SubItems.Add(acc.joindate.ToShortDateString());
                item.SubItems.Add(acc.last_ip);
                item.SubItems.Add(acc.failed_logins.ToString());
                item.SubItems.Add(acc.last_login.ToString());
                item.SubItems.Add(acc.expansion.ToString());

                item.Tag = acc;

                listViewAccounts.Items.Add(item);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MySQLConnection.CloseConnections();
            SshConnection.CloseConnections();
        }

        private void realmmanagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RealmManagerForm frmRealmManager = new RealmManagerForm(this);
            frmRealmManager.ShowInTaskbar = false;
            frmRealmManager.ShowDialog();

            if (frmRealmManager.connSuccess)
                LoadAccounts();
        }

        private void verbindenToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripCloseConnections_Click(object sender, EventArgs e)
        {
            MySQLConnection.CloseConnections();
            SshConnection.CloseConnections();
            Account.ClearAccounts();
        }

        private void toolStripBtnDisconnect_Click(object sender, EventArgs e)
        {
            MySQLConnection.CloseConnections();
            SshConnection.CloseConnections();
            Account.ClearAccounts();
        }

        private void listViewAccounts_DoubleClick(object sender, EventArgs e)
        {
            new EditAccountForm((Account)listViewAccounts.SelectedItems[0].Tag).Show();
        }

        public void ResetStatusStripColors()
        {
            statusStripAuth.ForeColor = Color.Gray;
            statusStripChar.ForeColor = Color.Gray;
            statusStripWorld.ForeColor = Color.Gray;
            statusStripSSH.ForeColor = Color.Gray;
        }
    }
}