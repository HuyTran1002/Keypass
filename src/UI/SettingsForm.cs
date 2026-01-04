using System;
using System.Windows.Forms;
using Keypass.Services;

namespace Keypass.UI
{
    public class SettingsForm : Form
    {
        private CheckBox chkAutoFill;
        private CheckBox chkAutoSave;
        private CheckBox chkRunOnStartup;
        private Button btnSave;
        private Button btnCancel;

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Auto Fill
            chkAutoFill = new CheckBox()
            {
                Text = "Enable Auto-Fill",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(200, 30),
                Checked = true,
                AutoSize = true
            };
            this.Controls.Add(chkAutoFill);

            // Auto Save
            chkAutoSave = new CheckBox()
            {
                Text = "Ask to save new credentials",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(250, 30),
                Checked = true,
                AutoSize = true
            };
            this.Controls.Add(chkAutoSave);

            // Run on Startup
            chkRunOnStartup = new CheckBox()
            {
                Text = "Run on Windows Startup",
                Location = new System.Drawing.Point(20, 110),
                Size = new System.Drawing.Size(200, 30),
                Checked = false,
                AutoSize = true
            };
            this.Controls.Add(chkRunOnStartup);

            // Save Button
            btnSave = new Button()
            {
                Text = "Save",
                Location = new System.Drawing.Point(210, 200),
                Size = new System.Drawing.Size(75, 30)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(295, 200),
                Size = new System.Drawing.Size(75, 30)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void LoadSettings()
        {
            chkAutoFill.Checked = SettingsService.GetSetting("AutoFill", true);
            chkAutoSave.Checked = SettingsService.GetSetting("AutoSave", true);
            chkRunOnStartup.Checked = SettingsService.GetSetting("RunOnStartup", false);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SettingsService.SaveSetting("AutoFill", chkAutoFill.Checked);
            SettingsService.SaveSetting("AutoSave", chkAutoSave.Checked);
            SettingsService.SaveSetting("RunOnStartup", chkRunOnStartup.Checked);
            MessageBox.Show("Settings saved successfully.");
            this.Close();
        }
    }
}
