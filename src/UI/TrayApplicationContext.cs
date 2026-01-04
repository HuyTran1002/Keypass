using System;
using System.Drawing;
using System.Windows.Forms;
using Keypass.Services;

namespace Keypass.UI
{
    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip contextMenu;

        public TrayApplicationContext()
        {
            // Create tray icon
            trayIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Shield,
                Visible = true,
                Text = "Keypass - Password Manager"
            };

            // Create context menu
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Manage Passwords", null, OnManagePasswords);
            contextMenu.Items.Add("Settings", null, OnSettings);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, OnExit);

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += OnTrayIconDoubleClick;
        }

        private void OnManagePasswords(object sender, EventArgs e)
        {
            var managerForm = new PasswordManagerForm();
            managerForm.Show();
        }

        private void OnSettings(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void OnTrayIconDoubleClick(object sender, EventArgs e)
        {
            OnManagePasswords(null, null);
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
