using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Integrator.Connection.Token
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Credentials credentials = new Credentials(this.groupTextBox.Text, this.usernameTextBox.Text, this.passwordTextBox.Text);
            this.tokenTextBox.Text = credentials.Token;
        }
    }
}
