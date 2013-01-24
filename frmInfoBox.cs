using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VAGSuite
{
    public partial class frmInfoBox : DevExpress.XtraEditors.XtraForm
    {
        public frmInfoBox(string Message)
        {
            InitializeComponent();
            label1.Text = Message;
            this.ShowDialog();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}