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
    public partial class frmChangeNote : DevExpress.XtraEditors.XtraForm
    {
        public frmChangeNote()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public string Note
        {
            get
            {
                return textEdit1.Text;
            }
            set
            {
                textEdit1.Text = value;
            }
        }
    }
}