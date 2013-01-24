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
    public partial class frmChecksumIncorrect : DevExpress.XtraEditors.XtraForm
    {
        public frmChecksumIncorrect()
        {
            InitializeComponent();
        }

        public string ChecksumType
        {
            set
            {
                textEdit1.Text = value;
            }
        }

        public int NumberChecksums
        {
            set
            {
                textEdit2.Text = value.ToString();
            }
        }

        public int NumberChecksumsFailed
        {
            set
            {
                textEdit3.Text = value.ToString();
            }
        }

        public int NumberChecksumsPassed
        {
            set
            {
                textEdit4.Text = value.ToString();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}