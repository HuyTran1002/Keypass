using System;
using System.Windows.Forms;
using Keypass.Models;
using Keypass.Services;

namespace Keypass.UI
{
    public class AddEditPasswordForm : Form
    {
        private Label lblWebsite;
        private TextBox txtWebsite;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnSave;
        private Button btnCancel;
        private Credential credential;

        public AddEditPasswordForm(Credential cred = null)
        {
            credential = cred;
            InitializeComponent();
            
            if (cred != null)
            {
                this.Text = "Edit Password";
                txtWebsite.Text = cred.Website;
                txtWebsite.Enabled = false;
                txtUsername.Text = cred.Username;
                txtPassword.Text = cred.Password;
            }
            else
            {
                this.Text = "Add New Password";
            }
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Website
            lblWebsite = new Label() { Text = "Website:", Location = new System.Drawing.Point(10, 20), Size = new System.Drawing.Size(100, 20) };
            this.Controls.Add(lblWebsite);

            txtWebsite = new TextBox() { Location = new System.Drawing.Point(120, 20), Size = new System.Drawing.Size(250, 20) };
            this.Controls.Add(txtWebsite);

            // Username
            lblUsername = new Label() { Text = "Username:", Location = new System.Drawing.Point(10, 60), Size = new System.Drawing.Size(100, 20) };
            this.Controls.Add(lblUsername);

            txtUsername = new TextBox() { Location = new System.Drawing.Point(120, 60), Size = new System.Drawing.Size(250, 20) };
            this.Controls.Add(txtUsername);

            // Password
            lblPassword = new Label() { Text = "Password:", Location = new System.Drawing.Point(10, 100), Size = new System.Drawing.Size(100, 20) };
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox() { Location = new System.Drawing.Point(120, 100), Size = new System.Drawing.Size(250, 20), PasswordChar = '*' };
            this.Controls.Add(txtPassword);

            // Save Button
            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(210, 160), Size = new System.Drawing.Size(75, 30) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(295, 160), Size = new System.Drawing.Size(75, 30) };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWebsite.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (credential == null)
            {
                credential = new Credential
                {
                    Website = txtWebsite.Text,
                    Username = txtUsername.Text,
                    Password = txtPassword.Text,
                    CreatedAt = DateTime.Now
                };
                DatabaseService.SaveCredential(credential);
            }
            else
            {
                credential.Username = txtUsername.Text;
                credential.Password = txtPassword.Text;
                credential.UpdatedAt = DateTime.Now;
                DatabaseService.UpdateCredential(credential);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
