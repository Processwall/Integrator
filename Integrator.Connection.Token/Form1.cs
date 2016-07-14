using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Integrator.Connection.Token
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Load Settings
            this.assemblyTextBox.Text = Properties.Settings.Default.Assembly;
            this.groupTextBox.Text = Properties.Settings.Default.Group;
            this.usernameTextBox.Text = Properties.Settings.Default.Username;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(assemblyTextBox.Text))
                {
                    MessageBox.Show("Assembly not specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (String.IsNullOrEmpty(usernameTextBox.Text))
                {
                    MessageBox.Show("Username not specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (String.IsNullOrEmpty(passwordTextBox.Text))
                {
                    MessageBox.Show("Password not specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Assembly assembly = Assembly.LoadFile(this.assemblyTextBox.Text);

                    if (assembly != null)
                    {
                        // Save Assembly
                        Properties.Settings.Default.Assembly = assemblyTextBox.Text;
                        Properties.Settings.Default.Save();

                        Type classtype = null;
                        Type interfacetype = typeof(Integrator.Connection.ISession);

                        foreach (Type t in assembly.GetTypes())
                        {
                            if (interfacetype.IsAssignableFrom(t))
                            {
                                classtype = t;
                                break;
                            }
                        }

                        if (classtype != null)
                        {
                            try
                            {
                                Integrator.Connection.ISession session = (Connection.ISession)Activator.CreateInstance(classtype, "Test");
                                this.tokenTextBox.Text = session.Token(this.groupTextBox.Text, usernameTextBox.Text, passwordTextBox.Text);

                                // Save Group and Username
                                Properties.Settings.Default.Group = groupTextBox.Text;
                                Properties.Settings.Default.Username = usernameTextBox.Text;
                                Properties.Settings.Default.Save();
                            }
                            catch (Exception ex2)
                            {
                                MessageBox.Show("Failed to generate Token: " + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to find Connection in Assembly", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to open Assembly", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Assembly: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void assemblyButton_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                assemblyTextBox.Text = openFileDialog.FileName;
            }
        }
    }
}
