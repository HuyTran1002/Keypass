using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Keypass.Models;
using Keypass.Services;

namespace Keypass.UI
{
    public class PasswordManagerForm : Form
    {
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private DataGridView dgvPasswords;
        private TextBox txtSearch;
        private Label lblSearch;

        public PasswordManagerForm()
        {
            InitializeComponent();
            LoadPasswords();
        }

        private void InitializeComponent()
        {
            this.Text = "Keypass - Password Manager";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Shield;

            // Search label
            lblSearch = new Label()
            {
                Text = "Search:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(50, 20)
            };
            this.Controls.Add(lblSearch);

            // Search textbox
            txtSearch = new TextBox()
            {
                Location = new System.Drawing.Point(70, 10),
                Size = new System.Drawing.Size(300, 20)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // DataGridView
            dgvPasswords = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(760, 450),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };
            dgvPasswords.Columns.Add("Website", "Website");
            dgvPasswords.Columns.Add("Username", "Username");
            dgvPasswords.Columns.Add("Password", "Password");
            this.Controls.Add(dgvPasswords);

            // Add Button
            btnAdd = new Button()
            {
                Text = "Add",
                Location = new System.Drawing.Point(10, 500),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            // Edit Button
            btnEdit = new Button()
            {
                Text = "Edit",
                Location = new System.Drawing.Point(120, 500),
                Size = new System.Drawing.Size(100, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            // Delete Button
            btnDelete = new Button()
            {
                Text = "Delete",
                Location = new System.Drawing.Point(230, 500),
                Size = new System.Drawing.Size(100, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);
        }

        private void LoadPasswords()
        {
            dgvPasswords.Rows.Clear();
            var passwords = DatabaseService.GetAllCredentials();
            foreach (var cred in passwords)
            {
                dgvPasswords.Rows.Add(cred.Website, cred.Username, "****");
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvPasswords.Rows.Clear();
            var passwords = DatabaseService.GetAllCredentials();
            foreach (var cred in passwords)
            {
                if (cred.Website.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase))
                {
                    dgvPasswords.Rows.Add(cred.Website, cred.Username, "****");
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new AddEditPasswordForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPasswords();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvPasswords.SelectedRows.Count > 0)
            {
                var website = dgvPasswords.SelectedRows[0].Cells[0].Value.ToString();
                var username = dgvPasswords.SelectedRows[0].Cells[1].Value.ToString();
                var cred = DatabaseService.GetCredential(website, username);
                
                if (cred != null)
                {
                    var form = new AddEditPasswordForm(cred);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadPasswords();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a password entry to edit.");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPasswords.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var website = dgvPasswords.SelectedRows[0].Cells[0].Value.ToString();
                    var username = dgvPasswords.SelectedRows[0].Cells[1].Value.ToString();
                    DatabaseService.DeleteCredential(website, username);
                    LoadPasswords();
                }
            }
            else
            {
                MessageBox.Show("Please select a password entry to delete.");
            }
        }
    }
}
